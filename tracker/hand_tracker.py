"""
hand_tracker.py - Detecta las dos manos y el cuerpo con MediaPipe y los envia por OSC a Unity.

Mensajes OSC (por cada mano, <lado> = Left o Right):
    /hand/<lado>/present   int    1 si la mano esta detectada, 0 si se perdio
    /hand/<lado>/open      int    1 si la mano esta abierta, 0 si no
    /hand/<lado>/lm        63 floats  (21 landmarks x [x, y, z])
    /hand/<lado>/fingers   int    dedos estirados sin contar el pulgar (0 = puño, 1 = un dedo...)

Cuerpo (lado = como se ve en la pantalla, que es un espejo):
    /body/present          int    1 si se ve el cuerpo
    /body/lean             float  inclinacion hacia la camara (0 = postura inicial,
                                  + = hacia adelante, - = hacia atras)
    /body/tilt             float  inclinacion de costado en grados (+ = hacia la derecha)
    /body/arms             2 floats  altura de cada brazo [izquierdo, derecho]:
                                  0 = muñeca a la altura del hombro, 1 = bien arriba, negativo = abajo
    /body/cross            int    1 si los brazos estan cruzados en X delante del pecho
    /body/lm               99 floats  (33 puntos del cuerpo x [x, y, z])

La postura inicial se toma sola en los primeros segundos en que se ve el cuerpo.
Para volver a tomarla: pararse derecho y apretar C.

Todos los mensajes de un cuadro viajan juntos en UN solo paquete (bundle OSC),
lo que es mucho mas estable por wifi.

Uso:
    python hand_tracker.py --backend msmf                         # prueba local
    python hand_tracker.py --backend msmf --ip 192.168.1.35       # directo al celular

Opciones utiles:
    --smooth 0.6    suavizado (1.0 = sin suavizar, mas bajo = mas suave pero mas lento)
    --hold 0.25     segundos que se mantiene la mano si MediaPipe la pierde un instante
    --no-bundle     manda cada mensaje por separado (si algo no llega)
    --no-body       solo manos (mas rapido en computadoras lentas)

Teclas: Q = salir, C = tomar de nuevo la postura inicial del cuerpo
"""
import argparse
import math
import time

import cv2
import mediapipe as mp
from pythonosc.osc_bundle_builder import IMMEDIATELY, OscBundleBuilder
from pythonosc.osc_message_builder import OscMessageBuilder
from pythonosc.udp_client import SimpleUDPClient

PORT = 5052
NAMES = ("Left", "Right")

# (punta, articulacion PIP) de indice a menique
FINGERS = [(8, 6), (12, 10), (16, 14), (20, 18)]


class Sender:
    """Junta todos los mensajes de un cuadro en un solo paquete OSC."""

    def __init__(self, client, bundled):
        self.client = client
        self.bundled = bundled
        self.bundle = None
        self.count = 0

    def begin(self):
        self.bundle = OscBundleBuilder(IMMEDIATELY)
        self.count = 0

    def emit(self, address, value):
        if not self.bundled:
            self.client.send_message(address, value)
            return
        mb = OscMessageBuilder(address=address)
        for v in (value if isinstance(value, list) else [value]):
            mb.add_arg(v)
        self.bundle.add_content(mb.build())
        self.count += 1

    def flush(self):
        if self.bundled and self.count > 0:
            self.client.send(self.bundle.build())


class Slot:
    """Estado de una mano (Left o Right)."""

    def __init__(self):
        self.pts = None          # ultimos 63 floats suavizados
        self.wrist = (0.5, 0.5)  # ultima posicion de la muneca (0..1)
        self.last_seen = 0.0
        self.sent_present = False
        self.open = False
        self.counter = 0         # histeresis para abierta/cerrada
        self.fingers = 0


def count_extended(lm, w, h):
    """Cuenta dedos extendidos (sin el pulgar) usando coordenadas isotropicas
    (x e y en pixeles, para que no se deformen por el aspecto 16:9)."""
    pts = [(p.x * w, p.y * h, p.z * w) for p in lm]
    wrist = pts[0]
    n = 0
    for tip, pip in FINGERS:
        if math.dist(wrist, pts[tip]) > math.dist(wrist, pts[pip]) * 1.15:
            n += 1
    return n


class Body:
    """Postura del cuerpo: inclinacion hacia la camara, de costado y altura de los brazos."""

    CALIBRATION_FRAMES = 45

    def __init__(self, smooth):
        self.smooth = smooth
        self.base_width = None     # ancho de hombros en la postura inicial
        self.base_head = None      # distancia entre las orejas en la postura inicial
        self.samples = []
        self.head_samples = []
        self.lean = 0.0
        self.tilt = 0.0
        self.arms = [0.0, 0.0]
        self.cross = False
        self.cross_count = 0       # histeresis
        self.pts = None

    def recalibrate(self):
        self.base_width = None
        self.base_head = None
        self.samples = []
        self.head_samples = []

    def update(self, lm, w, h):
        px = lambda i: (lm[i].x * w, lm[i].y * h)
        a, b = px(11), px(12)
        left_sh, right_sh = (a, b) if a[0] < b[0] else (b, a)
        width = math.dist(left_sh, right_sh)
        if width < 1:
            return

        # Ancho de la cabeza (entre las orejas): al levantar los brazos los hombros se juntan,
        # asi que la inclinacion se mide con la cabeza cuando las orejas se ven
        head = None
        if lm[7].visibility > 0.5 and lm[8].visibility > 0.5:
            head = math.dist(px(7), px(8))
            if head < 1:
                head = None

        if self.base_width is None:
            self.samples.append(width)
            if head is not None:
                self.head_samples.append(head)
            if len(self.samples) >= self.CALIBRATION_FRAMES:
                self.samples.sort()
                self.base_width = self.samples[len(self.samples) // 2]
                if len(self.head_samples) >= self.CALIBRATION_FRAMES // 2:
                    self.head_samples.sort()
                    self.base_head = self.head_samples[len(self.head_samples) // 2]
        if self.base_head and head is not None:
            lean = head / self.base_head - 1.0
        elif self.base_width:
            lean = width / self.base_width - 1.0
        else:
            lean = 0.0

        # Inclinacion de costado: angulo de la linea de los hombros
        tilt = math.degrees(math.atan2(right_sh[1] - left_sh[1], right_sh[0] - left_sh[0]))

        # Brazos: cuanto esta cada muñeca por encima de su hombro (en anchos de hombros).
        # Con los brazos arriba las muñecas suelen salir de cuadro: entonces cuenta el codo.
        # El ancho de referencia es el de la postura inicial (con los brazos arriba los hombros se juntan)
        ref = self.base_width or width
        pairs = sorted([(15, 13), (16, 14)], key=lambda p: lm[p[1]].x)   # (muñeca, codo) por lado
        arms = []
        for sh, (wr, el) in zip((left_sh, right_sh), pairs):
            score = -1.0
            if lm[wr].visibility > 0.5:
                score = (sh[1] - lm[wr].y * h) / ref
            if lm[el].visibility > 0.5:
                score = max(score, 1.5 * (sh[1] - lm[el].y * h) / ref)
            elif lm[wr].visibility <= 0.5 and lm[el].y * h < sh[1]:
                score = 1.0   # codo arriba del hombro aunque casi no se vea
            arms.append(max(-1.0, min(1.5, score)))

        # Brazos cruzados en X: cada muñeca queda del lado del hombro contrario,
        # a la altura del pecho o mas arriba, y las muñecas cerca una de la otra
        crossed = False
        if min(lm[i].visibility for i in (11, 12, 15, 16)) > 0.5:
            sh_l, sh_r = lm[11].x, lm[12].x          # hombros (orden anatomico, no de pantalla)
            wr_l, wr_r = lm[15].x, lm[16].x
            sides_swapped = (wr_l - wr_r) * (sh_l - sh_r) < 0
            chest = max(lm[11].y, lm[12].y) * h + ref * 1.2
            high = lm[15].y * h < chest and lm[16].y * h < chest
            close = math.dist(px(15), px(16)) < ref * 1.2
            crossed = sides_swapped and high and close
        self.cross_count = min(self.cross_count + 1, 4) if crossed else max(self.cross_count - 1, 0)
        if self.cross_count >= 3:
            self.cross = True
        elif self.cross_count == 0:
            self.cross = False

        a = self.smooth
        self.lean = a * lean + (1 - a) * self.lean
        self.tilt = a * tilt + (1 - a) * self.tilt
        self.arms = [a * n + (1 - a) * o for n, o in zip(arms, self.arms)]
        raw = []
        for p in lm:
            raw.extend((p.x, p.y, p.z))
        self.pts = raw


def assign_slots(dets, slots, now):
    """Decide si cada mano detectada es Left o Right.
    No usa la etiqueta de MediaPipe (poco confiable de lejos): usa la posicion
    y la continuidad con el cuadro anterior."""
    result = {}
    if len(dets) >= 2:
        two = sorted(dets[:2], key=lambda d: d.landmark[0].x)
        result["Left"] = two[0]
        result["Right"] = two[1]
    elif len(dets) == 1:
        d = dets[0]
        wx, wy = d.landmark[0].x, d.landmark[0].y
        recent = [n for n in NAMES if now - slots[n].last_seen < 0.6]
        if recent:
            def dist(n):
                sx, sy = slots[n].wrist
                return math.hypot(wx - sx, wy - sy)
            best = min(recent, key=dist)
            if dist(best) > 0.35 and len(recent) == 1:
                best = "Right" if best == "Left" else "Left"
        else:
            best = "Left" if wx < 0.5 else "Right"
        result[best] = d
    return result


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--ip", default="127.0.0.1")
    ap.add_argument("--port", type=int, default=PORT)
    ap.add_argument("--cam", type=int, default=0)
    ap.add_argument("--width", type=int, default=1280)
    ap.add_argument("--height", type=int, default=720)
    ap.add_argument("--backend", choices=["dshow", "msmf", "any"], default="dshow",
                    help="Si la imagen sale negra proba msmf o any")
    ap.add_argument("--smooth", type=float, default=0.6)
    ap.add_argument("--hold", type=float, default=0.25)
    ap.add_argument("--no-bundle", action="store_true")
    ap.add_argument("--no-body", action="store_true", help="No seguir el cuerpo, solo las manos")
    args = ap.parse_args()

    client = SimpleUDPClient(args.ip, args.port, allow_broadcast=True)
    sender = Sender(client, bundled=not args.no_bundle)

    backends = {"dshow": cv2.CAP_DSHOW, "msmf": cv2.CAP_MSMF, "any": cv2.CAP_ANY}
    cap = cv2.VideoCapture(args.cam, backends[args.backend])
    if args.width > 0 and args.height > 0:
        cap.set(cv2.CAP_PROP_FRAME_WIDTH, args.width)
        cap.set(cv2.CAP_PROP_FRAME_HEIGHT, args.height)
    if not cap.isOpened():
        raise SystemExit("No pude abrir la camara. Proba con --cam 1")

    mp_hands = mp.solutions.hands
    mp_draw = mp.solutions.drawing_utils
    hands = mp_hands.Hands(
        static_image_mode=False,
        max_num_hands=2,
        model_complexity=1,
        min_detection_confidence=0.6,
        min_tracking_confidence=0.5,
    )

    pose = None
    if not args.no_body:
        pose = mp.solutions.pose.Pose(
            static_image_mode=False,
            model_complexity=0,
            min_detection_confidence=0.5,
            min_tracking_confidence=0.5,
        )
    body = Body(args.smooth)
    body_sent = False

    slots = {n: Slot() for n in NAMES}
    mode = "1 paquete por cuadro" if sender.bundled else "mensajes sueltos"
    print(f"Enviando OSC a {args.ip}:{args.port} ({mode})  (Q para salir)")
    last = time.time()
    fps = 0.0

    while True:
        ok, frame = cap.read()
        if not ok:
            break

        frame = cv2.flip(frame, 1)  # espejo
        h, w = frame.shape[:2]
        now = time.time()
        rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        result = hands.process(rgb)
        body_result = pose.process(rgb) if pose is not None else None

        dets = list(result.multi_hand_landmarks) if result.multi_hand_landmarks else []
        assigned = assign_slots(dets, slots, now)

        sender.begin()

        for name, lms in assigned.items():
            slot = slots[name]
            lm = lms.landmark

            # Suavizado
            raw = []
            for p in lm:
                raw.extend((p.x, p.y, p.z))
            if slot.pts is None or now - slot.last_seen > 0.5:
                slot.pts = raw
            else:
                a = args.smooth
                slot.pts = [a * r + (1 - a) * o for r, o in zip(raw, slot.pts)]

            # Abierta / cerrada con histeresis
            slot.fingers = count_extended(lm, w, h)
            if slot.fingers >= 3:
                slot.counter = min(slot.counter + 1, 3)
            else:
                slot.counter = max(slot.counter - 1, -3)
            if slot.counter >= 2:
                slot.open = True
            elif slot.counter <= -2:
                slot.open = False

            slot.wrist = (lm[0].x, lm[0].y)
            slot.last_seen = now
            slot.sent_present = True

            sender.emit(f"/hand/{name}/present", 1)
            sender.emit(f"/hand/{name}/open", 1 if slot.open else 0)
            sender.emit(f"/hand/{name}/fingers", slot.fingers)
            sender.emit(f"/hand/{name}/lm", [round(v, 4) for v in slot.pts])

            mp_draw.draw_landmarks(frame, lms, mp_hands.HAND_CONNECTIONS)
            color = (0, 255, 0) if slot.open else (255, 255, 255)
            label = f"{name} {'ABIERTA' if slot.open else 'cerrada'} ({slot.fingers})"
            cv2.putText(frame, label, (int(lm[0].x * w) - 60, int(lm[0].y * h) + 30),
                        cv2.FONT_HERSHEY_SIMPLEX, 0.7, color, 2)

        # Manos que no se vieron en este cuadro: mantener un instante y despues avisar
        for name in NAMES:
            if name in assigned:
                continue
            slot = slots[name]
            if slot.pts is not None and slot.sent_present:
                if now - slot.last_seen < args.hold:
                    sender.emit(f"/hand/{name}/present", 1)
                    sender.emit(f"/hand/{name}/open", 1 if slot.open else 0)
                    sender.emit(f"/hand/{name}/fingers", slot.fingers)
                    sender.emit(f"/hand/{name}/lm", [round(v, 4) for v in slot.pts])
                else:
                    sender.emit(f"/hand/{name}/present", 0)
                    sender.emit(f"/hand/{name}/open", 0)
                    slot.sent_present = False
                    slot.pts = None
                    slot.open = False
                    slot.counter = 0

        # Cuerpo
        if body_result is not None and body_result.pose_landmarks:
            blm = body_result.pose_landmarks.landmark
            body.update(blm, w, h)
            sender.emit("/body/present", 1)
            sender.emit("/body/lean", round(body.lean, 4))
            sender.emit("/body/tilt", round(body.tilt, 2))
            sender.emit("/body/arms", [round(v, 3) for v in body.arms])
            sender.emit("/body/cross", 1 if body.cross else 0)
            if body.pts is not None:
                sender.emit("/body/lm", [round(v, 4) for v in body.pts])
            body_sent = True

            mp_draw.draw_landmarks(frame, body_result.pose_landmarks, mp.solutions.pose.POSE_CONNECTIONS)
            if body.base_width is None:
                status = "cuerpo: quedate derecho, tomando postura..."
            else:
                arms_up = sum(1 for v in body.arms if v > 0.3)
                status = f"cuerpo: inclinacion {body.lean:+.2f}  brazos arriba {arms_up}"
                if body.cross:
                    status += "  BRAZOS CRUZADOS"
            cv2.putText(frame, status, (10, 65), cv2.FONT_HERSHEY_SIMPLEX, 0.7, (0, 200, 255), 2)
        elif body_sent:
            sender.emit("/body/present", 0)
            body_sent = False

        sender.flush()

        fps = 0.9 * fps + 0.1 * (1.0 / max(now - last, 1e-6))
        last = now
        cv2.putText(frame, f"manos: {len(assigned)}  fps: {fps:.0f}", (10, 30),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.9, (0, 255, 0), 2)
        cv2.imshow("Hand tracker (Q para salir)", frame)
        key = cv2.waitKey(1) & 0xFF
        if key in (ord("q"), ord("Q")):
            break
        if key in (ord("c"), ord("C")):
            body.recalibrate()
            print("Tomando de nuevo la postura inicial: quedate derecho")

    cap.release()
    cv2.destroyAllWindows()


if __name__ == "__main__":
    main()

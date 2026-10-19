"""
hand_tracker.py - Detecta las dos manos con MediaPipe y las envia por OSC a Unity.

Mensajes OSC (por cada mano, <lado> = Left o Right):
    /hand/<lado>/present   int    1 si la mano esta detectada, 0 si se perdio
    /hand/<lado>/open      int    1 si la mano esta abierta, 0 si no
    /hand/<lado>/lm        63 floats  (21 landmarks x [x, y, z])

Todos los mensajes de un cuadro viajan juntos en UN solo paquete (bundle OSC),
lo que es mucho mas estable por wifi.

Uso:
    python hand_tracker.py --backend msmf                         # prueba local
    python hand_tracker.py --backend msmf --ip 192.168.1.35       # directo al celular

Opciones utiles:
    --smooth 0.6    suavizado (1.0 = sin suavizar, mas bajo = mas suave pero mas lento)
    --hold 0.25     segundos que se mantiene la mano si MediaPipe la pierde un instante
    --no-bundle     manda cada mensaje por separado (si algo no llega)

Teclas: Q = salir
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
        result = hands.process(cv2.cvtColor(frame, cv2.COLOR_BGR2RGB))

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
                    sender.emit(f"/hand/{name}/lm", [round(v, 4) for v in slot.pts])
                else:
                    sender.emit(f"/hand/{name}/present", 0)
                    sender.emit(f"/hand/{name}/open", 0)
                    slot.sent_present = False
                    slot.pts = None
                    slot.open = False
                    slot.counter = 0

        sender.flush()

        fps = 0.9 * fps + 0.1 * (1.0 / max(now - last, 1e-6))
        last = now
        cv2.putText(frame, f"manos: {len(assigned)}  fps: {fps:.0f}", (10, 30),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.9, (0, 255, 0), 2)
        cv2.imshow("Hand tracker (Q para salir)", frame)
        if cv2.waitKey(1) & 0xFF in (ord("q"), ord("Q")):
            break

    cap.release()
    cv2.destroyAllWindows()


if __name__ == "__main__":
    main()

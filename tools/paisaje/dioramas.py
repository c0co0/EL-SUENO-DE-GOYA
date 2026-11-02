"""
Separa en capas las Pinturas Negras que se recorren por dentro antes de Saturno: El gran cabron (el aquelarre),
Las Parcas y El perro. Cada capa tiene el tamaño de la pintura (asi coinciden vistas desde la entrada); el fondo se
extiende mas alla del cuadro y se funde con la oscuridad, y donde estaban las figuras se rellena.
Uso:  python tools/paisaje/dioramas.py      (necesita numpy, opencv-python, Pillow y rembg)
Las capas quedan en Assets/Resources/Dioramas. Imprime el "frame" de cada fondo para GoyaDiorama.
"""
import os, numpy as np, cv2
from PIL import Image

ROOT = os.path.join(os.path.dirname(__file__), "..", "..")
OUT = os.path.join(ROOT, "Assets", "Resources", "Dioramas")
_session = None

def rembg_mask(bgr):
    global _session
    from rembg import remove, new_session
    if _session is None: _session = new_session("u2net")
    out = remove(Image.fromarray(cv2.cvtColor(bgr, cv2.COLOR_BGR2RGB)), session=_session)
    return np.array(out)[:, :, 3]

def mask_in_box(im, box):
    """Silueta (rembg) de lo que hay en un recuadro, en una mascara del tamaño de la pintura."""
    x0, y0, x1, y1 = box
    full = np.zeros(im.shape[:2], np.uint8)
    full[y0:y1, x0:x1] = rembg_mask(im[y0:y1, x0:x1])
    return full

def solid(m, close=9):
    """Figura sin agujeros, con borde suave."""
    b = (m > 100).astype(np.uint8) * 255
    b = cv2.morphologyEx(b, cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (close, close)))
    cnts, _ = cv2.findContours(b, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    b = np.zeros_like(b); cv2.drawContours(b, cnts, -1, 255, -1)
    return b

def fill(im, hole_mask, grow=31, seed=3):
    """Rellena donde estaban las figuras: color de alrededor (pull-push) y textura del propio fondo."""
    h, w = hole_mask.shape
    hole = cv2.dilate(hole_mask, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (grow, grow))) > 0
    known = (~hole).astype(np.float32)
    f = im.astype(np.float32)
    def pullpush(img, wgt):
        if min(img.shape[:2]) < 8:
            mean = (img * wgt[..., None]).sum((0, 1)) / max(wgt.sum(), 1e-3)
            return np.ones_like(img) * mean
        small = cv2.resize(img * wgt[..., None], (img.shape[1] // 2, img.shape[0] // 2), interpolation=cv2.INTER_AREA)
        sw = cv2.resize(wgt, (img.shape[1] // 2, img.shape[0] // 2), interpolation=cv2.INTER_AREA)
        coarse = pullpush(small / np.maximum(sw, 1e-4)[..., None], np.minimum(sw * 4, 1))
        up = cv2.resize(coarse, (img.shape[1], img.shape[0]), interpolation=cv2.INTER_LINEAR)
        return img * wgt[..., None] + up * (1 - wgt[..., None])
    smooth = pullpush(f, known)
    smooth = cv2.GaussianBlur(smooth, (0, 0), 6) * hole[..., None] + f * (~hole)[..., None]
    detail = f - cv2.GaussianBlur(f, (0, 0), 8)
    rng = np.random.default_rng(seed)
    P = max(32, min(h, w) // 6)
    win = cv2.getGaussianKernel(P, P / 3) @ cv2.getGaussianKernel(P, P / 3).T; win /= win.max()
    tex = np.zeros_like(f); cnt = np.zeros((h, w), np.float32)
    ys, xs = np.where(known > 0)
    for _ in range(1500):
        i = rng.integers(len(ys)); sy, sx = ys[i], xs[i]
        if sy + P > h or sx + P > w: continue
        if known[sy:sy + P, sx:sx + P].mean() < 0.98: continue
        ty, tx = rng.integers(0, h - P), rng.integers(0, w - P)
        tex[ty:ty + P, tx:tx + P] += detail[sy:sy + P, sx:sx + P] * win[..., None]
        cnt[ty:ty + P, tx:tx + P] += win
    tex /= np.maximum(cnt, 1e-3)[..., None]
    hs = cv2.GaussianBlur(hole.astype(np.float32), (0, 0), 8)[..., None]
    return np.clip(f * (1 - hs) + (smooth + tex * 0.9) * hs, 0, 255)

def extend(bg, pad_x, pad_y, dark=(3, 4, 5)):
    """El fondo llena el ambiente: sigue mas alla del cuadro, mas borroso, y se funde con la oscuridad."""
    h, w = bg.shape[:2]
    PW, PH = int(w * pad_x), int(h * pad_y)
    # se estira el borde (no se espeja: si no aparecen caras y figuras repetidas)
    big = cv2.copyMakeBorder(bg, PH, PH, PW, PW, cv2.BORDER_REPLICATE).astype(np.float32)
    H, W = big.shape[:2]
    yy, xx = np.mgrid[0:H, 0:W].astype(np.float32)
    dx = np.maximum(0, np.maximum(PW - xx, xx - (PW + w))) / max(PW, 1)
    dy = np.maximum(0, np.maximum(PH - yy, yy - (PH + h))) / max(PH, 1)
    d = np.clip(np.sqrt(dx ** 2 + dy ** 2), 0, 1)
    blur = cv2.GaussianBlur(big, (0, 0), max(h, w) / 14)
    # desde el borde del cuadro hacia afuera pasa enseguida a la version borrosa (sin rayas del estirado)
    # empieza un poco adentro del cuadro, asi el borde estirado no se ve
    inner = np.maximum(np.maximum(PW - xx, xx - (PW + w)) / w, np.maximum(PH - yy, yy - (PH + h)) / h)
    t = np.clip((inner + 0.04) / 0.06, 0, 1)
    t = (t * t * (3 - 2 * t))[..., None]
    grain = cv2.GaussianBlur(np.random.default_rng(5).normal(0, 5, (H, W)).astype(np.float32), (0, 0), 1.2)[..., None]
    big = big * (1 - t) + (blur + grain) * t
    fade = np.clip(d ** 0.8, 0, 1)[..., None]
    big = big * (1 - fade) + np.array(dark, np.float32) * fade
    k = min(1.0, 4096 / max(H, W))   # que entre en una textura de 4096 (el meta tiene maxTextureSize 4096)
    if k < 1: big = cv2.resize(big, (int(W * k), int(H * k)), interpolation=cv2.INTER_AREA)
    return np.clip(big, 0, 255).astype(np.uint8), h / H

def edge_fade(alpha, r):
    """Donde la figura toca el borde del cuadro se desvanece en vez de cortarse recto."""
    h, w = alpha.shape
    e = np.ones((h, w), np.float32); ramp = np.linspace(0, 1, r, dtype=np.float32)
    e[:, -r:] *= ramp[::-1]; e[:, :r] *= ramp; e[:r, :] *= ramp[:, None]
    return (alpha * e).astype(np.uint8)

def save_layer(im, alpha, name):
    cv2.imwrite(os.path.join(OUT, name + ".png"), cv2.merge([im[:, :, 0], im[:, :, 1], im[:, :, 2], alpha]))

def cabron():
    """El gran cabron (el aquelarre): el macho cabrio, la muchacha de blanco y los dos grupos de los costados
    adelante; atras queda la multitud."""
    im = cv2.imread(os.path.join(ROOT, "Assets", "Una tercera de las Pinturas negras.jpg"))
    im = im[3:-3, 3:-3]
    im = cv2.resize(im, None, fx=2, fy=2, interpolation=cv2.INTER_CUBIC)   # la imagen es chica
    im = cv2.addWeighted(im, 1.4, cv2.GaussianBlur(im, (0, 0), 2), -0.4, 0)
    boxes = {   # recuadros en la imagen al doble
        "cabron_macho": (300, 0, 700, 600),
        "cabron_blanca": (700, 280, 1090, 600),
        "cabron_derecha": (1580, 110, 1914, 580),
        "cabron_izquierda": (40, 160, 400, 600),
    }
    holes = np.zeros(im.shape[:2], np.uint8)
    for name, box in boxes.items():
        m = solid(mask_in_box(im, box))
        holes = cv2.max(holes, m)
        save_layer(im, edge_fade(cv2.GaussianBlur(cv2.dilate(m, np.ones((3, 3), np.uint8)), (0, 0), 1.5), 12), name)
    bg = fill(im, holes, 25)
    big, frame = extend(bg, 0.5, 0.8, dark=(4, 6, 7))
    cv2.imwrite(os.path.join(OUT, "cabron_fondo.jpg"), big, [cv2.IMWRITE_JPEG_QUALITY, 90])
    print("cabron", im.shape[1], im.shape[0], "frame", round(frame, 4))

def perro():
    """El perro: el campo ocre vacio atras, el perro en el medio y la ladera oscura adelante (el perro asoma detras)."""
    im = cv2.imread(os.path.join(ROOT, "Assets", "Obras", "27 El perro.jpg"))
    im = im[6:-6, 6:-6]
    H, W = im.shape[:2]
    dog = solid(mask_in_box(im, (350, 1540, 580, 1700)), 7)
    # la ladera: lo oscuro que sube desde abajo (el perro queda detras)
    L = cv2.GaussianBlur(cv2.cvtColor(im, cv2.COLOR_BGR2LAB)[..., 0], (0, 0), 6)
    slope = (L < 105).astype(np.uint8) * 255
    slope[:1250] = 0
    slope[dog > 0] = 0
    n, lbl, st, _ = cv2.connectedComponentsWithStats(slope)
    slope = np.where(lbl == lbl[H - 20, W // 2], 255, 0).astype(np.uint8)
    slope = cv2.morphologyEx(slope, cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (31, 31)))
    slope_a = cv2.GaussianBlur(slope, (0, 0), 6)
    # el perro sigue un poco por debajo de la ladera (pintado con su color), asi no se ve el corte
    body = cv2.dilate(dog, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (5, 5)))
    below = np.zeros_like(dog)   # solo lo que queda tapado por la ladera
    ys, xs = np.where(dog > 0)
    cv2.fillPoly(below, [np.array([(xs.min() + 15, 1640), (xs.max() - 10, 1630), (xs.max() - 10, 1760), (xs.min() + 15, 1760)], np.int32)], 255)
    below[slope == 0] = 0
    dog_img = im.copy()
    dark = cv2.mean(im, mask=slope)[:3]
    dog_img[(below > 0) & (body == 0)] = np.array(dark, np.uint8)
    save_layer(dog_img, cv2.GaussianBlur(cv2.max(body, below), (0, 0), 1.5), "perro_perro")
    save_layer(im, slope_a, "perro_ladera")
    bg = fill(im, dog, 41)
    big, frame = extend(bg, 1.2, 0.6, dark=(6, 9, 11))
    cv2.imwrite(os.path.join(OUT, "perro_fondo.jpg"), big, [cv2.IMWRITE_JPEG_QUALITY, 90])
    print("perro", W, H, "frame", round(frame, 4))

def parcas():
    """Las Parcas: el paisaje con el rio atras, las cuatro figuras que flotan en dos grupos (las dos de la izquierda
    mas atras, la de adelante y el hombre atado mas cerca) y los arbustos oscuros de abajo, lo mas cerca."""
    im = cv2.imread(os.path.join(ROOT, "tools", "paisaje", "fuentes", "las_parcas.jpg"))
    im = im[4:-4, 4:-4]
    H, W = im.shape[:2]
    right = solid(mask_in_box(im, (1390, 186, 2190, 956)), 15)
    left = solid(mask_in_box(im, (816, 330, 1560, 900)), 15)
    left[right > 0] = 0
    save_layer(im, edge_fade(cv2.GaussianBlur(left, (0, 0), 1.5), 12), "parcas_izquierda")
    save_layer(im, edge_fade(cv2.GaussianBlur(right, (0, 0), 1.5), 12), "parcas_derecha")
    # arbustos de abajo: lo oscuro que sube desde el borde de abajo
    L = cv2.GaussianBlur(cv2.cvtColor(im, cv2.COLOR_BGR2LAB)[..., 0], (0, 0), 3)
    bush = (L < 80).astype(np.uint8) * 255
    bush[: int(H * 0.72)] = 0
    bush[(left > 0) | (right > 0)] = 0
    bush = cv2.morphologyEx(bush, cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (9, 9)))
    n, lbl, st, _ = cv2.connectedComponentsWithStats(bush)
    bush = np.where(lbl == lbl[H - 5, W // 2], 255, 0).astype(np.uint8)
    save_layer(im, edge_fade(cv2.GaussianBlur(bush, (0, 0), 2), 12), "parcas_arbustos")
    bg = fill(im, cv2.max(left, right), 31)
    big, frame = extend(bg, 0.5, 0.8, dark=(8, 12, 14))
    cv2.imwrite(os.path.join(OUT, "parcas_fondo.jpg"), big, [cv2.IMWRITE_JPEG_QUALITY, 90])
    print("parcas", W, H, "frame", round(frame, 4))

if __name__ == "__main__":
    os.makedirs(OUT, exist_ok=True)
    cabron(); parcas(); perro()

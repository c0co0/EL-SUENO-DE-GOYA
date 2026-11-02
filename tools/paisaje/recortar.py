"""
Recorta de las pinturas de Goya los elementos del camino (arbol, muro con gatos, roca con la ciudad, cerro)
y los guarda con transparencia en Assets/Resources/Paisaje.
Uso:  python tools/paisaje/recortar.py      (necesita numpy, opencv-python, Pillow y rembg)
"""
import os, numpy as np, cv2
from PIL import Image

ROOT = os.path.join(os.path.dirname(__file__), "..", "..")
OBRAS = os.path.join(ROOT, "Assets", "Obras")
OUT = os.path.join(ROOT, "Assets", "Resources", "Paisaje")

def load(name, box):
    im = cv2.imread(os.path.join(OBRAS, name))
    x0, y0, x1, y1 = box
    return im[y0:y1, x0:x1].copy()

def rembg_mask(bgr):
    from rembg import remove, new_session
    out = remove(Image.fromarray(cv2.cvtColor(bgr, cv2.COLOR_BGR2RGB)), session=new_session("u2net"))
    return np.array(out)[:, :, 3]

def poly(shape, pts):
    m = np.zeros(shape[:2], np.uint8)
    cv2.fillPoly(m, [np.array(pts, np.int32)], 255)
    return m

def finish(bgr, mask, name, soft=2.0, fade_bottom=0):
    mask = cv2.GaussianBlur(mask, (0, 0), soft)
    if fade_bottom:
        h = mask.shape[0]
        ramp = np.ones(h, np.float32); ramp[-fade_bottom:] = np.linspace(1, 0.6, fade_bottom)
        mask = (mask * ramp[:, None]).astype(np.uint8)
    ys, xs = np.where(mask > 8)
    y0, y1, x0, x1 = ys.min(), ys.max() + 1, xs.min(), xs.max() + 1
    rgba = cv2.cvtColor(bgr, cv2.COLOR_BGR2BGRA); rgba[:, :, 3] = mask
    cv2.imwrite(os.path.join(OUT, name + ".png"), rgba[y0:y1, x0:x1])
    print(name, x1 - x0, y1 - y0)

def arbol_cometa():
    """El arbol de La cometa: copa por color (verde contra el cielo) y tronco a mano."""
    im = load("02 La cometa.jpg", (540, 0, 1024, 860))
    lab = cv2.cvtColor(im, cv2.COLOR_BGR2LAB).astype(np.int16)
    L, a, b = lab[..., 0], lab[..., 1] - 128, lab[..., 2] - 128
    leaves = ((a < -4) & (b > 6) & (L < 210)) | (L < 90)   # verdes oliva y sombras (el cielo es azulado: b < 0)
    leaves = leaves.astype(np.uint8) * 255
    leaves = cv2.morphologyEx(leaves, cv2.MORPH_OPEN, np.ones((3, 3), np.uint8))
    leaves = cv2.morphologyEx(leaves, cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (9, 9)))
    # tronco y rama principal (el tronco es claro, como el cielo: va a mano)
    trunk = np.zeros(im.shape[:2], np.uint8)
    spine = [(238, 80, 12), (252, 200, 14), (272, 300, 17), (330, 400, 20), (368, 500, 24), (390, 600, 28), (405, 700, 32), (420, 800, 38), (425, 860, 42)]
    for (x0, y0, w0), (x1, y1, w1) in zip(spine, spine[1:]):
        cv2.fillPoly(trunk, [np.array([(x0 - w0, y0), (x0 + w0, y0), (x1 + w1, y1), (x1 - w1, y1)], np.int32)], 255)
    cv2.line(trunk, (262, 335), (0, 180), 255, 16)        # rama larga a la izquierda
    keep = poly(im.shape, [(0, 0), (484, 0), (484, 860), (360, 860), (300, 640), (200, 480), (0, 480)])
    people = poly(im.shape, [(270, 630), (390, 630), (390, 800), (270, 800)])
    mask = cv2.bitwise_and(cv2.max(leaves, trunk), keep)
    mask[people > 0] = trunk[people > 0]
    # quedarse con lo conectado al arbol
    n, lbl, st, _ = cv2.connectedComponentsWithStats((mask > 127).astype(np.uint8))
    main = lbl[800, 420]
    mask = np.where((lbl == main) | (st[lbl, cv2.CC_STAT_AREA] > 1500), mask, 0).astype(np.uint8)
    finish(im, mask, "arbol_cometa", 1.5, 40)

def muro_gatos():
    """El muro de Riña de gatos con los dos gatos encima."""
    im = load("13 Rina de gatos.jpg", (250, 0, 1560, 559))
    cats = rembg_mask(im)
    wall = poly(im.shape, [(225, 559), (230, 410), (240, 395), (950, 355), (1210, 350), (1210, 559)])
    finish(im, cv2.max((cats > 100).astype(np.uint8) * 255, wall), "muro_gatos", 1.5, 30)

def roca_ciudad():
    im = load("16 Ciudad sobre una roca.jpg", (250, 120, 1750, 1480))
    m = rembg_mask(im)
    m = cv2.morphologyEx((m > 100).astype(np.uint8) * 255, cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (25, 25)))
    finish(im, m, "roca_ciudad", 2.0, 60)

def cerro_duelo():
    im = load("26 Duelo a garrotazos.jpg", (1150, 300, 2000, 922))
    m = rembg_mask(im)
    m = cv2.morphologyEx((m > 100).astype(np.uint8) * 255, cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (15, 15)))
    finish(im, m, "cerro_duelo", 2.0, 20)

if __name__ == "__main__":
    os.makedirs(OUT, exist_ok=True)
    arbol_cometa(); muro_gatos(); roca_ciudad(); cerro_duelo()

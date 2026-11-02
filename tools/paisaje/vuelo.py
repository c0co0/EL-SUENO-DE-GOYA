"""
Separa Vuelo de brujas en capas para animarlo en Unity: el cielo y la tierra (sin las brujas) y el grupo que vuela.
El grupo sale del recorte de Assets/Resources/Cortes (sin la sombra de abajo).
Uso:  python tools/paisaje/vuelo.py      (necesita numpy, opencv-python)
Las capas quedan en Assets/Resources/Animadas; vuelo_capas.txt guarda donde va el grupo (en pixeles de la pintura).
"""
import os, numpy as np, cv2

ROOT = os.path.join(os.path.dirname(__file__), "..", "..")
SRC = os.path.join(ROOT, "Assets", "Obras", "18 Vuelo de brujas.jpg")
CUT = os.path.join(ROOT, "Assets", "Resources", "Cortes", "18 Vuelo de brujas.png")
OUT = os.path.join(ROOT, "Assets", "Resources", "Animadas")

im = cv2.imread(SRC); H, W = im.shape[:2]
cut = cv2.imread(CUT, cv2.IMREAD_UNCHANGED)[:, :, 3]
cut = cv2.resize(cut, (W, H), interpolation=cv2.INTER_LINEAR)
m = (cut > 140).astype(np.uint8) * 255
# solo el grupo que vuela (la mancha mas grande), sin la sombra tenue de abajo
n, lbl, st, _ = cv2.connectedComponentsWithStats(m)
big = 1 + np.argmax(st[1:, cv2.CC_STAT_AREA])
m = np.where(lbl == big, 255, 0).astype(np.uint8)
m = cv2.morphologyEx(m, cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (21, 21)))
alpha = cv2.GaussianBlur(m, (0, 0), 1.5)

# Cielo sin las brujas: relleno con lo de alrededor y grano de tela
hole = cv2.dilate(m, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (41, 41)))
s = 0.25
small = cv2.resize(im, None, fx=s, fy=s, interpolation=cv2.INTER_AREA)
fill = cv2.inpaint(small, cv2.resize(hole, (small.shape[1], small.shape[0])), 25, cv2.INPAINT_TELEA)
fill = cv2.resize(cv2.GaussianBlur(fill, (0, 0), 3), (W, H)).astype(np.float32)
hb = cv2.GaussianBlur(hole.astype(np.float32) / 255, (0, 0), 8)[..., None]
grain = cv2.GaussianBlur(np.random.default_rng(2).normal(0, 4, (H, W)).astype(np.float32), (0, 0), 0.8)[..., None]
bg = np.clip(im * (1 - hb) + (fill + grain) * hb, 0, 255).astype(np.uint8)

os.makedirs(OUT, exist_ok=True)
cv2.imwrite(os.path.join(OUT, "vuelo_fondo.jpg"), bg, [cv2.IMWRITE_JPEG_QUALITY, 90])
ys, xs = np.where(alpha > 6)
a0, a1, b0, b1 = ys.min(), ys.max() + 1, xs.min(), xs.max() + 1
rgba = cv2.cvtColor(im, cv2.COLOR_BGR2BGRA); rgba[:, :, 3] = alpha
cv2.imwrite(os.path.join(OUT, "vuelo_figura.png"), rgba[a0:a1, b0:b1])
c = ((b0 + b1) / 2, (a0 + a1) / 2)
with open(os.path.join(OUT, "vuelo_capas.txt"), "w") as f:
    f.write(f"pintura {W} {H}\nfigura {c[0]:.0f} {c[1]:.0f}\n")
print(W, H, c, b1 - b0, a1 - a0)

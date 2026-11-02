"""
Separa El pelele en capas para animarlo en Unity: fondo (sin el pelele), la manta y el pelele.
Uso:  python tools/paisaje/pelele.py      (necesita numpy, opencv-python, Pillow y rembg)
Las capas quedan en Assets/Resources/Animadas; pelele_capas.txt guarda donde va cada una (en pixeles de la pintura).
"""
import os, numpy as np, cv2
from PIL import Image

ROOT = os.path.join(os.path.dirname(__file__), "..", "..")
SRC = os.path.join(ROOT, "Assets", "Obras", "12 El pelele.jpg")
OUT = os.path.join(ROOT, "Assets", "Resources", "Animadas")
K = 1920 / 900   # las coordenadas de abajo se tomaron sobre la imagen achicada a 900 de alto

def P(pts): return np.array([(x * K, y * K) for x, y in pts], np.int32)

im = cv2.imread(SRC); H, W = im.shape[:2]

# Pelele: rembg sobre su recorte, limitado a un poligono alrededor de la figura
from rembg import remove, new_session
x0, y0, x1, y1 = int(100 * K), int(235 * K), int(370 * K), int(495 * K)
crop = im[y0:y1, x0:x1]
m = np.array(remove(Image.fromarray(cv2.cvtColor(crop, cv2.COLOR_BGR2RGB)), session=new_session("u2net")))[:, :, 3]
area = np.zeros((H, W), np.uint8)
# (sin la cara de la mujer del medio, que queda justo al lado del pie del pelele)
cv2.fillPoly(area, [P([(105, 240), (200, 245), (250, 318), (300, 335), (335, 375), (362, 425), (335, 447), (300, 410), (258, 410), (258, 492), (205, 492), (198, 395), (160, 400), (105, 330)])], 255)
pel = np.zeros((H, W), np.uint8); pel[y0:y1, x0:x1] = m
pel = cv2.bitwise_and(pel, area)
pel = cv2.morphologyEx(pel, cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (15, 15)))
pel = cv2.GaussianBlur(pel, (0, 0), 1.5)

# Fondo sin el pelele (relleno con lo que hay alrededor: cielo y arboles)
hole = cv2.dilate((pel > 20).astype(np.uint8) * 255, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (25, 25)))
s = 0.5
small = cv2.resize(im, None, fx=s, fy=s, interpolation=cv2.INTER_AREA)
fill = cv2.inpaint(small, cv2.resize(hole, (small.shape[1], small.shape[0])), 15, cv2.INPAINT_TELEA)
fill = cv2.resize(cv2.GaussianBlur(fill, (0, 0), 2), (W, H))
hb = cv2.GaussianBlur(hole.astype(np.float32) / 255, (0, 0), 5)[..., None]
# textura: el trazo fino del cielo de mas arriba, para que el relleno no quede liso
# textura: solo el grano de la tela, para que el relleno no quede liso
detail = cv2.GaussianBlur(np.random.default_rng(1).normal(0, 7, (H, W)).astype(np.float32), (0, 0), 0.8)[..., None]
bg = np.clip(im * (1 - hb) + (fill + detail) * hb, 0, 255).astype(np.uint8)

# Manta: franja entre las manos de las mujeres
man = np.zeros((H, W), np.uint8)
cv2.fillPoly(man, [P([(25, 512), (120, 518), (250, 522), (380, 530), (495, 535), (495, 548), (420, 580), (300, 605), (200, 603), (110, 585), (40, 540)])], 255)
man = cv2.GaussianBlur(man, (0, 0), 4)

def save(name, alpha):
    ys, xs = np.where(alpha > 6)
    a0, a1, b0, b1 = ys.min(), ys.max() + 1, xs.min(), xs.max() + 1
    rgba = cv2.cvtColor(im, cv2.COLOR_BGR2BGRA); rgba[:, :, 3] = alpha
    cv2.imwrite(os.path.join(OUT, name + ".png"), rgba[a0:a1, b0:b1])
    return (b0 + b1) / 2, (a0 + a1) / 2   # centro en pixeles

os.makedirs(OUT, exist_ok=True)
cv2.imwrite(os.path.join(OUT, "pelele_fondo.jpg"), bg, [cv2.IMWRITE_JPEG_QUALITY, 90])
cp = save("pelele_figura", pel)
cm = save("pelele_manta", man)
with open(os.path.join(OUT, "pelele_capas.txt"), "w") as f:
    f.write(f"pintura {W} {H}\nfigura {cp[0]:.0f} {cp[1]:.0f}\nmanta {cm[0]:.0f} {cm[1]:.0f}\n")
print(W, H, cp, cm)

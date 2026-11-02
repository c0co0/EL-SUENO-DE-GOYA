"""
Texturas en gris para el terreno y el pasto (el color lo pone Unity por vertice, con la niebla de cada epoca).
Uso:  python tools/paisaje/texturas.py
"""
import os, math, numpy as np
from PIL import Image, ImageDraw, ImageFilter

OUT = os.path.join(os.path.dirname(__file__), "..", "..", "Assets", "Resources", "Paisaje")
rng = np.random.default_rng(5)

def pinceladas(n=256):
    """Pinceladas cortas en todas direcciones, que se repiten sin costura."""
    img = Image.new("L", (n, n), 200)
    for _ in range(1400):
        x, y = rng.uniform(0, n, 2); L = rng.uniform(6, 22); ang = rng.normal(0.2, 0.6)
        v = int(np.clip(rng.normal(205, 28), 120, 255)); w = int(rng.integers(2, 5))
        dx, dy = math.cos(ang) * L, math.sin(ang) * L * 0.5
        for ox in (-n, 0, n):
            for oy in (-n, 0, n):
                ImageDraw.Draw(img).line([(x - dx + ox, y - dy + oy), (x + dx + ox, y + dy + oy)], fill=v, width=w)
    return img.filter(ImageFilter.GaussianBlur(0.8))

def mata(w=256, h=256):
    """Una mata de pasto: hojas finas que salen de abajo, mas claras en la punta."""
    img = Image.new("LA", (w, h), (0, 0))
    d = ImageDraw.Draw(img)
    for _ in range(int(rng.integers(40, 70))):
        x0 = w / 2 + rng.normal(0, w * 0.13); y0 = h - 2
        H = rng.uniform(0.35, 0.95) * h; lean = rng.normal(0, 0.35)
        pts = []
        for k in range(8):
            t = k / 7
            pts.append((x0 + lean * H * t * t, y0 - H * t))
        base = int(rng.uniform(110, 170))
        for k in range(7):
            t = k / 6
            v = int(base + (250 - base) * t)
            d.line([pts[k], pts[k + 1]], fill=(v, 255), width=max(1, int(5 * (1 - t)) + 1))
    return img

def suelo_goya():
    """Las pinceladas del suelo de Duelo a garrotazos, en gris y repetibles (espejadas)."""
    im = Image.open(os.path.join(OUT, "..", "..", "Obras", "26 Duelo a garrotazos.jpg")).convert("L")
    a = np.asarray(im.crop((0, 790, 1000, 922)).resize((512, 256), Image.LANCZOS), np.float32)
    from scipy.ndimage import gaussian_filter
    a = 205 + (a - gaussian_filter(a, 18)) * 1.1   # solo el trazo, sin las manchas grandes: el color lo pone Unity
    tile = np.vstack([np.hstack([a, a[:, ::-1]]), np.hstack([a[::-1], a[::-1, ::-1]])])
    return Image.fromarray(np.clip(tile, 60, 255).astype(np.uint8)).resize((512, 256), Image.LANCZOS)

if __name__ == "__main__":
    os.makedirs(OUT, exist_ok=True)
    suelo_goya().save(os.path.join(OUT, "pinceladas.png"))
    atlas = Image.new("LA", (1024, 256), (0, 0))
    for i in range(4): atlas.paste(mata(), (i * 256, 0))
    atlas.convert("RGBA").save(os.path.join(OUT, "pasto.png"))
    print("ok")

"""
Dibuja los arboles y las rocas del camino (pinceladas al estilo de los fondos de Goya) en Assets/Resources/Paisaje.
Uso:  python tools/paisaje/generar.py      (necesita numpy y Pillow)
"""
import os, math, numpy as np
from PIL import Image, ImageDraw, ImageFilter

OUT = os.path.join(os.path.dirname(__file__), "..", "..", "Assets", "Resources", "Paisaje")
rng = np.random.default_rng(11)

def dab(layer, x, y, rx, ry, ang, col, a):
    """Una pincelada: elipse suave rotada."""
    s = int(max(rx, ry) * 2.6) + 4
    m = Image.new("L", (s, s), 0)
    ImageDraw.Draw(m).ellipse([s / 2 - rx, s / 2 - ry, s / 2 + rx, s / 2 + ry], fill=int(255 * a))
    m = m.rotate(ang, resample=Image.BICUBIC).filter(ImageFilter.GaussianBlur(max(1, min(rx, ry) * 0.35)))
    c = Image.new("RGBA", (s, s), tuple(int(v) for v in col) + (0,))
    c.putalpha(m)
    layer.alpha_composite(c, (int(x - s / 2), int(y - s / 2)))

def jitter(col, amt=18):
    return np.clip(np.array(col) + rng.normal(0, amt, 3), 0, 255)

def tronco(img, x0, y0, x1, y1, w0, w1, col, n=80):
    # cuerpo liso (poligono) y encima pinceladas de corteza con luz a la izquierda
    dx, dy = x1 - x0, y1 - y0; L = math.hypot(dx, dy); nx, ny = -dy / L, dx / L
    poly = [(x0 + nx * w0 / 2, y0 + ny * w0 / 2), (x1 + nx * w1 / 2, y1 + ny * w1 / 2),
            (x1 - nx * w1 / 2, y1 - ny * w1 / 2), (x0 - nx * w0 / 2, y0 - ny * w0 / 2)]
    body = Image.new("RGBA", img.size, (0, 0, 0, 0))
    ImageDraw.Draw(body).polygon(poly, fill=tuple(int(v * 0.8) for v in col) + (255,))
    img.alpha_composite(body.filter(ImageFilter.GaussianBlur(2)))
    for i in range(n // 2):
        t = rng.random(); w = w0 + (w1 - w0) * t
        side = rng.uniform(-0.4, 0.15)
        x = x0 + dx * t + nx * w * side; y = y0 + dy * t + ny * w * side
        c = np.array(col) * (1.35 if side < -0.15 else 1.0)
        dab(img, x, y, w * 0.12, w * 0.4, math.degrees(math.atan2(dy, dx)) + 90 + 90, jitter(c, 10), 0.35)
    return
    for i in range(n):
        t = i / (n - 1)
        x = x0 + (x1 - x0) * t + rng.normal(0, w0 * 0.08); y = y0 + (y1 - y0) * t
        w = w0 + (w1 - w0) * t
        ang = math.degrees(math.atan2(y1 - y0, x1 - x0)) + 90
        dab(img, x, y, w * rng.uniform(0.35, 0.55), w * 0.25, ang + rng.normal(0, 8), jitter(col, 14), 0.8)

def arbol(seed, W=1024, H=1536):
    global rng; rng = np.random.default_rng(seed)
    img = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    trunk = (72, 52, 34); dark = (44, 50, 26); mid = (86, 96, 46); light = (160, 158, 92); gold = (196, 178, 112)
    base_x = W * rng.uniform(0.42, 0.58)
    top = (W * rng.uniform(0.4, 0.6), H * 0.42)
    tronco(img, base_x, H * 0.98, top[0], top[1], W * 0.13, W * 0.06, trunk, 120)
    # ramas
    branches = []
    for _ in range(rng.integers(4, 7)):
        t = rng.uniform(0.15, 0.6)
        bx = base_x + (top[0] - base_x) * (1 - t); by = H * 0.98 + (top[1] - H * 0.98) * (1 - t)
        ex = bx + rng.choice([-1, 1]) * W * rng.uniform(0.1, 0.22); ey = by - H * rng.uniform(0.12, 0.25)
        tronco(img, bx, by, ex, ey, W * 0.045, W * 0.018, trunk, 40)
        branches.append((ex, ey))
    branches.append(top)
    # copa: masas de follaje, de oscuro a claro (la luz viene de arriba a la izquierda)
    masses = [(x + rng.normal(0, W * 0.05), y + rng.normal(0, H * 0.03)) for x, y in branches for _ in range(2)]
    for layer_col, count, rad, off, a in [(dark, 900, 0.07, 0.0, 0.55), (mid, 700, 0.055, -0.02, 0.5), (light, 380, 0.04, -0.045, 0.45), (gold, 120, 0.025, -0.06, 0.4)]:
        for _ in range(count):
            cx, cy = masses[rng.integers(len(masses))]
            r = W * 0.12 * math.sqrt(rng.random())
            th = rng.uniform(0, 2 * math.pi)
            x = cx + r * math.cos(th) + off * W; y = cy + r * math.sin(th) * 0.8 + off * H
            if layer_col is not dark and (y > cy + W * 0.06): continue   # las luces solo arriba
            rx = W * rad * rng.uniform(0.5, 1.2)
            dab(img, x, y, rx, rx * rng.uniform(0.35, 0.6), rng.uniform(-40, 40), jitter(layer_col, 12), a)
    # pasto al pie
    for _ in range(160):
        x = base_x + rng.normal(0, W * 0.15); y = H * rng.uniform(0.95, 1.0)
        dab(img, x, y, W * 0.02, W * 0.008, rng.uniform(-110, -70), jitter((80, 84, 44), 15), 0.6)
    return img

def roca(seed, W=1536, H=1024):
    global rng; rng = np.random.default_rng(seed)
    img = Image.new("RGBA", (W, H), (0, 0, 0, 0))
    # silueta: borde superior irregular (fbm), abajo recto
    xs = np.arange(W)
    prof = np.zeros(W)
    for f, a in [(2, 0.18), (5, 0.08), (13, 0.035), (31, 0.015)]:
        prof += a * np.sin(xs / W * f * math.pi + rng.uniform(0, 6))
    prof = H * (0.25 + prof - prof.min()) 
    side = np.minimum(xs, W - xs) / (W * 0.18)
    prof = np.minimum(prof + H * 0.6 * np.clip(1 - side, 0, 1) ** 2, H * 0.92)   # los costados bajan
    base = np.array([150, 128, 96], float)
    # facetas de piedra: cada celda mira para un lado y la luz viene de arriba a la izquierda
    pts = np.column_stack([rng.uniform(0, W, 260), rng.uniform(0, H, 260) ** 1.0])
    pts[:, 1] *= 1.0
    yy, xx = np.mgrid[0:H:2, 0:W:2]
    d = (xx[..., None] - pts[None, None, :, 0]) ** 2 + ((yy[..., None] - pts[None, None, :, 1]) * 1.8) ** 2
    cell = d.argmin(-1)
    normals = rng.normal(0, 1, (len(pts), 3)); normals[:, 2] = np.abs(normals[:, 2]) + 1.2
    normals /= np.linalg.norm(normals, axis=1, keepdims=True)
    light = np.array([-0.5, -0.6, 0.62]); light /= np.linalg.norm(light)
    shade = np.clip(normals @ light, 0.15, 1)[cell]
    tone = rng.uniform(0.85, 1.15, len(pts))[cell]
    depth = (yy / H)
    col = base[None, None, :] * (0.45 + 0.8 * shade[..., None]) * tone[..., None] * (1.05 - 0.35 * depth[..., None])
    moss = (rng.random(len(pts)) < 0.12)[cell] & (shade > 0.6)
    col[moss] = col[moss] * 0.5 + np.array([98, 100, 58]) * 0.5
    tex = Image.fromarray(np.clip(col, 0, 255).astype(np.uint8)).resize((W, H), Image.BICUBIC).filter(ImageFilter.GaussianBlur(1.5))
    img.alpha_composite(tex.convert("RGBA"))
    # pinceladas encima para que no quede liso
    for _ in range(1500):
        x = rng.uniform(0, W); y = rng.uniform(0, H)
        c = np.array(tex.getpixel((int(x) % W, int(y) % H)), float)
        rx = W * rng.uniform(0.008, 0.02)
        dab(img, x, y, rx, rx * 0.4, rng.normal(0, 15), jitter(c, 10), 0.4)
    # grietas
    d = ImageDraw.Draw(img)
    for _ in range(18):
        x = rng.uniform(W * 0.1, W * 0.9); y = prof[int(x)] + 20
        pts = [(x, y)]
        for _ in range(rng.integers(4, 10)):
            x += rng.normal(0, 20); y += rng.uniform(25, 60)
            if y >= H: break
            pts.append((x, y))
        d.line(pts, fill=(52, 44, 36, 160), width=int(rng.integers(2, 5)))
    # recortar a la silueta, con borde suave
    m = np.zeros((H, W), np.uint8)
    yy = np.arange(H)[:, None]
    m[yy >= prof[None, :]] = 255
    mask = Image.fromarray(m).filter(ImageFilter.GaussianBlur(4))
    a = np.minimum(np.array(img.split()[3]), np.array(mask))
    img.putalpha(Image.fromarray(a))
    return img

if __name__ == "__main__":
    os.makedirs(OUT, exist_ok=True)
    for i in range(3):
        arbol(100 + i).save(os.path.join(OUT, f"arbol_{i+1}.png")); print("arbol", i + 1)
        roca(200 + i).save(os.path.join(OUT, f"roca_{i+1}.png")); print("roca", i + 1)

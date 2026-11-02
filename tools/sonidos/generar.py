"""
Genera los sonidos del recorrido (sintetizados, sin grabaciones) en Assets/Resources/Sonidos.
Uso:  python tools/sonidos/generar.py      (necesita numpy, scipy y ffmpeg)

Cada zona tiene 4 "camas" (ambiente continuo, en loop) que Unity reparte alrededor tuyo,
y sonidos sueltos que aparecen en lugares al azar (pajaros arriba, cañones lejos, susurros cerca).
Para cambiar un sonido por una grabacion: reemplazar el .ogg con el mismo nombre.
"""
import os, subprocess, tempfile, numpy as np
from scipy import signal
from scipy.io import wavfile

SR = 44100
OUT = os.path.join(os.path.dirname(__file__), "..", "..", "Assets", "Resources", "Sonidos")
rng = np.random.default_rng(7)

# ------------------------------------------------------------------ herramientas
def t(sec): return np.arange(int(sec * SR)) / SR
def noise(sec): return rng.standard_normal(int(sec * SR))
def band(x, lo, hi, order=2):
    sos = signal.butter(order, [lo, hi], "bandpass", fs=SR, output="sos"); return signal.sosfilt(sos, x)
def low(x, f, order=2):
    sos = signal.butter(order, f, "lowpass", fs=SR, output="sos"); return signal.sosfilt(sos, x)
def high(x, f, order=2):
    sos = signal.butter(order, f, "highpass", fs=SR, output="sos"); return signal.sosfilt(sos, x)
def brown(sec):
    x = np.cumsum(noise(sec)); x = high(x, 20); return x / (np.abs(x).max() + 1e-9)
def smooth_rand(sec, rate, lo=0.0, hi=1.0):
    """Curva al azar que cambia suave (rate = cambios por segundo)."""
    n = max(4, int(sec * rate) + 3)
    pts = rng.uniform(lo, hi, n)
    xs = np.linspace(0, sec, n)
    return np.interp(t(sec), xs, pts) if rate < 1 else signal.resample(pts, int(sec * SR))[: int(sec * SR)]
def curve(sec, rate, lo=0.0, hi=1.0):
    n = max(4, int(sec * rate) + 4)
    pts = rng.uniform(lo, hi, n)
    from scipy.interpolate import CubicSpline
    cs = CubicSpline(np.linspace(0, sec, n), pts)
    return np.clip(cs(t(sec)), min(lo, hi), max(lo, hi))
def env(n, a, d):
    """Ataque y caida exponencial (segundos)."""
    x = np.arange(n) / SR
    return np.minimum(1, x / max(a, 1e-4)) * np.exp(-np.maximum(0, x - a) / max(d, 1e-4))
def reverb(x, sec=2.0, mix=0.3, bright=4000):
    n = int(sec * SR)
    ir = rng.standard_normal(n) * np.exp(-np.arange(n) / SR / (sec / 6.9))
    ir = low(ir, bright); ir /= np.abs(ir).sum() ** 0.5 * 8
    wet = np.pad(signal.fftconvolve(x, ir), (0, 1))[: len(x) + n]
    dry = np.concatenate([x, np.zeros(n)])
    return dry * (1 - mix) + wet * mix
def loopify(x, fade=1.5):
    """El final se funde con el principio: sin corte al repetir."""
    f = int(fade * SR); body = x[:-f].copy()
    w = np.linspace(0, 1, f)
    body[:f] = body[:f] * np.sqrt(w) + x[-f:] * np.sqrt(1 - w)
    return body
def place(total, clip, at):
    i = int(at * SR); n = min(len(clip), len(total) - i)
    if n > 0: total[i:i + n] += clip[:n]
def norm(x, peak=0.9): return x / (np.abs(x).max() + 1e-9) * peak
def trim(x, thr=1e-4):
    idx = np.where(np.abs(x) > thr * np.abs(x).max())[0]
    return x[: idx[-1] + 1] if len(idx) else x

META = """fileFormatVersion: 2
guid: {guid}
AudioImporter:
  externalObjects: {{}}
  serializedVersion: 8
  defaultSettings:
    serializedVersion: 2
    loadType: {load}
    sampleRateSetting: 0
    sampleRateOverride: 44100
    compressionFormat: 1
    quality: 0.6
    conversionMode: 0
    preloadAudioData: 1
  platformSettingOverrides: {{}}
  forceToMono: 1
  normalize: 0
  loadInBackground: 0
  ambisonic: 0
  3D: 1
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""
def save(name, x, peak=0.9, loop=False):
    os.makedirs(OUT, exist_ok=True)
    if loop:   # camas: mismo volumen promedio, para que ninguna zona tape a otra
        x = np.tanh(x / (np.sqrt(np.mean(x ** 2)) + 1e-9) * 0.09 / 0.95) * 0.95
    else:
        x = norm(x, peak)
    x = x.astype(np.float32)
    path = os.path.join(OUT, name + ".ogg")
    with tempfile.NamedTemporaryFile(suffix=".wav", delete=False) as f: tmp = f.name
    wavfile.write(tmp, SR, x)
    subprocess.run(["ffmpeg", "-y", "-loglevel", "error", "-i", tmp, "-c:a", "libvorbis", "-q:a", "3", path], check=True)
    os.remove(tmp)
    meta = path + ".meta"
    if not os.path.exists(meta):   # el guid queda fijo para que no cambie en cada compu
        import hashlib
        guid = hashlib.md5(("goya-sonido-" + name).encode()).hexdigest()
        # Camas largas: comprimidas en memoria. Sueltos cortos: descomprimidos (sin demora al sonar)
        open(meta, "w").write(META.format(guid=guid, load=1 if loop else 0))
    print(name, f"{len(x)/SR:.1f}s")

# ------------------------------------------------------------------ pasos (tierra)
def paso():
    n = int(0.45 * SR)
    thump = low(rng.standard_normal(n), 220) * env(n, 0.004, 0.035) * 3
    crunch = np.zeros(n)
    for _ in range(rng.integers(25, 45)):        # granitos de tierra
        at = abs(rng.normal(0.03, 0.05)); g = int(rng.uniform(0.002, 0.008) * SR)
        grain = rng.standard_normal(g) * np.hanning(g) * rng.uniform(0.2, 1)
        i = int(at * SR)
        if i + g < n: crunch[i:i + g] += grain
    crunch = band(crunch, 900, rng.uniform(4000, 6500)) * np.exp(-np.arange(n) / SR / 0.12)
    x = thump + crunch * 0.8
    return reverb(trim(x), 0.4, 0.12)

# ------------------------------------------------------------------ fuego
def fuego_loop(sec=12):
    roar = low(brown(sec + 2), 380) * curve(sec + 2, 3, 0.5, 1.0)
    hiss = high(noise(sec + 2), 3000) * 0.04 * curve(sec + 2, 6, 0.3, 1)
    crack = np.zeros(int((sec + 2) * SR))
    k = 0.0
    while k < sec + 1.8:
        k += rng.exponential(1 / 14)
        n = int(rng.uniform(0.004, 0.03) * SR)
        c = band(rng.standard_normal(n), rng.uniform(700, 2500), rng.uniform(3000, 8000)) * env(n, 0.0005, rng.uniform(0.002, 0.012))
        place(crack, c * (rng.uniform(0.2, 1) ** 2) * (4 if rng.random() < 0.08 else 1), k)
    x = roar * 0.9 + hiss + crack * 0.6
    return loopify(x, 2)
def fuego_encendido():
    sec = 2.2; tt = t(sec)
    x = np.zeros(len(tt)); nz = noise(sec)
    for lo, hi, at in [(150, 500, 0.0), (400, 1200, 0.25), (900, 3000, 0.5), (2000, 6000, 0.7)]:
        e = np.clip((tt - at) / 0.5, 0, 1) * np.exp(-np.maximum(0, tt - at - 0.5) / 0.6)
        x += band(nz, lo, hi) * e
    x += low(brown(sec), 120) * np.minimum(1, tt / 0.3) * np.exp(-tt / 1.2) * 2
    return reverb(x, 1.2, 0.25)

# ------------------------------------------------------------------ zona 1: cartones (campo)
def viento(sec, lo=200, hi=1200, gust=0.15):
    g = curve(sec, gust * 2, 0.25, 1.0)
    w = band(noise(sec), lo, hi) * g
    w += band(noise(sec), lo * 2.5, hi * 2) * g ** 2 * 0.4
    return w
def hojas(sec):
    g = curve(sec, 0.4, 0, 1) ** 3
    flutter = curve(sec, 18, 0.2, 1)
    return high(noise(sec), 2800) * g * flutter * 0.35
def cama_campo(sec=16):
    x = viento(sec + 2) + hojas(sec + 2)
    return loopify(x, 2)
def ave():
    sec = 2.4; out = np.zeros(int(sec * SR)); at = 0.05
    base = rng.uniform(2500, 4500); kind = rng.integers(3)
    for _ in range(rng.integers(3, 9)):
        d = rng.uniform(0.04, 0.12); n = int(d * SR); tt = np.arange(n) / SR
        f0, f1 = base * rng.uniform(0.8, 1.2), base * rng.uniform(0.7, 1.5)
        if kind == 2: f1 = f0 * 1.6
        f = np.linspace(f0, f1, n) + 400 * np.sin(2 * np.pi * rng.uniform(20, 60) * tt) * (kind == 1)
        ph = 2 * np.pi * np.cumsum(f) / SR
        s = (np.sin(ph) + 0.25 * np.sin(2 * ph)) * np.sin(np.pi * tt / d) ** 2
        place(out, s, at); at += d + rng.uniform(0.02, 0.15)
        if at > sec - 0.2: break
    return reverb(trim(out), 1.2, 0.2, 7000)

# ------------------------------------------------------------------ zona 2: la corte (salon)
VOCALES = [(730, 1090), (530, 1840), (270, 2290), (570, 840), (300, 870), (440, 1020)]
def voz(sec, f0, whisper=False):
    """Una persona hablando bajo, sin palabras: pulsos de voz por filtros de vocales."""
    n = int(sec * SR); tt = t(sec)
    pitch = f0 * (1 + 0.12 * curve(sec, 2.5, -1, 1)) * (1 + 0.01 * np.sin(2 * np.pi * 5 * tt))
    src = noise(sec) if whisper else signal.sawtooth(2 * np.pi * np.cumsum(pitch) / SR) + 0.05 * noise(sec)
    # Vocales que cambian cada ~0.1 s, superpuestas a la mitad para que no se note el corte
    seg = int(rng.uniform(0.09, 0.15) * SR); hop = seg // 2
    out = np.zeros(n + seg); win = np.hanning(seg)
    padded = np.concatenate([src, np.zeros(seg)])
    for i in range(-int(rng.integers(hop)), n, hop):
        a = max(0, i); chunk = padded[a: a + seg]
        if len(chunk) < 64: continue
        f1, f2 = VOCALES[rng.integers(len(VOCALES))]
        y = band(chunk, f1 * 0.8, f1 * 1.2) + 0.6 * band(chunk, f2 * 0.85, f2 * 1.15)
        out[a: a + len(y)] += y * win[: len(y)]
    out = out[:n] * 2
    # silabas y pausas de frase
    syl = np.clip(np.sin(2 * np.pi * np.cumsum(curve(sec, 1.5, 3, 6.5)) / SR + rng.uniform(0, 7)), 0, 1) ** 0.5
    phrase = (curve(sec, 0.7, -0.6, 1) > 0).astype(float)
    phrase = low(phrase, 6)
    return out * syl * phrase
def cama_corte(sec=16):
    x = np.zeros(int((sec + 2) * SR))
    for _ in range(9):
        x += voz(sec + 2, rng.uniform(95, 230)) * rng.uniform(0.3, 1)
    x = low(x, 2200)
    x = reverb(x, 2.5, 0.55, 3000)[: int((sec + 2) * SR)]
    x += low(brown(sec + 2), 200) * 0.05
    return loopify(x, 2)
def clave():
    """Clave (clavecin) tocando unas notas, como desde otro salon."""
    notas = [146.8, 174.6, 220, 293.7, 349.2, 440, 587.3]
    sec = 3.5; out = np.zeros(int(sec * SR)); at = 0
    for _ in range(rng.integers(3, 7)):
        f = notas[rng.integers(len(notas))]; p = int(SR / f)
        buf = rng.uniform(-1, 1, p); n = int(1.6 * SR); y = np.zeros(n)
        for i in range(n):                         # Karplus-Strong
            y[i] = buf[i % p]; buf[i % p] = 0.996 * 0.5 * (buf[i % p] + buf[(i + 1) % p])
        place(out, high(y, 150) * rng.uniform(0.6, 1), at); at += rng.choice([0.18, 0.18, 0.36])
    return reverb(low(out, 3500), 2.0, 0.45, 3000)
def copa():
    n = int(1.6 * SR); tt = np.arange(n) / SR; f = rng.uniform(1800, 2600)
    x = sum(np.sin(2 * np.pi * f * r * tt) * np.exp(-tt / (0.5 / r)) * a for r, a in [(1, 1), (2.32, 0.5), (4.25, 0.25)])
    return reverb(x * env(n, 0.001, 1), 1.5, 0.35)

# ------------------------------------------------------------------ zona 3: brujas (noche)
def grillos(sec):
    out = np.zeros(int(sec * SR))
    for _ in range(3):
        f = rng.uniform(4200, 5200); rate = rng.uniform(2, 4); at = rng.uniform(0, 1)
        while at < sec - 0.3:
            for k in range(3):
                n = int(0.018 * SR); tt = np.arange(n) / SR
                place(out, np.sin(2 * np.pi * f * tt) * np.hanning(n) * 0.15, at + k * 0.03)
            at += 1 / rate * rng.uniform(0.9, 1.1)
    return out * curve(sec, 0.3, 0.3, 1)
def cama_noche(sec=16):
    howl = 0
    for _ in range(3):
        c = curve(sec + 2, 0.25, 250, 700)
        y = noise(sec + 2); out = np.zeros_like(y); blk = 2048
        for i in range(0, len(y), blk):
            fc = c[i]; out[i:i + blk] = band(y[max(0, i - blk): i + blk], fc * 0.92, fc * 1.08)[-len(y[i:i + blk]):]
        howl = howl + out * curve(sec + 2, 0.3, 0.1, 1)
    x = howl * 0.6 + viento(sec + 2, 100, 500, 0.1) * 0.5 + grillos(sec + 2)
    return loopify(x, 2)
def susurro():
    sec = rng.uniform(1.2, 2.2)
    x = high(voz(sec, 150, whisper=True), 900)
    x *= np.minimum(1, t(sec) / 0.1) * np.minimum(1, (sec - t(sec)) / 0.3)
    return reverb(x, 0.8, 0.2, 6000)
def buho():
    sec = 2.2; out = np.zeros(int(sec * SR))
    for at, d in [(0, 0.35), (0.6, 0.18), (0.85, 0.5)]:
        n = int(d * SR); tt = np.arange(n) / SR
        f = 380 * (1 + 0.06 * np.sin(np.pi * tt / d)) * (1 - 0.08 * tt / d)
        y = np.sin(2 * np.pi * np.cumsum(f) / SR) * np.sin(np.pi * tt / d) ** 1.5
        place(out, y + 0.15 * low(rng.standard_normal(n), 900) * np.sin(np.pi * tt / d), at)
    return reverb(out, 2.5, 0.45, 2500)

# ------------------------------------------------------------------ zona 4: la guerra
def cama_guerra(sec=16):
    rumble = low(brown(sec + 2), 110) * curve(sec + 2, 0.4, 0.3, 1)
    x = rumble * 1.4 + viento(sec + 2, 150, 800, 0.2) * 0.35
    return loopify(x, 2)
def canon():
    sec = 4.5; n = int(sec * SR); tt = t(sec)
    boom = np.sin(2 * np.pi * np.cumsum(60 * np.exp(-tt / 0.4) + 32) / SR) * np.exp(-tt / 0.35)
    blast = low(rng.standard_normal(n), 700) * np.exp(-tt / 0.15)
    x = boom * 1.2 + blast
    return reverb(low(x, 900), 3.5, 0.6, 1200)
def disparos():
    sec = 3; out = np.zeros(int(sec * SR)); at = 0.02
    for _ in range(rng.integers(3, 10)):
        n = int(0.25 * SR)
        s = low(rng.standard_normal(n), 2500) * env(n, 0.0005, 0.03) + low(rng.standard_normal(n), 300) * env(n, 0.001, 0.08)
        place(out, s * rng.uniform(0.4, 1), at); at += abs(rng.normal(0.12, 0.12))
    return reverb(out, 2.5, 0.55, 2000)
def tambor():
    sec = 4; out = np.zeros(int(sec * SR)); at = 0; bpm = rng.uniform(95, 115)
    patron = [1, 0, 0.5, 0.5, 1, 0, 0.6, 0.4, 0.4, 0.4, 1, 0, 1, 0, 0, 0]
    for i, a in enumerate(patron * 2):
        if a:
            n = int(0.3 * SR); tt = np.arange(n) / SR
            snare = band(rng.standard_normal(n), 1500, 6000) * np.exp(-tt / 0.06)
            skin = np.sin(2 * np.pi * 190 * tt) * np.exp(-tt / 0.05)
            place(out, (snare * 0.7 + skin) * a, at)
        at += 60 / bpm / 4
        if at > sec - 0.3: break
    return reverb(low(out, 2500), 2.5, 0.55, 2000)

# ------------------------------------------------------------------ zona 5: pinturas negras
def cama_negra(sec=16):
    tt = t(sec + 2); x = np.zeros(len(tt))
    for f, a in [(41.2, 1), (55, 0.8), (61.7, 0.35), (82.4, 0.4), (110.3, 0.18)]:
        x += a * np.sin(2 * np.pi * f * tt + rng.uniform(0, 6)) * curve(sec + 2, 0.15, 0.4, 1)
    breath = band(noise(sec + 2), 200, 900) * (np.sin(2 * np.pi * tt / 5.3) * 0.5 + 0.5) ** 3 * 0.25
    x += breath + low(brown(sec + 2), 60) * 0.3
    return loopify(x, 2)
def gemido():
    sec = rng.uniform(2.5, 4); tt = t(sec)
    f0 = rng.uniform(80, 120) * (1 - 0.25 * tt / sec)
    src = signal.sawtooth(2 * np.pi * np.cumsum(f0) / SR)
    x = band(src, 500, 900) + 0.5 * band(src, 900, 1300) + 0.3 * band(src, 250, 350)
    x *= np.sin(np.pi * tt / sec) ** 1.5
    return reverb(low(x, 1500), 3.0, 0.55, 1500)
def latido():
    sec = 2.2; out = np.zeros(int(sec * SR))
    for k in range(2):
        for at, a in [(0, 1), (0.28, 0.6)]:
            n = int(0.3 * SR); tt = np.arange(n) / SR
            place(out, np.sin(2 * np.pi * (45 + 30 * np.exp(-tt / 0.03)) * tt) * np.exp(-tt / 0.09) * a, k * 1.0 + at)
    return low(out, 200)
def mastica():
    """Saturno: crujidos y algo humedo, muy cerca."""
    sec = 3; out = np.zeros(int(sec * SR)); at = 0.1
    while at < sec - 0.4:
        n = int(0.25 * SR)
        c = band(rng.standard_normal(n), 300, 2500) * env(n, 0.005, 0.05) * curve(0.25, 40, 0.2, 1)[:n]
        place(out, c, at); at += rng.uniform(0.35, 0.6)
    return reverb(out, 1.0, 0.2, 3000)

# ------------------------------------------------------------------ cueva
def cama_cueva(sec=16):
    tt = t(sec + 2)
    x = low(brown(sec + 2), 90) * curve(sec + 2, 0.2, 0.5, 1)
    air = np.zeros(len(tt)); y = noise(sec + 2); c = curve(sec + 2, 0.15, 180, 420); blk = 2048
    for i in range(0, len(y), blk):
        fc = c[i]; air[i:i + blk] = band(y[max(0, i - blk): i + blk], fc * 0.95, fc * 1.05)[-len(y[i:i + blk]):]
    x = x + air * 0.5 * curve(sec + 2, 0.2, 0.2, 1)
    x = reverb(x, 3.0, 0.6, 1500)[: len(tt)]
    return loopify(x, 2)
def gota():
    n = int(0.25 * SR); tt = np.arange(n) / SR; f = rng.uniform(1300, 2600)
    y = np.sin(2 * np.pi * np.cumsum(f * (1 + 0.8 * np.exp(-tt / 0.01))) / SR) * np.exp(-tt / 0.04)
    return reverb(y, 2.8, 0.6, 5000)

# ------------------------------------------------------------------
if __name__ == "__main__":
    for i in range(6): save(f"pasos_{i+1}", paso(), 0.8)
    save("fuego_loop", fuego_loop(), loop=True)
    save("fuego_encendido", fuego_encendido())
    camas = [cama_campo, cama_corte, cama_noche, cama_guerra, cama_negra]
    for z, f in enumerate(camas):
        for v in range(4): save(f"zona{z+1}_cama_{v+1}", f(), 0.7, loop=True)
    for i in range(6): save(f"zona1_ave_{i+1}", ave(), 0.6)
    for i in range(4): save(f"zona2_clave_{i+1}", clave(), 0.6)
    for i in range(3): save(f"zona2_copa_{i+1}", copa(), 0.4)
    for i in range(5): save(f"zona3_susurro_{i+1}", susurro(), 0.6)
    for i in range(2): save(f"zona3_buho_{i+1}", buho(), 0.6)
    for i in range(3): save(f"zona4_canon_{i+1}", canon(), 0.9)
    for i in range(3): save(f"zona4_disparos_{i+1}", disparos(), 0.7)
    for i in range(2): save(f"zona4_tambor_{i+1}", tambor(), 0.6)
    for i in range(3): save(f"zona5_gemido_{i+1}", gemido(), 0.6)
    for i in range(2): save(f"zona5_latido_{i+1}", latido(), 0.8)
    for i in range(3): save(f"zona5_mastica_{i+1}", mastica(), 0.6)
    for v in range(2): save(f"cueva_cama_{v+1}", cama_cueva(), 0.7, loop=True)
    for i in range(5): save(f"cueva_gota_{i+1}", gota(), 0.5)

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// El soporte de una obra del camino (una tela colgada entre dos palos o una roca con una cara lisa, a la altura de la
/// vista) y la textura donde la obra se pinta.
///
/// La obra la pintan CAMINANTES (como en "Dibujando con sistema de particulas", Processing): cada caminante es una
/// particula con posicion, velocidad y una aceleracion al azar en cada paso; toma el color de la pintura donde esta
/// parado, deja una pincelada redonda y se va achicando hasta morir. La obra aparece trazo por trazo, donde caminaron.
/// GoyaParticles los hace nacer donde llega cada mota de polvo que juntaste con la mano.
/// No es un MonoBehaviour: lo usa GoyaParticles.
/// </summary>
public class GoyaCanvas
{
    public enum Kind { Cloth, Rock }

    public readonly Kind kind;
    public readonly Texture2D tex;
    public readonly int cw, ch;
    public GameObject support;
    /// <summary>Donde crece el arbol del que cuelga la tela, y su alto.</summary>
    public Vector3 TreeRoot;
    public float TreeHeight = 6f;
    public Vector3 supportAt;        // donde estaba la obra cuando se armo el soporte
    Mesh supportMesh;
    Color[] supportBase, supportTinted;

    readonly Color32[] px;
    readonly Color32[] basePx;
    readonly Texture2D source;      // copia chica de la pintura (para leer sus colores)
    readonly int[] cellPainted;     // pixeles pintados por celda (para saber que falta)
    const int Cells = 16;
    const int Res = 512;              // alto de la tela en pixeles (mas = mas definicion)
    const float Px = Res / 256f;      // los tamaños se dan para una tela de 256
    int painted;
    // La obra ocupa el centro de la tela; alrededor queda un borde de tela sin pintar (en la roca no hay borde)
    readonly int ix0, iy0, iw, ih;
    /// <summary>Proporcion del soporte que ocupa el borde, a cada lado.</summary>
    public readonly float margin;
    bool dirty;

    // Caminantes
    readonly List<float> wx = new List<float>(), wy = new List<float>(), vx = new List<float>(), vy = new List<float>(), ws = new List<float>(), wk = new List<float>();
    readonly List<Color32> wc = new List<Color32>();

    public int MaxWalkers = 260;
    public float Size = 6f;          // radio inicial de la pincelada (pixeles)
    public float Shrink = 0.035f;    // cuanto se achica en cada paso
    public float Speed = 1.4f;       // velocidad maxima (pixeles por paso)
    public int StepsPerFrame = 3;

    /// <summary>Si esta, los caminantes toman el color de aca (u, v de la obra) en vez de la pintura fija:
    /// asi pintan una obra animada, cuadro a cuadro.</summary>
    public System.Func<float, float, Color> Sampler;

    public static readonly Color Linen = new Color(0.86f, 0.8f, 0.68f, 1f);

    public GoyaCanvas(Kind kind, Texture2D source, float aspect, float margin)
    {
        this.kind = kind;
        this.source = source;
        this.margin = kind == Kind.Cloth ? Mathf.Clamp(margin, 0f, 0.3f) : 0f;
        ch = Res;
        cw = Mathf.Clamp(Mathf.RoundToInt(ch * aspect), 128, Res * 8 / 5);
        ix0 = Mathf.RoundToInt(cw * this.margin); iy0 = Mathf.RoundToInt(ch * this.margin);
        iw = cw - 2 * ix0; ih = ch - 2 * iy0;
        tex = new Texture2D(cw, ch, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        px = new Color32[cw * ch];
        basePx = new Color32[cw * ch];
        cellPainted = new int[Cells * Cells];
        // Tela: lino con trama; roca: transparente (se ve la piedra)
        for (int y = 0; y < ch; y++)
            for (int x = 0; x < cw; x++)
            {
                if (kind == Kind.Cloth)
                {
                    // Lino viejo: trama, manchas y los bordes mas oscuros (gastados)
                    float weave = 0.9f + 0.05f * Mathf.Sin(x * 1.9f) * Mathf.Sin(y * 2.1f) + 0.12f * (Mathf.PerlinNoise(x * 0.03f, y * 0.03f) - 0.5f);
                    float ex = Mathf.Min(x, cw - 1 - x) / (float)cw, ey = Mathf.Min(y, ch - 1 - y) / (float)ch;
                    weave *= Mathf.Lerp(0.72f, 1f, Mathf.Clamp01(Mathf.Min(ex, ey) * 14f));
                    Color c = Linen * weave; c.a = 1f;
                    basePx[y * cw + x] = c;
                }
                else basePx[y * cw + x] = new Color32(0, 0, 0, 0);
            }
        Clear();
    }

    /// <summary>Que parte de la obra ya esta pintada (0 a 1).</summary>
    public float Coverage { get { return (float)painted / Mathf.Max(1, iw * ih); } }
    public int Walkers { get { return wx.Count; } }

    public void Clear()
    {
        System.Array.Copy(basePx, px, px.Length);
        System.Array.Clear(cellPainted, 0, cellPainted.Length);
        painted = 0;
        wx.Clear(); wy.Clear(); vx.Clear(); vy.Clear(); ws.Clear(); wc.Clear(); wk.Clear();
        tex.SetPixels32(px);
        tex.Apply(false);
    }

    /// <summary>Nace un caminante en ese lugar de la obra (u, v de 0 a 1).</summary>
    public void Spawn(Vector2 uv, float sizeK = 1f, float fast = 1f)
    {
        if (wx.Count >= MaxWalkers) return;
        float a = Random.value * Mathf.PI * 2f;
        wx.Add(ix0 + uv.x * (iw - 1)); wy.Add(iy0 + uv.y * (ih - 1));
        vx.Add(Mathf.Cos(a) * Speed * Px * 0.5f); vy.Add(Mathf.Sin(a) * Speed * Px * 0.5f);
        // De lo grueso a lo fino: al principio pinceladas grandes, a medida que se cubre la tela, mas chicas (mas detalle)
        float fine = Mathf.Lerp(1f, 0.4f, Coverage);
        ws.Add(Size * Px * sizeK * fine * Random.Range(0.6f, 1.2f));
        wk.Add(fast);
        wc.Add(new Color32(0, 0, 0, 0));
    }

    /// <summary>Caminantes nuevos dentro de una parte de la obra (u, v de 0 a 1): donde se mueve una obra animada.</summary>
    public void SpawnIn(Rect area, int count, float sizeK, float fast)
    {
        for (int k = 0; k < count && wx.Count < MaxWalkers; k++)
            Spawn(new Vector2(Mathf.Clamp01(area.x + Random.value * area.width), Mathf.Clamp01(area.y + Random.value * area.height)), sizeK, fast);
    }

    Color Sample(float u, float v)
    {
        return Sampler != null ? Sampler(u, v) : source.GetPixelBilinear(u, v);
    }

    /// <summary>Caminantes en las partes que todavia no se pintaron (para terminar la obra).</summary>
    public void SpawnWhereMissing(int count)
    {
        int cellPx = (iw / Cells) * (ih / Cells);
        for (int k = 0; k < count && wx.Count < MaxWalkers; k++)
        {
            // Una celda al azar entre las menos pintadas (unos intentos)
            int best = -1; float bestFill = 2f;
            for (int t = 0; t < 6; t++)
            {
                int c = Random.Range(0, Cells * Cells);
                float fill = cellPainted[c] / (float)Mathf.Max(1, cellPx);
                if (fill < bestFill) { bestFill = fill; best = c; }
            }
            if (best < 0 || bestFill > 0.9f) continue;
            float u = ((best % Cells) + Random.value) / Cells, v = ((best / Cells) + Random.value) / Cells;
            Spawn(new Vector2(u, v));
        }
    }

    /// <summary>Mueve los caminantes y pinta.</summary>
    public void Step()
    {
        for (int s = 0; s < StepsPerFrame; s++)
            for (int i = wx.Count - 1; i >= 0; i--)
            {
                // Aceleracion al azar, velocidad limitada (camina sin rumbo fijo)
                float fast = wk[i], max = Speed * Px * fast;
                float ax = Random.Range(-0.35f, 0.35f) * Px * fast, ay = Random.Range(-0.35f, 0.35f) * Px * fast;
                float nvx = vx[i] + ax, nvy = vy[i] + ay;
                float sp = Mathf.Sqrt(nvx * nvx + nvy * nvy);
                if (sp > max) { nvx *= max / sp; nvy *= max / sp; }
                float x = wx[i] + nvx, y = wy[i] + nvy;
                if (x < ix0 || x > ix0 + iw - 1) { nvx = -nvx; x = Mathf.Clamp(x, ix0, ix0 + iw - 1); }
                if (y < iy0 || y > iy0 + ih - 1) { nvy = -nvy; y = Mathf.Clamp(y, iy0, iy0 + ih - 1); }
                wx[i] = x; wy[i] = y; vx[i] = nvx; vy[i] = nvy;

                // El color de la pintura donde esta parado (con un poco de variacion, como pincel)
                Color c = Sample((x - ix0) / (iw - 1), (y - iy0) / (ih - 1));
                float j = 1f + Random.Range(-0.03f, 0.03f);
                Stamp(x, y, ws[i], new Color(c.r * j, c.g * j, c.b * j, 1f));

                ws[i] -= Shrink * Px * fast;
                if (ws[i] < 0.6f)
                {
                    int last = wx.Count - 1;
                    wx[i] = wx[last]; wy[i] = wy[last]; vx[i] = vx[last]; vy[i] = vy[last]; ws[i] = ws[last]; wc[i] = wc[last]; wk[i] = wk[last];
                    wx.RemoveAt(last); wy.RemoveAt(last); vx.RemoveAt(last); vy.RemoveAt(last); ws.RemoveAt(last); wc.RemoveAt(last); wk.RemoveAt(last);
                }
            }
        if (dirty)
        {
            tex.SetPixels32(px);
            tex.Apply(false);
            dirty = false;
        }
    }

    void Stamp(float cx, float cy, float r, Color col)
    {
        int x0 = Mathf.Max(ix0, Mathf.FloorToInt(cx - r)), x1 = Mathf.Min(ix0 + iw - 1, Mathf.CeilToInt(cx + r));
        int y0 = Mathf.Max(iy0, Mathf.FloorToInt(cy - r)), y1 = Mathf.Min(iy0 + ih - 1, Mathf.CeilToInt(cy + r));
        float r2 = r * r;
        int cellW = Mathf.Max(1, iw / Cells), cellH = Mathf.Max(1, ih / Cells);
        for (int y = y0; y <= y1; y++)
            for (int x = x0; x <= x1; x++)
            {
                float dx = x - cx, dy = y - cy, d2 = dx * dx + dy * dy;
                if (d2 > r2) continue;
                float k = 0.95f * (1f - d2 / r2 * 0.4f);
                int idx = y * cw + x;
                Color32 dst = px[idx];
                bool fresh = kind == Kind.Cloth ? dst.a == 255 : dst.a < 240;   // todavia sin pintar
                Color d = dst;
                Color o = Color.Lerp(d, col, kind == Kind.Rock && dst.a < 10 ? 1f : k);
                o.a = Mathf.Max(d.a, kind == Kind.Rock ? 0.95f : 1f);
                Color32 o32 = o;
                o32.a = kind == Kind.Cloth ? (byte)254 : (byte)Mathf.Max(dst.a, 242);   // 254/242 = marca de "pintado"
                if (fresh)
                {
                    painted++;
                    int cell = Mathf.Min(Cells - 1, (y - iy0) / cellH) * Cells + Mathf.Min(Cells - 1, (x - ix0) / cellW);
                    cellPainted[cell]++;
                }
                px[idx] = o32;
            }
        dirty = true;
    }

    /// <summary>Termina la obra: lo que quedo sin pintar se completa con la pintura, un poco transparente (se ve la trama).</summary>
    public void FinishFill()
    {
        for (int y = iy0; y < iy0 + ih; y++)
            for (int x = ix0; x < ix0 + iw; x++)
            {
                int idx = y * cw + x;
                bool done = kind == Kind.Cloth ? px[idx].a == 254 : px[idx].a >= 240;
                Color c = Sample((float)(x - ix0) / (iw - 1), (float)(y - iy0) / (ih - 1));
                Color d = px[idx];
                // Lo pintado se afina hacia la pintura (quedan las pinceladas pero con definicion); lo que falta, se completa
                Color o = done ? Color.Lerp(d, c, 0.55f) : Color.Lerp(kind == Kind.Rock ? c : d, c, 0.85f);
                o.a = kind == Kind.Rock ? (done ? d.a : 0.8f) : 1f;
                Color32 o32 = o;
                if (kind == Kind.Cloth) o32.a = 254;
                px[idx] = o32;
                if (!done) painted++;
            }
        tex.SetPixels32(px);
        tex.Apply(false);
    }

    // ---------------------------------------------------------------- soportes

    /// <summary>Arma la tela colgada (dos palos y un travesaño) o la roca. front = la cara que mira a la persona.</summary>
    public void BuildSupport(Transform parent, Vector3 center, Vector3 right, Vector3 up, Vector3 front, float w, float h, float floorY, Material mat)
    {
        support = new GameObject(kind == Kind.Cloth ? "Soporte_Tela" : "Soporte_Roca");
        support.transform.SetParent(parent, false);
        support.transform.position = Vector3.zero;
        var verts = new List<Vector3>(); var cols = new List<Color>(); var uvs = new List<Vector2>(); var tris = new List<int>();
        float top = center.y + h * 0.5f;
        if (kind == Kind.Cloth)
        {
            // Cuelga de la rama baja de un arbol que crece al costado: la rama sale del tronco, pasa por arriba
            // de la tela y se va afinando; dos sogas la atan
            Color bark = new Color(0.33f, 0.25f, 0.18f);
            float trunkX = w * 0.5f + 0.45f, branchY = top + 0.32f;
            Vector3 c0 = new Vector3(center.x, 0f, center.z) - front * 0.12f;
            TreeRoot = new Vector3(c0.x, floorY, c0.z) + right * trunkX;
            TreeHeight = Mathf.Max(5.5f, (branchY - floorY) * 2.3f);
            const int segs = 10;
            Vector3 prev = Vector3.zero;
            for (int k = 0; k <= segs; k++)
            {
                float t = (float)k / segs;
                float x = Mathf.Lerp(trunkX, -w * 0.5f - 0.35f, t);
                float y = branchY + 0.18f * (1f - t) * (1f - t) - 0.12f * Mathf.Sin(t * Mathf.PI) + 0.05f * Mathf.Sin(t * 9f);
                Vector3 pt = new Vector3(c0.x, y, c0.z) + right * x;
                if (k > 0)
                {
                    Vector3 mid = (prev + pt) * 0.5f, dir = pt - prev;
                    float r = Mathf.Lerp(0.09f, 0.025f, t);
                    Box(verts, cols, uvs, tris, mid, dir.normalized, Vector3.Cross(front, dir).normalized, front, new Vector3(dir.magnitude + r, r * 2f, r * 2f), bark * (0.85f + 0.15f * t));
                }
                prev = pt;
            }
            // Sogas: de la rama a las puntas de arriba de la tela
            for (int s = -1; s <= 1; s += 2)
            {
                float x = s * (w * 0.5f - 0.04f), t = Mathf.InverseLerp(trunkX, -w * 0.5f - 0.35f, x);
                float y = branchY + 0.18f * (1f - t) * (1f - t) - 0.12f * Mathf.Sin(t * Mathf.PI) + 0.05f * Mathf.Sin(t * 9f);
                Vector3 a = new Vector3(c0.x, y, c0.z) + right * x, b = new Vector3(center.x, top, center.z) + right * x;
                Box(verts, cols, uvs, tris, (a + b) * 0.5f, right, Vector3.up, front, new Vector3(0.015f, Mathf.Max(0.02f, a.y - b.y), 0.015f), new Color(0.55f, 0.47f, 0.35f));
            }
        }
        else
        {
            Rock(verts, cols, uvs, tris, center, right, up, front, w, h, floorY);
        }
        var mesh = new Mesh { name = support.name };
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(verts); mesh.SetColors(cols); mesh.SetUVs(0, uvs); mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
        supportMesh = mesh;
        supportBase = cols.ToArray();
        supportTinted = new Color[supportBase.Length];
        supportAt = center;
        support.AddComponent<MeshFilter>().sharedMesh = mesh;
        var mr = support.AddComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    /// <summary>Color del soporte segun la epoca y la niebla: color = add + base * mul.</summary>
    public void TintSupport(Color mul, Color add)
    {
        if (supportMesh == null) return;
        for (int k = 0; k < supportBase.Length; k++)
        {
            Color b = supportBase[k];
            supportTinted[k] = new Color(add.r + b.r * mul.r, add.g + b.g * mul.g, add.b + b.b * mul.b, 1f);
        }
        supportMesh.colors = supportTinted;
    }

    public void DestroySupport()
    {
        if (support != null) Object.Destroy(support);
        if (supportMesh != null) Object.Destroy(supportMesh);
        support = null; supportMesh = null;
    }

    static void Box(List<Vector3> v, List<Color> c, List<Vector2> uv, List<int> t, Vector3 center, Vector3 rx, Vector3 ry, Vector3 rz, Vector3 size, Color col)
    {
        Vector3 hx = rx * size.x * 0.5f, hy = ry * size.y * 0.5f, hz = rz * size.z * 0.5f;
        Vector3[] n = { rx, -rx, ry, -ry, rz, -rz };
        foreach (var f in n)
        {
            Vector3 a, b;
            if (f == rx || f == -rx) { a = hz; b = hy; }
            else if (f == ry || f == -ry) { a = hx; b = hz; }
            else { a = hx; b = hy; }
            Vector3 fc = center + Vector3.Scale(new Vector3(1, 1, 1), f == rx ? hx : f == -rx ? -hx : f == ry ? hy : f == -ry ? -hy : f == rz ? hz : -hz);
            float light = 0.7f + 0.3f * Vector3.Dot(f, new Vector3(-0.4f, 0.8f, -0.4f).normalized);
            Color cc = col * light; cc.a = 1f;
            int start = v.Count;
            v.Add(fc - a - b); v.Add(fc + a - b); v.Add(fc + a + b); v.Add(fc - a + b);
            for (int k = 0; k < 4; k++) c.Add(cc);
            uv.Add(new Vector2(0, 0)); uv.Add(new Vector2(0.3f, 0)); uv.Add(new Vector2(0.3f, size.y * 0.5f)); uv.Add(new Vector2(0, size.y * 0.5f));
            t.AddRange(new[] { start, start + 1, start + 2, start, start + 2, start + 3 });
            t.AddRange(new[] { start, start + 2, start + 1, start, start + 3, start + 2 });   // las dos caras (no importa el giro)
        }
    }

    // Roca: una esfera "cuadrada" (superelipsoide) deformada con ruido, con la cara de adelante lisa para pintar
    static void Rock(List<Vector3> v, List<Color> c, List<Vector2> uv, List<int> t, Vector3 center, Vector3 right, Vector3 up, Vector3 front, float w, float h, float floorY)
    {
        float top = center.y + h * 0.5f + 0.18f, bottom = floorY - 0.3f;
        float W = w * 1.18f + 0.25f, H = top - bottom, D = 0.75f;
        float flat = D * 0.3f;   // la cara lisa queda a esta distancia del centro, hacia la persona
        Vector3 mid = new Vector3(center.x, (top + bottom) * 0.5f, center.z) - front * (flat + 0.012f);
        int la = 18, lo = 28;
        float seed = Random.value * 50f;
        Color dark = new Color(0.22f, 0.19f, 0.16f), light = new Color(0.52f, 0.47f, 0.39f), green = new Color(0.30f, 0.31f, 0.20f);
        int start = v.Count;
        for (int i = 0; i <= la; i++)
            for (int j = 0; j <= lo; j++)
            {
                float th = Mathf.PI * i / la, ph = 2f * Mathf.PI * j / lo;
                Vector3 n = new Vector3(Mathf.Sin(th) * Mathf.Cos(ph), Mathf.Cos(th), Mathf.Sin(th) * Mathf.Sin(ph));
                Vector3 q = new Vector3(Sp(n.x), Sp(n.y), Sp(n.z));   // mas cuadrada
                // Bultos grandes y chicos (no un bloque liso); arriba se angosta
                float noise = 0.86f + 0.2f * Mathf.PerlinNoise(n.x * 1.6f + seed, n.y * 1.6f + n.z * 1.3f)
                            + 0.08f * Mathf.PerlinNoise(n.x * 5f + seed, n.y * 5f + n.z * 4f);
                q.x *= 1f - 0.18f * Mathf.Clamp01(n.y);
                float x = q.x * W * 0.5f * noise, y = q.y * H * 0.5f * noise, z = q.z * D * 0.5f * noise;
                if (z > flat) z = flat + 0.015f * Mathf.PerlinNoise(x * 3f + seed, y * 3f);   // cara lisa
                v.Add(mid + right * x + Vector3.up * y + front * z);
                float lit = 0.55f + 0.45f * Mathf.Clamp01(Vector3.Dot(n, new Vector3(-0.3f, 0.8f, 0.5f).normalized));
                float patch = Mathf.PerlinNoise(x * 1.3f + seed, y * 1.3f);
                Color cc = Color.Lerp(dark, light, patch) * lit;
                cc = Color.Lerp(cc, green, Mathf.Clamp01(n.y) * 0.4f);
                cc *= Mathf.Lerp(0.6f, 1f, Mathf.Clamp01((y + H * 0.5f) / 0.6f));   // mas oscura abajo, donde toca la tierra
                cc.a = 1f;
                c.Add(cc);
                uv.Add(new Vector2((x + z) * 0.35f, y * 0.35f));   // pinceladas sin estirar
            }
        for (int i = 0; i < la; i++)
            for (int j = 0; j < lo; j++)
            {
                int a = start + i * (lo + 1) + j, b = a + lo + 1;
                t.AddRange(new[] { a, a + 1, b, a + 1, b + 1, b });
            }
    }

    static float Sp(float x) { return Mathf.Sign(x) * Mathf.Pow(Mathf.Abs(x), 0.55f); }
}

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
    /// <summary>La rama del arbol de la que cuelga la tela: alto (mundo), hacia donde va y su largo.</summary>
    public float HangY;
    public Vector3 HangDir;
    public float HangLength;
    public Vector3 Front;
    /// <summary>La rama ya plantada (para que la tela siga su vaiven).</summary>
    public GoyaFractalTrees.Hang hang;
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
    readonly List<int> wt = new List<int>();      // tipo de caminante (ver Style)
    readonly List<float> wp = new List<float>();  // dato propio del tipo (el giro de la espiral)

    /// <summary>Como caminan los caminantes, y por eso como se arma la obra (como en el video de la chica de la perla,
    /// donde la misma pintura aparece de formas distintas):
    /// Contorno = pinceladas que siguen los bordes de la figura; Puntos = saltitos que dejan puntos (puntillismo);
    /// Remolino = siguen un viento que da vueltas (trazos curvos que fluyen); Rayado = trazos rectos en una sola
    /// direccion (como un grabado); Espiral = cada uno da vueltas en circulo; Mezcla = de todos los tipos a la vez;
    /// Azar = cada vez que la obra empieza de nuevo elige uno de los anteriores.</summary>
    public enum Style { Contorno, Puntos, Remolino, Rayado, Espiral, Mezcla, Azar }
    Style style = Style.Contorno;
    public Style WalkStyle
    {
        get { return style; }
        set { if (value != style) { style = value; Current = PickStyle(); } }
    }
    Style PickStyle() { return style == Style.Azar ? (Style)Random.Range(0, (int)Style.Azar) : style; }
    /// <summary>El tipo que le toco esta vez (si style es Azar).</summary>
    public Style Current { get; private set; }
    float hatchAngle, flowSeed;

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
        wx.Clear(); wy.Clear(); vx.Clear(); vy.Clear(); ws.Clear(); wc.Clear(); wk.Clear(); wt.Clear(); wp.Clear();
        Current = PickStyle();
        hatchAngle = Random.Range(0.3f, 1.2f) * (Random.value < 0.5f ? 1f : -1f);
        flowSeed = Random.Range(0f, 100f);
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
        int type = Current == Style.Mezcla ? Random.Range(0, (int)Style.Mezcla) : (int)Current;
        float size = Size * Px * sizeK * fine * Random.Range(0.6f, 1.2f);
        if (type == (int)Style.Puntos) size *= 0.75f;
        if (type == (int)Style.Rayado) size *= 0.55f;    // rayas finas
        ws.Add(size);
        wk.Add(fast);
        wc.Add(new Color32(0, 0, 0, 0));
        wt.Add(type);
        wp.Add(Random.Range(0.07f, 0.2f) * (Random.value < 0.5f ? 1f : -1f));
    }

    /// <summary>Caminantes nuevos dentro de una parte de la obra (u, v de 0 a 1): donde se mueve una obra animada.</summary>
    public void SpawnIn(Rect area, int count, float sizeK, float fast)
    {
        for (int k = 0; k < count && wx.Count < MaxWalkers; k++)
            Spawn(new Vector2(Mathf.Clamp01(area.x + Random.value * area.width), Mathf.Clamp01(area.y + Random.value * area.height)), sizeK, fast);
    }

    static float Lum(Color c) { return 0.3f * c.r + 0.59f * c.g + 0.11f * c.b; }

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
            for (int t = 0; t < 10; t++)
            {
                int c = Random.Range(0, Cells * Cells);
                float fill = cellPainted[c] / (float)Mathf.Max(1, cellPx);
                if (fill < bestFill) { bestFill = fill; best = c; }
            }
            if (best < 0 || bestFill > 0.97f) continue;
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
                float fast = wk[i], max = Speed * Px * fast;
                float ax = Random.Range(-0.35f, 0.35f) * Px * fast, ay = Random.Range(-0.35f, 0.35f) * Px * fast;
                bool stroke = fast <= 1.01f;
                int type = wt[i];
                if (stroke)
                {
                    switch ((Style)type)
                    {
                        case Style.Puntos:
                            // Saltitos al azar: cada paso deja un punto separado del anterior
                            float ja = Random.value * Mathf.PI * 2f, jump = ws[i] * 1.6f;
                            ax = Mathf.Cos(ja) * jump - vx[i]; ay = Mathf.Sin(ja) * jump - vy[i];
                            break;
                        case Style.Remolino:
                        {
                            // Un viento que da vueltas por la tela (ruido de Perlin): trazos curvos que fluyen
                            float fa = Mathf.PerlinNoise((wx[i] - ix0) * 0.006f / Px + flowSeed, (wy[i] - iy0) * 0.006f / Px) * Mathf.PI * 4f;
                            ax = (Mathf.Cos(fa) * max - vx[i]) * 0.3f + ax * 0.2f;
                            ay = (Mathf.Sin(fa) * max - vy[i]) * 0.3f + ay * 0.2f;
                            break;
                        }
                        case Style.Rayado:
                        {
                            // Todos en la misma direccion (ida o vuelta), casi sin desviarse: rayas como de grabado
                            float hx = Mathf.Cos(hatchAngle), hy = Mathf.Sin(hatchAngle);
                            if (hx * vx[i] + hy * vy[i] < 0f) { hx = -hx; hy = -hy; }
                            ax = (hx * max - vx[i]) * 0.5f + ax * 0.1f;
                            ay = (hy * max - vy[i]) * 0.5f + ay * 0.1f;
                            break;
                        }
                        case Style.Espiral:
                        {
                            // Gira siempre para el mismo lado: dibuja rulos
                            float t = wp[i], cs = Mathf.Cos(t), sn = Mathf.Sin(t);
                            float rx = vx[i] * cs - vy[i] * sn, ry = vx[i] * sn + vy[i] * cs;
                            float rl = Mathf.Max(0.001f, Mathf.Sqrt(rx * rx + ry * ry));
                            ax = rx / rl * max - vx[i]; ay = ry / rl * max - vy[i];
                            break;
                        }
                        default:
                        {
                            // Pincelada: sigue los contornos de la pintura (va a lo largo de los bordes, no los cruza),
                            // con un poco de azar. Asi deja trazos largos, como un pincel, y no manchas redondas.
                            float u0 = (wx[i] - ix0) / (iw - 1), v0 = (wy[i] - iy0) / (ih - 1), e = 2.5f / iw, f = 2.5f / ih;
                            float gx = Lum(Sample(u0 + e, v0)) - Lum(Sample(u0 - e, v0));
                            float gy = Lum(Sample(u0, v0 + f)) - Lum(Sample(u0, v0 - f));
                            float g = Mathf.Sqrt(gx * gx + gy * gy);
                            if (g > 0.015f)
                            {
                                float tx = -gy / g, ty = gx / g;
                                if (tx * vx[i] + ty * vy[i] < 0f) { tx = -tx; ty = -ty; }   // sigue para el mismo lado
                                ax = (tx * max - vx[i]) * 0.25f + ax * 0.5f;
                                ay = (ty * max - vy[i]) * 0.25f + ay * 0.5f;
                            }
                            else { ax *= 0.4f; ay *= 0.4f; }   // zona pareja: sigue derecho
                            break;
                        }
                    }
                }
                float nvx = vx[i] + ax, nvy = vy[i] + ay;
                float sp = Mathf.Sqrt(nvx * nvx + nvy * nvy);
                float cap = type == (int)Style.Puntos && stroke ? ws[i] * 1.6f : max;
                if (sp > cap) { nvx *= cap / sp; nvy *= cap / sp; }
                float x = wx[i] + nvx, y = wy[i] + nvy;
                if (x < ix0 || x > ix0 + iw - 1) { nvx = -nvx; x = Mathf.Clamp(x, ix0, ix0 + iw - 1); }
                if (y < iy0 || y > iy0 + ih - 1) { nvy = -nvy; y = Mathf.Clamp(y, iy0, iy0 + ih - 1); }
                wx[i] = x; wy[i] = y; vx[i] = nvx; vy[i] = nvy;

                // El color de la pintura donde esta parado. La pincelada lleva su pintura y la va mezclando de a poco;
                // si cruza a un color muy distinto (un borde de la figura), el trazo termina ahi
                Color c = Sample((x - ix0) / (iw - 1), (y - iy0) / (ih - 1));
                bool cut = false;
                if (stroke && type != (int)Style.Puntos)
                {
                    Color32 carried = wc[i];
                    if (carried.a == 0) carried = new Color32((byte)(c.r * 255f), (byte)(c.g * 255f), (byte)(c.b * 255f), 255);
                    Color cc = carried;
                    float diff = Mathf.Abs(cc.r - c.r) + Mathf.Abs(cc.g - c.g) + Mathf.Abs(cc.b - c.b);
                    cut = diff > 0.5f;
                    cc = Color.Lerp(cc, c, 0.5f);
                    wc[i] = cc;
                    c = cc;
                }
                float j = 1f + (type == (int)Style.Puntos ? Random.Range(-0.08f, 0.08f) : Random.Range(-0.03f, 0.03f));
                if (!cut) Stamp(x, y, ws[i], new Color(c.r * j, c.g * j, c.b * j, 1f));

                float life = !stroke ? 1f : type == (int)Style.Puntos ? 2.5f : type == (int)Style.Rayado ? 0.25f : 0.6f;
                ws[i] -= Shrink * Px * fast * life;
                if (ws[i] < 0.6f || cut)
                {
                    int last = wx.Count - 1;
                    wx[i] = wx[last]; wy[i] = wy[last]; vx[i] = vx[last]; vy[i] = vy[last]; ws[i] = ws[last]; wc[i] = wc[last]; wk[i] = wk[last]; wt[i] = wt[last]; wp[i] = wp[last];
                    wx.RemoveAt(last); wy.RemoveAt(last); vx.RemoveAt(last); vy.RemoveAt(last); ws.RemoveAt(last); wc.RemoveAt(last); wk.RemoveAt(last); wt.RemoveAt(last); wp.RemoveAt(last);
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
            // Cuelga de la rama baja de un arbol que crece al costado. La rama es parte del arbol (GoyaFractalTrees):
            // sale del tronco, pasa justo detras del borde de arriba de la tela y se mece con el arbol; la tela la sigue
            float trunkX = w * 0.5f + 0.4f;
            Vector3 c0 = new Vector3(center.x, 0f, center.z) - front * 0.07f;
            TreeRoot = new Vector3(c0.x, floorY, c0.z) + right * trunkX;
            HangY = top + 0.04f;
            HangDir = -right;
            HangLength = trunkX + w * 0.5f + 0.3f;
            Front = front;
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

    public static void Box(List<Vector3> v, List<Color> c, List<Vector2> uv, List<int> t, Vector3 center, Vector3 rx, Vector3 ry, Vector3 rz, Vector3 size, Color col)
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

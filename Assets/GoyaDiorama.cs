using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pinturas negras "por dentro": al final del camino, pinturas separadas en capas (fondo y figuras)
/// puestas a distintas distancias, como un escenario de teatro. Desde la entrada se ve igual que la pintura,
/// pero al caminar las capas se separan y se puede pasar entre ellas, hasta quedar al lado de las figuras.
/// Una detras de otra: Saturno, El gran cabron, Las Parcas y El perro. Al atravesar el fondo de una
/// se entra en la siguiente. Las capas se generan con tools/paisaje/dioramas.py (Saturno aparte).
///
/// Las capas estan en Assets/Resources/Dioramas (fondo en .jpg, figuras en .png con transparencia),
/// todas del mismo tamaño que la pintura original, asi coinciden.
/// Con volume prendido, las figuras no son planas: cada una se infla como una escultura (gruesa en el medio,
/// fina en los bordes, con frente y espalda) con la pintura encima, y el fondo es un muro curvo con relieve.
/// Desde la entrada se sigue viendo igual que la pintura. Las formas se arman solas al empezar (shader Goya/Volumen).
/// GoyaGallery lo agrega solo en modo camino.
/// </summary>
public class GoyaDiorama : MonoBehaviour
{
    [System.Serializable]
    public class Layer
    {
        [Tooltip("Imagen dentro de Assets/Resources, sin la extension (ej: Dioramas/saturno_figura)")]
        public string image;
        [Tooltip("Distancia desde la entrada del diorama (metros)")]
        public float distance;
        [Tooltip("Que parte del alto de la imagen es la pintura (el fondo se extiende mas alla del cuadro para llenar el ambiente)")]
        public float frame = 1f;
        [Tooltip("Cuanto respira (se agranda y achica apenas), para que la figura parezca viva")]
        public float breathe;
    }

    [System.Serializable]
    public class Diorama
    {
        public string name;
        [Tooltip("Nombre de la obra de la lista que reemplaza (para que no aparezca tambien plana en el camino)")]
        public string replaces;
        [Tooltip("Alto que parece tener la pintura vista desde la entrada, si estuviera a 10 m")]
        public float height = 7f;
        [Tooltip("Cuanto mas arriba o abajo de los ojos queda el centro de la pintura (a 10 m)")]
        public float centerOffset = 0.5f;
        public Layer[] layers;
    }

    public Diorama[] dioramas =
    {
        // Saturno primero, apenas se sale de la cueva (es lo que se ve desde la salida)
        new Diorama
        {
            name = "Saturno",
            replaces = "28 Saturno devorando a su hijo",
            height = 8f,
            centerOffset = 1.2f,
            layers = new[]
            {
                new Layer { image = "Dioramas/saturno_fondo", distance = 16f, frame = 0.4168f },
                // La figura en tres planos: el cuerpo atras (mas oscuro), la cabeza y el hombro en el medio,
                // y el hijo con las manos adelante. Desde la entrada se ve como la pintura entera.
                new Layer { image = "Dioramas/saturno_figura_atras", distance = 12f, frame = 1f, breathe = 0.008f },
                new Layer { image = "Dioramas/saturno_figura_medio", distance = 9f, frame = 1f, breathe = 0.012f },
                new Layer { image = "Dioramas/saturno_figura_cerca", distance = 6f, frame = 1f, breathe = 0.018f },
            }
        },
        // El aquelarre: la multitud atras, adelante el macho cabrio, la muchacha de blanco y los dos grupos de los costados
        new Diorama
        {
            name = "Gran cabron",
            height = 4.2f,
            centerOffset = -0.1f,
            layers = new[]
            {
                new Layer { image = "Dioramas/cabron_fondo", distance = 16f, frame = 0.385f },
                new Layer { image = "Dioramas/cabron_derecha", distance = 12.5f, frame = 1f, breathe = 0.006f },
                new Layer { image = "Dioramas/cabron_izquierda", distance = 11.5f, frame = 1f, breathe = 0.008f },
                new Layer { image = "Dioramas/cabron_macho", distance = 8.5f, frame = 1f, breathe = 0.012f },
                new Layer { image = "Dioramas/cabron_blanca", distance = 5.5f, frame = 1f, breathe = 0.01f },
            }
        },
        // Las Parcas: el rio atras, las figuras que flotan en dos grupos y los arbustos oscuros adelante
        new Diorama
        {
            name = "Las Parcas",
            height = 5f,
            centerOffset = 0.8f,
            layers = new[]
            {
                new Layer { image = "Dioramas/parcas_fondo", distance = 16f, frame = 0.3848f },
                new Layer { image = "Dioramas/parcas_izquierda", distance = 11f, frame = 1f, breathe = 0.012f },
                new Layer { image = "Dioramas/parcas_derecha", distance = 8.5f, frame = 1f, breathe = 0.015f },
                new Layer { image = "Dioramas/parcas_arbustos", distance = 5f, frame = 1f },
            }
        },
        // El perro: el campo vacio atras, el perro en el medio y la ladera adelante (el perro asoma detras)
        new Diorama
        {
            name = "El perro",
            replaces = "27 El perro",
            height = 8f,
            centerOffset = 1.8f,
            layers = new[]
            {
                new Layer { image = "Dioramas/perro_fondo", distance = 16f, frame = 0.4547f },
                new Layer { image = "Dioramas/perro_perro", distance = 10f, frame = 1f, breathe = 0.01f },
                new Layer { image = "Dioramas/perro_ladera", distance = 7f, frame = 1f },
            }
        },
    };

    [Tooltip("Distancia entre la entrada de un diorama y el siguiente (metros)")]
    public float spacing = 20f;
    [Tooltip("Metros antes del final del camino donde empieza a aparecer (antes no se ve, para que sea sorpresa)")]
    public float revealBefore = 4f;
    [Tooltip("Metros que tarda en aparecer del todo")]
    public float revealLength = 5f;
    [Range(0f, 1f), Tooltip("Cuanto le afecta la niebla del ambiente")]
    public float fog = 0.6f;
    [Tooltip("Lo mas cerca que se puede llegar a la ultima capa (metros)")]
    public float minDistanceToLast = 0.4f;
    [Tooltip("Al atravesar una capa (ej: la figura de Saturno) se desvanece a esta distancia, para poder pasar por dentro sin que te tape")]
    public float passFade = 2.5f;

    [Header("Volumen")]
    [Tooltip("Figuras con volumen (infladas como esculturas) y fondo curvo con relieve; apagado = capas planas como antes")]
    public bool volume = true;
    [Tooltip("Cuanto se inflan las figuras (1 = casi tan gruesas como anchas)")]
    public float inflate = 0.6f;
    [Tooltip("Relieve de las pinceladas en las figuras (lo claro sale, lo oscuro se hunde)")]
    public float figureDetail = 1.5f;
    [Tooltip("Cuanto de grosor tiene la espalda de las figuras, en proporcion al frente")]
    [Range(0f, 1f)] public float backDepth = 0.6f;
    [Tooltip("Puntos a lo largo de cada figura (mas = mas suave, pero mas pesado para el celu)")]
    public int figureResolution = 110;
    [Tooltip("Puntos a lo ancho del fondo")]
    public int backgroundResolution = 80;
    [Tooltip("Cuanto se curva el fondo hacia adelante en los costados (te envuelve)")]
    public float backgroundBend = 0.25f;
    [Tooltip("Relieve de las pinceladas del fondo, en proporcion a su lado mas corto")]
    public float backgroundRelief = 0.015f;

    [Header("Mapas de normales y rugosidad")]
    [Tooltip("Cada capa lleva un mapa de normales (el relieve fino de cada pincelada, que la luz marca aunque la forma no lo tenga) "
           + "y uno de rugosidad (donde el oleo brilla y donde es mate). Se sacan de la misma pintura al empezar.")]
    public bool detailMaps = true;
    [Tooltip("Cuanto relieve dan las pinceladas (mapa de normales)")]
    public float normalStrength = 1f;
    [Tooltip("Cuanto marca la luz ese relieve (0 = nada)")]
    public float detailLight = 0.9f;
    [Tooltip("Cuanto brilla el oleo donde la pintura es lisa (mapa de rugosidad; 0 = todo mate)")]
    public float shine = 0.35f;
    [Tooltip("Lado mas largo de los mapas (pixeles). Mas = mas detalle, pero tarda mas al empezar")]
    public int detailResolution = 1024;

    class Built
    {
        public SpriteRenderer renderer;
        public Vector3 baseScale;
        public float breathe, phase;
        public bool fadesWhenPassed;
        public MeshRenderer mesh;     // el volumen (si hay)
        public Material material;
    }
    readonly List<Built> built = new List<Built>();
    GoyaGallery gallery;
    GoyaEnvironment environment;

    /// <summary>La obra tiene diorama (GoyaGallery la saca del camino, para que no aparezca dos veces).</summary>
    public bool Replaces(Sprite sprite)
    {
        if (sprite == null) return false;
        foreach (var d in dioramas) if (!string.IsNullOrEmpty(d.replaces) && sprite.name.StartsWith(d.replaces)) return true;
        return false;
    }

    /// <summary>Cuantas pinturas por dentro hay (para el contador del juego).</summary>
    public int Count { get { return dioramas.Length; } }

    /// <summary>A cuantas ya se entro (pasando unos metros de su entrada).</summary>
    public int Reached(GoyaGallery g)
    {
        float z = g.RigPosition.z - g.Center.z - g.PathLength;
        int n = 0;
        for (int k = 0; k < dioramas.Length; k++) if (z > k * spacing + 3f) n++;
        return n;
    }

    /// <summary>Metros que ocupan los dioramas despues del final del camino.</summary>
    public float Length
    {
        get
        {
            float last = 0f;
            for (int k = 0; k < dioramas.Length; k++)
                foreach (var l in dioramas[k].layers) last = Mathf.Max(last, k * spacing + l.distance);
            return last;
        }
    }

    public void Build(GoyaGallery g)
    {
        gallery = g;
        environment = GetComponent<GoyaEnvironment>();
        var volumeShader = volume ? Resources.Load<Shader>("Shaders/GoyaVolumen") : null;
        if (volume && volumeShader == null) Debug.LogWarning("[Diorama] Falta el shader Goya/Volumen: las capas quedan planas");
        Vector3 c = gallery.Center;
        float eyeY = gallery.RigPosition.y;

        for (int k = 0; k < dioramas.Length; k++)
        {
            var d = dioramas[k];
            float entryZ = c.z + gallery.PathLength + k * spacing;
            int order = -15 - k * 10;
            // Las capas lejanas primero
            var layers = new List<Layer>(d.layers);
            float maxDist = 0f;
            foreach (var ly in d.layers) maxDist = Mathf.Max(maxDist, ly.distance);
            layers.Sort((a, b) => b.distance.CompareTo(a.distance));
            foreach (var l in layers)
            {
                var sprite = Resources.Load<Sprite>(l.image);
                if (sprite == null)
                {
                    Debug.LogWarning("[Diorama] No encuentro la imagen " + l.image + " en Assets/Resources");
                    continue;
                }
                var go = new GameObject("Diorama_" + d.name + "_" + l.image);
                go.transform.SetParent(transform, false);
                // Mas lejos = mas grande, en la misma proporcion: desde la entrada todas las capas coinciden
                float k10 = l.distance / 10f;
                go.transform.position = new Vector3(c.x, eyeY + d.centerOffset * k10, entryZ + l.distance);
                float s = d.height * k10 / (sprite.bounds.size.y * Mathf.Clamp(l.frame, 0.05f, 1f));
                go.transform.localScale = Vector3.one * s;

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = order++;
                // Todas las capas se pueden atravesar, menos el fondo del ultimo diorama
                bool isFarthest = l.distance >= maxDist - 0.001f;
                bool last = k == dioramas.Length - 1;
                var b = new Built { renderer = sr, baseScale = go.transform.localScale, breathe = l.breathe, phase = Random.value * 6f, fadesWhenPassed = !(isFarthest && last) };

                // Volumen: la figura inflada (o el fondo curvo) reemplaza al dibujo plano
                if (volumeShader != null)
                {
                    bool background = l.frame < 0.999f || l.image.EndsWith("_fondo");
                    var mesh = background ? BackgroundMesh(sprite) : FigureMesh(sprite);
                    if (mesh != null)
                    {
                        var vg = new GameObject("Volumen");
                        vg.transform.SetParent(go.transform, false);
                        vg.AddComponent<MeshFilter>().sharedMesh = mesh;
                        b.mesh = vg.AddComponent<MeshRenderer>();
                        b.mesh.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        b.mesh.receiveShadows = false;
                        b.material = new Material(volumeShader) { mainTexture = sprite.texture };
                        if (detailMaps)
                        {
                            b.material.SetTexture("_DetailMap", DetailMap(sprite.texture, detailResolution));
                            b.material.SetFloat("_Bump", normalStrength);
                            b.material.SetFloat("_DetailLight", detailLight);
                            b.material.SetFloat("_Shine", shine);
                        }
                        else { b.material.SetFloat("_DetailLight", 0f); b.material.SetFloat("_Shine", 0f); }
                        b.mesh.sharedMaterial = b.material;
                        sr.forceRenderingOff = true;   // el plano sigue (tamaño, respirar), pero no se dibuja
                    }
                }
                built.Add(b);
            }
        }
    }

    void LateUpdate()
    {
        if (gallery == null) return;
        // No se ve desde el principio: aparece de la oscuridad recien al llegar al final del camino
        float z = gallery.RigPosition.z - gallery.Center.z;
        float reveal = Mathf.Clamp01((z - (gallery.PathLength - revealBefore)) / Mathf.Max(0.01f, revealLength));
        reveal = reveal * reveal * (3f - 2f * reveal);
        float appear = gallery.appear * reveal;
        foreach (var b in built)
        {
            if (b.breathe > 0f)
                b.renderer.transform.localScale = b.baseScale * (1f + b.breathe * Mathf.Sin(Time.time * 0.8f + b.phase));
            // Niebla mas suave que en las obras: el fondo es enorme y tiene que leerse desde la entrada
            Color col = environment != null ? Color.Lerp(Color.white, environment.Fogged(Color.white, b.renderer.transform.position), fog) : Color.white;
            col.a = appear;
            if (b.fadesWhenPassed)
            {
                // Al acercarte a la capa se vuelve transparente: la atravesas y entras en la pintura
                float dz = Mathf.Abs(b.renderer.transform.position.z - gallery.RigPosition.z);
                col.a *= Mathf.SmoothStep(0f, 1f, dz / Mathf.Max(0.01f, passFade));
            }
            b.renderer.color = col;
            b.renderer.enabled = appear > 0.001f;
            if (b.material != null)
            {
                b.material.color = col;
                b.material.SetFloat("_ZWrite", col.a > 0.98f ? 1f : 0f);   // al desvanecerse no tapa lo de atras
                b.mesh.enabled = col.a > 0.002f;
            }
        }
    }

    // ---------------------------------------------------------------- volumen

    // Una figura (imagen con transparencia) inflada como una escultura: lo mas grueso donde esta mas lejos del borde
    Mesh FigureMesh(Sprite s)
    {
        var tex = s.texture;
        Rect tr = s.textureRect;
        Bounds bounds = s.bounds;

        // Donde esta la figura dentro de la imagen (las capas tienen el tamaño de toda la pintura)
        int sw = 256, sh = Mathf.Clamp(Mathf.RoundToInt(256f * tr.height / tr.width), 16, 1024);
        var small = Read(tex, tr, sw, sh);
        int x0 = sw, y0 = sh, x1 = -1, y1 = -1;
        for (int y = 0; y < sh; y++)
            for (int x = 0; x < sw; x++)
                if (small[y * sw + x].a > 0.3f) { x0 = Mathf.Min(x0, x); x1 = Mathf.Max(x1, x); y0 = Mathf.Min(y0, y); y1 = Mathf.Max(y1, y); }
        if (x1 < 0) return null;
        x0 = Mathf.Max(0, x0 - 2); y0 = Mathf.Max(0, y0 - 2); x1 = Mathf.Min(sw - 1, x1 + 2); y1 = Mathf.Min(sh - 1, y1 + 2);
        var sub = new Rect(tr.x + tr.width * x0 / sw, tr.y + tr.height * y0 / sh,
                           tr.width * (x1 - x0 + 1) / sw, tr.height * (y1 - y0 + 1) / sh);

        // Grilla sobre la figura
        int res = Mathf.Clamp(figureResolution, 24, 256);
        float aspect = sub.width / sub.height;
        int gw = aspect >= 1f ? res : Mathf.Max(8, Mathf.RoundToInt(res * aspect));
        int gh = aspect >= 1f ? Mathf.Max(8, Mathf.RoundToInt(res / aspect)) : res;
        var px = Read(tex, sub, gw, gh);

        var mask = new float[gw, gh];
        for (int y = 0; y < gh; y++) for (int x = 0; x < gw; x++) mask[x, y] = px[y * gw + x].a > 0.5f ? 1f : 0f;
        var dist = GoyaOrigins.Chamfer(mask, gw, gh);
        float maxD = 1f;
        for (int y = 0; y < gh; y++) for (int x = 0; x < gw; x++) maxD = Mathf.Max(maxD, dist[x, y]);

        // Alto en celdas: crece con la distancia al borde (redondeado), mas el relieve de las pinceladas
        var hgt = new float[gw, gh];
        for (int y = 0; y < gh; y++)
            for (int x = 0; x < gw; x++)
            {
                float d = dist[x, y];
                if (d <= 0f) continue;
                float h = inflate * Mathf.Sqrt(d * Mathf.Min(d * 3f, maxD));
                h += figureDetail * (px[y * gw + x].grayscale - 0.5f) * Mathf.Clamp01(d / 3f);
                hgt[x, y] = Mathf.Max(0f, h);
            }
        hgt = Blur(Blur(hgt, gw, gh), gw, gh);

        // Celdas que se dibujan: la figura y un borde de una celda (el contorno exacto lo da la transparencia)
        var cover = new bool[gw, gh];
        for (int y = 0; y < gh; y++)
            for (int x = 0; x < gw; x++)
                for (int dy = -1; dy <= 1 && !cover[x, y]; dy++)
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        int xx = x + dx, yy = y + dy;
                        if (xx >= 0 && yy >= 0 && xx < gw && yy < gh && mask[xx, yy] > 0f) { cover[x, y] = true; break; }
                    }

        // De pixeles de la imagen a unidades del sprite
        float ux = bounds.size.x / tr.width, uy = bounds.size.y / tr.height;
        float cw = sub.width / (gw - 1) * ux, ch = sub.height / (gh - 1) * uy;
        float cell = (cw + ch) * 0.5f;
        Vector3 light = new Vector3(-0.5f, 0.6f, -0.6f).normalized;   // de arriba a la izquierda, de adelante

        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var cols = new List<Color>();
        var tris = new List<int>();
        int stride = gw * gh;
        for (int pass = 0; pass < 2; pass++)   // 0 = frente (hacia la persona), 1 = espalda
            for (int y = 0; y < gh; y++)
                for (int x = 0; x < gw; x++)
                {
                    float pxX = sub.x + sub.width * x / (gw - 1), pxY = sub.y + sub.height * y / (gh - 1);
                    float h = hgt[x, y] * cell;
                    float lx = bounds.min.x + (pxX - tr.x) * ux, ly = bounds.min.y + (pxY - tr.y) * uy;
                    verts.Add(new Vector3(lx, ly, pass == 0 ? -h : h * backDepth));
                    uvs.Add(new Vector2(pxX / tex.width, pxY / tex.height));
                    float hx = (hgt[Mathf.Min(gw - 1, x + 1), y] - hgt[Mathf.Max(0, x - 1), y]) * 0.5f;
                    float hy = (hgt[x, Mathf.Min(gh - 1, y + 1)] - hgt[x, Mathf.Max(0, y - 1)]) * 0.5f;
                    Vector3 n = pass == 0 ? new Vector3(-hx, -hy, -1f).normalized : new Vector3(-hx, -hy, 1f).normalized;
                    float shade = Mathf.Clamp(0.45f + Vector3.Dot(n, light), 0.5f, 1.2f);
                    if (pass == 1) shade *= 0.65f;   // la espalda, en sombra
                    cols.Add(new Color(shade, shade, shade, 1f));
                }
        for (int pass = 0; pass < 2; pass++)
            for (int y = 0; y < gh - 1; y++)
                for (int x = 0; x < gw - 1; x++)
                {
                    int n = (cover[x, y] ? 1 : 0) + (cover[x + 1, y] ? 1 : 0) + (cover[x, y + 1] ? 1 : 0) + (cover[x + 1, y + 1] ? 1 : 0);
                    if (n < 3) continue;
                    int i0 = pass * stride + y * gw + x, i1 = i0 + 1, i2 = i0 + gw, i3 = i2 + 1;
                    if (pass == 0) tris.AddRange(new[] { i0, i2, i1, i1, i2, i3 });
                    else tris.AddRange(new[] { i0, i1, i2, i1, i3, i2 });
                }
        return MakeMesh("Volumen_" + s.name, verts, uvs, cols, tris);
    }

    // El fondo: un muro que se curva hacia adelante en los costados, con el relieve de sus pinceladas
    Mesh BackgroundMesh(Sprite s)
    {
        var tex = s.texture;
        Rect tr = s.textureRect;
        Bounds bounds = s.bounds;
        int gw = Mathf.Clamp(backgroundResolution, 16, 200);
        int gh = Mathf.Clamp(Mathf.RoundToInt(gw * tr.height / tr.width), 8, 256);
        var px = Read(tex, tr, gw, gh);
        var lum = new float[gw, gh];
        for (int y = 0; y < gh; y++) for (int x = 0; x < gw; x++) lum[x, y] = px[y * gw + x].grayscale;
        lum = Blur(Blur(lum, gw, gh), gw, gh);

        float minSide = Mathf.Min(bounds.size.x, bounds.size.y);
        var z = new float[gw, gh];
        for (int y = 0; y < gh; y++)
            for (int x = 0; x < gw; x++)
            {
                float u = (float)x / (gw - 1) - 0.5f;
                z[x, y] = -backgroundBend * bounds.size.x * u * u - backgroundRelief * minSide * (lum[x, y] - 0.5f) * 2f;
            }

        float cw = bounds.size.x / (gw - 1), ch = bounds.size.y / (gh - 1);
        Vector3 light = new Vector3(-0.5f, 0.6f, -0.6f).normalized;
        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var cols = new List<Color>();
        var tris = new List<int>();
        for (int y = 0; y < gh; y++)
            for (int x = 0; x < gw; x++)
            {
                float u = (float)x / (gw - 1), v = (float)y / (gh - 1);
                verts.Add(new Vector3(bounds.min.x + u * bounds.size.x, bounds.min.y + v * bounds.size.y, z[x, y]));
                uvs.Add(new Vector2((tr.x + u * tr.width) / tex.width, (tr.y + v * tr.height) / tex.height));
                float hx = (z[Mathf.Min(gw - 1, x + 1), y] - z[Mathf.Max(0, x - 1), y]) / (2f * cw);
                float hy = (z[x, Mathf.Min(gh - 1, y + 1)] - z[x, Mathf.Max(0, y - 1)]) / (2f * ch);
                Vector3 n = new Vector3(hx, hy, -1f).normalized;
                float shade = Mathf.Clamp(0.55f + 0.6f * Vector3.Dot(n, light), 0.7f, 1.1f);
                cols.Add(new Color(shade, shade, shade, 1f));
            }
        for (int y = 0; y < gh - 1; y++)
            for (int x = 0; x < gw - 1; x++)
            {
                int i0 = y * gw + x, i1 = i0 + 1, i2 = i0 + gw, i3 = i2 + 1;
                tris.AddRange(new[] { i0, i2, i1, i1, i2, i3 });
            }
        return MakeMesh("Fondo_" + s.name, verts, uvs, cols, tris);
    }

    static Mesh MakeMesh(string name, List<Vector3> verts, List<Vector2> uvs, List<Color> cols, List<int> tris)
    {
        if (tris.Count == 0) return null;
        var m = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32, name = name };
        m.SetVertices(verts);
        m.SetUVs(0, uvs);
        m.SetColors(cols);
        m.SetTriangles(tris, 0);
        m.RecalculateNormals();   // para la luz de los mapas de normales y rugosidad
        m.RecalculateBounds();
        return m;
    }

    /// <summary>Mapa de normales y rugosidad de una pintura, en una sola textura:
    /// rojo y verde = hacia donde se inclina la superficie en cada punto (el relieve de la pincelada: lo claro sale,
    /// lo oscuro se hunde, como el oleo cargado); azul = hacia afuera; alfa = rugosidad (0 = lisa y brillante, 1 = mate).
    /// Las pinceladas marcadas quedan asperas; las partes lisas y oscuras brillan un poco, como oleo humedo.</summary>
    static Texture2D DetailMap(Texture2D tex, int maxSide)
    {
        float k = Mathf.Min(1f, maxSide / (float)Mathf.Max(tex.width, tex.height));
        int w = Mathf.Max(8, Mathf.RoundToInt(tex.width * k)), h = Mathf.Max(8, Mathf.RoundToInt(tex.height * k));
        var px = Read(tex, new Rect(0, 0, tex.width, tex.height), w, h);
        var lum = new float[w, h];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                Color c = px[y * w + x];
                lum[x, y] = 0.3f * c.r + 0.59f * c.g + 0.11f * c.b;
            }
        var hgt = Blur(lum, w, h);
        var smooth = Blur(Blur(hgt, w, h), w, h);
        var o = new Color[w * h];
        float bump = 6f * Mathf.Max(w, h) / 1024f;   // misma inclinacion sea cual sea el tamaño del mapa
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float dx = (hgt[Mathf.Min(w - 1, x + 1), y] - hgt[Mathf.Max(0, x - 1), y]) * 0.5f * bump;
                float dy = (hgt[x, Mathf.Min(h - 1, y + 1)] - hgt[x, Mathf.Max(0, y - 1)]) * 0.5f * bump;
                Vector3 n = new Vector3(-dx, -dy, 1f).normalized;
                float detail = Mathf.Abs(hgt[x, y] - smooth[x, y]) * 12f + Mathf.Sqrt(dx * dx + dy * dy) * 1.5f;
                float rough = Mathf.Clamp01(0.4f + detail - (1f - lum[x, y]) * 0.2f);
                o[y * w + x] = new Color(n.x * 0.5f + 0.5f, n.y * 0.5f + 0.5f, n.z, rough);
            }
        var map = new Texture2D(w, h, TextureFormat.RGBA32, true, true) { name = "Normales_" + tex.name, wrapMode = TextureWrapMode.Clamp };
        map.SetPixels(o);
        map.Apply(true, true);
        return map;
    }

    // Suaviza (promedio con los vecinos)
    static float[,] Blur(float[,] a, int w, int h)
    {
        var o = new float[w, h];
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                float sum = 0f; int n = 0;
                for (int dy = -1; dy <= 1; dy++)
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        int xx = x + dx, yy = y + dy;
                        if (xx < 0 || yy < 0 || xx >= w || yy >= h) continue;
                        sum += a[xx, yy]; n++;
                    }
                o[x, y] = sum / n;
            }
        return o;
    }

    // Copia chica de una parte de la imagen, para leer sus colores (sin tener que tildar Read/Write)
    static Color[] Read(Texture2D tex, Rect r, int w, int h)
    {
        var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(tex, rt, new Vector2(r.width / tex.width, r.height / tex.height), new Vector2(r.x / tex.width, r.y / tex.height));
        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        var small = new Texture2D(w, h, TextureFormat.RGBA32, false);
        small.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        small.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);
        var px = small.GetPixels();
        Destroy(small);
        return px;
    }
}

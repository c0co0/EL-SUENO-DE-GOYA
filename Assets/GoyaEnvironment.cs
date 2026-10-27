using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ambiente del camino de GoyaGallery: un paisaje pintado como el fondo de "La gallina ciega"
/// (cielo azul con nubes doradas, montañas azuladas a lo lejos, un lago plateado y tierra ocre),
/// que se va oscureciendo y cambiando de color zona por zona hasta la noche de las Pinturas Negras.
/// Todo se genera al empezar (no hace falta ninguna imagen).
///
/// Zonas (en el orden de Epoch Sizes): cartones para tapices, la corte, brujas y disparates,
/// la guerra y las Pinturas Negras.
///
/// GoyaGallery lo agrega solo al objeto "Galeria" cuando esta en modo camino; se puede ajustar en el Inspector
/// mientras corre (y despues copiar los valores).
/// </summary>
public class GoyaEnvironment : MonoBehaviour
{
    [System.Serializable]
    public class Zone
    {
        public string name;
        [Tooltip("Color del fondo (lo que se ve a lo lejos)")]
        public Color sky;
        [Tooltip("Color de la niebla que envuelve a lo lejano")]
        public Color fog;
        [Tooltip("Cuanta niebla hay (mas alto = se pierde mas cerca)")]
        public float fogDensity;
        [Tooltip("Color por el que se tiñe todo el paisaje (blanco = colores de La gallina ciega)")]
        public Color tint;
    }

    public Zone[] zones =
    {
        new Zone { name = "Cartones para tapices", sky = new Color(0.44f, 0.56f, 0.62f), fog = new Color(0.85f, 0.79f, 0.67f), fogDensity = 0.015f, tint = Color.white },
        new Zone { name = "La corte",              sky = new Color(0.30f, 0.22f, 0.12f), fog = new Color(0.55f, 0.40f, 0.22f), fogDensity = 0.03f,  tint = new Color(0.85f, 0.62f, 0.36f) },
        new Zone { name = "Brujas y disparates",   sky = new Color(0.05f, 0.06f, 0.11f), fog = new Color(0.12f, 0.14f, 0.20f), fogDensity = 0.06f,  tint = new Color(0.22f, 0.26f, 0.40f) },
        new Zone { name = "La guerra",             sky = new Color(0.12f, 0.05f, 0.03f), fog = new Color(0.30f, 0.12f, 0.05f), fogDensity = 0.07f,  tint = new Color(0.50f, 0.22f, 0.10f) },
        new Zone { name = "Pinturas Negras",       sky = new Color(0.015f, 0.012f, 0.008f), fog = new Color(0.05f, 0.04f, 0.03f), fogDensity = 0.09f, tint = new Color(0.10f, 0.08f, 0.06f) },
    };

    [Header("Paisaje (como el fondo de La gallina ciega)")]
    public bool landscape = true;
    public Color skyTop = new Color(0.40f, 0.56f, 0.70f);
    public Color skyHorizon = new Color(0.88f, 0.83f, 0.70f);
    public Color cloudLight = new Color(0.96f, 0.92f, 0.80f);
    public Color cloudGold = new Color(0.90f, 0.76f, 0.52f);
    [Range(0f, 1f)] public float cloudAmount = 0.55f;
    public Color farMountains = new Color(0.60f, 0.68f, 0.76f);
    public Color nearHills = new Color(0.42f, 0.46f, 0.47f);
    public Color lake = new Color(0.66f, 0.69f, 0.68f);
    public Color earth = new Color(0.56f, 0.47f, 0.36f);
    public Color grass = new Color(0.24f, 0.24f, 0.13f);
    [Tooltip("Ancho de la franja de tierra por donde va el camino (metros); mas alla esta el lago")]
    public float earthWidth = 26f;

    [Header("Paredes de pinturas (version anterior)")]
    [Tooltip("Paredes a los costados hechas con las pinturas de cada zona, borrosas")]
    public bool walls = false;
    [Tooltip("Distancia de las paredes al centro del camino (metros)")]
    public float wallDistance = 7f;
    public float wallHeight = 8f;
    [Tooltip("Suelo de cada zona, hecho tambien con sus pinturas")]
    public bool ground = false;
    [Tooltip("Altura de los ojos sobre el suelo (metros)")]
    public float eyeHeight = 1.6f;
    [Tooltip("Que tan borrosas quedan las pinturas de las paredes (mas chico = mas borroso)")]
    public int blurResolution = 24;
    [Range(0f, 1f)] public float wallBrightness = 0.55f;
    [Range(0f, 1f)] public float groundBrightness = 0.3f;

    [Header("Estado (solo lectura)")]
    [Tooltip("Zona en la que se esta (con decimales al pasar de una a otra)")]
    public float zoneNow;

    GoyaGallery gallery;
    Material mat;
    Color fogNow = Color.black;
    float densityNow;
    Camera[] cams = new Camera[4];

    class Piece
    {
        public MeshRenderer renderer;
        public Material material;
        public Color tint;
        public Vector3 center;   // para calcular la niebla
        public bool far;         // cielo, montañas y lago: siguen a la persona y no les afecta la niebla
    }
    readonly List<Piece> pieces = new List<Piece>();

    /// <summary>Color de la niebla ahora (lo usa GoyaGallery para que las obras lejanas se pierdan en ella).</summary>
    public Color FogColor { get { return fogNow; } }

    void Start()
    {
        gallery = GetComponent<GoyaGallery>();
        if (gallery == null || gallery.ZoneCount == 0) { enabled = false; return; }

        Shader sh = Shader.Find("Sprites/Default");
        if (sh == null) sh = Shader.Find("Unlit/Texture");
        mat = new Material(sh);
        BuildScenery();
    }

    // ---------------------------------------------------------------- escenografia

    void BuildScenery()
    {
        Vector3 c = gallery.Center;
        float len = gallery.zoneLength;
        float floorY = gallery.RigPosition.y - eyeHeight;
        if (landscape) BuildLandscape(c, floorY);

        for (int z = 0; z < gallery.ZoneCount; z++)
        {
            var zone = zones[Mathf.Min(z, zones.Length - 1)];
            var sprites = new List<Sprite>();
            foreach (var p in gallery.paintings) if (p.zone == z) sprites.Add(p.original);
            if (sprites.Count == 0) continue;

            float midZ = c.z + (z + 0.5f) * len;
            if (walls)
            {
                // Las paredes se superponen un poco con las de la zona vecina para que no haya cortes
                AddPiece("Pared_" + z + "_Izq", Blurred(sprites[0]), new Vector3(c.x - wallDistance, floorY + wallHeight * 0.5f, midZ),
                         Quaternion.LookRotation(Vector3.left), new Vector2(len * 1.15f, wallHeight), zone.tint * wallBrightness);
                AddPiece("Pared_" + z + "_Der", Blurred(sprites[Mathf.Min(1, sprites.Count - 1)]), new Vector3(c.x + wallDistance, floorY + wallHeight * 0.5f, midZ),
                         Quaternion.LookRotation(Vector3.right), new Vector2(len * 1.15f, wallHeight), zone.tint * wallBrightness);
            }
            if (ground)
            {
                AddPiece("Suelo_" + z, Blurred(sprites[sprites.Count - 1]), new Vector3(c.x, floorY, midZ),
                         Quaternion.Euler(90f, 0f, 0f), new Vector2(wallDistance * 2f, len), zone.tint * groundBrightness);
            }
        }

        if (!walls) return;

        // Al fondo del camino, la ultima pintura enorme esperando en la oscuridad; atras del comienzo, la primera
        var last = gallery.paintings[gallery.paintings.Count - 1].original;
        var first = gallery.paintings[0].original;
        var lastZone = zones[Mathf.Min(gallery.ZoneCount - 1, zones.Length - 1)];
        AddPiece("Fondo_Final", Blurred(last), new Vector3(c.x, floorY + wallHeight * 0.5f, c.z + gallery.PathLength + 4f),
                 Quaternion.LookRotation(Vector3.forward), new Vector2(wallDistance * 2f, wallHeight), lastZone.tint * wallBrightness);
        AddPiece("Fondo_Inicio", Blurred(first), new Vector3(c.x, floorY + wallHeight * 0.5f, c.z - 5f),
                 Quaternion.LookRotation(Vector3.back), new Vector2(wallDistance * 2f, wallHeight), zones[0].tint * wallBrightness);
    }

    void AddPiece(string label, Texture tex, Vector3 pos, Quaternion rot, Vector2 size, Color tint)
    {
        AddMesh(label, QuadMesh(), tex, pos, rot, new Vector3(size.x, size.y, 1f), tint, -20, false);
    }

    Piece AddMesh(string label, Mesh mesh, Texture tex, Vector3 pos, Quaternion rot, Vector3 scale, Color tint, int order, bool far)
    {
        var go = new GameObject(label);
        go.transform.SetParent(transform, false);
        go.transform.position = pos;
        go.transform.rotation = rot;
        go.transform.localScale = scale;

        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        var r = go.AddComponent<MeshRenderer>();
        var m = new Material(mat);
        m.mainTexture = tex;
        r.sharedMaterial = m;
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        r.receiveShadows = false;
        r.sortingOrder = order;   // siempre detras de las obras, sus auras y las particulas

        tint.a = 1f;
        var piece = new Piece { renderer = r, material = m, tint = tint, center = pos, far = far };
        pieces.Add(piece);
        return piece;
    }

    // ---------------------------------------------------------------- paisaje

    void BuildLandscape(Vector3 c, float floorY)
    {
        Vector3 here = new Vector3(c.x, floorY, c.z);
        // Cielo: una esfera enorme que acompaña a la persona
        AddMesh("Cielo", SphereMesh(24, 12), SkyTexture(), here, Quaternion.identity, Vector3.one * 90f, Color.white, -40, true);
        // Montañas lejanas azuladas y cerros mas cercanos, como dos anillos alrededor
        AddMesh("Montanas", BandMesh(64), MountainTexture(farMountains, 7, 0.35f, 0.55f), here + Vector3.down * 1f, Quaternion.identity,
                new Vector3(70f, 16f, 70f), Color.white, -39, true);
        AddMesh("Cerros", BandMesh(64), MountainTexture(nearHills, 23, 0.15f, 0.4f), here + Vector3.down * 0.5f, Quaternion.Euler(0f, 70f, 0f),
                new Vector3(55f, 9f, 55f), Color.white, -38, true);
        // Lago plateado alrededor y franja de tierra ocre por donde va el camino
        AddMesh("Lago", DiscMesh(48), WhiteTexture(), here + Vector3.down * 0.02f, Quaternion.identity, Vector3.one * 60f, lake, -37, true);
        float pathLen = gallery.WalkLength + 30f;
        AddMesh("Tierra", QuadMesh(), EarthTexture(), new Vector3(c.x, floorY, c.z + gallery.WalkLength * 0.5f), Quaternion.Euler(90f, 0f, 0f),
                new Vector3(earthWidth, pathLen, 1f), Color.white, -36, false);
    }

    static Color Smooth(Color a, Color b, float t) { return Color.Lerp(a, b, t * t * (3f - 2f * t)); }

    // Ruido que se repite sin costura alrededor (u va de 0 a 1 dando la vuelta)
    static float LoopNoise(float u, float v, float freq, float seed)
    {
        float a = Mathf.PerlinNoise(u * freq + seed, v);
        float b = Mathf.PerlinNoise((u - 1f) * freq + seed, v);
        return Mathf.Lerp(a, b, u);
    }

    static float Fbm(float u, float v, float freq, float seed)
    {
        return 0.55f * LoopNoise(u, v * freq, freq, seed)
             + 0.30f * LoopNoise(u, v * freq * 2f + 3.1f, freq * 2f, seed + 11f)
             + 0.15f * LoopNoise(u, v * freq * 4f + 7.7f, freq * 4f, seed + 23f);
    }

    Texture2D SkyTexture()
    {
        const int w = 512, h = 256;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        var px = new Color[w * h];
        for (int y = 0; y < h; y++)
        {
            float v = (y + 0.5f) / h;                      // 0 = abajo, 0.5 = horizonte, 1 = arriba
            float up = Mathf.Clamp01((v - 0.5f) * 2f);
            Color baseCol = Smooth(skyHorizon, skyTop, Mathf.Clamp01((up - 0.08f) / 0.6f));
            for (int x = 0; x < w; x++)
            {
                float u = (x + 0.5f) / w;
                // Nubes estiradas a lo ancho, mas cerca del horizonte, doradas abajo y claras arriba
                float n = Fbm(u, up * 1.6f, 5f, 0f);
                float band = Mathf.Clamp01(1f - Mathf.Abs(up - 0.3f) / 0.45f);
                float mask = Mathf.Clamp01((n - (1f - cloudAmount)) / 0.25f) * band;
                Color cloud = Color.Lerp(cloudGold, cloudLight, Mathf.Clamp01((n - 0.4f) * 2.5f));
                Color col = Color.Lerp(baseCol, cloud, mask * 0.9f);
                if (v < 0.5f) col = skyHorizon;
                px[y * w + x] = col;
            }
        }
        tex.SetPixels(px);
        tex.Apply();
        return tex;
    }

    // Silueta de montañas: opaca abajo, transparente arriba del perfil, con bruma clara al pie
    Texture2D MountainTexture(Color col, float seed, float minH, float maxH)
    {
        const int w = 1024, h = 128;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Repeat;
        var px = new Color[w * h];
        for (int x = 0; x < w; x++)
        {
            float u = (x + 0.5f) / w;
            float profile = Mathf.Lerp(minH, maxH, Fbm(u, 0.5f, 6f, seed));
            for (int y = 0; y < h; y++)
            {
                float v = (y + 0.5f) / h;
                float alpha = Mathf.Clamp01((profile - v) * 60f);
                Color c = Color.Lerp(skyHorizon, col, Mathf.Clamp01(v / Mathf.Max(0.01f, profile) * 1.3f));   // bruma al pie
                c.a = alpha;
                px[y * w + x] = c;
            }
        }
        tex.SetPixels(px);
        tex.Apply();
        return tex;
    }

    // Tierra ocre con manchas de pasto oscuro; se desvanece hacia los costados (donde empieza el lago)
    Texture2D EarthTexture()
    {
        const int w = 128, h = 512;
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        var px = new Color[w * h];
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float u = (x + 0.5f) / w, v = (y + 0.5f) / h;
                float n = Mathf.PerlinNoise(u * 4f + 5f, v * 16f);
                float g = Mathf.Clamp01((Mathf.PerlinNoise(u * 3f + 40f, v * 10f) - 0.5f) * 3f);
                Color c = Color.Lerp(earth * (0.85f + 0.3f * n), grass, g * 0.8f);
                c.a = Mathf.Clamp01((0.5f - Mathf.Abs(u - 0.5f)) / 0.2f);
                px[y * w + x] = c;
            }
        }
        tex.SetPixels(px);
        tex.Apply();
        return tex;
    }

    static Texture2D white;
    static Texture2D WhiteTexture()
    {
        if (white != null) return white;
        white = new Texture2D(2, 2);
        white.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
        white.Apply();
        return white;
    }

    static Mesh SphereMesh(int segU, int segV)
    {
        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var tris = new List<int>();
        for (int j = 0; j <= segV; j++)
        {
            float v = (float)j / segV;
            float lat = Mathf.Lerp(-Mathf.PI * 0.5f, Mathf.PI * 0.5f, v);
            for (int i = 0; i <= segU; i++)
            {
                float u = (float)i / segU;
                float lon = u * Mathf.PI * 2f;
                verts.Add(new Vector3(Mathf.Cos(lat) * Mathf.Sin(lon), Mathf.Sin(lat), Mathf.Cos(lat) * Mathf.Cos(lon)) * 0.5f);
                uvs.Add(new Vector2(u, v));
            }
        }
        for (int j = 0; j < segV; j++)
            for (int i = 0; i < segU; i++)
            {
                int a = j * (segU + 1) + i, b = a + segU + 1;
                tris.AddRange(new[] { a, b, a + 1, a + 1, b, b + 1 });
            }
        return Finish(verts, uvs, tris);
    }

    // Anillo vertical (como la pared de un cilindro) de radio 0.5 y alto 1, desde y = 0
    static Mesh BandMesh(int seg)
    {
        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var tris = new List<int>();
        for (int i = 0; i <= seg; i++)
        {
            float u = (float)i / seg, a = u * Mathf.PI * 2f;
            var dir = new Vector3(Mathf.Sin(a), 0f, Mathf.Cos(a)) * 0.5f;
            verts.Add(dir);
            verts.Add(dir + Vector3.up);
            uvs.Add(new Vector2(u, 0f));
            uvs.Add(new Vector2(u, 1f));
        }
        for (int i = 0; i < seg; i++)
        {
            int a = i * 2;
            tris.AddRange(new[] { a, a + 1, a + 2, a + 2, a + 1, a + 3 });
        }
        return Finish(verts, uvs, tris);
    }

    // Disco horizontal de radio 0.5
    static Mesh DiscMesh(int seg)
    {
        var verts = new List<Vector3> { Vector3.zero };
        var uvs = new List<Vector2> { new Vector2(0.5f, 0.5f) };
        var tris = new List<int>();
        for (int i = 0; i <= seg; i++)
        {
            float a = (float)i / seg * Mathf.PI * 2f;
            verts.Add(new Vector3(Mathf.Sin(a), 0f, Mathf.Cos(a)) * 0.5f);
            uvs.Add(new Vector2(0.5f + Mathf.Sin(a) * 0.5f, 0.5f + Mathf.Cos(a) * 0.5f));
        }
        for (int i = 1; i <= seg; i++) tris.AddRange(new[] { 0, i, i + 1 });
        return Finish(verts, uvs, tris);
    }

    static Mesh Finish(List<Vector3> verts, List<Vector2> uvs, List<int> tris)
    {
        var m = new Mesh();
        m.SetVertices(verts);
        m.SetUVs(0, uvs);
        var cols = new Color[verts.Count];
        for (int i = 0; i < cols.Length; i++) cols[i] = Color.white;
        m.colors = cols;
        m.SetTriangles(tris, 0);
        m.RecalculateNormals();
        m.RecalculateBounds();
        return m;
    }

    static Mesh quad;

    // Cuadrado de 1x1 con colores blancos (Sprites/Default los necesita) que se ve de los dos lados
    static Mesh QuadMesh()
    {
        if (quad != null) return quad;
        quad = new Mesh();
        quad.vertices = new[] { new Vector3(-0.5f, -0.5f, 0f), new Vector3(0.5f, -0.5f, 0f), new Vector3(-0.5f, 0.5f, 0f), new Vector3(0.5f, 0.5f, 0f) };
        quad.uv = new[] { new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f) };
        quad.colors = new[] { Color.white, Color.white, Color.white, Color.white };
        quad.triangles = new[] { 0, 2, 1, 2, 3, 1 };
        quad.RecalculateNormals();
        quad.RecalculateBounds();
        return quad;
    }

    // Version chiquita de la pintura: al estirarla sobre una pared enorme queda borrosa, como una mancha de color
    readonly Dictionary<Sprite, Texture> blurCache = new Dictionary<Sprite, Texture>();

    Texture Blurred(Sprite s)
    {
        Texture cached;
        if (blurCache.TryGetValue(s, out cached)) return cached;

        Rect r = s.rect;
        Texture2D src = s.texture;
        var scale = new Vector2(r.width / src.width, r.height / src.height);
        var offset = new Vector2(r.x / src.width, r.y / src.height);
        float aspect = r.width / Mathf.Max(1f, r.height);

        // Se achica en pasos para que el resultado sea un promedio suave y no puntos sueltos
        int w = 256, h = Mathf.Max(1, Mathf.RoundToInt(256 / aspect));
        var step = RenderTexture.GetTemporary(w, h, 0);
        Graphics.Blit(src, step, scale, offset);
        while (w > blurResolution * 2)
        {
            int nw = Mathf.Max(blurResolution, w / 2), nh = Mathf.Max(1, Mathf.RoundToInt(nw / aspect));
            var next = RenderTexture.GetTemporary(nw, nh, 0);
            next.filterMode = FilterMode.Bilinear;
            Graphics.Blit(step, next);
            RenderTexture.ReleaseTemporary(step);
            step = next;
            w = nw;
        }

        var result = new RenderTexture(step.width, step.height, 0);
        result.filterMode = FilterMode.Bilinear;
        result.wrapMode = TextureWrapMode.Clamp;
        Graphics.Blit(step, result);
        RenderTexture.ReleaseTemporary(step);

        blurCache[s] = result;
        return result;
    }

    // ---------------------------------------------------------------- ambiente segun donde se esta

    void LateUpdate()
    {
        if (gallery == null || zones.Length == 0) return;

        // Mezcla entre la zona actual y la vecina: en la mitad de una zona es pura, en el borde mitad y mitad
        zoneNow = gallery.ZoneAt(gallery.RigPosition);
        float u = Mathf.Clamp(zoneNow - 0.5f, 0f, zones.Length - 1);
        int a = Mathf.Min(Mathf.FloorToInt(u), zones.Length - 1);
        int b = Mathf.Min(a + 1, zones.Length - 1);
        float t = Mathf.SmoothStep(0f, 1f, u - a);

        Color sky = Color.Lerp(zones[a].sky, zones[b].sky, t);
        fogNow = Color.Lerp(zones[a].fog, zones[b].fog, t);
        densityNow = Mathf.Lerp(zones[a].fogDensity, zones[b].fogDensity, t);

        if (cams.Length < Camera.allCamerasCount) cams = new Camera[Camera.allCamerasCount];
        int n = Camera.GetAllCameras(cams);
        for (int i = 0; i < n; i++) cams[i].backgroundColor = sky;

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogColor = fogNow;
        RenderSettings.fogDensity = densityNow;
        RenderSettings.ambientLight = fogNow;

        // El paisaje se tiñe con la zona: de los colores de La gallina ciega a la noche de las Pinturas Negras
        Color zoneTint = Color.Lerp(zones[a].tint, zones[b].tint, t);
        float appear = gallery.appear;
        Vector3 rigPos = gallery.RigPosition;
        foreach (var piece in pieces)
        {
            Color col;
            if (piece.far)
            {
                // Cielo, montañas y lago estan "en el infinito": acompañan a la persona
                var tr = piece.renderer.transform;
                tr.position = new Vector3(rigPos.x, tr.position.y, rigPos.z);
                col = piece.tint * zoneTint;
            }
            else col = Fogged(piece.tint * (landscape ? zoneTint : Color.white), ClosestPoint(piece));
            col.a = appear;
            piece.material.color = col;
        }
    }

    Vector3 ClosestPoint(Piece piece)
    {
        var bounds = piece.renderer.bounds;
        return bounds.ClosestPoint(gallery.RigPosition);
    }

    /// <summary>Mezcla un color con la niebla segun lo lejos que esta ese punto (las cosas lejanas se pierden).</summary>
    public Color Fogged(Color c, Vector3 pos)
    {
        float d = Vector3.Distance(pos, gallery != null ? gallery.RigPosition : pos) * densityNow;
        float f = 1f - Mathf.Exp(-d * d);
        Color r = Color.Lerp(c, fogNow, f * 0.9f);
        r.a = c.a;
        return r;
    }
}

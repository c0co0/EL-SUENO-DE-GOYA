using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ambiente del camino de GoyaGallery: cada epoca es una zona con su propio cielo, niebla y suelo,
/// y a los costados unas paredes enormes hechas con sus mismas pinturas, borrosas y oscurecidas,
/// para que parezca que se camina adentro de ellas. Al avanzar, todo pasa de a poco a la zona siguiente.
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
        [Tooltip("Color por el que se tiñen las paredes y el suelo hechos con las pinturas")]
        public Color tint;
    }

    public Zone[] zones =
    {
        new Zone { name = "Cartones para tapices", sky = new Color(0.55f, 0.62f, 0.68f), fog = new Color(0.62f, 0.60f, 0.52f), fogDensity = 0.045f, tint = new Color(0.85f, 0.80f, 0.68f) },
        new Zone { name = "La corte",              sky = new Color(0.16f, 0.10f, 0.05f), fog = new Color(0.32f, 0.22f, 0.10f), fogDensity = 0.06f,  tint = new Color(0.70f, 0.52f, 0.30f) },
        new Zone { name = "Brujas y disparates",   sky = new Color(0.03f, 0.04f, 0.08f), fog = new Color(0.09f, 0.11f, 0.16f), fogDensity = 0.08f,  tint = new Color(0.30f, 0.34f, 0.45f) },
        new Zone { name = "La guerra",             sky = new Color(0.09f, 0.04f, 0.02f), fog = new Color(0.26f, 0.10f, 0.04f), fogDensity = 0.08f,  tint = new Color(0.55f, 0.28f, 0.14f) },
        new Zone { name = "Pinturas Negras",       sky = new Color(0.015f, 0.012f, 0.008f), fog = new Color(0.05f, 0.04f, 0.03f), fogDensity = 0.1f, tint = new Color(0.22f, 0.18f, 0.12f) },
    };

    [Header("Escenografia")]
    [Tooltip("Paredes a los costados hechas con las pinturas de cada zona, borrosas")]
    public bool walls = true;
    [Tooltip("Distancia de las paredes al centro del camino (metros)")]
    public float wallDistance = 7f;
    public float wallHeight = 8f;
    [Tooltip("Suelo de cada zona, hecho tambien con sus pinturas")]
    public bool ground = true;
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
        var go = new GameObject(label);
        go.transform.SetParent(transform, false);
        go.transform.position = pos;
        go.transform.rotation = rot;
        go.transform.localScale = new Vector3(size.x, size.y, 1f);

        go.AddComponent<MeshFilter>().sharedMesh = QuadMesh();
        var r = go.AddComponent<MeshRenderer>();
        var m = new Material(mat);
        m.mainTexture = tex;
        r.sharedMaterial = m;
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        r.receiveShadows = false;
        r.sortingOrder = -20;   // siempre detras de las obras, sus auras y las particulas

        tint.a = 1f;
        pieces.Add(new Piece { renderer = r, material = m, tint = tint, center = pos });
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

        float appear = gallery.appear;
        foreach (var piece in pieces)
        {
            Color col = Fogged(piece.tint, ClosestPoint(piece));
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

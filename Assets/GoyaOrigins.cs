using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// De donde nace cada obra del camino, y los arboles del paisaje. Los arboles estan recortados de pinturas de Goya
/// (La cometa, La nevada, La gallina ciega) y tienen volumen (relieve). Algunas obras nacen de un arbol: al armarla,
/// el arbol se deshace en particulas de sus propios colores que giran en el remolino y forman la pintura; al
/// desarmarla, vuelve a ser el arbol. Las demas obras nacen del suelo. Hay ademas arboles sueltos en las lomas.
/// Los recortes estan en Assets/Resources/Paisaje (se generan con tools/paisaje/recortar.py).
/// GoyaGallery lo agrega solo en modo camino.
/// </summary>
public class GoyaOrigins : MonoBehaviour
{
    [System.Serializable]
    public class Prop
    {
        public string image;
        [Tooltip("Zona (epoca) donde aparece: 0 cartones, 1 corte, 2 brujas, 3 guerra, 4 negras")]
        public int zone;
        [Tooltip("Alto en metros (si es 0, se usa el ancho)")]
        public float height;
        [Tooltip("Ancho en metros (si el alto es 0)")]
        public float width;
        [Tooltip("Grosor del relieve, en proporcion al lado mas corto")]
        public float depth = 0.25f;
        [Tooltip("Espejado (para que el mismo arbol no se repita igual)")]
        public bool mirror;
    }

    [System.Serializable]
    public class Tree : Prop
    {
        [Tooltip("En que parte de la zona (0 = comienzo, 1 = final)")]
        [Range(0f, 1f)] public float along = 0.5f;
        [Tooltip("Distancia al centro del camino (negativo = a la izquierda)")]
        public float side = 6f;
    }

    public Prop[] props =
    {
        new Prop { image = "Paisaje/arbol_cometa", zone = 0, height = 5.2f, depth = 0.25f },
        new Prop { image = "Paisaje/arbol_gallina", zone = 1, height = 6f, depth = 0.3f },
        new Prop { image = "Paisaje/arbol_nevada", zone = 2, height = 5.5f, depth = 0.2f, mirror = true },
    };

    [Tooltip("Arboles sueltos en el paisaje (no nace ninguna obra de ellos)")]
    public Tree[] trees =
    {
        new Tree { image = "Paisaje/arbol_gallina", zone = 0, along = 0.15f, side = 6.5f, height = 6.5f, depth = 0.3f },
        new Tree { image = "Paisaje/arbol_nevada", zone = 0, along = 0.8f, side = -5.5f, height = 4.6f, depth = 0.2f },
        new Tree { image = "Paisaje/arbol_cometa", zone = 1, along = 0.35f, side = -6.5f, height = 4.8f, depth = 0.25f, mirror = true },
        new Tree { image = "Paisaje/arbol_nevada", zone = 1, along = 0.9f, side = 7.5f, height = 5.8f, depth = 0.2f, mirror = true },
        new Tree { image = "Paisaje/arbol_cometa", zone = 2, along = 0.6f, side = 6f, height = 5.5f, depth = 0.25f },
        new Tree { image = "Paisaje/arbol_nevada", zone = 2, along = 0.75f, side = -6.5f, height = 6.2f, depth = 0.2f },
    };

    [Range(0f, 1f), Tooltip("Cuanto se oscurecen con la epoca (1 = igual que el paisaje)")]
    public float zoneTint = 0.7f;
    [Tooltip("Resolucion del relieve (puntos a lo ancho)")]
    public int reliefResolution = 56;
    public Color soil = new Color(0.45f, 0.36f, 0.26f);

    class Origin
    {
        public Prop prop;
        public MeshRenderer renderer;
        public Material material;
        public SpriteRenderer shadow;
        public Vector3 baseCenter, right, forward;
        public float width;
        public List<Vector3> points;     // puntos de la superficie (locales), para las particulas
        public List<Color> colors;
        public float presence = 1f;      // 1 = se ve el arbol/roca, 0 = ya se transformo en la obra
        public bool grounded = true;     // los arboles sueltos se apoyan en las lomas cuando esta el terreno
        public bool painting;            // va con una obra (para el pasto de alrededor)
    }

    readonly Dictionary<GoyaGallery.Painting, Origin> origins = new Dictionary<GoyaGallery.Painting, Origin>();
    readonly List<Origin> all = new List<Origin>();
    GoyaGallery gallery;
    GoyaEnvironment environment;
    GoyaTerrain terrain;
    float floorY;

    public void Build(GoyaGallery g)
    {
        gallery = g;
        environment = GetComponent<GoyaEnvironment>();
        float eye = environment != null ? environment.eyeHeight : 1.6f;
        floorY = gallery.RigPosition.y - eye;
        var soft = SoftSprite();

        // Cada elemento va con una obra de su zona (la del medio)
        var chosen = new Dictionary<GoyaGallery.Painting, Prop>();
        foreach (var pr in props)
        {
            var inZone = gallery.paintings.FindAll(p => p.zone == pr.zone && !p.locked && !chosen.ContainsKey(p));
            if (inZone.Count == 0) continue;
            chosen[inZone[inZone.Count / 2]] = pr;
        }

        foreach (var p in gallery.paintings)
        {
            if (p.locked) continue;   // las de la cueva estan en las paredes
            var o = new Origin();
            Vector3 fwd = p.root.forward; fwd.y = 0f; fwd.Normalize();   // la obra mira al camino: forward se aleja
            o.forward = fwd;
            o.right = Vector3.Cross(Vector3.up, fwd);
            o.baseCenter = new Vector3(p.root.position.x, floorY, p.root.position.z) + fwd * 0.15f;
            o.width = Mathf.Max(1.2f, p.halfW * 2.2f);

            Prop pr;
            if (chosen.TryGetValue(p, out pr)) BuildProp(o, pr, p.name);

            // Sombra blanda en el piso
            var sh = new GameObject("Sombra_" + p.name);
            sh.transform.SetParent(transform, false);
            sh.transform.position = o.baseCenter + Vector3.up * 0.02f;
            sh.transform.rotation = Quaternion.LookRotation(Vector3.down, fwd);
            sh.transform.localScale = new Vector3(o.width, o.width * 0.45f, 1f) / soft.bounds.size.x;
            o.shadow = sh.AddComponent<SpriteRenderer>();
            o.shadow.sprite = soft;
            o.shadow.sortingOrder = -34;

            o.painting = true;
            origins[p] = o;
            all.Add(o);
        }

        // Arboles sueltos: miran al camino y se apoyan en las lomas (cuando el terreno esta listo)
        var c = gallery.Center;
        int n = 0;
        foreach (var t in trees)
        {
            if (t == null || t.zone >= gallery.ZoneCount) continue;
            float z = c.z + (t.zone + t.along) * gallery.zoneLength;
            var cave = gallery.Cave;
            if (cave != null && cave.Footprint(c.x + t.side, z)) continue;
            var o = new Origin();
            float sd = Mathf.Sign(t.side);
            o.forward = new Vector3(sd, 0f, 0.3f).normalized;
            o.right = Vector3.Cross(Vector3.up, o.forward);
            o.baseCenter = new Vector3(c.x + t.side, floorY, z);
            o.width = 2f;
            o.grounded = false;
            BuildProp(o, t, "Arbol" + (n++));
            if (o.renderer != null) all.Add(o);
        }
    }

    void BuildProp(Origin o, Prop pr, string label)
    {
        var sprite = Resources.Load<Sprite>(pr.image);
        if (sprite == null) { Debug.LogWarning("[Origenes] No encuentro " + pr.image); return; }
        var tex = sprite.texture;
        float aspect = (float)tex.width / tex.height;
        float h = pr.height > 0f ? pr.height : pr.width / aspect;
        float w = h * aspect;

        // Copia chica de la imagen para leer transparencia y colores (sin tildar Read/Write)
        int gw = Mathf.Clamp(reliefResolution, 8, 128);
        int gh = Mathf.Clamp(Mathf.RoundToInt(gw / aspect), 8, 160);
        var px = ReadSmall(tex, gw, gh);
        var alpha = new float[gw, gh];
        for (int y = 0; y < gh; y++) for (int x = 0; x < gw; x++) alpha[x, y] = px[y * gw + x].a;

        // Relieve: mas grueso cuanto mas lejos del borde (como inflado), con un poco de la luz de la pintura
        var dist = Chamfer(alpha, gw, gh);
        float maxD = 0.001f;
        foreach (var d in dist) maxD = Mathf.Max(maxD, d);
        float thick = Mathf.Min(w, h) * pr.depth;

        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var tris = new List<int>();
        o.points = new List<Vector3>();
        o.colors = new List<Color>();
        int stride = gw * gh;
        for (int pass = 0; pass < 2; pass++)   // 0 = frente (hacia el camino), 1 = atras
            for (int y = 0; y < gh; y++)
                for (int x = 0; x < gw; x++)
                {
                    float u = (x + 0.5f) / gw, v = (y + 0.5f) / gh;
                    float dn = Mathf.Sqrt(dist[x, y] / maxD);
                    float lum = px[y * gw + x].grayscale;
                    float z = thick * dn * (0.8f + 0.4f * lum);
                    var pos = new Vector3((u - 0.5f) * w, v * h, pass == 0 ? -z : z * 0.6f);
                    verts.Add(pos);
                    uvs.Add(new Vector2(u, v));
                    if (pass == 0 && alpha[x, y] > 0.5f) { o.points.Add(pos); o.colors.Add(px[y * gw + x]); }
                }
        for (int pass = 0; pass < 2; pass++)
            for (int y = 0; y < gh - 1; y++)
                for (int x = 0; x < gw - 1; x++)
                {
                    if (alpha[x, y] + alpha[x + 1, y] + alpha[x, y + 1] + alpha[x + 1, y + 1] < 0.05f) continue;
                    int i0 = pass * stride + y * gw + x, i1 = i0 + 1, i2 = i0 + gw, i3 = i2 + 1;
                    if (pass == 0) tris.AddRange(new[] { i0, i2, i1, i1, i2, i3 });
                    else tris.AddRange(new[] { i0, i1, i2, i1, i3, i2 });
                }

        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();

        var go = new GameObject("Origen_" + label);
        go.transform.SetParent(transform, false);
        go.transform.position = o.baseCenter + Vector3.down * 0.25f;   // un poco enterrado: sale del pasto
        go.transform.rotation = Quaternion.LookRotation(o.forward);
        if (pr.mirror) go.transform.localScale = new Vector3(-1f, 1f, 1f);
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        o.renderer = go.AddComponent<MeshRenderer>();
        o.material = new Material(Shader.Find("Sprites/Default"));
        o.material.mainTexture = tex;
        o.renderer.sharedMaterial = o.material;
        o.renderer.sortingOrder = -3;
        o.prop = pr;
        o.width = w * 0.8f;
    }

    static Color[] ReadSmall(Texture tex, int w, int h)
    {
        var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32);
        var prev = RenderTexture.active;
        Graphics.Blit(tex, rt);
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

    // Distancia (en puntos) de cada punto opaco al borde transparente mas cercano
    public static float[,] Chamfer(float[,] a, int w, int h)
    {
        var d = new float[w, h];
        const float big = 1e6f, diag = 1.414f;
        for (int y = 0; y < h; y++) for (int x = 0; x < w; x++) d[x, y] = a[x, y] > 0.5f ? big : 0f;
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
            {
                if (d[x, y] == 0f) continue;
                float v = d[x, y];
                v = Mathf.Min(v, (x > 0 ? d[x - 1, y] : 0f) + 1f);
                v = Mathf.Min(v, (y > 0 ? d[x, y - 1] : 0f) + 1f);
                v = Mathf.Min(v, (x > 0 && y > 0 ? d[x - 1, y - 1] : 0f) + diag);
                v = Mathf.Min(v, (x < w - 1 && y > 0 ? d[x + 1, y - 1] : 0f) + diag);
                d[x, y] = v;
            }
        for (int y = h - 1; y >= 0; y--)
            for (int x = w - 1; x >= 0; x--)
            {
                if (d[x, y] == 0f) continue;
                float v = d[x, y];
                v = Mathf.Min(v, (x < w - 1 ? d[x + 1, y] : 0f) + 1f);
                v = Mathf.Min(v, (y < h - 1 ? d[x, y + 1] : 0f) + 1f);
                v = Mathf.Min(v, (x < w - 1 && y < h - 1 ? d[x + 1, y + 1] : 0f) + diag);
                v = Mathf.Min(v, (x > 0 && y < h - 1 ? d[x - 1, y + 1] : 0f) + diag);
                d[x, y] = v;
            }
        return d;
    }

    /// <summary>Pie de cada arbol (x, y, z) y su ancho (w), para rodearlos de pasto.</summary>
    public List<Vector4> PropBases()
    {
        var list = new List<Vector4>();
        foreach (var o in all) if (o.renderer != null) list.Add(new Vector4(o.baseCenter.x, o.baseCenter.y, o.baseCenter.z, o.width));
        return list;
    }

    /// <summary>Pie de cada obra del camino (x, y, z) y su ancho (w): ahi tambien crece pasto.</summary>
    public List<Vector4> PaintingBases()
    {
        var list = new List<Vector4>();
        foreach (var o in all) if (o.painting && o.renderer == null) list.Add(new Vector4(o.baseCenter.x, o.baseCenter.y, o.baseCenter.z, o.width));
        return list;
    }

    public bool Has(GoyaGallery.Painting p) { return p != null && origins.ContainsKey(p); }

    /// <summary>Cuanto se ve el arbol/roca de esta obra (GoyaParticles lo baja al armarla y lo sube al desarmarla).</summary>
    public void SetPresence(GoyaGallery.Painting p, float value)
    {
        Origin o;
        if (p != null && origins.TryGetValue(p, out o)) o.presence = Mathf.Clamp01(value);
    }

    /// <summary>Un punto del lugar de donde nace la obra (a y b entre 0 y 1).</summary>
    public Vector3 SourcePoint(GoyaGallery.Painting p, float a, float b)
    {
        Origin o;
        if (!origins.TryGetValue(p, out o)) return p.root.position;
        if (o.renderer != null && o.points.Count > 0)
        {
            int i = Mathf.Min(o.points.Count - 1, (int)(a * o.points.Count));
            return o.renderer.transform.TransformPoint(o.points[i] + new Vector3((b - 0.5f) * 0.1f, (Mathf.Repeat(b * 13.7f, 1f) - 0.5f) * 0.1f, 0f));
        }
        float ga = b * Mathf.PI * 2f, gr = Mathf.Sqrt(a) * o.width * 0.5f;
        return o.baseCenter + o.right * (Mathf.Cos(ga) * gr) + o.forward * (Mathf.Sin(ga) * gr * 0.5f) + Vector3.up * 0.03f;
    }

    /// <summary>Color de esa particula mientras todavia es arbol, roca o tierra.</summary>
    public Color32 SourceColor(GoyaGallery.Painting p, float a, float b)
    {
        Origin o;
        Color c = soil * (0.75f + 0.5f * Mathf.Repeat(b * 7.31f, 1f));
        if (origins.TryGetValue(p, out o) && o.renderer != null && o.colors.Count > 0)
            c = o.colors[Mathf.Min(o.colors.Count - 1, (int)(a * o.colors.Count))];
        c = Shade(c, p.root.position);
        c.a = 0.9f;
        return c;
    }

    Color Shade(Color c, Vector3 pos)
    {
        if (environment == null) return c;
        Color tint = Color.Lerp(Color.white, environment.ZoneTint(pos), zoneTint);
        return environment.Fogged(new Color(c.r * tint.r, c.g * tint.g, c.b * tint.b, c.a), pos);
    }

    void LateUpdate()
    {
        if (gallery == null) return;
        float appear = gallery.appear;
        if (terrain == null) terrain = GetComponent<GoyaTerrain>();
        foreach (var o in all)
        {
            if (!o.grounded && terrain != null && terrain.Ready && o.renderer != null)
            {
                o.grounded = true;
                float h = terrain.Height(o.baseCenter.x, o.baseCenter.z);
                o.baseCenter.y = floorY + h;
                o.renderer.transform.position = o.baseCenter + Vector3.down * 0.25f;
            }
            if (o.renderer != null)
            {
                Color c = Shade(Color.white, o.renderer.transform.position + Vector3.up);
                c.a = appear * o.presence;
                o.material.color = c;
                o.renderer.enabled = c.a > 0.01f;
            }
            if (o.shadow != null) o.shadow.color = new Color(0f, 0f, 0f, 0.35f * appear);
        }
    }

    static Sprite SoftSprite()
    {
        int n = 64;
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float dx = (x + 0.5f) / n * 2f - 1f, dy = (y + 0.5f) / n * 2f - 1f;
                float a = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a * a));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), n);
    }
}

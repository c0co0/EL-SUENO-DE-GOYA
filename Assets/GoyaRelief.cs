using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Las obras del camino (antes de la cueva) son parte del lugar: estan apoyadas en el piso, un poco enterradas
/// entre el pasto, con los bordes gastados y en relieve, como los arboles recortados de las pinturas.
/// El relieve sale de la pintura: las figuras (las de Assets/Resources/Cortes) sobresalen, el resto es una
/// lamina gruesa que se afina hacia los bordes, con la luz y la sombra de las pinceladas.
/// Reemplaza el dibujo plano del cuadro pero usa su color (velo, epoca, niebla) y su imagen, asi que todo lo
/// demas (gestos, particulas, obras que cambian) sigue igual. GoyaGallery lo agrega solo en modo camino.
/// </summary>
public class GoyaRelief : MonoBehaviour
{
    [Tooltip("Cuanto queda enterrada la base de cada obra (metros)")]
    public float sink = 0.15f;
    [Tooltip("Cuanto sobresalen las figuras, en proporcion al lado mas corto del cuadro")]
    public float figureDepth = 0.09f;
    [Tooltip("Grosor de la lamina, en proporcion al lado mas corto")]
    public float baseDepth = 0.035f;
    [Tooltip("Relieve de las pinceladas (lo claro sale, lo oscuro se hunde)")]
    public float detailDepth = 0.02f;
    [Tooltip("Cuanto se comen los bordes (proporcion del ancho)")]
    public float roughEdge = 0.035f;
    [Tooltip("Puntos a lo ancho del relieve")]
    public int resolution = 72;
    [Tooltip("Obras que quedan planas (las animadas: sus capas van encima)")]
    public string[] flat = { "12 El pelele", "18 Vuelo de brujas" };

    class Item
    {
        public GoyaGallery.Painting painting;
        public MeshRenderer renderer;
        public MeshFilter filter;
        public Material material;
        public Sprite built;
        public bool relief;
    }

    readonly List<Item> items = new List<Item>();
    readonly Dictionary<Sprite, Mesh> meshes = new Dictionary<Sprite, Mesh>();
    GoyaGallery gallery;
    Shader shader;
    float floorY;

    public void Build(GoyaGallery g)
    {
        gallery = g;
        var env = GetComponent<GoyaEnvironment>();
        floorY = g.RigPosition.y - (env != null ? env.eyeHeight : 1.6f);
        shader = Resources.Load<Shader>("Shaders/GoyaRelieve");
        if (shader == null) Debug.LogWarning("[Relieve] Falta el shader Goya/Relieve: las obras quedan planas");

        foreach (var p in g.paintings)
        {
            if (p.locked) continue;   // las de la cueva estan colgadas en las paredes
            var it = new Item { painting = p };
            it.relief = shader != null && !IsFlat(p);
            if (it.relief)
            {
                var go = new GameObject("Relieve");
                go.transform.SetParent(p.root, false);
                it.filter = go.AddComponent<MeshFilter>();
                it.renderer = go.AddComponent<MeshRenderer>();
                it.renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                it.renderer.receiveShadows = false;
                it.material = new Material(shader);
                it.renderer.sharedMaterial = it.material;
                p.renderer.forceRenderingOff = true;   // el cuadro plano sigue (para el resto), pero no se dibuja
            }
            items.Add(it);
            Refresh(it);
        }
    }

    bool IsFlat(GoyaGallery.Painting p)
    {
        if (p.original == null) return false;
        foreach (var f in flat) if (!string.IsNullOrEmpty(f) && p.original.name.StartsWith(f)) return true;
        return false;
    }

    // Cuando cambia la obra del cuadro: otro relieve y otra vez apoyada en el piso (cambia el tamaño)
    void Refresh(Item it)
    {
        var p = it.painting;
        var s = p.renderer.sprite;
        it.built = s;
        if (s == null) return;
        float bottom = p.root.position.y + s.bounds.min.y * p.root.lossyScale.y;
        p.root.position += Vector3.up * (floorY - sink - bottom);
        if (!it.relief) return;
        it.filter.sharedMesh = MeshFor(s);
        it.material.mainTexture = s.texture;
    }

    void LateUpdate()
    {
        foreach (var it in items)
        {
            var p = it.painting;
            if (p.renderer.sprite != it.built) Refresh(it);
            if (!it.relief) continue;
            bool on = p.renderer.enabled && p.root.gameObject.activeInHierarchy && p.renderer.color.a > 0.003f;
            it.renderer.enabled = on;
            if (on) it.material.color = p.renderer.color;
        }
    }

    // ---------------------------------------------------------------- el relieve de una pintura

    Mesh MeshFor(Sprite s)
    {
        Mesh m;
        if (meshes.TryGetValue(s, out m)) return m;

        var tex = s.texture;
        Rect rect = s.textureRect;
        Bounds b = s.bounds;
        float aspect = rect.width / rect.height;
        int gw = Mathf.Clamp(resolution, 16, 160);
        int gh = Mathf.Clamp(Mathf.RoundToInt(gw / aspect), 12, 220);
        var px = ReadSmall(tex, gw, gh);

        // Bordes gastados: lo que queda del cuadro
        var inside = new float[gw, gh];
        float seed = (s.name.GetHashCode() & 1023) * 0.37f;
        float rough = roughEdge * gw;
        for (int y = 0; y < gh; y++)
            for (int x = 0; x < gw; x++)
            {
                float e = Mathf.Min(Mathf.Min(x, gw - 1 - x), Mathf.Min(y, gh - 1 - y));
                float n = Mathf.PerlinNoise(x * 0.17f + seed, y * 0.17f + seed * 0.5f) * 0.7f + Mathf.PerlinNoise(x * 0.6f + seed, y * 0.6f) * 0.3f;
                inside[x, y] = e >= 1f + rough * n ? 1f : 0f;
            }
        var edge = GoyaOrigins.Chamfer(inside, gw, gh);

        // Las figuras de la obra (si hay recorte)
        float[,] fig = null;
        var cut = Resources.Load<Sprite>("Cortes/" + s.name);
        if (cut != null)
        {
            var cp = ReadSmall(cut.texture, gw, gh);
            var fa = new float[gw, gh];
            for (int y = 0; y < gh; y++) for (int x = 0; x < gw; x++) fa[x, y] = cp[y * gw + x].a > 0.5f && inside[x, y] > 0f ? 1f : 0f;
            fig = GoyaOrigins.Chamfer(fa, gw, gh);
        }

        float minSide = Mathf.Min(b.size.x, b.size.y);
        float pillow = gw * 0.07f, figRound = gw * 0.045f;
        var hgt = new float[gw, gh];
        for (int y = 0; y < gh; y++)
            for (int x = 0; x < gw; x++)
            {
                if (inside[x, y] <= 0f) continue;
                float dn = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((edge[x, y] - 1f) / pillow));
                float lum = px[y * gw + x].grayscale;
                float f = fig != null && fig[x, y] > 0f ? Mathf.Sqrt(Mathf.Clamp01(fig[x, y] / figRound)) : 0f;
                hgt[x, y] = minSide * dn * (baseDepth + figureDepth * f + detailDepth * (lum - 0.5f));
                hgt[x, y] = Mathf.Max(0f, hgt[x, y]);
            }

        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var cols = new List<Color>();
        var tris = new List<int>();
        float cw = b.size.x / (gw - 1), ch = b.size.y / (gh - 1);
        Vector3 light = new Vector3(-0.5f, 0.6f, -0.6f).normalized;   // de arriba a la izquierda, de adelante
        int stride = gw * gh;
        for (int pass = 0; pass < 2; pass++)   // 0 = frente, 1 = atras
            for (int y = 0; y < gh; y++)
                for (int x = 0; x < gw; x++)
                {
                    float u = (float)x / (gw - 1), v = (float)y / (gh - 1);
                    float h = hgt[x, y];
                    verts.Add(new Vector3(b.min.x + u * b.size.x, b.min.y + v * b.size.y, pass == 0 ? -h : h * 0.5f));
                    uvs.Add(new Vector2((rect.x + u * rect.width) / tex.width, (rect.y + v * rect.height) / tex.height));
                    if (pass == 0)
                    {
                        float hx = (hgt[Mathf.Min(gw - 1, x + 1), y] - hgt[Mathf.Max(0, x - 1), y]) / (2f * cw);
                        float hy = (hgt[x, Mathf.Min(gh - 1, y + 1)] - hgt[x, Mathf.Max(0, y - 1)]) / (2f * ch);
                        Vector3 n = new Vector3(-hx, -hy, -1f).normalized;
                        float shade = Mathf.Clamp(0.4f + Vector3.Dot(n, light), 0.55f, 1.15f);
                        cols.Add(new Color(shade, shade, shade, 1f));
                    }
                    else cols.Add(new Color(0.32f, 0.28f, 0.24f, 1f));   // por detras: tela vieja y tierra
                }
        for (int pass = 0; pass < 2; pass++)
            for (int y = 0; y < gh - 1; y++)
                for (int x = 0; x < gw - 1; x++)
                {
                    if (inside[x, y] + inside[x + 1, y] + inside[x, y + 1] + inside[x + 1, y + 1] < 3f) continue;
                    int i0 = pass * stride + y * gw + x, i1 = i0 + 1, i2 = i0 + gw, i3 = i2 + 1;
                    if (pass == 0) tris.AddRange(new[] { i0, i2, i1, i1, i2, i3 });
                    else tris.AddRange(new[] { i0, i1, i2, i1, i3, i2 });
                }

        m = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32, name = "Relieve_" + s.name };
        m.SetVertices(verts);
        m.SetUVs(0, uvs);
        m.SetColors(cols);
        m.SetTriangles(tris, 0);
        m.RecalculateBounds();
        meshes[s] = m;
        return m;
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
}

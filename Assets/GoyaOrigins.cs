using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// De donde nace cada obra del camino: del suelo, de un arbol o de una pared de roca.
/// Pone el arbol o la roca detras de la obra, y le dice a GoyaParticles de donde salen las particulas
/// (y a donde vuelven al desarmarse) y de que color son mientras todavia son tierra, hojas o piedra.
/// Los dibujos estan en Assets/Resources/Paisaje (se generan con tools/paisaje/generar.py).
/// GoyaGallery lo agrega solo en modo camino.
/// </summary>
public class GoyaOrigins : MonoBehaviour
{
    public enum Kind { Suelo, Arbol, Roca }

    [Tooltip("Alto de los arboles (metros)")]
    public float treeHeight = 4.6f;
    [Tooltip("Ancho de las paredes de roca (metros)")]
    public float rockWidth = 4.2f;
    [Tooltip("Que tan atras de la obra queda el arbol o la roca (metros)")]
    public float behind = 0.7f;
    [Range(0f, 1f), Tooltip("Cuanto se oscurecen con la epoca (1 = igual que el paisaje)")]
    public float zoneTint = 0.75f;
    public Color soil = new Color(0.45f, 0.36f, 0.26f);
    public Color leaves = new Color(0.40f, 0.43f, 0.22f);
    public Color bark = new Color(0.30f, 0.22f, 0.15f);
    public Color stone = new Color(0.55f, 0.48f, 0.38f);

    class Origin
    {
        public Kind kind;
        public SpriteRenderer prop, shadow;
        public Vector3 baseCenter;   // pie del arbol / de la roca, o centro del pedazo de suelo
        public float width, height;
        public Vector3 right, forward;
    }

    readonly Dictionary<GoyaGallery.Painting, Origin> origins = new Dictionary<GoyaGallery.Painting, Origin>();
    readonly List<Origin> all = new List<Origin>();
    GoyaGallery gallery;
    GoyaEnvironment environment;
    float floorY;

    public void Build(GoyaGallery g)
    {
        gallery = g;
        environment = GetComponent<GoyaEnvironment>();
        float eye = environment != null ? environment.eyeHeight : 1.6f;
        floorY = gallery.RigPosition.y - eye;

        var trees = new List<Sprite>();
        var rocks = new List<Sprite>();
        for (int i = 1; i <= 6; i++)
        {
            var t = Resources.Load<Sprite>("Paisaje/arbol_" + i); if (t != null) trees.Add(t);
            var r = Resources.Load<Sprite>("Paisaje/roca_" + i); if (r != null) rocks.Add(r);
        }
        var soft = SoftSprite();

        // Suelo, arbol, roca... intercalados, para que no se repita el mismo al lado
        var order = new[] { Kind.Arbol, Kind.Suelo, Kind.Roca, Kind.Arbol, Kind.Roca, Kind.Suelo };
        int k = Random.Range(0, order.Length);
        foreach (var p in gallery.paintings)
        {
            var o = new Origin { kind = order[k++ % order.Length] };
            if (o.kind == Kind.Arbol && trees.Count == 0) o.kind = Kind.Suelo;
            if (o.kind == Kind.Roca && rocks.Count == 0) o.kind = Kind.Suelo;

            // La obra mira hacia el camino: "atras" es su forward (alejandose del camino)
            Vector3 fwd = p.root.forward; fwd.y = 0f; fwd.Normalize();
            Vector3 right = Vector3.Cross(Vector3.up, fwd);
            o.forward = fwd;
            o.right = right;
            Vector3 under = new Vector3(p.root.position.x, floorY, p.root.position.z);

            if (o.kind == Kind.Suelo)
            {
                o.baseCenter = under + fwd * 0.1f;
                o.width = Mathf.Max(1.2f, p.halfW * 2.2f);
            }
            else
            {
                var sprite = o.kind == Kind.Arbol ? trees[Random.Range(0, trees.Count)] : rocks[Random.Range(0, rocks.Count)];
                o.baseCenter = under + fwd * (behind + (o.kind == Kind.Roca ? 0.3f : 0f));
                var go = new GameObject((o.kind == Kind.Arbol ? "Arbol_" : "Roca_") + p.name);
                go.transform.SetParent(transform, false);
                go.transform.position = o.baseCenter + Vector3.down * 0.05f;
                go.transform.rotation = Quaternion.LookRotation(fwd);   // mismo plano que la obra
                float size = o.kind == Kind.Arbol ? treeHeight / sprite.bounds.size.y : rockWidth / sprite.bounds.size.x;
                size *= Random.Range(0.9f, 1.1f);
                go.transform.localScale = new Vector3(Random.value < 0.5f ? -size : size, size, size);
                o.prop = go.AddComponent<SpriteRenderer>();
                o.prop.sprite = sprite;
                o.prop.sortingOrder = -3;
                o.width = sprite.bounds.size.x * size;
                o.height = sprite.bounds.size.y * size;
            }

            // Sombra blanda en el piso (asienta todo en el suelo)
            var sh = new GameObject("Sombra_" + p.name);
            sh.transform.SetParent(transform, false);
            sh.transform.position = o.baseCenter + Vector3.up * 0.02f;
            sh.transform.rotation = Quaternion.LookRotation(Vector3.down, fwd);
            float sw = o.kind == Kind.Suelo ? o.width : o.width * 0.8f;
            sh.transform.localScale = new Vector3(sw, sw * 0.45f, 1f) / soft.bounds.size.x;
            o.shadow = sh.AddComponent<SpriteRenderer>();
            o.shadow.sprite = soft;
            o.shadow.sortingOrder = -34;

            origins[p] = o;
            all.Add(o);
        }
    }

    public bool Has(GoyaGallery.Painting p) { return p != null && origins.ContainsKey(p); }

    /// <summary>Un punto al azar del lugar de donde nace la obra (seed 0..1 lo vuelve repetible).</summary>
    public Vector3 SourcePoint(GoyaGallery.Painting p, float a, float b)
    {
        Origin o;
        if (!origins.TryGetValue(p, out o)) return p.root.position;
        switch (o.kind)
        {
            case Kind.Arbol:
                // La mitad sale del tronco, la otra mitad de la copa
                if (a < 0.45f) return o.baseCenter + o.right * ((b - 0.5f) * o.width * 0.12f) + Vector3.up * (a / 0.45f * o.height * 0.5f) - o.forward * 0.05f;
                float ang = b * Mathf.PI * 2f, r = Mathf.Sqrt((a - 0.45f) / 0.55f);
                return o.baseCenter + Vector3.up * (o.height * 0.7f) + (o.right * Mathf.Cos(ang) * o.width * 0.3f + Vector3.up * Mathf.Sin(ang) * o.height * 0.2f) * r - o.forward * 0.1f;
            case Kind.Roca:
                return o.baseCenter + o.right * ((a - 0.5f) * o.width * 0.8f) + Vector3.up * (b * b * o.height * 0.75f) - o.forward * 0.1f;
            default:
                float ga = b * Mathf.PI * 2f, gr = Mathf.Sqrt(a) * o.width * 0.5f;
                return o.baseCenter + o.right * (Mathf.Cos(ga) * gr) + o.forward * (Mathf.Sin(ga) * gr * 0.5f) + Vector3.up * 0.03f;
        }
    }

    /// <summary>Color de las particulas mientras todavia son tierra, hojas o piedra.</summary>
    public Color32 SourceColor(GoyaGallery.Painting p, float a, float b)
    {
        Origin o;
        Color c = soil;
        if (origins.TryGetValue(p, out o))
        {
            if (o.kind == Kind.Arbol) c = a < 0.45f ? bark : leaves;
            else if (o.kind == Kind.Roca) c = stone;
        }
        c *= 0.75f + 0.5f * Mathf.Repeat(b * 7.31f, 1f);
        c = Shade(c, p.root.position);
        c.a = 0.85f;
        return c;
    }

    Color Shade(Color c, Vector3 pos)
    {
        if (environment == null) return c;
        Color tint = Color.Lerp(Color.white, environment.ZoneTint(pos), zoneTint);
        Color r = environment.Fogged(new Color(c.r * tint.r, c.g * tint.g, c.b * tint.b, c.a), pos);
        return r;
    }

    void LateUpdate()
    {
        if (gallery == null) return;
        float appear = gallery.appear;
        foreach (var o in all)
        {
            if (o.prop != null)
            {
                Color c = Shade(Color.white, o.prop.transform.position);
                c.a = appear;
                o.prop.color = c;
            }
            o.shadow.color = new Color(0f, 0f, 0f, 0.35f * appear);
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

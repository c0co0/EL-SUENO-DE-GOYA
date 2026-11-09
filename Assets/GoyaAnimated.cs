using UnityEngine;

/// <summary>
/// Una pintura animada: El pelele. Cuando la obra esta armada, el pelele salta en la manta:
/// las mujeres lo tiran para arriba, sale por arriba del cuadro dando vueltas y cae; la manta se hunde con el golpe.
/// Las capas (fondo sin el pelele, manta y pelele) estan en Assets/Resources/Animadas
/// (se generan con tools/paisaje/pelele.py). Se agrega solo.
///
/// Es animacion de recortes por codigo. Para animarlo con huesos (paquete 2D Animation de Unity),
/// se puede usar pelele_figura.png: Sprite Editor > Skinning Editor, ponerle huesos y animarlo con un Animator.
/// </summary>
public class GoyaAnimated : MonoBehaviour
{
    [Tooltip("Nombre (o comienzo del nombre) de la obra que se anima")]
    public string painting = "12 El pelele";
    [Tooltip("Segundos de cada salto")]
    public float period = 1.6f;
    [Tooltip("Parte del salto en que el pelele esta en la manta (0 a 1)")]
    [Range(0.05f, 0.6f)] public float contact = 0.18f;
    [Tooltip("Altura del salto, en proporcion al alto del cuadro (0.3 = sale por arriba)")]
    public float jump = 0.32f;
    [Tooltip("Cuanto gira en el aire (grados)")]
    public float tumble = 40f;
    [Tooltip("Cuanto se hunde la manta con el golpe (proporcion del alto del cuadro)")]
    public float sag = 0.02f;

    // Centro de cada capa en pixeles de la pintura (de pelele_capas.txt)
    public Vector2 paintingSize = new Vector2(1128, 1920);
    public Vector2 figureCenter = new Vector2(513, 798);
    public Vector2 blanketCenter = new Vector2(556, 1192);

    GoyaGallery gallery;
    GoyaGallery.Painting target;
    SpriteRenderer back, blanket, figure;
    Vector3 figureBase, blanketBase;
    float t;
    bool searched;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Auto()
    {
        var g = FindFirstObjectByType<GoyaGallery>();
        if (g != null && g.GetComponent<GoyaAnimated>() == null) g.gameObject.AddComponent<GoyaAnimated>();
    }

    void Update()
    {
        if (gallery == null) gallery = GetComponent<GoyaGallery>();
        if (gallery == null || gallery.paintings.Count == 0) return;
        if (!searched) { searched = true; Setup(); }
        if (target == null) return;

        var src = target.renderer;
        bool on = src.enabled && target.revealed && src.sprite == target.original;
        Color c = src.color;
        // En tela o roca no se ven las capas: los caminantes pintan la animacion (igual tiene que correr)
        bool painted = !on && GoyaLayers.Particles != null && GoyaLayers.Particles.PaintedOnSurface(target);
        foreach (var r in new[] { back, blanket, figure })
        {
            r.enabled = on;
            r.color = c;
        }
        if (!on && !painted) { t = 0f; return; }

        // Un salto: en la manta (se hunde) y en el aire (sube, gira y cae)
        t += Time.deltaTime / Mathf.Max(0.2f, period);
        float ph = Mathf.Repeat(t, 1f);
        float h = paintingSize.y / 100f;   // alto del cuadro en unidades del sprite
        if (ph < contact)
        {
            float k = Mathf.Sin(Mathf.PI * ph / contact);
            blanket.transform.localPosition = blanketBase + Vector3.down * (sag * h * k);
            blanket.transform.localScale = new Vector3(1f, 1f + 0.12f * k, 1f);
            figure.transform.localPosition = figureBase + Vector3.down * (sag * h * 1.4f * k);
            figure.transform.localRotation = Quaternion.Euler(0f, 0f, -6f * k);
        }
        else
        {
            float s = (ph - contact) / (1f - contact);
            float up = Mathf.Sin(Mathf.PI * s);
            blanket.transform.localPosition = blanketBase + Vector3.up * (sag * 0.3f * h * up);
            blanket.transform.localScale = Vector3.one;
            figure.transform.localPosition = figureBase + new Vector3(0.25f * Mathf.Sin(Mathf.PI * s), jump * h * up, 0f);
            figure.transform.localRotation = Quaternion.Euler(0f, 0f, -tumble * Mathf.Sin(2f * Mathf.PI * s));
        }
    }

    void Setup()
    {
        foreach (var p in gallery.paintings)
            if (p.original != null && p.original.name.StartsWith(painting)) { target = p; break; }
        if (target == null) return;

        var fondo = Resources.Load<Sprite>("Animadas/pelele_fondo");
        var manta = Resources.Load<Sprite>("Animadas/pelele_manta");
        var pelele = Resources.Load<Sprite>("Animadas/pelele_figura");
        if (fondo == null || manta == null || pelele == null)
        {
            Debug.LogWarning("[Animada] Faltan las capas de El pelele en Assets/Resources/Animadas");
            target = null;
            return;
        }
        back = Layer("Pelele_Fondo", fondo, Vector2.zero, 1);
        blanket = Layer("Pelele_Manta", manta, blanketCenter, 2);
        figure = Layer("Pelele_Figura", pelele, figureCenter, 3);
        blanketBase = blanket.transform.localPosition;
        figureBase = figure.transform.localPosition;
        var layers = new[] { back, blanket, figure };
        var pix = new Texture2D[layers.Length];
        Rect a = GoyaLayers.Area(target.root, target.original, figure, 0.08f, 0.12f, 0.04f, jump + 0.05f);
        Rect m = GoyaLayers.Area(target.root, target.original, blanket, 0.03f, 0.03f, 0.04f, 0.03f);
        Rect area = Rect.MinMaxRect(Mathf.Min(a.xMin, m.xMin), Mathf.Min(a.yMin, m.yMin), Mathf.Max(a.xMax, m.xMax), Mathf.Max(a.yMax, m.yMax));
        GoyaParticles.RegisterAnimation(target, (u, v) => GoyaLayers.Sample(target.root, target.original, layers, pix, u, v), area);
    }

    // Cada capa es hija del cuadro, con el mismo tamaño de pixel: queda justo encima de la pintura
    SpriteRenderer Layer(string label, Sprite s, Vector2 centerPx, int order)
    {
        var go = new GameObject(label);
        go.transform.SetParent(target.root, false);
        var src = target.original;
        float ppu = src.pixelsPerUnit;
        float k = ppu / s.pixelsPerUnit;   // por si las capas tienen otra escala
        go.transform.localScale = Vector3.one * k;
        if (centerPx != Vector2.zero)
            go.transform.localPosition = new Vector3((centerPx.x - paintingSize.x / 2f) / ppu, (paintingSize.y / 2f - centerPx.y) / ppu, -0.001f * order);
        else
        {
            go.transform.localPosition = new Vector3(0f, 0f, -0.001f * order);
            go.transform.localScale = new Vector3(src.bounds.size.x / s.bounds.size.x, src.bounds.size.y / s.bounds.size.y, 1f);
        }
        var r = go.AddComponent<SpriteRenderer>();
        r.sprite = s;
        r.sortingOrder = order;
        r.enabled = false;
        return r;
    }
}

/// <summary>
/// Para pintar una obra animada con caminantes (GoyaParticles en tela o roca): el color de la obra en un punto
/// (u, v de 0 a 1) en este momento, juntando sus capas como estan ahora (fondo, manta, figura...).
/// </summary>
public static class GoyaLayers
{
    static GoyaParticles particles;

    public static GoyaParticles Particles
    {
        get { if (particles == null) particles = Object.FindFirstObjectByType<GoyaParticles>(); return particles; }
    }

    // layers: de abajo hacia arriba. pix: copia de los colores de cada capa (se llena la primera vez)
    public static Color Sample(Transform root, Sprite original, SpriteRenderer[] layers, Texture2D[] pix, float u, float v)
    {
        var gp = Particles;
        Bounds ob = original.bounds;
        Vector3 world = root.TransformPoint(new Vector3(ob.min.x + u * ob.size.x, ob.min.y + v * ob.size.y, 0f));
        Color c = Color.clear;
        for (int i = 0; i < layers.Length; i++)
        {
            var r = layers[i];
            if (r == null || r.sprite == null) continue;
            if (pix[i] == null && gp != null) pix[i] = gp.GetPixels(r.sprite);
            if (pix[i] == null) continue;
            Vector3 lp = r.transform.InverseTransformPoint(world);
            Bounds b = r.sprite.bounds;
            float su = (lp.x - b.min.x) / b.size.x, sv = (lp.y - b.min.y) / b.size.y;
            if (su < 0f || su > 1f || sv < 0f || sv > 1f) continue;
            Color l = pix[i].GetPixelBilinear(su, sv);
            c = i == 0 ? l : Color.Lerp(c, l, l.a);
        }
        c.a = 1f;
        return c;
    }

    /// <summary>Lo que ocupa una capa en la obra (u, v), agrandado: la parte que se mueve.</summary>
    public static Rect Area(Transform root, Sprite original, SpriteRenderer r, float left, float right, float down, float up)
    {
        Bounds ob = original.bounds, b = r.sprite.bounds;
        float u0 = 1f, v0 = 1f, u1 = 0f, v1 = 0f;
        for (int k = 0; k < 4; k++)
        {
            Vector3 corner = new Vector3(k % 2 == 0 ? b.min.x : b.max.x, k < 2 ? b.min.y : b.max.y, 0f);
            Vector3 lp = root.InverseTransformPoint(r.transform.TransformPoint(corner));
            float u = (lp.x - ob.min.x) / ob.size.x, v = (lp.y - ob.min.y) / ob.size.y;
            u0 = Mathf.Min(u0, u); u1 = Mathf.Max(u1, u); v0 = Mathf.Min(v0, v); v1 = Mathf.Max(v1, v);
        }
        u0 = Mathf.Clamp01(u0 - left); u1 = Mathf.Clamp01(u1 + right); v0 = Mathf.Clamp01(v0 - down); v1 = Mathf.Clamp01(v1 + up);
        return Rect.MinMaxRect(u0, v0, u1, v1);
    }
}

using UnityEngine;

/// <summary>
/// Otra pintura animada: Vuelo de brujas. Cuando la obra esta armada, el grupo de brujas que lleva al hombre
/// flota en el cielo: sube y baja despacio, se mece y gira un poco, como si lo llevara el viento.
/// Las capas (el cielo sin las brujas y el grupo) estan en Assets/Resources/Animadas
/// (se generan con tools/paisaje/vuelo.py). Se agrega solo.
/// </summary>
public class GoyaVuelo : MonoBehaviour
{
    [Tooltip("Nombre (o comienzo del nombre) de la obra que se anima")]
    public string painting = "18 Vuelo de brujas";
    [Tooltip("Segundos de cada subida y bajada")]
    public float period = 4.5f;
    [Tooltip("Cuanto sube y baja (proporcion del alto del cuadro)")]
    public float rise = 0.035f;
    [Tooltip("Cuanto se mece de costado (proporcion del ancho)")]
    public float sway = 0.03f;
    [Tooltip("Cuanto gira (grados)")]
    public float tilt = 5f;

    // De vuelo_capas.txt
    public Vector2 paintingSize = new Vector2(1401, 2000);
    public Vector2 figureCenter = new Vector2(624, 734.5f);

    GoyaGallery gallery;
    GoyaGallery.Painting target;
    SpriteRenderer back, figure;
    Vector3 figureBase;
    float t;
    bool searched;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Auto()
    {
        var g = FindFirstObjectByType<GoyaGallery>();
        if (g != null && g.GetComponent<GoyaVuelo>() == null) g.gameObject.AddComponent<GoyaVuelo>();
    }

    void Update()
    {
        if (gallery == null) gallery = GetComponent<GoyaGallery>();
        if (gallery == null || gallery.paintings.Count == 0) return;
        if (!searched) { searched = true; Setup(); }
        if (target == null) return;

        var src = target.renderer;
        bool on = src.enabled && target.revealed && src.sprite == target.original;
        back.enabled = figure.enabled = on;
        back.color = figure.color = src.color;
        if (!on) { t = 0f; return; }

        t += Time.deltaTime;
        float w = paintingSize.x / 100f, h = paintingSize.y / 100f;
        float a = t * Mathf.PI * 2f / Mathf.Max(0.5f, period);
        figure.transform.localPosition = figureBase + new Vector3(
            sway * w * Mathf.Sin(a * 0.5f + 0.4f),
            rise * h * Mathf.Sin(a),
            0f);
        figure.transform.localRotation = Quaternion.Euler(0f, 0f, tilt * Mathf.Sin(a * 0.5f + 1.3f));
        float breathe = 1f + 0.015f * Mathf.Sin(a * 0.7f);
        figure.transform.localScale = figureScale * breathe;
    }

    Vector3 figureScale = Vector3.one;

    void Setup()
    {
        foreach (var p in gallery.paintings)
            if (p.original != null && p.original.name.StartsWith(painting) && !p.locked) { target = p; break; }
        if (target == null) return;

        var fondo = Resources.Load<Sprite>("Animadas/vuelo_fondo");
        var grupo = Resources.Load<Sprite>("Animadas/vuelo_figura");
        if (fondo == null || grupo == null)
        {
            Debug.LogWarning("[Animada] Faltan las capas de Vuelo de brujas en Assets/Resources/Animadas");
            target = null;
            return;
        }
        back = Layer("Vuelo_Fondo", fondo, Vector2.zero, 1);
        figure = Layer("Vuelo_Brujas", grupo, figureCenter, 2);
        figureBase = figure.transform.localPosition;
        figureScale = figure.transform.localScale;
    }

    // Cada capa es hija del cuadro, con el mismo tamaño de pixel: queda justo encima de la pintura
    SpriteRenderer Layer(string label, Sprite s, Vector2 centerPx, int order)
    {
        var go = new GameObject(label);
        go.transform.SetParent(target.root, false);
        var src = target.original;
        float ppu = src.pixelsPerUnit;
        go.transform.localScale = Vector3.one * (ppu / s.pixelsPerUnit);
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

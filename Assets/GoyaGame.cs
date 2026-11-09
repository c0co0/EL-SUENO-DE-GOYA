using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// El juego del sueño: cuando aparece la galeria arranca un reloj y hay que encontrar la mayor cantidad de obras
/// antes de que se termine el tiempo. Cuenta como encontrada cada obra del camino que se arma, cada obra de la
/// cueva que se alumbra de cerca, y cada Pintura Negra por dentro a la que se entra. Si te golpea una roca en la
/// cueva, perdes tiempo. Arriba de la vista se ve el tiempo que queda y cuantas obras llevas.
/// Al terminar el tiempo ya no se puede caminar y se muestra el resultado; cruzar los brazos vuelve a empezar.
/// Se agrega solo y funciona en modo camino.
/// </summary>
public class GoyaGame : MonoBehaviour
{
    [Tooltip("Prender o apagar el reloj (por ahora apagado: el foco esta en la bandada y las rocas)")]
    public bool timer = false;
    [Tooltip("Segundos para recorrer el sueño")]
    public float duration = 300f;
    [Tooltip("Desde cuantos segundos el reloj se pone rojo y suena el tic")]
    public float warnAt = 30f;
    [Tooltip("Distancia y altura del cartel del tiempo, delante de la vista")]
    public float hudDistance = 1.1f;
    public float hudHeight = 0.42f;
    public Color hudColor = new Color(1f, 0.9f, 0.72f);
    public Color warnColor = new Color(1f, 0.3f, 0.2f);

    public static GoyaGame Instance { get; private set; }
    /// <summary>El reloj esta corriendo.</summary>
    public bool Playing { get { return started && !over; } }
    public float TimeLeft { get { return left; } }

    GoyaGallery gallery;
    GoyaCave cave;
    GoyaDiorama diorama;
    TextMesh hud, note;
    AudioClip tick;
    AudioSource tickSource;
    bool started, over;
    float left, flash, noteTimer, lastTick;
    readonly HashSet<GoyaGallery.Painting> formed = new HashSet<GoyaGallery.Painting>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Auto()
    {
        var g = FindFirstObjectByType<GoyaGallery>();
        if (g != null && g.GetComponent<GoyaGame>() == null) g.gameObject.AddComponent<GoyaGame>();
    }

    void Awake() { Instance = this; }

    /// <summary>Resta tiempo (por ejemplo, una roca) y lo muestra.</summary>
    public void Penalty(float seconds, string why)
    {
        if (!Playing) return;
        left = Mathf.Max(0f, left - seconds);
        flash = 1f;
        Note(why + "\n-" + Mathf.RoundToInt(seconds) + " segundos", 2.5f);
    }

    void Update()
    {
        if (gallery == null) gallery = GetComponent<GoyaGallery>();
        bool path = timer && gallery != null && gallery.pathMode && gallery.PathLength > 0f;
        if (!path) { Show(false); return; }
        if (hud == null) MakeHud();
        if (cave == null) cave = gallery.Cave;
        if (diorama == null) diorama = GetComponent<GoyaDiorama>();
        float dt = Time.deltaTime;

        // Arranca cuando aparece la galeria; si todo vuelve a empezar (brazos cruzados), se reinicia
        if (gallery.appear < 0.01f)
        {
            if (started) { started = over = false; formed.Clear(); gallery.Frozen = false; }
            Show(false);
            return;
        }
        if (!started)
        {
            if (gallery.appear < 0.99f) { Show(false); return; }
            started = true;
            over = false;
            left = duration;
            formed.Clear();
            Note("Tenés " + Clock(duration) + " para encontrar\ntodas las obras que puedas", 6f);
        }

        // Obras encontradas
        foreach (var p in gallery.paintings) if (!p.locked && p.revealed) formed.Add(p);
        int total = 0;
        foreach (var p in gallery.paintings) if (!p.locked) total++;
        int foundCount = formed.Count;
        if (cave != null) { total += cave.PaintingCount; foundCount += cave.FoundCount; }
        if (diorama != null) { total += diorama.Count; foundCount += diorama.Reached(gallery); }

        if (!over)
        {
            left -= dt;
            if (foundCount >= total && total > 0) { over = true; gallery.Frozen = false; Note("¡Encontraste todas las obras!\nTe sobraron " + Clock(left), 1e6f); }
            else if (left <= 0f)
            {
                left = 0f;
                over = true;
                gallery.Frozen = true;
                Note("Se terminó el tiempo\nEncontraste " + foundCount + " de " + total + " obras\n(cruzá los brazos para volver a empezar)", 1e6f);
            }
            // Tic cada segundo cuando queda poco
            if (!over && left < warnAt && Mathf.Floor(left) != lastTick)
            {
                lastTick = Mathf.Floor(left);
                if (tick != null) tickSource.PlayOneShot(tick, 0.6f);
            }
        }

        // Cartel arriba de la vista
        bool warn = left < warnAt && !over;
        flash = Mathf.MoveTowards(flash, 0f, dt * 1.2f);
        Color c = warn ? Color.Lerp(hudColor, warnColor, 0.5f + 0.5f * Mathf.Sin(Time.time * 6f)) : hudColor;
        c = Color.Lerp(c, warnColor, flash);
        c.a = gallery.appear;
        hud.color = c;
        hud.text = Clock(left) + "      " + foundCount + " / " + total + " obras";
        noteTimer -= dt;
        var nc = hudColor;
        nc.a = Mathf.Clamp01(noteTimer) * gallery.appear;
        note.color = nc;
        Show(true);
        Place();
    }

    void Note(string text, float seconds)
    {
        if (note == null) MakeHud();
        note.text = text;
        noteTimer = seconds;
    }

    static string Clock(float s)
    {
        int t = Mathf.CeilToInt(Mathf.Max(0f, s));
        return (t / 60) + ":" + (t % 60).ToString("00");
    }

    void MakeHud()
    {
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hud = Text("Juego_Reloj", font, 0.006f);
        note = Text("Juego_Aviso", font, 0.005f);
        note.text = "";
        tick = Resources.Load<AudioClip>("Sonidos/reloj_tic");
        tickSource = gameObject.AddComponent<AudioSource>();
        tickSource.spatialBlend = 0f;
        tickSource.playOnAwake = false;
    }

    TextMesh Text(string label, Font font, float size)
    {
        var go = new GameObject(label);
        go.transform.SetParent(transform, false);
        var t = go.AddComponent<TextMesh>();
        t.font = font;
        go.GetComponent<MeshRenderer>().sharedMaterial = font.material;
        t.fontSize = 64;
        t.characterSize = size;
        t.anchor = TextAnchor.MiddleCenter;
        t.alignment = TextAlignment.Center;
        t.color = new Color(1f, 1f, 1f, 0f);
        return t;
    }

    // Delante de la vista: el reloj arriba y los avisos un poco mas abajo
    void Place()
    {
        var head = gallery.Head;
        if (head == null) return;
        hud.transform.position = head.position + head.forward * hudDistance + head.up * hudHeight;
        hud.transform.rotation = head.rotation;
        note.transform.position = head.position + head.forward * hudDistance + head.up * (hudHeight - 0.18f);
        note.transform.rotation = head.rotation;
    }

    void Show(bool on)
    {
        if (hud != null) hud.gameObject.SetActive(on);
        if (note != null) note.gameObject.SetActive(on);
    }
}

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Efecto de particulas para los cuadros de GoyaGallery.
/// Cuando una MANO ABIERTA BARRE un cuadro ya revelado (se mueve rapido sobre el), el cuadro se deshace en particulas
/// (que conservan los colores de la pintura), flotan llevadas por el viento y despues
/// se rearman como la OBRA SIGUIENTE, mas oscura (ver "Obras mas oscuras" en GoyaGallery).
/// Si no hay obras oscuras cargadas, vuelven a formar el mismo cuadro.
///
/// Cuanto mas agitadas estan las manos, mas fuerte sopla el viento. Si se activa "Auto Break",
/// con agitacion alta sostenida los cuadros se deshacen solos.
///
/// Si la Galeria tiene "Assemble From Dust", las obras empiezan como polvo casi invisible en el aire
/// y al BARRERLAS se arman, al reves que al desarmarse.
///
/// Si la Galeria tiene "Change All Together", un barrido sobre cualquier obra arma (o desarma) TODAS:
/// sus particulas vuelan y se desvanecen, y la tanda siguiente se arma desde el aire.
///
/// Ponelo en el mismo objeto "Galeria".
/// En el editor: tecla T = deshacer todos los cuadros ya revelados (para probar sin manos).
/// </summary>
public class GoyaParticles : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Si queda vacio, lo busca solo")]
    public GoyaGallery gallery;

    [Header("Particulas")]
    [Tooltip("Particulas por cuadro. Con 8 obras flotando como polvo, si el celular anda lento bajalo (ej. 2500)")]
    public int particleCount = 4000;
    public float particleScale = 1.8f;

    [Header("Toque (barrer con la mano abierta: la velocidad y el tiempo estan en GoyaGallery)")]
    [Tooltip("Segundos de espera antes de poder tocarlo de nuevo")]
    public float cooldown = 1f;

    [Header("Deshacerse")]
    [Tooltip("Tiempo que tarda la onda de desintegracion en recorrer el cuadro")]
    public float breakWaveTime = 1.8f;
    [Tooltip("Tiempo total que las particulas flotan antes de volver")]
    public float scatterTime = 4.5f;
    public float scatterSpeed = 0.9f;
    [Tooltip("Direccion del viento que se lleva las particulas")]
    public Vector3 wind = new Vector3(0.5f, 0.35f, 0.2f);
    public float drag = 1.2f;

    [Header("Rehacerse")]
    public float reformTime = 3.5f;
    [Tooltip("Retraso aleatorio maximo: que no vuelvan todas al mismo tiempo")]
    public float reformSpread = 1.5f;

    [Header("Magia negra (cambio de tanda)")]
    [Tooltip("Color del humo en que se convierten las obras al desarmarse")]
    public Color smokeColor = new Color(0.12f, 0.03f, 0.04f, 1f);
    [Tooltip("Que tan rapido gira el humo alrededor de cada obra")]
    public float swirl = 0.6f;
    [Tooltip("Segundos de vacio, sin obras, antes de que se arme la tanda siguiente")]
    public float batchPause = 1.2f;

    [Header("Agitacion (movimiento de las manos)")]
    [Tooltip("Con agitacion maxima, el viento sopla esta cantidad de veces mas fuerte")]
    public float agitationWind = 2.5f;
    [Tooltip("Que los cuadros revelados se deshagan solos con las manos muy agitadas")]
    public bool autoBreak = false;
    [Tooltip("Por encima de esta agitacion (0 a 1) los cuadros revelados se deshacen solos")]
    [Range(0f, 1f)] public float autoBreakAgitation = 0.75f;
    [Tooltip("Segundos de agitacion alta para que un cuadro se deshaga solo")]
    public float autoBreakTime = 1.5f;

    [Header("Polvo en el aire (obras sin armar)")]
    [Tooltip("Que tan desparramado esta el polvo alrededor del cuadro (multiplica el tamaño del cuadro)")]
    public float airSpread = 3f;
    [Tooltip("Profundidad de la nube de polvo (metros, hacia adelante y atras del cuadro)")]
    public float airDepth = 3f;
    [Tooltip("Cuanto se apagan los colores mientras es polvo (0 = colores de la obra, 1 = gris del aire)")]
    [Range(0f, 1f)] public float airGray = 0.9f;
    [Tooltip("Que tanto se ve el polvo antes de armarse (0 = invisible)")]
    [Range(0f, 1f)] public float airAlpha = 0.1f;
    [Tooltip("Tamaño de las particulas mientras son polvo (multiplica el tamaño normal)")]
    public float airSize = 0.4f;
    [Tooltip("Cuanto se mece el polvo (metros)")]
    public float airDrift = 0.35f;

    public enum EffectState { Idle, Scatter, Reform, Floating }

    class Effect
    {
        public GoyaGallery.Painting painting;
        public GameObject go;
        public ParticleSystem ps;
        public ParticleSystem.Particle[] parts;
        public int count;
        public float size;
        public Vector3[] target, pos, vel, startPos, floatBase;
        public Color32[] fromColor, toColor;
        public Sprite sampled;     // obra con la que se calcularon target y toColor
        public float[] delay, seed;
        public bool[] released;
        public EffectState state;
        public float t;
        public float touch;
        public float agitated;
        public float nextAllowed;
        public bool assembling;    // el Reform actual es el primer armado desde el polvo
    }

    // Avisos para otros scripts (por ejemplo el sonido de GoyaAmbience)
    public System.Action<GoyaGallery.Painting> OnBreak;
    public System.Action<GoyaGallery.Painting> OnReform;

    readonly List<Effect> effects = new List<Effect>();
    Material particleMat;
    bool built;
    bool batchBreaking;  // la tanda actual se esta desvaneciendo
    float batchTimer;

    void Update()
    {
        if (gallery == null) gallery = FindFirstObjectByType<GoyaGallery>();
        if (gallery == null) return;

        if (!built)
        {
            if (gallery.paintings.Count == 0) return;
            Build();
            if (!built) return;
        }

        float dt = Time.deltaTime;
        bool forceAll = Input.GetKeyDown(KeyCode.T);
        float agitation = gallery.agitation;
        bool together = gallery.changeAllTogether;
        if (together) UpdateTogether(forceAll);

        foreach (var e in effects)
        {
            var p = e.painting;
            switch (e.state)
            {
                case EffectState.Idle:
                    if (!p.active) break;   // cuadro que sobra en esta tanda
                    // Sin armar (al empezar, o despues de Backspace): vuelve a ser polvo
                    if (gallery.assembleFromDust && !p.revealed) { StartFloating(e); break; }
                    if (together) break;
                    if (forceAll && p.revealed) { Begin(e); break; }

                    bool ready = p.revealed && Time.time >= e.nextAllowed;
                    if (ready && p.swipe >= gallery.swipeDistance) { Begin(e); break; }

                    // Agitacion alta sostenida: el cuadro se deshace solo (si esta activado)
                    if (autoBreak && ready && agitation >= autoBreakAgitation)
                    {
                        e.agitated += dt * Random.Range(0.6f, 1.4f);
                        if (e.agitated >= autoBreakTime) Begin(e);
                    }
                    else
                    {
                        e.agitated = Mathf.Max(0f, e.agitated - dt);
                    }
                    break;

                case EffectState.Scatter:
                    UpdateScatter(e, dt);
                    break;

                case EffectState.Reform:
                    UpdateReform(e, dt);
                    break;

                case EffectState.Floating:
                    if (!together && (forceAll || p.swipe >= gallery.swipeDistance)) { BeginAssemble(e); break; }
                    UpdateFloating(e);
                    break;
            }
        }
    }

    // Todas las obras juntas: un barrido sobre cualquiera arma todo el polvo,
    // o desarma todas las obras armadas para que se arme la tanda siguiente
    void UpdateTogether(bool forceAll)
    {
        if (batchBreaking)
        {
            // Cuando el humo ya se desvanecio y paso la pausa, se arma la tanda siguiente
            batchTimer += Time.deltaTime;
            if (batchTimer >= scatterTime + batchPause) SwapBatch();
            return;
        }

        bool allFloating = true, allReady = true, swiped = forceAll;
        foreach (var e in effects)
        {
            var p = e.painting;
            if (!p.active) continue;
            if (e.state != EffectState.Floating) allFloating = false;
            if (e.state != EffectState.Idle || !p.revealed || Time.time < e.nextAllowed) allReady = false;
            if (p.swipe >= gallery.swipeDistance) swiped = true;
        }
        if (!swiped || effects.Count == 0) return;

        if (allFloating)
        {
            foreach (var e in effects) BeginAssemble(e);
        }
        else if (allReady && gallery.HasNextBatch())
        {
            batchBreaking = true;
            batchTimer = 0f;
            foreach (var e in effects) if (e.painting.active) Begin(e);
        }
        foreach (var e in effects) e.painting.swipe = 0f;
    }

    void SwapBatch()
    {
        batchBreaking = false;
        gallery.NextBatch();
        foreach (var e in effects)
        {
            if (!e.painting.active) { Hide(e); continue; }
            // La obra nueva se arma desde el humo, dispersa en el aire
            if (!Sample(e)) continue;
            AirPositions(e, e.startPos);
            Color32 smoke = smokeColor;
            for (int i = 0; i < e.count; i++) { e.fromColor[i] = smoke; e.fromColor[i].a = 0; }
            e.painting.renderer.enabled = false;
            e.go.SetActive(true);
            e.ps.Play();
            e.assembling = true;
            e.state = EffectState.Reform;
            e.t = 0f;
            if (OnReform != null) OnReform(e.painting);
        }
    }

    void Hide(Effect e)
    {
        e.ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        e.go.SetActive(false);
        e.painting.renderer.enabled = false;
        e.assembling = false;
        e.state = EffectState.Idle;
    }

    // ---------------------------------------------------------------- construccion

    void Build()
    {
        Shader sh = Shader.Find("Sprites/Default");
        if (sh == null) sh = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (sh == null) sh = Shader.Find("Particles/Standard Unlit");
        if (sh == null)
        {
            Debug.LogError("[Particulas] No encuentro un shader para las particulas.");
            enabled = false;
            return;
        }

        particleMat = new Material(sh);
        particleMat.mainTexture = MakeSoftTexture();

        foreach (var p in gallery.paintings)
        {
            var e = CreateEffect(p);
            if (e != null) effects.Add(e);
        }
        built = true;
    }

    static Texture2D MakeSoftTexture()
    {
        const int n = 32;
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < n; x++)
            {
                float dx = (x + 0.5f) / n * 2f - 1f;
                float dy = (y + 0.5f) / n * 2f - 1f;
                float a = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Sqrt(a)));
            }
        }
        tex.Apply();
        return tex;
    }

    Effect CreateEffect(GoyaGallery.Painting p)
    {
        int n = Mathf.Max(1, particleCount);

        var e = new Effect
        {
            painting = p,
            count = n,
            parts = new ParticleSystem.Particle[n],
            target = new Vector3[n],
            pos = new Vector3[n],
            vel = new Vector3[n],
            startPos = new Vector3[n],
            floatBase = new Vector3[n],
            fromColor = new Color32[n],
            toColor = new Color32[n],
            delay = new float[n],
            seed = new float[n],
            released = new bool[n]
        };
        for (int i = 0; i < n; i++) e.seed[i] = Random.value;

        if (!Sample(e)) return null;

        // Sistema de particulas (solo se usa para dibujar: las posiciones las manejamos nosotros)
        e.go = new GameObject("Particulas_" + p.name);
        e.go.transform.SetParent(transform, false);
        e.ps = e.go.AddComponent<ParticleSystem>();
        e.ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = e.ps.main;
        main.loop = false;
        main.playOnAwake = false;
        main.maxParticles = n;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = 1000f;
        main.startSpeed = 0f;
        main.startSize = e.size;

        var emission = e.ps.emission;
        emission.enabled = false;
        var shape = e.ps.shape;
        shape.enabled = false;

        var rend = e.go.GetComponent<ParticleSystemRenderer>();
        rend.sharedMaterial = particleMat;
        rend.renderMode = ParticleSystemRenderMode.Billboard;
        rend.sortingOrder = 5;

        e.go.SetActive(false);
        return e;
    }

    // Calcula donde va cada particula y de que color es, segun la obra que muestra el cuadro ahora
    bool Sample(Effect e)
    {
        var p = e.painting;
        var sprite = p.renderer.sprite;
        var pixels = GetPixels(sprite);
        if (pixels == null) return false;

        int n = e.count;
        float w = p.halfW * 2f;
        float h = p.halfH * 2f;
        int cols = Mathf.Max(1, Mathf.RoundToInt(Mathf.Sqrt(n * (w / h))));
        int rows = Mathf.Max(1, Mathf.CeilToInt((float)n / cols));
        e.size = Mathf.Max(0.008f, Mathf.Sqrt(w * h / n) * particleScale);

        Vector3 center = p.root.position;
        Vector3 right = p.root.right;
        Vector3 up = p.root.up;

        for (int i = 0; i < n; i++)
        {
            int col = i % cols;
            int row = i / cols;
            float fu = (col + Random.value) / cols;
            float fv = Mathf.Clamp01((row + Random.value) / rows);
            e.target[i] = center + right * ((fu - 0.5f) * w) + up * ((fv - 0.5f) * h);
            e.toColor[i] = pixels.GetPixelBilinear(fu, fv);
        }

        e.sampled = sprite;
        return true;
    }

    // Copia chica de cada obra para leer sus colores (asi no hace falta tildar Read/Write)
    readonly Dictionary<Sprite, Texture2D> pixelCache = new Dictionary<Sprite, Texture2D>();

    Texture2D GetPixels(Sprite sprite)
    {
        if (sprite == null) return null;
        Texture2D cached;
        if (pixelCache.TryGetValue(sprite, out cached)) return cached;

        const int maxSide = 512;
        Rect r = sprite.rect;
        Texture2D src = sprite.texture;
        float k = Mathf.Min(1f, maxSide / Mathf.Max(r.width, r.height));
        int w = Mathf.Max(1, Mathf.RoundToInt(r.width * k));
        int h = Mathf.Max(1, Mathf.RoundToInt(r.height * k));

        var rt = RenderTexture.GetTemporary(w, h, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        var scale = new Vector2(r.width / src.width, r.height / src.height);
        var offset = new Vector2(r.x / src.width, r.y / src.height);
        Graphics.Blit(src, rt, scale, offset);

        var prev = RenderTexture.active;
        RenderTexture.active = rt;
        var copy = new Texture2D(w, h, TextureFormat.RGBA32, false, false);
        copy.ReadPixels(new Rect(0, 0, w, h), 0, 0);
        copy.Apply();
        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(rt);

        copy.wrapMode = TextureWrapMode.Clamp;
        pixelCache[sprite] = copy;
        return copy;
    }

    // ---------------------------------------------------------------- efecto

    void Begin(Effect e)
    {
        var p = e.painting;
        // Si la obra cambio por otro lado (por ejemplo al reiniciar con Backspace), recalcular
        if (e.sampled != p.renderer.sprite && !Sample(e)) return;
        Color tint = gallery.CurrentTint();
        for (int i = 0; i < e.count; i++) e.fromColor[i] = Tinted(e.toColor[i], tint);

        Vector3 hit = p.hitPoint == Vector3.zero ? p.root.position : p.hitPoint;

        // La onda de desintegracion arranca donde se toco y se expande
        float maxD = 0.0001f;
        for (int i = 0; i < e.count; i++)
        {
            e.delay[i] = Vector3.Distance(e.target[i], hit);
            if (e.delay[i] > maxD) maxD = e.delay[i];
        }
        for (int i = 0; i < e.count; i++)
        {
            e.delay[i] = e.delay[i] / maxD * breakWaveTime;
            e.pos[i] = e.target[i];
            e.vel[i] = Vector3.zero;
            e.released[i] = false;
        }

        for (int i = 0; i < e.count; i++) e.parts[i].startColor = e.fromColor[i];
        p.renderer.enabled = false;   // el cuadro "real" se oculta: ahora son solo particulas
        e.go.SetActive(true);
        e.ps.Play();
        Write(e);

        e.state = EffectState.Scatter;
        if (OnBreak != null) OnBreak(p);
        e.t = 0f;
        e.touch = 0f;
        e.agitated = 0f;
        p.swipe = 0f;
    }

    void UpdateScatter(Effect e, float dt)
    {
        e.t += dt;
        // Toda la tanda junta: las particulas se vuelven humo oscuro, giran y se desvanecen
        bool together = gallery.changeAllTogether;
        float fadeStart = breakWaveTime * 0.5f;
        float k = Mathf.Clamp01((e.t - fadeStart) / Mathf.Max(0.01f, scatterTime - fadeStart));
        float fade = together ? 1f - k * k : 1f;
        float dark = together ? Mathf.Clamp01(e.t / Mathf.Max(0.01f, scatterTime * 0.5f)) : 0f;
        Color32 smoke = smokeColor;
        Vector3 axis = e.painting.root.position;
        Vector3 towardViewer = -e.painting.root.forward;
        Vector3 gust = wind * (agitationWind - 1f) * gallery.agitation * dt;

        for (int i = 0; i < e.count; i++)
        {
            if (!e.released[i])
            {
                if (e.t < e.delay[i]) continue;
                e.released[i] = true;
                e.vel[i] = Random.onUnitSphere * (scatterSpeed * 0.5f)
                         + wind * (0.5f + e.seed[i])
                         + towardViewer * (0.3f * e.seed[i]);
            }

            float s = e.seed[i] * 20f;
            e.vel[i] += new Vector3(Mathf.Sin(e.t * 2f + s),
                                    Mathf.Cos(e.t * 1.7f + s * 0.7f),
                                    Mathf.Sin(e.t * 1.3f + s * 0.4f)) * (0.4f * dt);
            e.vel[i] += gust * (0.5f + e.seed[i]);
            if (together)
            {
                // Remolino alrededor del eje vertical de la obra, subiendo como humo
                Vector3 r = e.pos[i] - axis; r.y = 0f;
                e.vel[i] += (Vector3.Cross(Vector3.up, r) * swirl + Vector3.up * 0.15f) * dt;
            }
            e.pos[i] += e.vel[i] * dt;
            e.vel[i] *= Mathf.Max(0f, 1f - drag * dt);
            if (together)
            {
                Color32 c = Color32.Lerp(e.fromColor[i], smoke, dark);
                c.a = (byte)(e.fromColor[i].a * fade);
                e.parts[i].startColor = c;
            }
        }

        Write(e);

        if (e.t >= scatterTime)
        {
            // Toda la tanda junta: el humo sigue flotando invisible hasta que UpdateTogether arma la siguiente
            if (together) return;

            for (int i = 0; i < e.count; i++) e.startPos[i] = e.pos[i];

            // Elegir la obra con la que se rearma (mas oscura) y recalcular hacia donde van
            var next = gallery.PickNextSprite(e.painting);
            if (next != null)
            {
                gallery.SetSprite(e.painting, next);
                Sample(e);
            }

            e.state = EffectState.Reform;
            if (OnReform != null) OnReform(e.painting);
            e.t = 0f;
        }
    }

    // ---------------------------------------------------------------- polvo y armado

    void StartFloating(Effect e)
    {
        var p = e.painting;
        if (!Sample(e)) return;   // siempre: en tandas el cuadro pudo cambiar de lugar

        AirPositions(e, e.floatBase);
        for (int i = 0; i < e.count; i++) e.fromColor[i] = AirColor(e.toColor[i]);

        p.renderer.enabled = false;
        e.go.SetActive(true);
        e.ps.Play();
        e.t = Random.value * 10f;
        e.state = EffectState.Floating;
        UpdateFloating(e);
    }

    // Lugares al azar en una nube grande alrededor del cuadro: asi el polvo se mezcla con el aire
    void AirPositions(Effect e, Vector3[] into)
    {
        var p = e.painting;
        Vector3 right = p.root.right, up = p.root.up, fwd = p.root.forward;
        float w = p.halfW * 2f * airSpread, h = p.halfH * 2f * airSpread;
        for (int i = 0; i < e.count; i++)
        {
            into[i] = e.target[i]
                    + right * ((Random.value - 0.5f) * w)
                    + up * ((Random.value - 0.5f) * h + 0.15f)
                    + fwd * ((Random.value - 0.5f) * airDepth);
        }
    }

    Color32 AirColor(Color32 c)
    {
        var air = new Color32(200, 205, 225, 255);
        Color32 r = Color32.Lerp(c, air, airGray);
        r.a = (byte)(255 * airAlpha);
        return r;
    }

    void UpdateFloating(Effect e)
    {
        e.t += Time.deltaTime;
        float appear = gallery.appear;
        for (int i = 0; i < e.count; i++)
        {
            float s = e.seed[i] * 20f;
            float tt = e.t * (0.25f + 0.2f * e.seed[i]);
            e.pos[i] = e.floatBase[i] + new Vector3(Mathf.Sin(tt + s),
                                                    Mathf.Sin(tt * 0.8f + s * 0.7f),
                                                    Mathf.Cos(tt * 0.6f + s * 0.4f)) * airDrift;
            Color32 c = e.fromColor[i];
            c.a = (byte)(c.a * appear);
            e.parts[i].startColor = c;
        }
        Write(e, airSize);
    }

    void BeginAssemble(Effect e)
    {
        for (int i = 0; i < e.count; i++) e.startPos[i] = e.pos[i];
        e.painting.swipe = 0f;
        e.assembling = true;
        e.state = EffectState.Reform;
        if (OnReform != null) OnReform(e.painting);
        e.t = 0f;
    }

    static Color32 Tinted(Color32 c, Color tint)
    {
        return new Color32((byte)(c.r * tint.r), (byte)(c.g * tint.g), (byte)(c.b * tint.b), c.a);
    }

    void UpdateReform(Effect e, float dt)
    {
        e.t += dt;
        Color tint = gallery.CurrentTint();

        for (int i = 0; i < e.count; i++)
        {
            float k = Mathf.Clamp01((e.t - e.seed[i] * reformSpread) / reformTime);
            k = k * k * (3f - 2f * k);

            float s = e.seed[i] * 20f;
            Vector3 wobble = new Vector3(Mathf.Sin(e.t * 3f + s),
                                         Mathf.Cos(e.t * 2.4f + s * 0.7f),
                                         Mathf.Sin(e.t * 1.9f + s * 0.4f)) * (0.12f * (1f - k));

            e.pos[i] = Vector3.Lerp(e.startPos[i], e.target[i], k) + wobble;
            e.parts[i].startColor = Color32.Lerp(e.fromColor[i], Tinted(e.toColor[i], tint), k);
        }

        float progress = Mathf.Clamp01(e.t / (reformSpread + reformTime));
        Write(e, e.assembling ? Mathf.Lerp(airSize, 1f, progress) : 1f);
        if (e.assembling) e.painting.reveal = progress;

        if (e.t >= reformSpread + reformTime)
        {
            // Termino: volver a mostrar el cuadro y apagar las particulas
            if (e.assembling) gallery.MarkRevealed(e.painting);
            e.painting.swipe = 0f;   // que el mismo barrido no la desarme apenas se arma
            e.assembling = false;
            e.painting.renderer.enabled = true;
            e.ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            e.go.SetActive(false);
            e.state = EffectState.Idle;
            e.nextAllowed = Time.time + cooldown;
            e.touch = 0f;
        }
    }

    void Write(Effect e, float sizeMul = 1f)
    {
        for (int i = 0; i < e.count; i++)
        {
            e.parts[i].position = e.pos[i];
            e.parts[i].velocity = Vector3.zero;
            e.parts[i].startLifetime = 1000f;
            e.parts[i].remainingLifetime = 1000f;
            e.parts[i].startSize = e.size * sizeMul;
        }
        e.ps.SetParticles(e.parts, e.count);
    }
}

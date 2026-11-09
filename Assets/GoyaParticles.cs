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

    [Tooltip("En el camino, el polvo de las obras mas lejanas que esto (metros) no se dibuja, para que ande fluido")]
    public float visibleDistance = 10f;

    [Header("Toque (barrer con la mano abierta: la velocidad y el tiempo estan en GoyaGallery)")]
    [Tooltip("Segundos de espera antes de poder tocarlo de nuevo")]
    public float cooldown = 1f;

    [Header("Deshacerse")]
    [Tooltip("Tiempo que tarda la onda de desintegracion en recorrer el cuadro")]
    public float breakWaveTime = 1.8f;
    [Tooltip("Tiempo total que las particulas flotan antes de volver")]
    public float scatterTime = 3.5f;
    public float scatterSpeed = 0.9f;
    [Tooltip("Direccion del viento que se lleva las particulas")]
    public Vector3 wind = new Vector3(0.5f, 0.35f, 0.2f);
    public float drag = 1.2f;

    [Header("Rehacerse")]
    public float reformTime = 3.5f;
    [Tooltip("Retraso aleatorio maximo: que no vuelvan todas al mismo tiempo")]
    public float reformSpread = 1.5f;

    [Header("Magia negra (cambio de obra)")]
    [Tooltip("Color de brasa en que se convierten las obras al desarmarse")]
    public Color emberSmoke = new Color(0.9f, 0.16f, 0.05f, 1f);
    [Tooltip("Que tan visible queda el remolino de brasas (0 a 1)")]
    [Range(0f, 1f)] public float emberAlpha = 0.8f;
    [Tooltip("Cuanto se tiñen de color brasa las particulas al desarmarse: 0 = conservan los colores de la obra, 1 = brasa roja (como antes)")]
    [Range(0f, 1f)] public float emberMix = 0f;
    [Tooltip("Que tan rapido gira el remolino alrededor de cada obra")]
    public float swirl = 0.6f;
    [Tooltip("Segundos extra que gira el remolino antes de viajar hacia la obra siguiente")]
    public float swirlPause = 0.6f;
    [Tooltip("Las brasas viajan por el aire hasta donde aparece la obra siguiente (si se apaga: se desvanecen y la nueva se arma desde el aire)")]
    public bool travel = true;
    [Tooltip("Cuanto suben las brasas en el medio del viaje (metros)")]
    public float travelArc = 0.4f;

    [Header("Agitacion (movimiento de las manos)")]
    [Tooltip("Con agitacion maxima, el viento sopla esta cantidad de veces mas fuerte")]
    public float agitationWind = 2.5f;
    [Tooltip("Que los cuadros revelados se deshagan solos con las manos muy agitadas")]
    public bool autoBreak = false;
    [Tooltip("Por encima de esta agitacion (0 a 1) los cuadros revelados se deshacen solos")]
    [Range(0f, 1f)] public float autoBreakAgitation = 0.75f;
    [Tooltip("Segundos de agitacion alta para que un cuadro se deshaga solo")]
    public float autoBreakTime = 1.5f;

    [Header("Remolino (camino): como en el video de referencia")]
    [Tooltip("En el camino las obras salen del suelo, de un arbol o de una roca, giran en un remolino y se arman (y al desarmarse vuelven)")]
    public bool vortex = true;
    [Tooltip("Tamaño del remolino (multiplica el tamaño de la obra)")]
    public float vortexSize = 0.8f;
    [Tooltip("Vueltas por segundo del remolino (radianes)")]
    public float vortexSpeed = 2.6f;
    [Tooltip("Cuanto se acerca el remolino a la mano (0 = en la obra, 1 = en la mano)")]
    [Range(0f, 1f)] public float towardHand = 0.4f;
    [Tooltip("Cintas doradas que giran con el remolino")]
    public int vortexRibbons = 6;
    public Color ribbonColor = new Color(1f, 0.8f, 0.4f, 0.9f);
    [Tooltip("Que tan estiradas se ven las particulas al girar (como trazos)")]
    public float streak = 0.05f;

    [Header("Polvo en el aire (obras sin armar)")]
    [Tooltip("Que tan desparramado esta el polvo alrededor del cuadro (multiplica el tamaño del cuadro)")]
    public float airSpread = 3f;
    [Tooltip("Profundidad de la nube de polvo (metros, hacia adelante y atras del cuadro)")]
    public float airDepth = 3f;
    [Tooltip("Cuanto se apagan los colores mientras es polvo (0 = colores de la obra, 1 = gris del aire)")]
    [Range(0f, 1f)] public float airGray = 0.9f;
    [Tooltip("Que tanto se ve el polvo antes de armarse (0 = invisible)")]
    [Range(0f, 1f)] public float airAlpha = 0f;
    [Tooltip("Tamaño de las particulas mientras son polvo (multiplica el tamaño normal)")]
    public float airSize = 0.4f;
    [Tooltip("Cuanto se mece el polvo (metros)")]
    public float airDrift = 0.35f;

    [Header("Obras vivas (camino): la obra armada sigue siendo particulas")]
    [Tooltip("Al armarse, la obra no vuelve a ser un cuadro plano: queda hecha de particulas con volumen, que respiran, se mecen con el aire, se deshilachan en los bordes y se apartan de la mano")]
    public bool livingPaintings = true;
    [Tooltip("Volumen: cuanto salen hacia adelante las partes claras (metros, se multiplica por el tamaño de la obra)")]
    public float livingDepth = 0.22f;
    [Tooltip("Ancho del borde que se deshace en el aire (proporcion del cuadro)")]
    [Range(0f, 0.4f)] public float edgeDissolve = 0.16f;
    [Tooltip("Cuanto se alejan del cuadro las particulas del borde (metros)")]
    public float edgeDrift = 0.45f;
    [Tooltip("Fuerza del resorte que lleva cada particula a su lugar")]
    public float spring = 7f;
    [Tooltip("Freno de cada particula (con poco freno tiemblan mas)")]
    public float damping = 2.6f;
    [Tooltip("Fuerza del aire que las mece (un campo de corrientes que cambia con el tiempo)")]
    public float flow = 0.9f;
    [Tooltip("Fuerza con que la mano (o el puntero) aparta las particulas")]
    public float handPush = 6f;
    [Tooltip("Radio de la mano que aparta (metros)")]
    public float handRadius = 0.45f;
    [Tooltip("Las obras vivas mas lejanas que esto (metros) no se simulan ni se dibujan")]
    public float livingDistance = 14f;
    [Tooltip("Puntos a lo alto de la tela de particulas que lleva la imagen (mas = mas suave, mas pesado)")]
    public int sheetResolution = 56;
    [Tooltip("Fuerza de los resortes entre puntos vecinos de la tela (asi el empujon de la mano se propaga como una onda)")]
    public float neighborSpring = 14f;
    [Tooltip("Tamaño de las particulas sueltas del borde (multiplica el tamaño normal)")]
    public float edgeParticleSize = 0.45f;
    [Tooltip("Obras que quedan como cuadro (las animadas)")]
    public string[] keepFlat = { "12 El pelele", "18 Vuelo de brujas" };

    public enum EffectState { Idle, Scatter, Reform, Floating, Living }

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
        public Vector2[] uv;       // lugar de cada particula en la obra (0 a 1)
        public Vector3[] home;     // lugar de cada particula en la obra viva (con volumen)
        public float[] loose;      // 0 = adentro de la obra, 1 = en el borde (se deshace en el aire)
        // Tela: una malla de particulas unidas por resortes que lleva la imagen con toda su definicion
        public GameObject sheetGo;
        public Mesh sheet;
        public Sprite sheetSprite;
        public int sx, sy;
        public Vector3[] sHome, sPos, sVel, sRest;
        public float[] sEdge;
        public Color[] sColor;
        public float[] delay, seed;
        public bool[] released;
        public EffectState state;
        public float t;
        public float touch;
        public float agitated;
        public float nextAllowed;
        public bool assembling;    // el Reform actual es el primer armado desde el polvo
        public bool traveling;     // el Reform actual viaja en arco alrededor de la persona
        // Remolino (camino): la obra nace del suelo/arbol/roca, gira alrededor de la mano y se arma
        public bool vortex;
        public Vector3 center;
        public Vector3[] orbU, orbV, sink;
        public float[] orbR, orbW;
        public Color32[] srcColor;
        public TrailRenderer[] ribbons;
    }

    // Avisos para otros scripts (por ejemplo el sonido de GoyaAmbience)
    public System.Action<GoyaGallery.Painting> OnBreak;
    public System.Action<GoyaGallery.Painting> OnReform;

    readonly List<Effect> effects = new List<Effect>();
    Material particleMat, ribbonMat;
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
        bool path = gallery.pathMode && gallery.PathLength > 0f;
        bool together = gallery.changeAllTogether && !path;
        if (together) UpdateTogether(forceAll);
        if (path) { UpdatePath(forceAll); forceAll = false; }

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
                    if (ready && gallery.swipeGestures && p.swipe >= gallery.swipeDistance) { Begin(e); break; }

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

                case EffectState.Living:
                    // Sin armar (Backspace): vuelve a ser polvo
                    if (gallery.assembleFromDust && !p.revealed) { StartFloating(e); break; }
                    UpdateLiving(e, dt);
                    break;

                case EffectState.Floating:
                    if (!together && (forceAll || (gallery.swipeGestures && p.swipe >= gallery.swipeDistance))) { BeginAssemble(e); break; }
                    // En el camino solo se anima el polvo de las obras cercanas (son muchas); si es invisible, ninguna
                    if (airAlpha <= 0f || (path && Vector3.Distance(p.root.position, gallery.RigPosition) > visibleDistance))
                    {
                        if (e.go.activeSelf) e.go.SetActive(false);
                        break;
                    }
                    if (!e.go.activeSelf) { e.go.SetActive(true); e.ps.Play(); }
                    UpdateFloating(e);
                    break;
            }
        }
    }

    // Camino: los gestos actuan sobre la obra que se esta mirando.
    // Dos brazos arriba arman su polvo; dos puños la desarman en brasas que vuelven a ser polvo.
    void UpdatePath(bool forceAll)
    {
        bool form = gallery.ConsumeForm() || forceAll;
        bool brk = gallery.ConsumeBreak() || forceAll;
        foreach (var e in effects) e.painting.swipe = 0f;
        if (!form && !brk) return;

        var target = gallery.PaintingInView(gallery.gestureDistance);
        if (target == null || target.locked) return;   // las de la cueva no se arman ni se desarman
        foreach (var e in effects)
        {
            if (e.painting != target) continue;
            if (form && e.state == EffectState.Floating)
            {
                // Tapada por la bandada de murcielagos: el gesto los asusta pero la obra no se arma
                var bats = GoyaFlock.Instance;
                if (bats != null && bats.Covered(target)) { bats.Scare(target.root.position, 1.8f, 0.6f); return; }
                if (!e.go.activeSelf) { e.go.SetActive(true); e.ps.Play(); }
                BeginAssemble(e);
            }
            else if (brk && (e.state == EffectState.Idle || e.state == EffectState.Living) && target.revealed && Time.time >= e.nextAllowed)
                Begin(e);
            return;
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
            gallery.ConsumeForm();    // un gesto durante el cambio no queda guardado para despues
            gallery.ConsumeBreak();
            if (batchTimer >= scatterTime + swirlPause) SwapBatch();
            return;
        }

        bool form = gallery.ConsumeForm(), brk = gallery.ConsumeBreak();
        bool allFloating = true, allReady = true, swiped = false;
        foreach (var e in effects)
        {
            var p = e.painting;
            if (!p.active) continue;
            if (e.state != EffectState.Floating) allFloating = false;
            if (e.state != EffectState.Idle || !p.revealed || Time.time < e.nextAllowed) allReady = false;
            if (gallery.swipeGestures && p.swipe >= gallery.swipeDistance) swiped = true;
        }
        if (effects.Count == 0) return;
        if (!gallery.swipeGestures) foreach (var e in effects) e.painting.swipe = 0f;
        if (!(forceAll || form || brk || swiped)) return;

        if (allFloating)
        {
            if (forceAll || form || swiped) foreach (var e in effects) BeginAssemble(e);
        }
        else if (allReady && (forceAll || brk || swiped) && gallery.HasNextBatch())
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
            bool wasFlying = e.state == EffectState.Scatter;
            if (!Sample(e)) continue;
            Color32 smoke = emberSmoke;
            bool keepColors = false;
            if (travel && wasFlying)
            {
                keepColors = true;
                // Las brasas del remolino viajan hasta la obra nueva
                for (int i = 0; i < e.count; i++) e.startPos[i] = e.pos[i];
                smoke.a = (byte)(255 * emberAlpha);
            }
            else
            {
                // La obra nueva se arma desde el aire
                AirPositions(e, e.startPos);
                smoke.a = 0;
            }
            for (int i = 0; i < e.count; i++)
            {
                if (keepColors)
                {
                    // Conservan el color que tenian al desarmarse (mezclado con la brasa segun emberMix)
                    Color32 c = Color32.Lerp(e.parts[i].startColor, smoke, emberMix);
                    c.a = smoke.a;
                    e.fromColor[i] = c;
                }
                else e.fromColor[i] = smoke;
            }
            e.traveling = travel && wasFlying;
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
        HideSheet(e);
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
            if (e.uv == null || e.uv.Length != n) e.uv = new Vector2[n];
            e.uv[i] = new Vector2(fu, fv);
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
        HideSheet(e);
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
        if (UseVortex(e))
        {
            SetupVortex(e);
            for (int i = 0; i < e.count; i++) e.startPos[i] = e.target[i];
        }
        if (OnBreak != null) OnBreak(p);
        e.t = 0f;
        e.touch = 0f;
        e.agitated = 0f;
        p.swipe = 0f;
    }

    void UpdateScatter(Effect e, float dt)
    {
        if (e.vortex) { UpdateVortex(e, dt, false); return; }
        e.t += dt;
        // Toda la tanda junta (o en el camino): las particulas se vuelven humo de brasas, giran y se desvanecen
        bool path = gallery.pathMode && gallery.PathLength > 0f;
        bool travelNext = gallery.changeAllTogether && !path;   // las brasas viajan a la obra siguiente
        bool together = travelNext || path;
        float fadeStart = breakWaveTime * 0.5f;
        float k = Mathf.Clamp01((e.t - fadeStart) / Mathf.Max(0.01f, scatterTime - fadeStart));
        float fade = !together ? 1f : (travelNext && travel ? Mathf.Lerp(1f, emberAlpha, k) : 1f - k * k);
        float dark = together ? Mathf.Clamp01(e.t / Mathf.Max(0.01f, scatterTime * 0.5f)) * emberMix : 0f;
        Color32 smoke = emberSmoke;
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
            if (travelNext) return;

            // Camino: la obra vuelve a ser polvo en el aire, y se puede armar otra vez
            if (path)
            {
                e.painting.revealed = false;
                e.painting.reveal = 0f;
                e.nextAllowed = Time.time + cooldown;
                StartFloating(e);
                return;
            }

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
        HideSheet(e);
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
        if (!e.go.activeSelf) { e.go.SetActive(true); e.ps.Play(); }
        if (UseVortex(e))
        {
            SetupVortex(e);
            for (int i = 0; i < e.count; i++)
            {
                e.startPos[i] = e.pos[i] = e.sink[i];   // sale del suelo, del arbol o de la roca
                e.fromColor[i] = e.srcColor[i];
            }
        }
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
        if (e.vortex) { UpdateVortex(e, dt, true); return; }
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

            e.pos[i] = (e.traveling ? ArcLerp(e.startPos[i], e.target[i], k) : Vector3.Lerp(e.startPos[i], e.target[i], k)) + wobble;
            e.parts[i].startColor = Color32.Lerp(e.fromColor[i], Tinted(e.toColor[i], tint), k);
        }

        float progress = Mathf.Clamp01(e.t / (reformSpread + reformTime));
        Write(e, e.assembling && !e.traveling ? Mathf.Lerp(airSize, 1f, progress) : 1f);
        if (e.assembling) e.painting.reveal = progress;

        if (e.t >= reformSpread + reformTime)
        {
            e.traveling = false;
            // Termino: volver a mostrar el cuadro y apagar las particulas
            if (e.assembling) gallery.MarkRevealed(e.painting);
            e.painting.swipe = 0f;   // que el mismo barrido no la desarme apenas se arma
            e.assembling = false;
            e.nextAllowed = Time.time + cooldown;
            e.touch = 0f;
            if (Living(e)) { StartLiving(e); return; }
            e.painting.renderer.enabled = true;
            e.ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            e.go.SetActive(false);
            e.state = EffectState.Idle;
        }
    }

    // Viaje en arco alrededor del centro del anillo (para no pasar a traves de la persona)
    Vector3 ArcLerp(Vector3 a, Vector3 b, float k)
    {
        Vector3 c = gallery.Center;
        Vector3 va = a - c, vb = b - c;
        float ra = new Vector2(va.x, va.z).magnitude, rb = new Vector2(vb.x, vb.z).magnitude;
        float angA = Mathf.Atan2(va.x, va.z) * Mathf.Rad2Deg, angB = Mathf.Atan2(vb.x, vb.z) * Mathf.Rad2Deg;
        float ang = (angA + Mathf.DeltaAngle(angA, angB) * k) * Mathf.Deg2Rad;
        float r = Mathf.Lerp(ra, rb, k);
        float y = Mathf.Lerp(va.y, vb.y, k) + Mathf.Sin(k * Mathf.PI) * travelArc;
        return c + new Vector3(Mathf.Sin(ang) * r, y, Mathf.Cos(ang) * r);
    }


    // ---------------------------------------------------------------- obras vivas

    bool Living(Effect e)
    {
        var p = e.painting;
        if (!livingPaintings || p.locked || !gallery.pathMode || gallery.PathLength <= 0f) return false;
        if (p.original != null)
            foreach (var f in keepFlat) if (!string.IsNullOrEmpty(f) && p.original.name.StartsWith(f)) return false;
        return true;
    }

    // Cada particula tiene su lugar en la obra, con volumen: lo claro sale hacia adelante, lo oscuro se hunde.
    // Las del borde quedan sueltas, mas lejos del plano y mas transparentes: la obra no tiene marco, se deshace en el aire.
    void StartLiving(Effect e)
    {
        var p = e.painting;
        int n = e.count;
        if (e.home == null || e.home.Length != n) { e.home = new Vector3[n]; e.loose = new float[n]; }
        Vector3 toViewer = gallery.RigPosition - p.root.position; toViewer.y = 0f;
        Vector3 front = Vector3.Dot(p.root.forward, toViewer) >= 0f ? p.root.forward : -p.root.forward;
        float size = Mathf.Max(p.halfW, p.halfH);
        for (int i = 0; i < n; i++)
        {
            Vector2 uv = e.uv != null ? e.uv[i] : new Vector2(0.5f, 0.5f);
            float edge = Mathf.Min(Mathf.Min(uv.x, 1f - uv.x), Mathf.Min(uv.y, 1f - uv.y));   // 0 en el borde, 0.5 en el centro
            float loose = edgeDissolve > 0f ? 1f - Mathf.Clamp01(edge / edgeDissolve) : 0f;
            loose *= loose;
            Color32 c = e.toColor[i];
            float lum = (0.3f * c.r + 0.59f * c.g + 0.11f * c.b) / 255f;
            float depth = (lum - 0.35f) * livingDepth * size + (e.seed[i] - 0.5f) * 0.04f * size;
            // Las del borde se alejan del plano y hacia afuera, como humo
            Vector3 outward = (p.root.right * (uv.x - 0.5f) + p.root.up * (uv.y - 0.5f)).normalized;
            Vector3 drift = (outward * 0.6f + front * (e.seed[i] - 0.5f) * 1.6f + Vector3.up * 0.3f) * edgeDrift * loose * (0.5f + e.seed[i]);
            e.home[i] = e.target[i] + front * depth + drift;
            e.loose[i] = loose;
            e.vel[i] = Vector3.zero;
        }
        p.renderer.enabled = false;
        BuildSheet(e, front, size);
        e.state = EffectState.Living;
        e.t = 0f;
    }

    void HideSheet(Effect e)
    {
        if (e.sheetGo != null && e.sheetGo.activeSelf) e.sheetGo.SetActive(false);
    }

    // La tela: una grilla de particulas (cada vertice es una) con la imagen pegada encima. Cada una tiene su lugar con
    // volumen (lo claro adelante) y esta unida a sus 4 vecinas por resortes. Los bordes se vuelven transparentes y
    // se deshacen en las particulas sueltas.
    void BuildSheet(Effect e, Vector3 front, float size)
    {
        var p = e.painting;
        var sprite = p.renderer.sprite;
        var pixels = GetPixels(sprite);
        if (sprite == null || pixels == null) return;
        if (e.sheetGo == null)
        {
            e.sheetGo = new GameObject("Tela_" + p.name);
            e.sheetGo.transform.SetParent(transform, false);
            e.sheetGo.transform.position = Vector3.zero;   // vertices en coordenadas del mundo
            e.sheet = new Mesh { name = "Tela" };
            e.sheet.MarkDynamic();
            e.sheetGo.AddComponent<MeshFilter>().sharedMesh = e.sheet;
            var mr = e.sheetGo.AddComponent<MeshRenderer>();
            mr.sharedMaterial = new Material(particleMat.shader);
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;
            mr.sortingOrder = 4;
        }
        var rend = e.sheetGo.GetComponent<MeshRenderer>();
        rend.sharedMaterial.mainTexture = sprite.texture;

        float w = p.halfW * 2f, h = p.halfH * 2f;
        int sy = Mathf.Max(8, sheetResolution);
        int sx = Mathf.Max(8, Mathf.RoundToInt(sy * w / h));
        int n = (sx + 1) * (sy + 1);
        if (e.sPos == null || e.sPos.Length != n)
        {
            e.sHome = new Vector3[n]; e.sPos = new Vector3[n]; e.sVel = new Vector3[n]; e.sRest = new Vector3[n];
            e.sEdge = new float[n]; e.sColor = new Color[n];
        }
        e.sx = sx; e.sy = sy;
        Rect r = sprite.textureRect;
        float tw = sprite.texture.width, th = sprite.texture.height;
        var uvs = new Vector2[n];
        var tris = new int[sx * sy * 6];
        Vector3 c = p.root.position, right = p.root.right, up = p.root.up;
        for (int j = 0; j <= sy; j++)
            for (int i = 0; i <= sx; i++)
            {
                int k = j * (sx + 1) + i;
                float u = (float)i / sx, v = (float)j / sy;
                Color col = pixels.GetPixelBilinear(u, v);
                float lum = 0.3f * col.r + 0.59f * col.g + 0.11f * col.b;
                float edge = Mathf.Min(Mathf.Min(u, 1f - u), Mathf.Min(v, 1f - v));
                // Borde gastado: no es una linea recta, cambia con el ruido
                float ragged = edgeDissolve * (0.6f + 0.8f * Mathf.PerlinNoise(u * 6f + p.phase, v * 6f));
                float fade = ragged > 0f ? Mathf.Clamp01(edge / ragged) : 1f;
                e.sEdge[k] = fade * fade * (3f - 2f * fade);
                Vector3 flat = c + right * ((u - 0.5f) * w) + up * ((v - 0.5f) * h);
                e.sHome[k] = flat + front * ((lum - 0.35f) * livingDepth * size);
                e.sPos[k] = flat;            // arranca plana y se acomoda (sale el volumen)
                e.sVel[k] = Vector3.zero;
                uvs[k] = new Vector2((r.x + u * r.width) / tw, (r.y + v * r.height) / th);
            }
        // Largo de reposo de cada resorte con la vecina de la derecha y la de arriba (se guarda en sRest: x = derecha, y = arriba)
        for (int j = 0; j <= sy; j++)
            for (int i = 0; i <= sx; i++)
            {
                int k = j * (sx + 1) + i;
                e.sRest[k] = Vector3.zero;
            }
        int t = 0;
        for (int j = 0; j < sy; j++)
            for (int i = 0; i < sx; i++)
            {
                int a = j * (sx + 1) + i, b = a + 1, d = a + sx + 1, f = d + 1;
                tris[t++] = a; tris[t++] = d; tris[t++] = b;
                tris[t++] = b; tris[t++] = d; tris[t++] = f;
            }
        e.sheet.Clear();
        e.sheet.vertices = e.sPos;
        e.sheet.uv = uvs;
        e.sheet.colors = e.sColor;
        e.sheet.triangles = tris;
        e.sheet.bounds = new Bounds(Vector3.zero, Vector3.one * 100000f);
        e.sheetSprite = sprite;
        e.sheetGo.SetActive(true);
    }

    // Cada punto de la tela: resorte a su lugar, resortes con sus vecinas, freno, corrientes de aire y la mano
    void UpdateSheet(Effect e, float dt, int pushes, Vector3 pushA, Vector3 pushB, float strength, Color tint, Color fogA, Color fogB, float alpha, float wind)
    {
        if (e.sheet == null || e.sPos == null) return;
        if (!e.sheetGo.activeSelf) e.sheetGo.SetActive(true);
        int sx = e.sx, sy = e.sy, w = sx + 1;
        float r2 = handRadius * handRadius;
        float tt = Time.time * 0.35f;
        var pos = e.sPos; var home = e.sHome; var vel = e.sVel;
        for (int j = 0; j <= sy; j++)
            for (int i = 0; i <= sx; i++)
            {
                int k = j * w + i;
                Vector3 x = pos[k], v = vel[k];
                float loose = 1f - e.sEdge[k];
                Vector3 a = (home[k] - x) * (spring * (1f - 0.6f * loose)) - v * damping;
                // Vecinas: cada una tira para mantener la misma distancia que en reposo (la onda se propaga)
                Vector3 off = x - home[k];
                if (i > 0) a += ((pos[k - 1] - home[k - 1]) - off) * neighborSpring;
                if (i < sx) a += ((pos[k + 1] - home[k + 1]) - off) * neighborSpring;
                if (j > 0) a += ((pos[k - w] - home[k - w]) - off) * neighborSpring;
                if (j < sy) a += ((pos[k + w] - home[k + w]) - off) * neighborSpring;
                float sxn = x.x * 1.3f, syn = x.y * 1.3f, szn = x.z * 1.3f;
                a += new Vector3(Mathf.PerlinNoise(syn + tt, szn) - 0.5f,
                                 Mathf.PerlinNoise(szn + 3.1f, sxn + tt) - 0.45f,
                                 Mathf.PerlinNoise(sxn + 7.7f + tt, syn) - 0.5f) * (flow * wind * (0.25f + 1.5f * loose));
                if (pushes > 0)
                {
                    Vector3 d = x - pushA; float d2 = d.sqrMagnitude;
                    if (d2 < r2 && d2 > 1e-6f) a += d / Mathf.Sqrt(d2) * strength * (1f - d2 / r2);
                    if (pushes > 1)
                    {
                        d = x - pushB; d2 = d.sqrMagnitude;
                        if (d2 < r2 && d2 > 1e-6f) a += d / Mathf.Sqrt(d2) * strength * (1f - d2 / r2);
                    }
                }
                vel[k] = v + a * dt;
            }
        for (int k = 0; k < pos.Length; k++) pos[k] += vel[k] * dt;

        // Color: Sprites/Default multiplica la imagen por el color de cada vertice. Niebla aproximada: oscurece hacia la niebla
        Color mul = new Color(tint.r * (fogB.r - fogA.r), tint.g * (fogB.g - fogA.g), tint.b * (fogB.b - fogA.b), 1f);
        for (int k = 0; k < pos.Length; k++)
        {
            Color c = mul;
            c.a = alpha * e.sEdge[k];
            e.sColor[k] = c;
        }
        e.sheet.vertices = pos;
        e.sheet.colors = e.sColor;
    }

    void UpdateLiving(Effect e, float dt)
    {
        var p = e.painting;
        bool near = Vector3.Distance(p.root.position, gallery.RigPosition) < livingDistance;
        if (!near)
        {
            if (e.go.activeSelf) e.go.SetActive(false);
            return;
        }
        if (!e.go.activeSelf) { e.go.SetActive(true); e.ps.Play(); }
        e.t += dt;
        dt = Mathf.Min(dt, 0.04f);
        float time = Time.time;

        // Lo que aparta las particulas: el punto de la obra que toca la mano (o el puntero) y las palmas
        int pushes = 0;
        Vector3 pushA = Vector3.zero, pushB = Vector3.zero;
        float strength = handPush * (0.5f + gallery.agitation * 2f);
        if (p.hovered) { pushA = p.hitPoint; pushes = 1; }
        Vector3 palm;
        if (GoyaHands.Palm(out palm) && Vector3.Distance(palm, p.root.position) < Mathf.Max(p.halfW, p.halfH) + 1f)
        {
            if (pushes == 0) pushA = palm; else pushB = palm;
            pushes++;
        }
        float r2 = handRadius * handRadius;

        // Color: la epoca (oscuridad), la niebla del lugar y la aparicion de la galeria, igual que el cuadro plano
        Color tint = gallery.CurrentTint();
        var env = gallery.GetComponent<GoyaEnvironment>();
        Color fogA = Color.black, fogB = Color.white;
        if (env != null) { fogA = env.Fogged(Color.black, p.root.position); fogB = env.Fogged(Color.white, p.root.position); }
        float alpha = gallery.appear * p.visibility;
        float wind = 1f + gallery.agitation * agitationWind;
        if (e.sheetSprite != p.renderer.sprite && p.renderer.sprite != null)
        {
            Vector3 toViewer = gallery.RigPosition - p.root.position; toViewer.y = 0f;
            BuildSheet(e, Vector3.Dot(p.root.forward, toViewer) >= 0f ? p.root.forward : -p.root.forward, Mathf.Max(p.halfW, p.halfH));
        }
        UpdateSheet(e, dt, pushes, pushA, pushB, strength, tint, fogA, fogB, alpha, wind);

        for (int i = 0; i < e.count; i++)
        {
            float loose = e.loose[i];
            Vector3 x = e.pos[i];
            Vector3 v = e.vel[i];

            // Resorte hacia su lugar (mas flojo en los bordes) y freno
            Vector3 a = (e.home[i] - x) * (spring * (1f - 0.75f * loose)) - v * damping;

            // Corrientes de aire: un campo de ruido que cambia con el tiempo; en los bordes sopla mas
            float sx = x.x * 1.3f, sy = x.y * 1.3f, sz = x.z * 1.3f, tt = time * 0.35f;
            a += new Vector3(Mathf.PerlinNoise(sy + tt, sz) - 0.5f,
                             Mathf.PerlinNoise(sz + 3.1f, sx + tt) - 0.45f,
                             Mathf.PerlinNoise(sx + 7.7f + tt, sy) - 0.5f) * (flow * wind * (0.35f + 2.5f * loose));

            // La mano las aparta; cuando se va, el resorte las devuelve (y se ve la onda)
            if (pushes > 0)
            {
                Vector3 d = x - pushA; float d2 = d.sqrMagnitude;
                if (d2 < r2 && d2 > 1e-6f) a += d / Mathf.Sqrt(d2) * strength * (1f - d2 / r2);
                if (pushes > 1)
                {
                    d = x - pushB; d2 = d.sqrMagnitude;
                    if (d2 < r2 && d2 > 1e-6f) a += d / Mathf.Sqrt(d2) * strength * (1f - d2 / r2);
                }
            }

            v += a * dt;
            x += v * dt;
            e.vel[i] = v;
            e.pos[i] = x;

            Color32 c = e.toColor[i];
            float cr = c.r / 255f * tint.r, cg = c.g / 255f * tint.g, cb = c.b / 255f * tint.b;
            // Adentro la imagen la lleva la tela: solo se ven las particulas sueltas del borde (el humo)
            Color col = new Color(fogA.r + cr * (fogB.r - fogA.r), fogA.g + cg * (fogB.g - fogA.g), fogA.b + cb * (fogB.b - fogA.b),
                                  c.a / 255f * alpha * Mathf.Clamp01(loose * 2f) * (1f - 0.5f * loose));
            e.parts[i].startColor = col;
        }

        // Al aparecer, se van acomodando (no hay salto desde el plano)
        Write(e, edgeParticleSize);
    }

    // ---------------------------------------------------------------- remolino

    bool UseVortex(Effect e)
    {
        return vortex && gallery.pathMode && gallery.PathLength > 0f && gallery.Origins != null && gallery.Origins.Has(e.painting);
    }

    Vector3 VortexCenter(Effect e)
    {
        var p = e.painting;
        Vector3 front = p.root.position - p.root.forward * 0.3f;
        Vector3 palm;
        if (GoyaHands.Palm(out palm) && Vector3.Distance(palm, front) < 10f) return Vector3.Lerp(front, palm, towardHand);
        return front;
    }

    void SetupVortex(Effect e)
    {
        int n = e.count;
        if (e.orbU == null)
        {
            e.orbU = new Vector3[n]; e.orbV = new Vector3[n]; e.sink = new Vector3[n];
            e.orbR = new float[n]; e.orbW = new float[n]; e.srcColor = new Color32[n];
        }
        var p = e.painting;
        e.vortex = true;
        e.center = VortexCenter(e);

        // Unos pocos planos de giro, inclinados (como las orbitas del video)
        const int planes = 5;
        var us = new Vector3[planes]; var vs = new Vector3[planes]; var ws = new float[planes];
        for (int k = 0; k < planes; k++)
        {
            Vector3 normal = (-p.root.forward + Random.insideUnitSphere * 1.2f).normalized;
            us[k] = Vector3.Cross(normal, Vector3.up).normalized;
            if (us[k].sqrMagnitude < 0.01f) us[k] = p.root.right;
            vs[k] = Vector3.Cross(normal, us[k]).normalized;
            ws[k] = vortexSpeed * Random.Range(0.7f, 1.4f);
        }
        float size = Mathf.Max(p.halfW, p.halfH) * vortexSize;
        for (int i = 0; i < n; i++)
        {
            int k = Random.Range(0, planes);
            e.orbU[i] = us[k]; e.orbV[i] = vs[k];
            e.orbR[i] = size * Mathf.Lerp(0.25f, 1.1f, Mathf.Sqrt(Random.value));
            e.orbW[i] = ws[k] * (1.3f - 0.5f * e.orbR[i] / Mathf.Max(0.01f, size)) ;   // los de adentro giran mas rapido
            float a = Random.value, b = Random.value;
            e.sink[i] = gallery.Origins.SourcePoint(p, a, b);
            e.srcColor[i] = gallery.Origins.SourceColor(p, a, b);
            e.vel[i] = Vector3.zero;
        }

        var rend = e.go.GetComponent<ParticleSystemRenderer>();
        rend.renderMode = ParticleSystemRenderMode.Stretch;
        rend.velocityScale = streak;
        rend.lengthScale = 1f;

        // Cintas doradas
        if (e.ribbons == null || e.ribbons.Length != vortexRibbons) e.ribbons = new TrailRenderer[vortexRibbons];
        for (int r = 0; r < e.ribbons.Length; r++)
        {
            if (e.ribbons[r] == null)
            {
                var go = new GameObject("Cinta_" + r);
                go.transform.SetParent(e.go.transform, false);
                var tr = go.AddComponent<TrailRenderer>();
                if (ribbonMat == null) ribbonMat = new Material(particleMat.shader);   // sin textura: trazo liso
                tr.sharedMaterial = ribbonMat;
                tr.time = 0.6f;
                tr.widthMultiplier = 0.012f;
                tr.minVertexDistance = 0.02f;
                tr.sortingOrder = 6;
                e.ribbons[r] = tr;
            }
            e.ribbons[r].Clear();
            e.ribbons[r].emitting = false;
        }
    }

    Vector3 OrbitPoint(Effect e, int i, float time)
    {
        float ang = e.seed[i] * 40f + time * e.orbW[i];
        return e.center + (e.orbU[i] * Mathf.Cos(ang) + e.orbV[i] * Mathf.Sin(ang)) * e.orbR[i];
    }

    // forming = true: del suelo/arbol/roca al remolino y del remolino a la obra.
    // forming = false: de la obra al remolino y del remolino de vuelta al suelo/arbol/roca.
    void UpdateVortex(Effect e, float dt, bool forming)
    {
        e.t += dt;
        e.center = Vector3.Lerp(e.center, VortexCenter(e), 1f - Mathf.Exp(-2f * dt));
        Color tint = gallery.CurrentTint();
        float total = reformSpread + reformTime;
        float invDt = 1f / Mathf.Max(0.001f, dt);

        for (int i = 0; i < e.count; i++)
        {
            float k = Mathf.Clamp01((e.t - e.seed[i] * reformSpread) / reformTime);
            Vector3 orbit = OrbitPoint(e, i, e.t);
            Vector3 from = e.startPos[i];
            Vector3 to = forming ? e.target[i] : e.sink[i];
            Vector3 next = k < 0.5f
                ? Vector3.Lerp(from, orbit, Smooth(k * 2f))
                : Vector3.Lerp(orbit, to, Smooth((k - 0.5f) * 2f));
            e.vel[i] = (next - e.pos[i]) * invDt;
            e.pos[i] = next;

            Color32 a = forming ? e.srcColor[i] : e.fromColor[i];
            Color32 b = forming ? Tinted(e.toColor[i], tint) : e.srcColor[i];
            Color32 c = Color32.Lerp(a, b, Smooth(k));
            if (!forming) c.a = (byte)(c.a * Mathf.Clamp01((1f - k) / 0.25f));   // al volver, se funde con la tierra
            e.parts[i].startColor = c;
        }

        // Cintas: aparecen con el remolino y se van cuando termina
        float prog = Mathf.Clamp01(e.t / total);
        float glow = Mathf.Sin(prog * Mathf.PI);
        Vector3 axisN = -e.painting.root.forward;
        for (int r = 0; r < e.ribbons.Length; r++)
        {
            var tr = e.ribbons[r];
            if (tr == null) continue;
            tr.emitting = glow > 0.05f;
            float ang = e.t * vortexSpeed * (1.3f + 0.25f * r) + r * 1.7f;
            float tilt = r * 37f;
            Vector3 u = Quaternion.AngleAxis(tilt, axisN) * e.painting.root.right;
            Vector3 v = Vector3.Cross(Quaternion.AngleAxis(tilt * 0.6f, u) * axisN, u).normalized;
            float rad = Mathf.Max(e.painting.halfW, e.painting.halfH) * vortexSize * (0.6f + 0.12f * r);
            tr.transform.position = e.center + (u * Mathf.Cos(ang) + v * Mathf.Sin(ang)) * rad;
            Color col = ribbonColor; col.a *= glow;
            Color end = col; end.a = 0f;
            tr.startColor = col; tr.endColor = end;
        }

        if (forming) e.painting.reveal = prog;
        // El arbol/roca se deshace mientras se forma la obra, y vuelve a aparecer cuando se desarma
        gallery.Origins.SetPresence(e.painting, forming ? 1f - Smooth(prog * 1.4f) : Smooth((prog - 0.3f) / 0.7f));
        Write(e);

        if (e.t < total) return;

        // Termino
        EndVortex(e);
        if (forming)
        {
            if (e.assembling) gallery.MarkRevealed(e.painting);
            e.assembling = false;
            e.painting.swipe = 0f;
            e.nextAllowed = Time.time + cooldown;
            if (Living(e)) { StartLiving(e); return; }
            e.painting.renderer.enabled = true;
            e.ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            e.go.SetActive(false);
            e.state = EffectState.Idle;
        }
        else
        {
            e.painting.revealed = false;
            e.painting.reveal = 0f;
            e.nextAllowed = Time.time + cooldown;
            StartFloating(e);
        }
    }

    void EndVortex(Effect e)
    {
        e.vortex = false;
        var rend = e.go.GetComponent<ParticleSystemRenderer>();
        rend.renderMode = ParticleSystemRenderMode.Billboard;
        if (e.ribbons != null) foreach (var tr in e.ribbons) if (tr != null) tr.emitting = false;
    }

    static float Smooth(float x) { x = Mathf.Clamp01(x); return x * x * (3f - 2f * x); }

    void Write(Effect e, float sizeMul = 1f)
    {
        for (int i = 0; i < e.count; i++)
        {
            e.parts[i].position = e.pos[i];
            e.parts[i].velocity = e.vortex ? e.vel[i] : Vector3.zero;
            e.parts[i].startLifetime = 1000f;
            e.parts[i].remainingLifetime = 1000f;
            e.parts[i].startSize = e.size * sizeMul;
        }
        e.ps.SetParticles(e.parts, e.count);
    }
}

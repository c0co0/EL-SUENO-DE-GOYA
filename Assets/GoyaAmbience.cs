using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ambiente onirico para la galeria:
///  - Polvo flotante alrededor de la persona.
///  - Un aura calida detras de cada cuadro que crece al revelarlo y destella al deshacerse.
///  - Sonido (opcional): ambiente continuo y efectos al revelar / deshacer / rehacer un cuadro.
///
/// Ponelo en el mismo objeto "Galeria". Todo se genera por codigo, usa solo shaders
/// que funcionan en el celular (Sprites/Default).
/// </summary>
public class GoyaAmbience : MonoBehaviour
{
    [Header("Referencias (se buscan solas si quedan vacias)")]
    public GoyaGallery gallery;
    public GoyaParticles particles;

    [Header("Polvo flotante")]
    public bool dust = false;
    public int dustMax = 400;
    public Vector3 dustArea = new Vector3(8f, 3.5f, 8f);
    public Color dustColor = new Color(0.8f, 0.85f, 1f, 0.45f);

    [Header("Aura de cada cuadro")]
    public bool halos = true;
    public Color haloColor = new Color(1f, 0.72f, 0.45f, 1f);
    [Tooltip("Intensidad del aura de un cuadro revelado (0 a 1)")]
    public float haloStrength = 0.22f;
    [Tooltip("Tamaño del aura respecto del cuadro")]
    public float haloSize = 2f;

    [Header("Sonido (opcional: arrastra clips de audio)")]
    public AudioClip ambientLoop;
    public AudioClip revealSound;
    public AudioClip breakSound;
    public AudioClip reformSound;
    [Range(0f, 1f)] public float ambientVolume = 0.35f;
    [Range(0f, 1f)] public float effectsVolume = 0.8f;

    [Header("Oscuridad (sigue a la oscuridad acumulada de la Galeria)")]
    [Tooltip("Color del fondo con oscuridad total")]
    public Color darkBackground = new Color(0.01f, 0.005f, 0.005f, 1f);
    [Tooltip("Color de las auras con oscuridad total")]
    public Color darkHaloColor = new Color(0.55f, 0.08f, 0.05f, 1f);
    [Tooltip("Tono del sonido ambiente con oscuridad total (1 = normal, mas bajo = mas grave)")]
    public float darkAmbientPitch = 0.8f;

    class Halo
    {
        public GoyaGallery.Painting painting;
        public SpriteRenderer sr;
        public float flash;
        public float phase;
    }

    readonly Dictionary<GoyaGallery.Painting, Halo> haloMap = new Dictionary<GoyaGallery.Painting, Halo>();
    readonly HashSet<GoyaGallery.Painting> revealedSeen = new HashSet<GoyaGallery.Painting>();
    Material softMat;
    Sprite softSprite;
    bool halosBuilt;
    bool subscribed;
    AudioSource ambientSource;
    ParticleSystem dustSystem;
    StereoGyroRig rigScript;
    Camera[] cams = new Camera[8];

    void Start()
    {
        Texture2D soft = MakeSoftTexture(64);
        Shader sh = Shader.Find("Sprites/Default");
        if (sh == null) sh = Shader.Find("Particles/Standard Unlit");
        if (sh != null)
        {
            softMat = new Material(sh);
            softMat.mainTexture = soft;
        }
        softSprite = Sprite.Create(soft, new Rect(0, 0, soft.width, soft.height), new Vector2(0.5f, 0.5f), soft.width);

        if (dust && softMat != null) CreateDust();

        if (ambientLoop != null)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.clip = ambientLoop;
            src.loop = true;
            src.spatialBlend = 0f;
            src.volume = ambientVolume;
            src.Play();
            ambientSource = src;
        }
    }

    void OnDestroy()
    {
        if (particles != null)
        {
            particles.OnBreak -= HandleBreak;
            particles.OnReform -= HandleReform;
        }
    }

    // ---------------------------------------------------------------- creacion

    static Texture2D MakeSoftTexture(int n)
    {
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < n; x++)
            {
                float dx = (x + 0.5f) / n * 2f - 1f;
                float dy = (y + 0.5f) / n * 2f - 1f;
                float a = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a * a));
            }
        }
        tex.Apply();
        return tex;
    }

    void CreateDust()
    {
        var rigObj = FindFirstObjectByType<StereoGyroRig>();
        Vector3 center = rigObj != null ? rigObj.transform.position : transform.position;

        var go = new GameObject("PolvoFlotante");
        go.transform.SetParent(transform, false);
        go.transform.position = center;

        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        dustSystem = ps;

        var main = ps.main;
        main.loop = true;
        main.prewarm = true;
        main.maxParticles = dustMax;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = new ParticleSystem.MinMaxCurve(8f, 14f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.01f, 0.05f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.012f, 0.04f);
        main.startColor = dustColor;
        main.gravityModifier = 0f;

        var emission = ps.emission;
        emission.rateOverTime = dustMax / 10f;

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = dustArea;

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.12f;
        noise.frequency = 0.25f;
        noise.scrollSpeed = 0.1f;

        // Aparece y desaparece suavemente
        var col = ps.colorOverLifetime;
        col.enabled = true;
        var g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
            new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.2f),
                    new GradientAlphaKey(1f, 0.8f), new GradientAlphaKey(0f, 1f) });
        col.color = g;

        var rend = go.GetComponent<ParticleSystemRenderer>();
        rend.sharedMaterial = softMat;
        rend.renderMode = ParticleSystemRenderMode.Billboard;
        rend.sortingOrder = 3;

        ps.Play();
    }

    void BuildHalos()
    {
        halosBuilt = true;
        if (!halos) return;

        foreach (var p in gallery.paintings)
        {
            var go = new GameObject("Aura_" + p.name);
            go.transform.SetParent(transform, false);
            go.transform.position = p.root.position + p.root.forward * 0.08f;
            go.transform.rotation = p.root.rotation;
            go.transform.localScale = new Vector3(p.halfW * 2f * haloSize, p.halfH * 2f * haloSize, 1f);

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = softSprite;
            sr.sortingOrder = -1;
            sr.color = new Color(haloColor.r, haloColor.g, haloColor.b, 0f);

            haloMap[p] = new Halo { painting = p, sr = sr, phase = Random.value * 6.28f };
        }
    }

    // ---------------------------------------------------------------- eventos y sonido

    void HandleBreak(GoyaGallery.Painting p)
    {
        PlayAt(breakSound, p.root.position);
        Halo h;
        if (haloMap.TryGetValue(p, out h)) h.flash = 1f;
    }

    void HandleReform(GoyaGallery.Painting p)
    {
        PlayAt(reformSound, p.root.position);
    }

    readonly Dictionary<AudioClip, float> lastPlayed = new Dictionary<AudioClip, float>();

    void PlayAt(AudioClip clip, Vector3 pos)
    {
        if (clip == null) return;
        // Si todas las obras cambian juntas, que el sonido no se repita 7 veces encima
        float last;
        if (lastPlayed.TryGetValue(clip, out last) && Time.time - last < 0.3f) return;
        lastPlayed[clip] = Time.time;
        AudioSource.PlayClipAtPoint(clip, pos, effectsVolume);
    }

    // ---------------------------------------------------------------- actualizacion

    void Update()
    {
        if (gallery == null) gallery = FindFirstObjectByType<GoyaGallery>();
        if (particles == null) particles = FindFirstObjectByType<GoyaParticles>();
        if (gallery == null) return;

        if (!subscribed && particles != null)
        {
            particles.OnBreak += HandleBreak;
            particles.OnReform += HandleReform;
            subscribed = true;
        }

        if (!halosBuilt && gallery.paintings.Count > 0 && softSprite != null) BuildHalos();

        // En el camino, el polvo del aire acompaña a la persona mientras camina
        if (dustSystem != null && gallery.pathMode && gallery.PathLength > 0f)
            dustSystem.transform.position = gallery.RigPosition;

        // Sonido al revelar un cuadro
        foreach (var p in gallery.paintings)
        {
            if (p.revealed && !revealedSeen.Contains(p))
            {
                revealedSeen.Add(p);
                PlayAt(revealSound, p.root.position);
            }
            else if (!p.revealed && revealedSeen.Contains(p))
            {
                revealedSeen.Remove(p);
            }
        }

        float dark = gallery.darkness;

        // Auras (siguen el lugar y el tamano del cuadro, que cambian al pasar a otra obra)
        Color haloNow = Color.Lerp(haloColor, darkHaloColor, dark);
        foreach (var h in haloMap.Values)
        {
            var p = h.painting;
            // Sigue al cuadro: en las tandas los cuadros cambian de lugar
            h.sr.transform.position = p.root.position + p.root.forward * 0.08f;
            h.sr.transform.rotation = p.root.rotation;
            h.sr.transform.localScale = new Vector3(p.halfW * 2f * haloSize, p.halfH * 2f * haloSize, 1f);
            h.sr.enabled = p.active;
            float target = haloStrength * Mathf.SmoothStep(0f, 1f, p.reveal);
            h.flash = Mathf.Max(0f, h.flash - Time.deltaTime / 1.8f);
            float a = target + haloStrength * 1.6f * h.flash;
            float breathe = 1f + 0.15f * Mathf.Sin(Time.time * (1.2f + 1.5f * gallery.agitation) + h.phase);
            Color c = haloNow;
            c.a = Mathf.Clamp01(a * breathe);
            h.sr.color = c;
        }

        UpdateDarkness(dark);
    }

    // Fondo, polvo y sonido ambiente se van apagando con la oscuridad acumulada
    void UpdateDarkness(float dark)
    {
        if (rigScript == null) rigScript = FindFirstObjectByType<StereoGyroRig>();
        if (rigScript != null)
        {
            Color bg = Color.Lerp(rigScript.background, darkBackground, dark);
            if (cams.Length < Camera.allCamerasCount) cams = new Camera[Camera.allCamerasCount];
            int n = Camera.GetAllCameras(cams);
            for (int i = 0; i < n; i++) cams[i].backgroundColor = bg;
        }

        if (dustSystem != null)
        {
            var main = dustSystem.main;
            Color dc = Color.Lerp(dustColor, new Color(0.35f, 0.3f, 0.3f, dustColor.a * 0.7f), dark);
            main.startColor = dc;
        }

        if (ambientSource != null)
            ambientSource.pitch = Mathf.Lerp(1f, darkAmbientPitch, dark);
    }
}

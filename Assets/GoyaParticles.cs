using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Efecto de particulas para los cuadros de GoyaGallery.
/// Cuando una MANO ABIERTA toca un cuadro ya revelado, el cuadro se deshace en particulas
/// (que conservan los colores de la pintura), flotan llevadas por el viento y despues
/// vuelven a formar el cuadro.
///
/// Ponelo en el mismo objeto "Galeria". Las imagenes tienen que tener Read/Write activado.
/// En el editor: tecla T = deshacer todos los cuadros ya revelados (para probar sin manos).
/// </summary>
public class GoyaParticles : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Si queda vacio, lo busca solo")]
    public GoyaGallery gallery;

    [Header("Particulas")]
    [Tooltip("Particulas por cuadro. Si el celular anda lento, bajalo (ej. 5000)")]
    public int particleCount = 8000;
    public float particleScale = 1.8f;

    [Header("Toque (mano abierta sobre un cuadro revelado)")]
    [Tooltip("Segundos con la mano abierta sobre el cuadro para que se deshaga")]
    public float touchHoldTime = 0.35f;
    [Tooltip("Segundos de espera antes de poder tocarlo de nuevo")]
    public float cooldown = 1f;

    [Header("Deshacerse")]
    [Tooltip("Tiempo que tarda la onda de desintegracion en recorrer el cuadro")]
    public float breakWaveTime = 1f;
    [Tooltip("Tiempo total que las particulas flotan antes de volver")]
    public float scatterTime = 2.6f;
    public float scatterSpeed = 0.9f;
    [Tooltip("Direccion del viento que se lleva las particulas")]
    public Vector3 wind = new Vector3(0.5f, 0.35f, 0.2f);
    public float drag = 1.2f;

    [Header("Rehacerse")]
    public float reformTime = 2.2f;
    [Tooltip("Retraso aleatorio maximo: que no vuelvan todas al mismo tiempo")]
    public float reformSpread = 0.9f;

    public enum EffectState { Idle, Scatter, Reform }

    class Effect
    {
        public GoyaGallery.Painting painting;
        public GameObject go;
        public ParticleSystem ps;
        public ParticleSystem.Particle[] parts;
        public int count;
        public float size;
        public Vector3[] target, pos, vel, startPos;
        public float[] delay, seed;
        public bool[] released;
        public EffectState state;
        public float t;
        public float touch;
        public float nextAllowed;
    }

    // Avisos para otros scripts (por ejemplo el sonido de GoyaAmbience)
    public System.Action<GoyaGallery.Painting> OnBreak;
    public System.Action<GoyaGallery.Painting> OnReform;

    readonly List<Effect> effects = new List<Effect>();
    Material particleMat;
    bool built;

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

        foreach (var e in effects)
        {
            var p = e.painting;
            switch (e.state)
            {
                case EffectState.Idle:
                    if (forceAll && p.revealed) { Begin(e); break; }

                    bool touching = p.revealed && p.openHandOver && Time.time >= e.nextAllowed;
                    if (touching)
                    {
                        e.touch += dt;
                        if (e.touch >= touchHoldTime) Begin(e);
                    }
                    else
                    {
                        e.touch = Mathf.Max(0f, e.touch - dt * 2f);
                    }
                    break;

                case EffectState.Scatter:
                    UpdateScatter(e, dt);
                    break;

                case EffectState.Reform:
                    UpdateReform(e, dt);
                    break;
            }
        }
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
        var sprite = p.renderer.sprite;
        var tex = sprite.texture;
        if (!tex.isReadable)
        {
            Debug.LogError("[Particulas] La imagen de '" + p.name + "' no tiene Read/Write activado. " +
                           "Seleccionala en Project, abri 'Advanced', tilda 'Read/Write' y apreta Apply.");
            return null;
        }

        float w = p.halfW * 2f;
        float h = p.halfH * 2f;
        int cols = Mathf.Max(1, Mathf.RoundToInt(Mathf.Sqrt(particleCount * (w / h))));
        int rows = Mathf.Max(1, particleCount / cols);
        int n = cols * rows;

        var e = new Effect
        {
            painting = p,
            count = n,
            parts = new ParticleSystem.Particle[n],
            target = new Vector3[n],
            pos = new Vector3[n],
            vel = new Vector3[n],
            startPos = new Vector3[n],
            delay = new float[n],
            seed = new float[n],
            released = new bool[n],
            size = Mathf.Max(0.008f, Mathf.Sqrt(w * h / n) * particleScale)
        };

        Rect r = sprite.rect;
        Vector3 center = p.root.position;
        Vector3 right = p.root.right;
        Vector3 up = p.root.up;

        int i = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                float fu = (col + Random.value) / cols;
                float fv = (row + Random.value) / rows;
                Color c = tex.GetPixelBilinear((r.x + fu * r.width) / tex.width,
                                               (r.y + fv * r.height) / tex.height);

                e.target[i] = center + right * ((fu - 0.5f) * w) + up * ((fv - 0.5f) * h);
                e.seed[i] = Random.value;
                e.parts[i].startColor = c;
                e.parts[i].startSize = e.size;
                i++;
            }
        }

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

    // ---------------------------------------------------------------- efecto

    void Begin(Effect e)
    {
        var p = e.painting;
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

        p.renderer.enabled = false;   // el cuadro "real" se oculta: ahora son solo particulas
        e.go.SetActive(true);
        e.ps.Play();
        Write(e);

        e.state = EffectState.Scatter;
        if (OnBreak != null) OnBreak(p);
        e.t = 0f;
        e.touch = 0f;
    }

    void UpdateScatter(Effect e, float dt)
    {
        e.t += dt;
        Vector3 towardViewer = -e.painting.root.forward;

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
            e.pos[i] += e.vel[i] * dt;
            e.vel[i] *= Mathf.Max(0f, 1f - drag * dt);
        }

        Write(e);

        if (e.t >= scatterTime)
        {
            for (int i = 0; i < e.count; i++) e.startPos[i] = e.pos[i];
            e.state = EffectState.Reform;
            if (OnReform != null) OnReform(e.painting);
            e.t = 0f;
        }
    }

    void UpdateReform(Effect e, float dt)
    {
        e.t += dt;

        for (int i = 0; i < e.count; i++)
        {
            float k = Mathf.Clamp01((e.t - e.seed[i] * reformSpread) / reformTime);
            k = k * k * (3f - 2f * k);

            float s = e.seed[i] * 20f;
            Vector3 wobble = new Vector3(Mathf.Sin(e.t * 3f + s),
                                         Mathf.Cos(e.t * 2.4f + s * 0.7f),
                                         Mathf.Sin(e.t * 1.9f + s * 0.4f)) * (0.12f * (1f - k));

            e.pos[i] = Vector3.Lerp(e.startPos[i], e.target[i], k) + wobble;
        }

        Write(e);

        if (e.t >= reformSpread + reformTime)
        {
            // Termino: volver a mostrar el cuadro y apagar las particulas
            e.painting.renderer.enabled = true;
            e.ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            e.go.SetActive(false);
            e.state = EffectState.Idle;
            e.nextAllowed = Time.time + cooldown;
            e.touch = 0f;
        }
    }

    void Write(Effect e)
    {
        for (int i = 0; i < e.count; i++)
        {
            e.parts[i].position = e.pos[i];
            e.parts[i].velocity = Vector3.zero;
            e.parts[i].startLifetime = 1000f;
            e.parts[i].remainingLifetime = 1000f;
            e.parts[i].startSize = e.size;
        }
        e.ps.SetParticles(e.parts, e.count);
    }
}

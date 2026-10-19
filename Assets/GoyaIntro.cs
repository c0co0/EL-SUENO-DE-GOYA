using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Introduccion de la experiencia:
///  1. Tu objeto de Blender flota delante de la persona (gira lento y respira).
///  2. Al apuntarlo con la MANO ABIERTA un instante, el objeto se deshace en particulas.
///  3. Mientras las particulas se dispersan, la galeria de cuadros aparece de a poco.
///
/// Ponelo en el objeto "Galeria". Arrastra tu modelo (ya puesto en la escena) en "Intro Object".
/// El modelo tiene que tener activado Read/Write en su Inspector de importacion (pestaña Model).
/// En el editor: tecla I = deshacer el objeto sin usar las manos.
/// </summary>
public class GoyaIntro : MonoBehaviour
{
    [Header("Objeto de Blender")]
    [Tooltip("Arrastra aca el modelo que pusiste en la escena (desde la Hierarchy)")]
    public GameObject introObject;
    [Tooltip("Distancia delante de la persona (metros)")]
    public float distance = 1.4f;
    public float heightOffset = -0.05f;
    [Tooltip("El lado mas largo del objeto se ajusta a este tamaño (metros)")]
    public float targetSize = 0.5f;
    public float rotateSpeed = 18f;
    public float bobAmount = 0.04f;

    [Header("Referencias (se buscan solas)")]
    public HandOSCReceiver hands;
    public GoyaGallery gallery;

    [Header("Toque")]
    [Tooltip("Que tan cerca del objeto tiene que apuntar la mano (multiplica el tamaño)")]
    public float touchRadiusFactor = 0.7f;
    [Tooltip("Segundos con la mano abierta apuntando al objeto")]
    public float touchHoldTime = 0.4f;
    public float reach = 1.6f;
    public int pointerLandmark = 9;
    public float cursorSize = 0.14f;

    [Header("Deshacerse")]
    public int particleCount = 6000;
    public float particleSize = 0.012f;
    [Tooltip("Usar el color del material del objeto para las particulas")]
    public bool useObjectColor = true;
    public Color particleColor = new Color(1f, 0.85f, 0.6f);
    public float releaseWaveTime = 0.8f;
    public float burstSpeed = 0.9f;
    public Vector3 wind = new Vector3(0.4f, 0.3f, 0.3f);
    public float drag = 1f;
    [Tooltip("Duracion total hasta que desaparecen las particulas")]
    public float dissolveTime = 4.5f;
    [Tooltip("Segundos despues del toque en que empieza a aparecer la galeria")]
    public float galleryDelay = 1.2f;
    public float galleryFadeTime = 3f;

    [Header("Niebla (afecta a objetos con iluminacion, como el tuyo)")]
    public bool fog = true;
    public Color fogColor = new Color(0.03f, 0.03f, 0.06f);
    public float fogDensity = 0.22f;

    [Header("Sonido (opcional)")]
    public AudioClip dissolveSound;
    [Range(0f, 1f)] public float soundVolume = 0.9f;

    [Header("Pruebas")]
    [Tooltip("Saltear la introduccion y mostrar la galeria directo")]
    public bool skipIntro = false;

    enum IntroState { Waiting, Dissolving, Done }
    IntroState state = IntroState.Waiting;

    Transform rig, pivot;
    Vector3 basePos;
    float touch;
    SpriteRenderer cursorLeft, cursorRight;
    Texture2D softTex;

    // particulas
    GameObject psObj;
    ParticleSystem ps;
    ParticleSystem.Particle[] parts;
    Vector3[] pos, vel;
    float[] delay, seed;
    bool[] released;
    Color baseColor;
    int count;
    float t;
    Vector3 dissolveCenter;
    bool galleryStarted;

    void Awake()
    {
        if (gallery == null) gallery = FindFirstObjectByType<GoyaGallery>();
        if (gallery != null && !skipIntro)
        {
            // La galeria no se crea hasta que se active
            gallery.enabled = false;
            gallery.appear = 0f;
        }
    }

    void Start()
    {
        if (hands == null) hands = FindFirstObjectByType<HandOSCReceiver>();
        rig = (hands != null && hands.transform.parent != null) ? hands.transform.parent : transform;

        if (fog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;
        }

        if (introObject == null || skipIntro)
        {
            if (introObject == null && !skipIntro)
                Debug.LogWarning("[Intro] Falta arrastrar el objeto en 'Intro Object'. Se muestra la galeria directo.");
            if (introObject != null) introObject.SetActive(false);
            if (gallery != null) { gallery.enabled = true; gallery.appear = 1f; }
            state = IntroState.Done;
            return;
        }

        softTex = MakeSoftTexture(64);
        var circle = Sprite.Create(softTex, new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 64);
        cursorLeft = CreateCursor("CursorIntroIzq", circle);
        cursorRight = CreateCursor("CursorIntroDer", circle);

        PlaceObject();
    }

    // ---------------------------------------------------------------- preparacion

    void PlaceObject()
    {
        introObject.SetActive(true);

        // Ajustar el tamaño: el lado mas largo mide targetSize
        var rends = introObject.GetComponentsInChildren<Renderer>();
        if (rends.Length == 0) return;
        Bounds b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);
        float maxExt = Mathf.Max(b.size.x, Mathf.Max(b.size.y, b.size.z));
        if (maxExt > 0.0001f)
            introObject.transform.localScale *= targetSize / maxExt;

        // Volver a medir para centrarlo (el pivote de Blender puede no estar al centro)
        b = rends[0].bounds;
        for (int i = 1; i < rends.Length; i++) b.Encapsulate(rends[i].bounds);

        basePos = rig.position + Vector3.forward * distance + Vector3.up * heightOffset;
        pivot = new GameObject("IntroPivot").transform;
        pivot.SetParent(transform, false);
        pivot.position = basePos;
        introObject.transform.SetParent(pivot, true);
        introObject.transform.position += basePos - b.center;
    }

    SpriteRenderer CreateCursor(string label, Sprite circle)
    {
        var go = new GameObject(label);
        go.transform.SetParent(transform, false);
        go.transform.localScale = Vector3.one * cursorSize;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = circle;
        sr.sortingOrder = 10;
        sr.enabled = false;
        return sr;
    }

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

    // ---------------------------------------------------------------- actualizacion

    void Update()
    {
        if (state == IntroState.Waiting) UpdateWaiting();
        else if (state == IntroState.Dissolving) UpdateDissolving();
    }

    bool GetRayDirection(HandOSCReceiver.HandState hand, out Vector3 dir)
    {
        dir = Vector3.forward;
        if (hands == null || !hand.visible) return false;
        Vector3 local = hands.transform.localPosition + hand.points[pointerLandmark];
        Vector3 amplified = new Vector3(local.x * reach, local.y * reach, local.z);
        dir = rig.TransformDirection(amplified).normalized;
        return true;
    }

    // Devuelve true si la mano apunta cerca del objeto
    bool ProcessHand(HandOSCReceiver.HandState hand, SpriteRenderer cursor, Color baseCol, out bool open)
    {
        open = false;
        Vector3 dir;
        if (!GetRayDirection(hand, out dir))
        {
            cursor.enabled = false;
            return false;
        }

        Vector3 origin = rig.position;
        Vector3 center = basePos;
        Vector3 toC = center - origin;
        float proj = Vector3.Dot(toC, dir);
        float dist = (toC - dir * proj).magnitude;
        bool near = proj > 0f && dist < targetSize * touchRadiusFactor;

        cursor.enabled = true;
        cursor.transform.position = origin + dir * Mathf.Max(0.3f, proj > 0f ? proj : distance);
        cursor.transform.rotation = Quaternion.LookRotation(dir);
        cursor.color = hand.open ? Color.green : baseCol;

        open = hand.open;
        return near;
    }

    void UpdateWaiting()
    {
        float dt = Time.deltaTime;
        if (pivot == null) return;

        bool leftOpen, rightOpen;
        bool nearL = ProcessHand(hands.Left, cursorLeft, hands.leftColor, out leftOpen);
        bool nearR = ProcessHand(hands.Right, cursorRight, hands.rightColor, out rightOpen);
        bool touching = (nearL && leftOpen) || (nearR && rightOpen);

        touch = touching ? touch + dt : Mathf.Max(0f, touch - dt * 2f);
        float progress = Mathf.Clamp01(touch / touchHoldTime);

        // Flota, gira y tiembla cuanto mas cerca esta de deshacerse
        Vector3 tremble = Random.insideUnitSphere * (0.012f * progress);
        pivot.position = basePos + Vector3.up * (Mathf.Sin(Time.time * 1.1f) * bobAmount) + tremble;
        pivot.Rotate(0f, rotateSpeed * dt, 0f, Space.World);
        pivot.localScale = Vector3.one * (1f + 0.03f * Mathf.Sin(Time.time * 2f) + 0.08f * progress);

        if (progress >= 1f || Input.GetKeyDown(KeyCode.I)) BeginDissolve();
    }

    // ---------------------------------------------------------------- deshacer en particulas

    void BeginDissolve()
    {
        dissolveCenter = pivot.position;
        count = particleCount;

        // Color de las particulas
        baseColor = particleColor;
        if (useObjectColor)
        {
            var r = introObject.GetComponentInChildren<Renderer>();
            if (r != null && r.sharedMaterial != null)
            {
                var m = r.sharedMaterial;
                if (m.HasProperty("_BaseColor")) baseColor = m.GetColor("_BaseColor");
                else if (m.HasProperty("_Color")) baseColor = m.GetColor("_Color");
            }
        }
        baseColor.a = 1f;

        pos = SampleSurface(introObject, count);
        vel = new Vector3[count];
        delay = new float[count];
        seed = new float[count];
        released = new bool[count];
        parts = new ParticleSystem.Particle[count];

        float maxD = 0.0001f;
        for (int i = 0; i < count; i++)
        {
            delay[i] = Vector3.Distance(pos[i], dissolveCenter);
            if (delay[i] > maxD) maxD = delay[i];
            seed[i] = Random.value;
        }
        for (int i = 0; i < count; i++)
            delay[i] = delay[i] / maxD * releaseWaveTime;

        // Sistema de particulas (solo dibuja: las posiciones las manejamos nosotros)
        psObj = new GameObject("Particulas_Intro");
        psObj.transform.SetParent(transform, false);
        ps = psObj.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.loop = false;
        main.playOnAwake = false;
        main.maxParticles = count;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = 1000f;
        main.startSpeed = 0f;
        main.startSize = particleSize;
        var emission = ps.emission;
        emission.enabled = false;
        var shape = ps.shape;
        shape.enabled = false;

        Shader sh = Shader.Find("Sprites/Default");
        if (sh == null) sh = Shader.Find("Particles/Standard Unlit");
        var mat = new Material(sh);
        mat.mainTexture = softTex;
        var rend = psObj.GetComponent<ParticleSystemRenderer>();
        rend.sharedMaterial = mat;
        rend.renderMode = ParticleSystemRenderMode.Billboard;
        rend.sortingOrder = 6;

        ps.Play();
        WriteParticles(1f);

        introObject.SetActive(false);
        cursorLeft.enabled = false;
        cursorRight.enabled = false;

        if (dissolveSound != null)
            AudioSource.PlayClipAtPoint(dissolveSound, dissolveCenter, soundVolume);

        t = 0f;
        state = IntroState.Dissolving;
    }

    // Puntos al azar sobre la superficie del modelo (en coordenadas del mundo)
    Vector3[] SampleSurface(GameObject root, int n)
    {
        var tris = new List<Vector3[]>();
        var cum = new List<float>();
        float total = 0f;

        foreach (var mf in root.GetComponentsInChildren<MeshFilter>())
        {
            Mesh m = mf.sharedMesh;
            if (m == null) continue;
            if (!m.isReadable)
            {
                Debug.LogError("[Intro] El modelo no tiene Read/Write activado. Seleccionalo en Project, " +
                               "pestaña 'Model', tilda 'Read/Write' y apreta Apply.");
                continue;
            }

            Vector3[] v = m.vertices;
            int[] idx = m.triangles;
            Matrix4x4 l2w = mf.transform.localToWorldMatrix;
            for (int k = 0; k + 2 < idx.Length; k += 3)
            {
                Vector3 a = l2w.MultiplyPoint3x4(v[idx[k]]);
                Vector3 b = l2w.MultiplyPoint3x4(v[idx[k + 1]]);
                Vector3 c = l2w.MultiplyPoint3x4(v[idx[k + 2]]);
                total += 0.5f * Vector3.Cross(b - a, c - a).magnitude;
                tris.Add(new[] { a, b, c });
                cum.Add(total);
            }
        }

        var result = new Vector3[n];
        for (int i = 0; i < n; i++)
        {
            if (tris.Count == 0 || total <= 0f)
            {
                // Plan B: nube de puntos alrededor del centro
                result[i] = dissolveCenter + Random.insideUnitSphere * (targetSize * 0.4f);
                continue;
            }

            int ti = cum.BinarySearch(Random.value * total);
            if (ti < 0) ti = ~ti;
            if (ti >= tris.Count) ti = tris.Count - 1;

            float u = Random.value, w = Random.value;
            if (u + w > 1f) { u = 1f - u; w = 1f - w; }
            var tr = tris[ti];
            result[i] = tr[0] + (tr[1] - tr[0]) * u + (tr[2] - tr[0]) * w;
        }
        return result;
    }

    void UpdateDissolving()
    {
        float dt = Time.deltaTime;
        t += dt;

        for (int i = 0; i < count; i++)
        {
            if (!released[i])
            {
                if (t < delay[i]) continue;
                released[i] = true;
                Vector3 outward = (pos[i] - dissolveCenter).normalized;
                vel[i] = outward * (burstSpeed * (0.4f + seed[i]))
                       + wind * (0.5f + seed[i])
                       + Random.onUnitSphere * (burstSpeed * 0.3f);
            }

            float s = seed[i] * 20f;
            vel[i] += new Vector3(Mathf.Sin(t * 2f + s),
                                  Mathf.Cos(t * 1.7f + s * 0.7f),
                                  Mathf.Sin(t * 1.3f + s * 0.4f)) * (0.4f * dt);
            pos[i] += vel[i] * dt;
            vel[i] *= Mathf.Max(0f, 1f - drag * dt);
        }

        // Se desvanecen hacia el final
        float fadeLen = 1.8f;
        float alpha = 1f - Mathf.Clamp01((t - (dissolveTime - fadeLen)) / fadeLen);
        WriteParticles(alpha);

        // La galeria aparece mientras las particulas se dispersan
        if (t >= galleryDelay && gallery != null)
        {
            if (!galleryStarted)
            {
                galleryStarted = true;
                gallery.enabled = true;
            }
            gallery.appear = Mathf.SmoothStep(0f, 1f, (t - galleryDelay) / galleryFadeTime);
        }

        if (t >= dissolveTime && (gallery == null || gallery.appear >= 1f))
        {
            Destroy(psObj);
            if (pivot != null) Destroy(pivot.gameObject);
            state = IntroState.Done;
        }
    }

    void WriteParticles(float alpha)
    {
        Color c = baseColor;
        c.a = alpha;
        for (int i = 0; i < count; i++)
        {
            parts[i].position = pos[i];
            parts[i].velocity = Vector3.zero;
            parts[i].startColor = c;
            parts[i].startSize = particleSize;
            parts[i].startLifetime = 1000f;
            parts[i].remainingLifetime = 1000f;
        }
        ps.SetParticles(parts, count);
    }
}

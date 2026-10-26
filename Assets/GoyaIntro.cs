using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Introduccion de la experiencia:
///  1. Tu objeto de Blender flota delante de la persona (gira lento y respira) y la recibe:
///     arriba de el aparecen, una por una, las reglas de la experiencia ("Rules").
///  2. Gesto para deshacerlo: apuntarlo con el PUÑO CERRADO y despues ABRIR la mano,
///     manteniendola abierta sobre el objeto un momento. Asi no se deshace con cualquier
///     movimiento de manos. El objeto tiembla y crece mientras se carga el gesto.
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

    [Header("Gesto con las manos extendidas")]
    [Tooltip("Deshacer al cabrio extendiendo las dos manos abiertas hacia el (no hace falta apuntar)")]
    public bool openHandsGesture = true;

    [Header("Gesto con el cuerpo (necesita el tracker con el cuerpo)")]
    [Tooltip("Deshacer al cabrio levantando los dos brazos e inclinandose un poco hacia adelante")]
    public bool bodyGesture = true;
    [Tooltip("Que tan arriba tienen que estar los brazos (0 = a la altura del hombro, 1 = bien arriba)")]
    public float armsUpLevel = 0.6f;
    [Tooltip("Cuanto hay que inclinarse hacia adelante (0.05 = un poco)")]
    public float leanForward = 0.05f;
    [Tooltip("Si no se detecta la inclinacion, alcanza con sostener los brazos arriba estos segundos (0 = siempre hace falta inclinarse)")]
    public float armsOnlyTime = 3f;

    [Header("Toque con la mano")]
    [Tooltip("Deshacer al cabrio apuntandolo con la mano (ademas del gesto con el cuerpo)")]
    public bool handGesture = false;
    [Tooltip("Que tan cerca del objeto tiene que apuntar la mano (multiplica el tamaño)")]
    public float touchRadiusFactor = 0.45f;
    [Tooltip("Segundos con la mano abierta apuntando al objeto (despues de haberlo apuntado con el puño)")]
    public float touchHoldTime = 1.2f;
    [Tooltip("Exigir el puño cerrado antes de abrir la mano (si se apaga, alcanza con la mano abierta)")]
    public bool requireFistFirst = true;
    [Tooltip("Segundos al comienzo en los que el objeto no se puede deshacer")]
    public float startGrace = 2f;
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

    [Header("Fuego (antes de deshacerse)")]
    [Tooltip("El cabrio se prende fuego, se carboniza y despues se dispersa en brasas")]
    public bool burn = true;
    [Tooltip("Segundos que arde antes de dispersarse")]
    public float burnTime = 2.5f;
    [Tooltip("Llamas por segundo cuando el fuego esta al maximo")]
    public float fireRate = 300f;
    public Color fireColor = new Color(1f, 0.5f, 0.1f, 1f);
    [Tooltip("Color de las brasas en que se deshace")]
    public Color emberColor = new Color(1f, 0.3f, 0.05f, 1f);

    [Header("Niebla (afecta a objetos con iluminacion, como el tuyo)")]
    public bool fog = true;
    public Color fogColor = new Color(0.03f, 0.03f, 0.06f);
    public float fogDensity = 0.22f;

    [Header("Sonido (opcional)")]
    public AudioClip dissolveSound;
    [Range(0f, 1f)] public float soundVolume = 0.9f;

    [Header("Bienvenida: el cabrio explica las reglas")]
    public bool showRules = true;
    [Tooltip("Frases que dice el cabrio, en orden. La ultima queda fija hasta que se haga el gesto. " +
             "Usar \\n para cortar la linea.")]
    [TextArea(2, 4)]
    public string[] rules =
    {
        "Bienvenido al sueño de Goya.",
        "Soy el Gran Cabrón.\nTe voy a mostrar sus visiones.",
        "Sus visiones están en el aire.\nBarré con la mano abierta y aparecen.",
        "Barré una visión y se hará brasa,\nque viaja a otro lugar a tu alrededor.",
        "Levantá los brazos para invocar\nla visión siguiente.",
        "Inclinate hacia adelante para acercarte,\nhacia atrás para alejarte.",
        "Cuanto más te muevas,\nmás oscuro será el sueño.",
        "Extendé las dos manos abiertas\nhacia mí para empezar."
    };
    [Tooltip("Segundos que se ve cada frase")]
    public float ruleDuration = 4.5f;
    [Tooltip("No se puede deshacer al cabrio hasta que termine de explicar (la tecla I siempre saltea)")]
    public bool waitForRules = true;
    public Color textColor = new Color(1f, 0.92f, 0.8f, 1f);
    [Tooltip("Tamaño de las letras")]
    public float textSize = 0.012f;
    [Tooltip("Voz opcional para cada frase (mismo orden que Rules). Puede quedar vacio.")]
    public AudioClip[] narration;

    [Header("Pruebas")]
    [Tooltip("Saltear la introduccion y mostrar la galeria directo")]
    public bool skipIntro = false;

    enum IntroState { Waiting, Burning, Dissolving, Done }
    IntroState state = IntroState.Waiting;

    Transform rig, pivot;
    Vector3 basePos;
    float touch;
    float armsHeld;
    bool armedLeft, armedRight;   // la mano ya apunto al objeto con el puño cerrado
    float waitingSince = -1f;
    TextMesh rulesText;
    int lastRule = -1;
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

    // fuego
    ParticleSystem fire;
    float burnT;
    Material[] burnMats;
    Color[] burnOriginal;
    Color[] emberColors;

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
        if (showRules && rules != null && rules.Length > 0) CreateRulesText();
    }

    // ---------------------------------------------------------------- reglas

    void CreateRulesText()
    {
        var go = new GameObject("ReglasCabrio");
        go.transform.SetParent(transform, false);
        go.transform.position = basePos + Vector3.up * (targetSize * 0.6f + 0.12f);
        // TextMesh se lee de frente cuando su eje "forward" apunta en contra de la camara
        go.transform.rotation = Quaternion.LookRotation(go.transform.position - rig.position);

        rulesText = go.AddComponent<TextMesh>();
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        rulesText.font = font;
        go.GetComponent<MeshRenderer>().sharedMaterial = font.material;
        rulesText.fontSize = 64;
        rulesText.characterSize = textSize;
        rulesText.anchor = TextAnchor.LowerCenter;
        rulesText.alignment = TextAlignment.Center;
        rulesText.color = new Color(textColor.r, textColor.g, textColor.b, 0f);
        rulesText.text = "";
    }

    // Devuelve true cuando ya se mostraron todas las frases
    bool UpdateRules()
    {
        if (rulesText == null) return true;

        float elapsed = Time.time - waitingSince;
        int last = rules.Length - 1;
        int index = Mathf.Min(last, Mathf.FloorToInt(elapsed / Mathf.Max(0.5f, ruleDuration)));
        float local = elapsed - index * ruleDuration;

        if (index != lastRule)
        {
            lastRule = index;
            rulesText.text = rules[index].Replace("\\n", "\n");
            if (narration != null && index < narration.Length && narration[index] != null)
                AudioSource.PlayClipAtPoint(narration[index], basePos, soundVolume);
        }

        // Aparece y se desvanece; la ultima frase queda fija
        float fadeIn = Mathf.Clamp01(local / 0.6f);
        float fadeOut = index == last ? 1f : Mathf.Clamp01((ruleDuration - local) / 0.6f);
        Color c = textColor;
        c.a = textColor.a * Mathf.Min(fadeIn, fadeOut);
        rulesText.color = c;

        return index == last && local >= 0.6f;
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
        else if (state == IntroState.Burning) UpdateBurning();
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

        if (waitingSince < 0f) waitingSince = Time.time;
        bool leftOpen, rightOpen;
        bool nearL = ProcessHand(hands.Left, cursorLeft, hands.leftColor, out leftOpen);
        bool nearR = ProcessHand(hands.Right, cursorRight, hands.rightColor, out rightOpen);

        // Puño cerrado sobre el objeto = gesto "cargado"; se pierde si la mano sale de cuadro
        armedLeft = Arm(armedLeft, hands.Left, nearL, leftOpen);
        armedRight = Arm(armedRight, hands.Right, nearR, rightOpen);

        bool rulesDone = UpdateRules();
        bool allowed = Time.time - waitingSince >= startGrace && (rulesDone || !waitForRules);
        var body = hands.Body;
        bool armsUp = bodyGesture && body.visible && body.armLeft > armsUpLevel && body.armRight > armsUpLevel;
        armsHeld = armsUp ? armsHeld + dt : 0f;
        bool bodyPose = armsUp && (body.lean > leanForward || (armsOnlyTime > 0f && armsHeld >= armsOnlyTime));
        bool handPose = handGesture &&
                        ((nearL && leftOpen && (armedLeft || !requireFistFirst)) ||
                         (nearR && rightOpen && (armedRight || !requireFistFirst)));
        bool openHands = openHandsGesture && hands.Left.visible && hands.Right.visible && leftOpen && rightOpen;
        bool touching = allowed && (openHands || bodyPose || handPose);

        touch = touching ? touch + dt : Mathf.Max(0f, touch - dt * 2f);
        float progress = Mathf.Clamp01(touch / touchHoldTime);

        // Flota, gira y tiembla cuanto mas cerca esta de deshacerse
        Vector3 tremble = Random.insideUnitSphere * (0.012f * progress);
        pivot.position = basePos + Vector3.up * (Mathf.Sin(Time.time * 1.1f) * bobAmount) + tremble;
        pivot.Rotate(0f, rotateSpeed * dt, 0f, Space.World);
        pivot.localScale = Vector3.one * (1f + 0.03f * Mathf.Sin(Time.time * 2f) + 0.08f * progress);

        if (progress >= 1f || Input.GetKeyDown(KeyCode.I))
        {
            if (burn) BeginBurn();
            else BeginDissolve();
        }
    }

    // ---------------------------------------------------------------- fuego

    void BeginBurn()
    {
        if (rulesText != null) Destroy(rulesText.gameObject);
        cursorLeft.enabled = false;
        cursorRight.enabled = false;

        // Materiales propios del modelo, para irlos carbonizando
        var mats = new List<Material>();
        foreach (var r in introObject.GetComponentsInChildren<Renderer>()) mats.AddRange(r.materials);
        burnMats = mats.ToArray();
        burnOriginal = new Color[burnMats.Length];
        for (int i = 0; i < burnMats.Length; i++) burnOriginal[i] = GetColor(burnMats[i]);

        fire = CreateFire();
        burnT = 0f;
        state = IntroState.Burning;
    }

    void UpdateBurning()
    {
        float dt = Time.deltaTime;
        burnT += dt;
        float k = Mathf.Clamp01(burnT / Mathf.Max(0.1f, burnTime));

        // El fuego crece, el cabrio tiembla y se va poniendo negro, con destellos de brasa
        var em = fire.emission;
        em.rateOverTime = fireRate * Mathf.Clamp01(k * 2.5f);
        float flicker = 0.5f + 0.5f * Mathf.PerlinNoise(Time.time * 8f, 0f);
        Color charred = Color.Lerp(new Color(0.05f, 0.02f, 0.02f), emberColor, 0.35f * flicker * k);
        for (int i = 0; i < burnMats.Length; i++)
            SetColor(burnMats[i], Color.Lerp(burnOriginal[i], charred, Mathf.SmoothStep(0f, 1f, k)));

        Vector3 tremble = Random.insideUnitSphere * (0.012f + 0.02f * k);
        pivot.position = basePos + Vector3.up * (Mathf.Sin(Time.time * 1.1f) * bobAmount) + tremble;
        pivot.Rotate(0f, rotateSpeed * (1f + k) * dt, 0f, Space.World);

        if (burnT >= burnTime) BeginDissolve();
    }

    ParticleSystem CreateFire()
    {
        var go = new GameObject("Fuego_Cabrio");
        go.transform.SetParent(transform, false);
        go.transform.position = basePos;
        var f = go.AddComponent<ParticleSystem>();
        f.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = f.main;
        main.loop = true;
        main.playOnAwake = false;
        main.maxParticles = 3000;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.6f, 1.4f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.05f, 0.25f);
        main.startSize = new ParticleSystem.MinMaxCurve(targetSize * 0.08f, targetSize * 0.22f);
        main.startColor = fireColor;
        main.gravityModifier = -0.12f;   // las llamas suben

        var emission = f.emission;
        emission.rateOverTime = 0f;

        // Las llamas salen de la superficie del modelo (si se puede leer), si no de una esfera
        var shape = f.shape;
        var mr = introObject.GetComponentInChildren<MeshRenderer>();
        var mf = mr != null ? mr.GetComponent<MeshFilter>() : null;
        if (mf != null && mf.sharedMesh != null && mf.sharedMesh.isReadable)
        {
            shape.shapeType = ParticleSystemShapeType.MeshRenderer;
            shape.meshRenderer = mr;
            go.transform.position = Vector3.zero;
        }
        else
        {
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = targetSize * 0.35f;
        }

        // Amarillo al nacer, naranja, rojo oscuro y humo
        var col = f.colorOverLifetime;
        col.enabled = true;
        var g = new Gradient();
        g.SetKeys(
            new[] {
                new GradientColorKey(new Color(1f, 0.9f, 0.5f), 0f),
                new GradientColorKey(fireColor, 0.3f),
                new GradientColorKey(new Color(0.5f, 0.06f, 0.02f), 0.7f),
                new GradientColorKey(new Color(0.08f, 0.06f, 0.06f), 1f)
            },
            new[] {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.9f, 0.1f),
                new GradientAlphaKey(0.6f, 0.6f),
                new GradientAlphaKey(0f, 1f)
            });
        col.color = g;

        var size = f.sizeOverLifetime;
        size.enabled = true;
        size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 0.6f, 1f, 1.4f));

        var noise = f.noise;
        noise.enabled = true;
        noise.strength = 0.25f;
        noise.frequency = 1.2f;

        Shader sh = Shader.Find("Sprites/Default");
        if (sh == null) sh = Shader.Find("Particles/Standard Unlit");
        var mat = new Material(sh);
        mat.mainTexture = softTex;
        var rend = go.GetComponent<ParticleSystemRenderer>();
        rend.sharedMaterial = mat;
        rend.renderMode = ParticleSystemRenderMode.Billboard;
        rend.sortingOrder = 7;

        f.Play();
        return f;
    }

    static Color GetColor(Material m)
    {
        if (m.HasProperty("_BaseColor")) return m.GetColor("_BaseColor");
        if (m.HasProperty("_Color")) return m.GetColor("_Color");
        return Color.white;
    }

    static void SetColor(Material m, Color c)
    {
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (m.HasProperty("_Color")) m.SetColor("_Color", c);
    }

    static bool Arm(bool armed, HandOSCReceiver.HandState hand, bool near, bool open)
    {
        if (!hand.visible) return false;
        if (near && !open) return true;
        return armed;
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

        // Despues del fuego se deshace en brasas: cada particula de un tono entre rojo y amarillo
        emberColors = new Color[count];
        for (int i = 0; i < count; i++)
            emberColors[i] = Color.Lerp(emberColor, new Color(1f, 0.8f, 0.3f), seed[i] * seed[i]);
        if (fire != null)
        {
            var em = fire.emission;
            em.rateOverTime = 0f;
            Destroy(fire.gameObject, 2f);
            fire = null;
        }

        ps.Play();
        WriteParticles(1f);

        introObject.SetActive(false);
        if (rulesText != null) Destroy(rulesText.gameObject);
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
            if (burn) vel[i] += Vector3.up * (0.25f * dt);   // las brasas suben
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
        // Brasas: se van apagando de rojo encendido a ceniza
        float cool = Mathf.Clamp01(t / Mathf.Max(0.1f, dissolveTime * 0.8f));
        var ash = new Color(0.15f, 0.12f, 0.12f, alpha);
        for (int i = 0; i < count; i++)
        {
            parts[i].position = pos[i];
            parts[i].velocity = Vector3.zero;
            if (burn && emberColors != null)
            {
                Color e = Color.Lerp(emberColors[i], ash, cool * (0.5f + 0.5f * seed[i]));
                e.a = alpha;
                parts[i].startColor = e;
            }
            else parts[i].startColor = c;
            parts[i].startSize = particleSize;
            parts[i].startLifetime = 1000f;
            parts[i].remainingLifetime = 1000f;
        }
        ps.SetParticles(parts, count);
    }
}

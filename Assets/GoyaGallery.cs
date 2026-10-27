using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Galeria onirica: pone 3 cuadros en arco frente a la persona, cubiertos por un velo.
/// Las manos funcionan como PUNTEROS: se traza un rayo desde la cabeza a traves de la mano,
/// y al pasar por un cuadro el velo se disuelve y la pintura se revela.
///
/// Usa SpriteRenderer (shader Sprites/Default), que funciona sin problemas en el celular.
///
/// Progresion hacia lo oscuro: cuanto mas se mueven las manos, mas sube "darkness" (0 a 1).
/// Cada vez que un cuadro se rearma con las particulas, vuelve como la obra siguiente de
/// "Obras mas oscuras" (cuanto mas oscuridad acumulada, mas adelante en la lista salta).
///
/// Galeria 360: si "Painting Sequence" tiene obras, los cuadros se reparten en un anillo
/// alrededor de la persona. Las primeras de la lista llenan el anillo y cada cuadro que se
/// desarma vuelve como la siguiente de la lista: ordenarla de lo cotidiano a lo mas oscuro.
/// Si la lista esta vacia, se usan los 3 cuadros de siempre (y "Darker Paintings").
///
/// Uso: objeto vacio "Galeria" con este script. Las imagenes se importan como Sprite
/// (Texture Type: Sprite (2D and UI)); no hace falta tildarles Read/Write.
/// Gestos:
///  - Apuntar una obra (cualquier mano): se revela.
///  - BARRER una obra revelada con la mano abierta (moverla rapido de lado a lado): se desarma.
///  - Apuntar una obra y CERRAR EL PUÑO: te acercas a esa obra mientras mantengas el puño.
///    Los DOS PUÑOS cerrados: te alejas.
/// Sin tracker (en la PC): clic derecho + mouse o flechas = mirar / girar, W / S o flechas
/// arriba / abajo = caminar, mantener clic izquierdo sobre una obra = revelarla (apunta al centro
/// de la vista), mantener E = barrerla (la desarma), O = sumar oscuridad.
/// </summary>
public class GoyaGallery : MonoBehaviour
{
    [Header("Cuadros (arrastra las imagenes importadas como Sprite)")]
    public Sprite centerPainting;
    public Sprite leftPainting;
    public Sprite rightPainting;

    [Header("Camino: un recorrido con una zona por epoca")]
    [Tooltip("Las obras se reparten a los costados de un camino que se recorre caminando; cada epoca es una zona con su propio ambiente (GoyaEnvironment)")]
    public bool pathMode = true;
    [Tooltip("Largo de cada zona (metros)")]
    public float zoneLength = 9f;
    [Tooltip("Medio ancho del camino por donde se puede caminar (metros)")]
    public float corridorHalfWidth = 1.6f;
    [Tooltip("Velocidad al caminar por el camino (metros por segundo)")]
    public float pathMoveSpeed = 1.1f;
    [Tooltip("Hasta que distancia (metros) los gestos de armar y desarmar alcanzan a la obra que se mira")]
    public float gestureDistance = 7f;

    [Header("Galeria 360 (si tiene obras, reemplaza a los 3 cuadros de arriba)")]
    [Tooltip("Todas las obras en orden: primero las de la vida cotidiana, al final las mas oscuras. " +
             "Las primeras llenan el anillo; cada cuadro que se desarma vuelve como la siguiente.")]
    public Sprite[] paintingSequence;
    [Tooltip("Un barrido cambia TODAS las obras juntas a la tanda siguiente. Si se apaga, cada obra cambia sola (anillo de 360)")]
    public bool changeAllTogether = true;
    [Tooltip("Cuantas obras de la lista tiene cada epoca, en orden. Las tandas nunca mezclan epocas")]
    public int[] epochSizes = { 6, 7, 5, 4, 5 };
    [Tooltip("En cuantas tandas se reparte cada epoca")]
    public int batchesPerEpoch = 2;
    [Tooltip("Sin epocas cargadas: cuantas obras por tanda. Si se cambian de a una: cuantos cuadros hay en el anillo")]
    public int batchSize = 7;
    [Tooltip("Grados entre las obras de una tanda. Cada tanda aparece alrededor de donde se esta mirando")]
    public float batchSpacing = 55f;
    [Tooltip("Una obra a la vez, en orden, y cada una aparece en un lugar al azar de la galeria (ignora las epocas por tanda)")]
    public bool oneAtATime = true;
    [Tooltip("Grados de la galeria, centrados adelante (180 = de un costado al otro, sin nada atras)")]
    public float arcDegrees = 180f;
    [Tooltip("Lugares posibles dentro de la galeria para la obra que aparece")]
    public int arcSlots = 5;
    [Tooltip("Cuantas obras de la lista puede saltear de golpe si las manos se mueven mucho")]
    public int maxSkip = 1;

    [Header("Moverse (acercarse y alejarse de las obras)")]
    public bool allowMove = true;
    [Tooltip("Velocidad al caminar (metros por segundo)")]
    public float moveSpeed = 0.6f;
    [Tooltip("Segundos con el puño cerrado antes de empezar a moverse (evita moverse sin querer)")]
    public float fistHoldTime = 0.4f;
    [Tooltip("Lo mas cerca que se puede llegar a los cuadros (metros)")]
    public float minDistanceToPaintings = 0.7f;

    [Header("Gestos")]
    [Tooltip("Un dedo levantado (cualquier mano) te acerca a la obra; un dedo en cada mano te aleja")]
    public bool fingerWalk = true;
    [Tooltip("Gesto viejo: puño sobre una obra para acercarse, dos puños para alejarse")]
    public bool fistWalk = false;
    [Tooltip("Gesto viejo: barrer la obra con la mano abierta para armarla o desarmarla")]
    public bool swipeGestures = false;
    [Tooltip("Que tan arriba tiene que estar la mano levantada en la imagen de la camara (0 = borde de arriba, 1 = borde de abajo)")]
    [Range(0f, 1f)] public float handHighY = 0.45f;
    [Tooltip("Segundos que hay que sostener los gestos de armar (dos brazos arriba) y desarmar (dos puños)")]
    public float gestureHoldTime = 0.6f;

    [Header("Cuerpo (necesita el tracker con el cuerpo activado)")]
    [Tooltip("Inclinarse hacia la camara para avanzar hacia donde se mira, y echarse atras para retroceder")]
    public bool leanWalk = false;
    [Tooltip("Cuanto hay que inclinarse hacia adelante para empezar a avanzar (0.1 = hombros 10% mas grandes en la camara)")]
    public float leanForward = 0.1f;
    [Tooltip("Cuanto hay que echarse atras para retroceder (negativo)")]
    public float leanBack = -0.08f;
    [Tooltip("Si el tracker ve el cuerpo, un brazo levantado tambien cuenta aunque la mano quede baja en la imagen (0 = hombro, 1 = bien arriba)")]
    public float armsUpLevel = 0.6f;

    [Header("Referencias")]
    [Tooltip("Si queda vacio, lo busca solo")]
    public HandOSCReceiver hands;
    [Tooltip("Las manos y el puntero giran hacia donde mira la cabeza (necesario para tocar los cuadros de atras)")]
    public bool handsFollowHead = true;

    [Tooltip("Visibilidad general de los cuadros (0 a 1). Lo usa la introduccion para hacerlos aparecer de a poco.")]
    [Range(0f, 1f)] public float appear = 1f;

    [Header("Disposicion")]
    [Tooltip("Distancia de los cuadros a la persona (metros)")]
    public float distance = 2.5f;
    [Tooltip("Cuantos grados se separan los cuadros laterales del central")]
    public float sideAngle = 35f;
    [Tooltip("Lado mas largo de un cuadro (metros)")]
    public float paintingMaxSize = 1.6f;
    [Tooltip("El cuadro central se agranda por este factor")]
    public float centerScale = 1.15f;

    [Header("Mano como puntero")]
    [Tooltip("Cuanto se amplifica el movimiento de la mano para llegar a los cuadros")]
    public float reach = 1.6f;
    [Tooltip("Punto de la mano que apunta (9 = centro de la palma)")]
    public int pointerLandmark = 9;
    public float cursorSize = 0.18f;

    [Header("Revelado")]
    [Tooltip("Las obras empiezan como polvo en el aire y se arman al barrerlas (si se apaga: velo que se revela al apuntar)")]
    public bool assembleFromDust = true;
    [Tooltip("Segundos de mano sobre el cuadro para revelarlo del todo")]
    public float revealTime = 1.2f;
    [Tooltip("Que tan rapido se 'olvida' un revelado a medias (0 = nunca)")]
    public float forgetSpeed = 0.25f;
    [Tooltip("Velocidad minima de la mano abierta (m/s) para que cuente como 'barrer' (mas bajo = mas facil)")]
    public float swipeSpeed = 0.5f;
    [Tooltip("Cuanto recorrido de mano abierta (m) sobre la obra hace falta para desarmarla (mas bajo = mas facil)")]
    public float swipeDistance = 0.35f;
    [Tooltip("Color y transparencia del cuadro cubierto por el velo")]
    public Color veilColor = new Color(0.35f, 0.35f, 0.5f, 0.12f);

    [Header("Progresion hacia lo oscuro")]
    [Tooltip("Pinturas Negras ordenadas de menos a mas oscura. Al rearmarse, cada cuadro vuelve como una de estas.")]
    public Sprite[] darkerPaintings;
    [Tooltip("Oscuridad acumulada (0 = inicio, 1 = lo mas oscuro). Se calcula sola con el movimiento de las manos.")]
    [Range(0f, 1f)] public float darkness;
    [Tooltip("Cuanto movimiento de manos (en metros recorridos) hace falta para llegar a oscuridad total")]
    public float movementForFullDark = 30f;
    [Tooltip("Por debajo de esta velocidad (m/s) el movimiento no cuenta (temblor del tracker)")]
    public float minHandSpeed = 0.15f;
    [Tooltip("Cuanto baja la oscuridad por segundo con las manos quietas (0 = nunca vuelve atras)")]
    public float darknessRecovery = 0.004f;
    [Tooltip("Velocidad de mano (m/s) que cuenta como agitacion maxima")]
    public float agitationSpeed = 1.5f;
    [Tooltip("Agitacion actual (0 a 1): que tan rapido se mueven las manos ahora")]
    [Range(0f, 1f)] public float agitation;
    [Tooltip("Color que toman los cuadros con oscuridad total")]
    public Color darkTint = new Color(0.6f, 0.5f, 0.45f, 1f);

    public class Painting
    {
        public string name;
        public Transform root;
        public SpriteRenderer renderer;
        public float halfW, halfH;
        public float reveal;       // 0 = cubierto, 1 = revelado
        public bool revealed;
        public bool hovered;       // alguna mano esta encima este cuadro
        public bool openHandOver;  // una mano ABIERTA esta encima
        public float swipeNow;     // recorrido de barrido en este cuadro (frame)
        public float swipe;        // recorrido acumulado de barrido (GoyaParticles la desarma con swipeDistance)
        public float phase;
        public Vector3 hitPoint;   // ultimo punto del cuadro tocado por una mano
        public float maxSize;      // lado mas largo en metros (se mantiene al cambiar de obra)
        public int darkIndex = -1; // -1 = obra original, si no posicion en darkerPaintings
        public Sprite original;
        public int originalIndex = -1; // posicion de la obra original en la lista (galeria 360)
        public bool active = true; // en una tanda mas chica que la mas grande, los cuadros que sobran se apagan
        public int zone;           // zona del camino (epoca)
    }

    public List<Painting> paintings = new List<Painting>();

    Transform rig;
    SpriteRenderer cursorLeft, cursorRight, cursorMouse;

    Sprite[] darkSprites;   // obras que pueden aparecer al rearmarse, en orden
    int nextDark;           // primera obra de darkSprites que todavia no se mostro
    int firstDark;          // valor inicial de nextDark (para reiniciar)
    bool ringMode;
    readonly List<Vector2Int> batches = new List<Vector2Int>(); // cada tanda: (primera obra, cantidad)
    int currentBatch;
    int currentSlot;     // lugar del anillo donde esta la obra (una a la vez)

    /// <summary>Centro del anillo (donde empieza la persona).</summary>
    public Vector3 Center { get { return center; } }

    /// <summary>Donde esta la persona ahora.</summary>
    public Vector3 RigPosition { get { return rig != null ? rig.position : transform.position; } }

    /// <summary>Largo total del camino (metros) y cantidad de zonas.</summary>
    public float PathLength { get; private set; }
    public int ZoneCount { get; private set; }

    /// <summary>Zona del camino en un punto, como numero con decimales (0 = principio de la primera, 1.5 = mitad de la segunda).</summary>
    public float ZoneAt(Vector3 pos)
    {
        return Mathf.Clamp((pos.z - center.z) / Mathf.Max(0.1f, zoneLength), 0f, Mathf.Max(0.001f, ZoneCount - 0.001f));
    }

    /// <summary>Cuanto se avanzo por el camino (0 a 1).</summary>
    public float PathProgress
    {
        get { return PathLength > 0f ? Mathf.Clamp01((RigPosition.z - center.z) / PathLength) : 0f; }
    }

    GoyaEnvironment environment;
    GoyaDiorama diorama;

    /// <summary>Hasta donde se puede caminar: el camino de obras mas los dioramas del final.</summary>
    public float WalkLength
    {
        get { return PathLength + (diorama != null ? Mathf.Max(0f, diorama.Length - diorama.minDistanceToLast) : 0f); }
    }
    Vector3 lastLeft, lastRight;
    bool hadLeft, hadRight;
    float speedLeft, speedRight;   // velocidad suavizada de cada mano
    float travelLeft, travelRight; // recorrido de cada mano en este cuadro (frame)

    Transform head;
    Vector3 handsBaseLocal;
    float handsYaw;

    Vector3 center;      // centro del anillo (donde empieza la persona)
    float fistTimer;
    Painting pointedLeft, pointedRight; // obra que apunta cada mano en este cuadro
    Painting walkTarget;                // obra hacia la que te acercas con el puño
    Painting mousePointed;              // obra apuntada con el mouse (PC)
    float walk;          // velocidad actual, suavizada (-1 atras, 1 adelante)
    Vector3 lastWalkDir = Vector3.forward;
    float formTimer, breakTimer, fingerTimer;
    bool formReady, breakReady;          // gesto hecho, listo para que lo use GoyaParticles
    bool formReleased, breakReleased;    // hay que soltar el gesto antes de repetirlo

    void Start()
    {
        if (hands == null) hands = FindFirstObjectByType<HandOSCReceiver>();
        if (hands == null)
        {
            Debug.LogError("[Galeria] No encuentro el objeto con HandOSCReceiver (Manos).");
            enabled = false;
            return;
        }
        rig = hands.transform.parent != null ? hands.transform.parent : transform;
        head = rig.Find("Head");
        handsBaseLocal = hands.transform.localPosition;
        center = rig.position;

        var sequence = new List<Sprite>();
        if (paintingSequence != null)
            foreach (var sp in paintingSequence) if (sp != null) sequence.Add(sp);

        ringMode = sequence.Count > 0;
        if (ringMode && pathMode) CreatePath(sequence);
        else if (ringMode && changeAllTogether) CreateBatches(sequence);
        else if (ringMode) CreateRing(sequence);
        else
        {
            CreatePainting(centerPainting, "Centro", 0f, centerScale);
            CreatePainting(leftPainting, "Izquierda", -sideAngle, 1f);
            CreatePainting(rightPainting, "Derecha", sideAngle, 1f);
            BuildDarkSprites();
        }

        var circle = MakeCircleSprite();
        cursorLeft = CreateCursor("CursorIzquierdo", circle);
        cursorRight = CreateCursor("CursorDerecho", circle);
        cursorMouse = CreateCursor("CursorMouse", circle);
    }

    // Anillo de 360: el primero adelante y los siguientes alternando derecha e izquierda,
    // asi las primeras obras de la lista quedan mas cerca de la vista inicial
    void CreateRing(List<Sprite> sequence)
    {
        int count = Mathf.Clamp(batchSize, 1, sequence.Count);
        float step = 360f / count;
        float spacing = 2f * Mathf.PI * distance / count;
        float maxSize = Mathf.Min(paintingMaxSize, spacing * 0.85f);

        for (int i = 0; i < count; i++)
        {
            int side = (i + 1) / 2;
            float angle = (i % 2 == 1 ? 1f : -1f) * side * step;
            float scale = (i == 0 ? centerScale : 1f) * maxSize / paintingMaxSize;
            var p = CreatePainting(sequence[i], "Anillo" + i, angle, scale);
            if (p != null) p.darkIndex = p.originalIndex = i;
        }

        darkSprites = sequence.ToArray();
        nextDark = firstDark = count;
    }

    // Camino: cada epoca es una zona; sus obras quedan a los costados, alternando izquierda y derecha,
    // algunas mas lejos que otras para que haya profundidad. Todas empiezan como polvo en el aire.
    void CreatePath(List<Sprite> sequence)
    {
        // Las pinturas negras que se recorren por dentro van al final, no planas en el camino
        diorama = GetComponent<GoyaDiorama>();
        if (diorama == null) diorama = gameObject.AddComponent<GoyaDiorama>();
        sequence = sequence.FindAll(s => !diorama.Replaces(s));

        var sizes = new List<int>();
        int n = sequence.Count, used = 0;
        if (epochSizes != null)
            foreach (int size in epochSizes)
            {
                int s = Mathf.Min(size, n - used);
                if (s <= 0) continue;
                sizes.Add(s);
                used += s;
            }
        if (used < n) sizes.Add(n - used);   // obras fuera de las epocas: una zona mas al final

        int start = 0;
        for (int zone = 0; zone < sizes.Count; zone++)
        {
            int count = sizes[zone];
            for (int j = 0; j < count; j++)
            {
                int idx = start + j;
                float side = j % 2 == 0 ? -1f : 1f;
                float t = count == 1 ? 0.5f : (float)j / (count - 1);
                float z = zone * zoneLength + 2f + t * (zoneLength - 3.5f);
                float depth = (j / 2) % 2 == 0 ? 0f : 1.3f;     // una de cada dos parejas queda mas atras
                float x = side * (corridorHalfWidth + 0.8f + depth);
                Vector3 pos = center + new Vector3(x, 0.12f * ((j % 3) - 1), z);
                // Mira hacia el camino y un poco hacia quien viene caminando
                var rot = Quaternion.LookRotation(new Vector3(side, 0f, 0.45f).normalized);
                var p = CreatePaintingAt(sequence[idx], "Obra" + idx, pos, rot, 1f + 0.3f * depth);
                if (p == null) continue;
                p.darkIndex = p.originalIndex = idx;
                p.zone = zone;
            }
            start += count;
        }

        ZoneCount = sizes.Count;
        PathLength = ZoneCount * zoneLength;
        darkSprites = sequence.ToArray();
        nextDark = firstDark = 0;

        environment = GetComponent<GoyaEnvironment>();
        if (environment == null) environment = gameObject.AddComponent<GoyaEnvironment>();
        diorama.Build(this);
    }

    // Tandas por epoca: cada epoca se reparte en "batchesPerEpoch" tandas, sin mezclarse con la siguiente
    void CreateBatches(List<Sprite> sequence)
    {
        int n = sequence.Count, start = 0;
        if (oneAtATime)
        {
            for (; start < n; start++) batches.Add(new Vector2Int(start, 1));
        }
        else if (epochSizes != null)
        {
            foreach (int size in epochSizes)
            {
                int s = Mathf.Min(size, n - start);
                if (s <= 0) continue;
                int parts = Mathf.Clamp(batchesPerEpoch, 1, s);
                for (int k = 0; k < parts; k++)
                {
                    int a = start + s * k / parts, b = start + s * (k + 1) / parts;
                    batches.Add(new Vector2Int(a, b - a));
                }
                start += s;
            }
        }
        // Obras que quedaron fuera de las epocas: tandas de "batchSize"
        int chunk = Mathf.Max(1, batchSize);
        for (; start < n; start += chunk) batches.Add(new Vector2Int(start, Mathf.Min(chunk, n - start)));

        int slots = 0;
        foreach (var b in batches) slots = Mathf.Max(slots, b.y);
        for (int i = 0; i < slots; i++)
        {
            var p = CreatePainting(sequence[i], "Tanda" + i, 0f, 1f);
            if (p != null) p.darkIndex = p.originalIndex = i;
        }

        darkSprites = sequence.ToArray();
        currentSlot = Mathf.Max(2, arcSlots) / 2;   // la primera obra aparece adelante
        ApplyBatch(0, 0f);
        firstDark = nextDark;
    }

    // Pone la tanda "b" en los cuadros, repartida alrededor del angulo "yawCenter"
    void ApplyBatch(int b, float yawCenter)
    {
        currentBatch = b;
        var batch = batches[b];
        for (int i = 0; i < paintings.Count; i++)
        {
            var p = paintings[i];
            p.active = i < batch.y;
            p.reveal = 0f;
            p.revealed = false;
            p.swipe = 0f;
            if (!p.active)
            {
                p.renderer.enabled = false;
                continue;
            }
            float angle = yawCenter + (i - (batch.y - 1) * 0.5f) * batchSpacing;
            Quaternion yaw = Quaternion.Euler(0f, angle, 0f);
            p.root.position = center + yaw * Vector3.forward * distance;
            p.root.rotation = yaw;
            p.darkIndex = batch.x + i;
            SetSprite(p, darkSprites[batch.x + i]);
        }
        nextDark = batch.x + batch.y;
    }

    float ViewYaw()
    {
        Vector3 f = head != null ? head.forward : rig.forward;
        f.y = 0f;
        return f.sqrMagnitude < 1e-4f ? 0f : Mathf.Atan2(f.x, f.z) * Mathf.Rad2Deg;
    }

    Painting CreatePainting(Sprite sprite, string label, float angle, float scaleMul)
    {
        if (sprite == null)
        {
            Debug.LogWarning("[Galeria] Falta la imagen del cuadro: " + label);
            return null;
        }

        Quaternion yaw = Quaternion.Euler(0f, angle, 0f);
        return CreatePaintingAt(sprite, label, rig.position + yaw * Vector3.forward * distance, yaw, scaleMul);
    }

    Painting CreatePaintingAt(Sprite sprite, string label, Vector3 position, Quaternion rotation, float scaleMul)
    {
        if (sprite == null)
        {
            Debug.LogWarning("[Galeria] Falta la imagen del cuadro: " + label);
            return null;
        }

        var root = new GameObject("Cuadro_" + label).transform;
        root.SetParent(transform, false);
        root.position = position;
        root.rotation = rotation;

        var sr = root.gameObject.AddComponent<SpriteRenderer>();
        sr.color = veilColor;
        sr.sortingOrder = 0;

        var p = new Painting
        {
            name = label,
            root = root,
            renderer = sr,
            maxSize = paintingMaxSize * scaleMul,
            phase = Random.value * 6.28f
        };
        p.original = sprite;
        SetSprite(p, sprite);
        paintings.Add(p);
        return p;
    }

    /// <summary>Cambia la obra de un cuadro manteniendo su lugar y su tamano maximo.</summary>
    public void SetSprite(Painting p, Sprite sprite)
    {
        Vector3 size = sprite.bounds.size;
        float s = p.maxSize / Mathf.Max(size.x, size.y);
        p.root.localScale = Vector3.one * s;
        p.renderer.sprite = sprite;
        p.halfW = size.x * s * 0.5f;
        p.halfH = size.y * s * 0.5f;
    }

    /// <summary>
    /// Elige la obra con la que se va a rearmar un cuadro: la siguiente de la lista, o mas
    /// adelante si la oscuridad acumulada ya va mas lejos. Nunca repite una que ya se ve en otro cuadro.
    /// Devuelve null si no hay obras oscuras cargadas (el cuadro se rearma igual que antes).
    /// </summary>
    public Sprite PickNextSprite(Painting p)
    {
        int n = darkSprites != null ? darkSprites.Length : 0;
        if (n == 0) return null;

        // La siguiente de la lista; si las manos se movieron mucho, saltea hasta "maxSkip" obras
        int byDarkness = firstDark + Mathf.FloorToInt(darkness * (n - firstDark));
        int start = Mathf.Max(nextDark, Mathf.Min(byDarkness, nextDark + maxSkip));
        if (start >= n) start = n - 1;

        // Buscar desde "start" hacia adelante y despues hacia atras una que no este en pantalla
        for (int i = start; i < n; i++)
            if (TryTake(p, i)) return darkSprites[i];
        for (int i = start - 1; i >= 0; i--)
            if (TryTake(p, i)) return darkSprites[i];
        return null;
    }

    /// <summary>Hay otra tanda de obras despues de la que se ve ahora.</summary>
    public bool HasNextBatch()
    {
        return currentBatch + 1 < batches.Count;
    }

    /// <summary>
    /// Pone la tanda siguiente: una sola obra en un lugar al azar del anillo, o la tanda entera
    /// alrededor de donde se esta mirando. Los cuadros que sobran quedan apagados.
    /// </summary>
    public void NextBatch()
    {
        if (!HasNextBatch()) return;
        ApplyBatch(currentBatch + 1, oneAtATime ? RandomSlotAngle() : ViewYaw());
    }

    float RandomSlotAngle()
    {
        int slots = Mathf.Max(2, arcSlots);
        int next = Random.Range(1, slots);           // nunca el mismo lugar
        currentSlot = (currentSlot + next) % slots;
        return -arcDegrees * 0.5f + currentSlot * arcDegrees / (slots - 1);
    }

    bool TryTake(Painting p, int i)
    {
        if (i == p.darkIndex || IsShown(i)) return false;
        p.darkIndex = i;
        nextDark = Mathf.Max(nextDark, i + 1);
        return true;
    }

    bool IsShown(int darkIndex)
    {
        foreach (var q in paintings) if (q.darkIndex == darkIndex) return true;
        return false;
    }

    void BuildDarkSprites()
    {
        nextDark = firstDark = 0;
        var list = new List<Sprite>();
        if (darkerPaintings != null)
        {
            foreach (var sp in darkerPaintings)
                if (sp != null) list.Add(sp);
        }
        darkSprites = list.ToArray();
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

    // Circulo suave generado por codigo (no hace falta ninguna imagen)
    static Sprite MakeCircleSprite()
    {
        const int n = 64;
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
        return Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), n);
    }

    bool GetRayDirection(HandOSCReceiver.HandState hand, out Vector3 dir)
    {
        dir = Vector3.forward;
        if (!hand.visible) return false;

        // Posicion de la mano relativa al Rig (Manos es hijo del Rig, sin escala),
        // girada hacia donde mira la cabeza
        Vector3 local = handsBaseLocal + hand.points[pointerLandmark];
        Vector3 amplified = new Vector3(local.x * reach, local.y * reach, local.z);
        dir = rig.TransformDirection(Quaternion.Euler(0f, handsYaw, 0f) * amplified).normalized;
        return true;
    }

    // Moverse: puño cerrado apuntando una obra = acercarse a esa obra; dos puños = alejarse.
    // Siempre dentro del anillo de cuadros.
    void UpdateWalk()
    {
        if (!allowMove) return;
        float dt = Time.deltaTime;

        bool fistL = fistWalk && hands.Left.visible && !hands.Left.open;
        bool fistR = fistWalk && hands.Right.visible && !hands.Right.open;
        bool both = fistL && fistR;

        // La obra se elige al cerrar el puño y queda fija mientras siga cerrado
        if (!fistL && !fistR) walkTarget = null;
        else if (walkTarget == null && !both) walkTarget = fistL ? pointedLeft : pointedRight;
        fistTimer = (fistL || fistR) ? fistTimer + dt : 0f;

        float wanted = 0f;
        Painting target = null;
        if (fistTimer >= fistHoldTime)
        {
            if (both) wanted = -1f;
            else if (walkTarget != null) { wanted = 1f; target = walkTarget; }
        }

        // Un dedo levantado: acercarse a la obra que se esta mirando. Un dedo en cada mano: alejarse
        if (fingerWalk)
        {
            bool oneL = HandOSCReceiver.IsOneFinger(hands.Left), oneR = HandOSCReceiver.IsOneFinger(hands.Right);
            fingerTimer = (oneL || oneR) ? fingerTimer + dt : 0f;
            if (fingerTimer >= fistHoldTime)
            {
                if (oneL && oneR) wanted = -1f;
                else { wanted = 1f; target = PaintingInView(); }
            }
        }

        // Cuerpo: inclinarse hacia adelante avanza hacia donde se mira, echarse atras retrocede
        var body = hands.Body;
        if (leanWalk && body.visible && wanted == 0f)
        {
            if (body.lean > leanForward) wanted = Mathf.Clamp01((body.lean - leanForward) / Mathf.Max(0.01f, leanForward));
            else if (body.lean < leanBack) wanted = -Mathf.Clamp01((body.lean - leanBack) / Mathf.Min(-0.01f, leanBack));
        }

        // En la PC: flecha arriba (o W) = hacia la obra apuntada con el clic izquierdo, o hacia donde miras
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) { wanted = 1f; target = mousePointed; }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { wanted = -1f; target = null; }

        Vector3 look = head != null ? head.forward : rig.forward;
        look.y = 0f;
        if (look.sqrMagnitude < 0.0001f) look = Vector3.forward;
        look.Normalize();

        Vector3 dir = look;  // hacia atras se mueve en sentido contrario (wanted < 0)
        if (wanted > 0f && target != null)
        {
            // Hacia la obra, frenando al llegar cerca
            Vector3 to = target.root.position - rig.position;
            to.y = 0f;
            float remaining = to.magnitude - minDistanceToPaintings;
            wanted *= Mathf.Clamp01(remaining / 0.5f);
            if (to.sqrMagnitude > 0.0001f) dir = to.normalized;
        }

        walk = Mathf.Lerp(walk, wanted, 1f - Mathf.Exp(-4f * dt));
        lastWalkDir = Vector3.Slerp(lastWalkDir, dir, 1f - Mathf.Exp(-6f * dt));
        if (Mathf.Abs(walk) < 0.01f) return;

        Vector3 pos = rig.position + lastWalkDir * (walk * (pathMode && PathLength > 0f ? pathMoveSpeed : moveSpeed) * dt);
        Vector3 offset = pos - center;
        offset.y = 0f;
        if (pathMode && PathLength > 0f)
        {
            // Dentro del camino: entre las obras de los costados, del principio al final
            offset.x = Mathf.Clamp(offset.x, -corridorHalfWidth, corridorHalfWidth);
            offset.z = Mathf.Clamp(offset.z, -1f, WalkLength);
            rig.position = new Vector3(center.x + offset.x, rig.position.y, center.z + offset.z);
            return;
        }
        float maxRadius = Mathf.Max(0f, distance - minDistanceToPaintings);
        if (offset.magnitude > maxRadius) offset = offset.normalized * maxRadius;
        rig.position = new Vector3(center.x + offset.x, rig.position.y, center.z + offset.z);
    }

    // Las manos se ubican adelante de la vista (solo el giro horizontal, suavizado)
    void FollowHead()
    {
        if (!handsFollowHead || head == null)
        {
            handsYaw = 0f;
            return;
        }
        float target = head.localEulerAngles.y;
        handsYaw = Mathf.LerpAngle(handsYaw, target, 1f - Mathf.Exp(-8f * Time.deltaTime));
        var rot = Quaternion.Euler(0f, handsYaw, 0f);
        hands.transform.localPosition = rot * handsBaseLocal;
        hands.transform.localRotation = rot;
    }

    Painting ProcessHand(HandOSCReceiver.HandState hand, SpriteRenderer cursor, Color baseColor, float speed, float travel)
    {
        Vector3 dir;
        if (!GetRayDirection(hand, out dir))
        {
            cursor.enabled = false;
            return null;
        }
        float swipeAmount = hand.open && speed >= swipeSpeed ? travel : 0f;
        return PointAt(dir, hand.open, swipeAmount, cursor, baseColor);
    }

    // Apunta un rayo desde la cabeza: revela lo que toca y, si la mano esta abierta, lo marca para desarmar
    // Devuelve la obra mas cercana que toca el rayo (o null)
    Painting PointAt(Vector3 dir, bool open, float swipeAmount, SpriteRenderer cursor, Color baseColor)
    {
        Vector3 origin = rig.position;
        Vector3 cursorPos = origin + dir * (distance * 0.96f);
        Painting nearest = null;
        float nearestEnter = float.MaxValue;

        foreach (var p in paintings)
        {
            if (!p.active) continue;
            float enter;
            var plane = new Plane(p.root.forward, p.root.position);
            if (!plane.Raycast(new Ray(origin, dir), out enter)) continue;

            Vector3 hit = origin + dir * enter;
            Vector3 offset = hit - p.root.position;
            float x = Vector3.Dot(offset, p.root.right);
            float y = Vector3.Dot(offset, p.root.up);

            if (Mathf.Abs(x) <= p.halfW && Mathf.Abs(y) <= p.halfH && enter < nearestEnter)
            {
                nearest = p;
                nearestEnter = enter;
                cursorPos = hit - dir * 0.03f;
            }
        }

        if (nearest != null)
        {
            nearest.hovered = true;
            nearest.hitPoint = origin + dir * nearestEnter;
            if (open) nearest.openHandOver = true;
            nearest.swipeNow += swipeAmount;
        }

        cursor.enabled = true;
        cursor.transform.position = cursorPos;
        cursor.transform.rotation = Quaternion.LookRotation(dir);
        bool swiping = swipeAmount > 0f || (nearest != null && nearest.swipe > 0.05f && open);
        cursor.color = swiping ? new Color(1f, 0.4f, 0.2f) : (open ? Color.green : baseColor);
        return nearest;
    }

    // Sin tracker (en la PC): clic izquierdo = apuntar con la mano cerrada al centro de la vista,
    // E = barrer con la mano abierta (desarma la obra revelada)
    void ProcessMouse()
    {
        if (Application.isMobilePlatform) return; // en el celular tocar la pantalla es para recentrar
        bool point = Input.GetMouseButton(0);
        bool open = Input.GetKey(KeyCode.E);
        mousePointed = null;
        if (!(point || open) || head == null)
        {
            cursorMouse.enabled = false;
            return;
        }
        // E mantenida equivale a barrer a 1 m/s
        mousePointed = PointAt(head.forward, open, open ? Time.deltaTime : 0f, cursorMouse, Color.white);
    }

    // Mide cuanto se mueven las manos y lo convierte en oscuridad acumulada y agitacion
    void UpdateMovement()
    {
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        travelLeft = HandTravel(hands.Left, ref lastLeft, ref hadLeft, ref speedLeft, dt);
        travelRight = HandTravel(hands.Right, ref lastRight, ref hadRight, ref speedRight, dt);
        float speed = speedLeft + speedRight;

        float counted = Mathf.Max(0f, speed - minHandSpeed);
        darkness += counted * dt / Mathf.Max(0.01f, movementForFullDark);
        if (counted <= 0f) darkness -= darknessRecovery * dt;
        if (Input.GetKeyDown(KeyCode.O)) darkness += 0.25f;

        // Lo que ya avanzo la lista tambien cuenta como oscuridad (no vuelve atras)
        int total = darkSprites != null ? darkSprites.Length : 0;
        if (pathMode && PathLength > 0f) darkness = Mathf.Max(darkness, PathProgress);   // en el camino: cuanto mas adentro, mas oscuro
        else if (total > firstDark)
            darkness = Mathf.Max(darkness, (float)(nextDark - firstDark) / (total - firstDark));
        darkness = Mathf.Clamp01(darkness);

        float target = Mathf.Clamp01(speed / Mathf.Max(0.01f, agitationSpeed));
        agitation = Mathf.Lerp(agitation, target, 1f - Mathf.Exp(-3f * dt));
    }

    // Las posiciones de la mano llegan a saltos (unos 30 paquetes por segundo, menos que los cuadros
    // de Unity), asi que la velocidad se suaviza y el barrido se mide por recorrido, no por tiempo.
    float HandTravel(HandOSCReceiver.HandState hand, ref Vector3 last, ref bool had, ref float speed, float dt)
    {
        if (!hand.visible) { had = false; speed = 0f; return 0f; }
        Vector3 pos = hand.points[pointerLandmark];
        float d = had ? Vector3.Distance(pos, last) : 0f;
        if (d > 0.5f) d = 0f; // un salto del tracker no cuenta como movimiento
        last = pos;
        had = true;
        speed = Mathf.Lerp(speed, d / dt, 1f - Mathf.Exp(-8f * dt));
        return d;
    }

    void Update()
    {
        foreach (var p in paintings)
        {
            p.hovered = false;
            p.openHandOver = false;
            p.swipeNow = 0f;
        }

        FollowHead();
        UpdateMovement();

        pointedLeft = ProcessHand(hands.Left, cursorLeft, hands.leftColor, speedLeft, travelLeft);
        pointedRight = ProcessHand(hands.Right, cursorRight, hands.rightColor, speedRight, travelRight);
        ProcessMouse();
        UpdateWalk();
        UpdateGestures();

        foreach (var p in paintings)
        {
            // El recorrido se acumula mientras se barre y se va olvidando de a poco si se para
            if ((p.revealed || assembleFromDust) && p.swipeNow > 0f) p.swipe += p.swipeNow;
            else p.swipe = Mathf.Max(0f, p.swipe - swipeDistance * 0.5f * Time.deltaTime);
        }

        // Tecla Backspace (en el editor): vuelve a cubrir todos los cuadros para probar de nuevo
        if (Input.GetKeyDown(KeyCode.Backspace)) ResetAll();

        foreach (var p in paintings)
        {
            if (!p.revealed && !assembleFromDust)
            {
                if (p.hovered) p.reveal += Time.deltaTime / revealTime;
                else p.reveal -= Time.deltaTime / revealTime * forgetSpeed;
                p.reveal = Mathf.Clamp01(p.reveal);
                if (p.reveal >= 1f) p.revealed = true;
            }

            float t = Mathf.SmoothStep(0f, 1f, p.reveal);
            Color veil = veilColor;
            veil.a *= 1f + 0.5f * Mathf.Sin(Time.time * 1.5f + p.phase); // el velo "respira"
            Color shown = Color.Lerp(veil, CurrentTint(), t);
            if (environment != null) shown = environment.Fogged(shown, p.root.position);
            shown.a *= appear;
            p.renderer.color = shown;
        }
    }

    // Al aparecer la galeria (despues del cabrio) hay que bajar el brazo antes del primer gesto
    void OnEnable() { formReleased = breakReleased = false; }

    // Dos brazos levantados con las manos abiertas = armar la obra. Dos puños (a cualquier altura) = desarmarla
    void UpdateGestures()
    {
        var body = hands.Body;
        bool upL = HandOSCReceiver.IsRaisedOpen(hands.Left, handHighY) ||
                   (hands.Left.visible && hands.Left.open && body.visible && body.armLeft > armsUpLevel);
        bool upR = HandOSCReceiver.IsRaisedOpen(hands.Right, handHighY) ||
                   (hands.Right.visible && hands.Right.open && body.visible && body.armRight > armsUpLevel);
        Hold(upL && upR, ref formTimer, ref formReleased, ref formReady);
        Hold(HandOSCReceiver.IsFist(hands.Left) && HandOSCReceiver.IsFist(hands.Right),
             ref breakTimer, ref breakReleased, ref breakReady);

        // En la PC, E hace el gesto que corresponda (armar o desarmar)
        if (!Application.isMobilePlatform && Input.GetKeyDown(KeyCode.E)) formReady = breakReady = true;
    }

    void Hold(bool pose, ref float timer, ref bool released, ref bool ready)
    {
        if (!pose) { timer = 0f; released = true; return; }
        if (!released) return;
        timer += Time.deltaTime;
        if (timer >= gestureHoldTime) { ready = true; released = false; }
    }

    /// <summary>Devuelve true una sola vez por gesto de armar.</summary>
    public bool ConsumeForm() { bool r = formReady; formReady = false; return r; }

    /// <summary>Devuelve true una sola vez por gesto de desarmar.</summary>
    public bool ConsumeBreak() { bool r = breakReady; breakReady = false; return r; }

    // La obra activa mas cerca de donde se mira (o null si ninguna esta adelante)
    Painting PaintingInView()
    {
        return PaintingInView(float.MaxValue);
    }

    /// <summary>
    /// La obra que se esta mirando (dentro de unos 55 grados y a menos de maxDistance metros).
    /// Si hay varias, gana la mas centrada en la vista, con algo de ventaja para la mas cercana.
    /// </summary>
    public Painting PaintingInView(float maxDistance)
    {
        Vector3 look = head != null ? head.forward : rig.forward;
        Painting best = null;
        float bestScore = float.MinValue;
        foreach (var p in paintings)
        {
            if (!p.active) continue;
            Vector3 to = p.root.position - rig.position;
            float dist = to.magnitude;
            if (dist > maxDistance || dist < 0.01f) continue;
            float dot = Vector3.Dot(look, to / dist);
            if (dot < 0.57f) continue;
            float score = dot - dist * 0.03f;
            if (score > bestScore) { bestScore = score; best = p; }
        }
        return best;
    }

    /// <summary>Lo llama GoyaParticles cuando una obra termina de armarse desde el polvo.</summary>
    public void MarkRevealed(Painting p)
    {
        p.reveal = 1f;
        p.revealed = true;
    }

    /// <summary>Color por el que se multiplica una obra revelada segun la oscuridad acumulada.</summary>
    public Color CurrentTint()
    {
        return Color.Lerp(Color.white, darkTint, darkness);
    }

    public void ResetAll()
    {
        if (batches.Count > 0)
        {
            currentSlot = Mathf.Max(2, arcSlots) / 2;
            ApplyBatch(0, oneAtATime ? 0f : ViewYaw());
            darkness = 0f;
            return;
        }
        foreach (var p in paintings)
        {
            p.reveal = 0f;
            p.revealed = false;
            p.darkIndex = p.originalIndex;
            SetSprite(p, p.original);
        }
        darkness = 0f;
        nextDark = firstDark;
    }
}

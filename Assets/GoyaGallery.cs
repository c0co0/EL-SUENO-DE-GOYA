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
/// Uso: objeto vacio "Galeria" con este script, y arrastrar las 3 imagenes (importadas
/// como Sprite) en los casilleros. Las obras oscuras tambien se importan como Sprite
/// (Texture Type: Sprite (2D and UI)), pero no hace falta tildarles Read/Write.
/// En el editor: tecla O = sumar oscuridad (para probar sin manos).
/// </summary>
public class GoyaGallery : MonoBehaviour
{
    [Header("Cuadros (arrastra las imagenes importadas como Sprite)")]
    public Sprite centerPainting;
    public Sprite leftPainting;
    public Sprite rightPainting;

    [Header("Referencias")]
    [Tooltip("Si queda vacio, lo busca solo")]
    public HandOSCReceiver hands;

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
    [Tooltip("Segundos de mano sobre el cuadro para revelarlo del todo")]
    public float revealTime = 1.2f;
    [Tooltip("Que tan rapido se 'olvida' un revelado a medias (0 = nunca)")]
    public float forgetSpeed = 0.25f;
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
        public bool openHandOver;  // una mano ABIERTA esta encima (se usa en el paso de particulas)
        public float phase;
        public Vector3 hitPoint;   // ultimo punto del cuadro tocado por una mano
        public float maxSize;      // lado mas largo en metros (se mantiene al cambiar de obra)
        public int darkIndex = -1; // -1 = obra original, si no posicion en darkerPaintings
        public Sprite original;
    }

    public List<Painting> paintings = new List<Painting>();

    Transform rig;
    SpriteRenderer cursorLeft, cursorRight;

    Sprite[] darkSprites;
    int nextDark;
    Vector3 lastLeft, lastRight;
    bool hadLeft, hadRight;

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

        CreatePainting(centerPainting, "Centro", 0f, centerScale);
        CreatePainting(leftPainting, "Izquierda", -sideAngle, 1f);
        CreatePainting(rightPainting, "Derecha", sideAngle, 1f);

        var circle = MakeCircleSprite();
        cursorLeft = CreateCursor("CursorIzquierdo", circle);
        cursorRight = CreateCursor("CursorDerecho", circle);
    }

    void CreatePainting(Sprite sprite, string label, float angle, float scaleMul)
    {
        if (sprite == null)
        {
            Debug.LogWarning("[Galeria] Falta la imagen del cuadro: " + label);
            return;
        }

        Quaternion yaw = Quaternion.Euler(0f, angle, 0f);
        var root = new GameObject("Cuadro_" + label).transform;
        root.SetParent(transform, false);
        root.position = rig.position + yaw * Vector3.forward * distance;
        root.rotation = yaw;

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
        if (darkSprites == null) BuildDarkSprites();
        int n = darkSprites.Length;
        if (n == 0) return null;

        int start = Mathf.Max(nextDark, Mathf.FloorToInt(darkness * n), p.darkIndex + 1);
        if (start >= n) start = n - 1;

        // Buscar desde "start" hacia adelante y despues hacia atras una que no este en pantalla
        for (int i = start; i < n; i++)
            if (TryTake(p, i)) return darkSprites[i];
        for (int i = start - 1; i >= 0; i--)
            if (TryTake(p, i)) return darkSprites[i];
        return null;
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

        // Posicion de la mano relativa al Rig (Manos es hijo del Rig, sin rotacion ni escala)
        Vector3 local = hands.transform.localPosition + hand.points[pointerLandmark];
        Vector3 amplified = new Vector3(local.x * reach, local.y * reach, local.z);
        dir = rig.TransformDirection(amplified).normalized;
        return true;
    }

    void ProcessHand(HandOSCReceiver.HandState hand, SpriteRenderer cursor, Color baseColor)
    {
        Vector3 dir;
        if (!GetRayDirection(hand, out dir))
        {
            cursor.enabled = false;
            return;
        }

        Vector3 origin = rig.position;
        Vector3 cursorPos = origin + dir * (distance * 0.96f);

        foreach (var p in paintings)
        {
            float enter;
            var plane = new Plane(p.root.forward, p.root.position);
            if (!plane.Raycast(new Ray(origin, dir), out enter)) continue;

            Vector3 hit = origin + dir * enter;
            Vector3 offset = hit - p.root.position;
            float x = Vector3.Dot(offset, p.root.right);
            float y = Vector3.Dot(offset, p.root.up);

            if (Mathf.Abs(x) <= p.halfW && Mathf.Abs(y) <= p.halfH)
            {
                p.hovered = true;
                p.hitPoint = hit;
                if (hand.open) p.openHandOver = true;
                cursorPos = hit - dir * 0.03f;
            }
        }

        cursor.enabled = true;
        cursor.transform.position = cursorPos;
        cursor.transform.rotation = Quaternion.LookRotation(dir);
        cursor.color = hand.open ? Color.green : baseColor;
    }

    // Mide cuanto se mueven las manos y lo convierte en oscuridad acumulada y agitacion
    void UpdateMovement()
    {
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        float speed = 0f;
        speed += HandSpeed(hands.Left, ref lastLeft, ref hadLeft, dt);
        speed += HandSpeed(hands.Right, ref lastRight, ref hadRight, dt);

        float counted = Mathf.Max(0f, speed - minHandSpeed);
        darkness += counted * dt / Mathf.Max(0.01f, movementForFullDark);
        if (counted <= 0f) darkness -= darknessRecovery * dt;
        if (Input.GetKeyDown(KeyCode.O)) darkness += 0.25f;
        darkness = Mathf.Clamp01(darkness);

        float target = Mathf.Clamp01(speed / Mathf.Max(0.01f, agitationSpeed));
        agitation = Mathf.Lerp(agitation, target, 1f - Mathf.Exp(-3f * dt));
    }

    float HandSpeed(HandOSCReceiver.HandState hand, ref Vector3 last, ref bool had, float dt)
    {
        if (!hand.visible) { had = false; return 0f; }
        Vector3 pos = hand.points[pointerLandmark];
        float v = had ? Vector3.Distance(pos, last) / dt : 0f;
        last = pos;
        had = true;
        return Mathf.Min(v, 6f); // un salto del tracker no cuenta como movimiento enorme
    }

    void Update()
    {
        foreach (var p in paintings)
        {
            p.hovered = false;
            p.openHandOver = false;
        }

        UpdateMovement();

        ProcessHand(hands.Left, cursorLeft, hands.leftColor);
        ProcessHand(hands.Right, cursorRight, hands.rightColor);

        // Tecla Backspace (en el editor): vuelve a cubrir todos los cuadros para probar de nuevo
        if (Input.GetKeyDown(KeyCode.Backspace)) ResetAll();

        foreach (var p in paintings)
        {
            if (!p.revealed)
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
            shown.a *= appear;
            p.renderer.color = shown;
        }
    }

    /// <summary>Color por el que se multiplica una obra revelada segun la oscuridad acumulada.</summary>
    public Color CurrentTint()
    {
        return Color.Lerp(Color.white, darkTint, darkness);
    }

    public void ResetAll()
    {
        foreach (var p in paintings)
        {
            p.reveal = 0f;
            p.revealed = false;
            p.darkIndex = -1;
            SetSprite(p, p.original);
        }
        darkness = 0f;
        nextDark = 0;
    }
}

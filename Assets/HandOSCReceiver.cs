using UnityEngine;
using extOSC;

/// <summary>
/// Recibe por OSC las manos que manda hand_tracker.py (MediaPipe) y las muestra
/// como esferas de debug. Los datos quedan en Left / Right para usarlos despues
/// (revelar cuadros, tocar, etc.).
///
/// Mensajes esperados:
///   /hand/Left|Right/present  int
///   /hand/Left|Right/open     int
///   /hand/Left|Right/lm       63 floats (21 puntos x,y,z)
///   /hand/Left|Right/fingers  int (dedos estirados sin el pulgar: 0 = puño, 1 = un dedo)
///   /body/present             int
///   /body/lean                float (+ = inclinado hacia la camara, - = hacia atras)
///   /body/tilt                float (grados de costado)
///   /body/arms                2 floats (altura de cada brazo: 0 = hombro, 1 = bien arriba)
///   /body/cross               int (1 = brazos cruzados en X)
///   /body/lm                  99 floats (33 puntos del cuerpo)
/// </summary>
public class HandOSCReceiver : MonoBehaviour
{
    [Header("Red")]
    public int port = 5052;

    [Header("Mapeo al espacio 3D (en unidades de Unity = metros)")]
    [Tooltip("Ancho del area donde se mueve la mano")]
    public float width = 1f;
    [Tooltip("Alto del area donde se mueve la mano")]
    public float height = 0.6f;
    [Tooltip("Cuanto se exagera la profundidad (z de MediaPipe)")]
    public float depthScale = 1f;

    [Header("Suavizado")]
    [Tooltip("Que tan rapido siguen las esferas a la mano (mas alto = mas pegado, mas bajo = mas suave)")]
    public float followSpeed = 25f;
    [Tooltip("Si no llegan datos por este tiempo (segundos), la mano se oculta")]
    public float dataTimeout = 0.5f;

    [Header("Debug")]
    public bool showDebugSpheres = true;
    public float sphereSize = 0.02f;
    [Tooltip("En el celular, usar un shader Unlit (evita el color magenta de error)")]
    public bool unlitOnDevice = true;
    public Color leftColor = new Color(0.2f, 0.8f, 1f);
    public Color rightColor = new Color(1f, 0.3f, 0.8f);

    [Header("Reiniciar")]
    [Tooltip("Cruzar los brazos en X (sostenidos) vuelve a empezar toda la experiencia desde el cabrio")]
    public bool crossToRestart = true;
    [Tooltip("Segundos con los brazos cruzados para reiniciar")]
    public float crossHoldTime = 1.5f;

    [Header("Estado (solo lectura, para ver en el Inspector)")]
    public bool leftVisible;
    public bool leftOpen;
    public bool rightVisible;
    public bool rightOpen;
    public bool bodyVisible;
    public float bodyLean;
    public float bodyTilt;
    public float armLeft;
    public float armRight;

    public class HandState
    {
        public bool present;
        public bool open;
        public int fingers = -1;     // -1 = el tracker no lo manda (version vieja)
        // Posiciones locales (relativas a este objeto), ya mapeadas al espacio 3D
        public Vector3[] points = new Vector3[21];
        public float[] raw = new float[63];
        public bool hasData;
        public bool visible;

        public int packets;          // se incrementa por cada mensaje de landmarks
        public int lastPacketsSeen;
        public float lastDataTime;
        public bool wasVisible;

        public Transform[] spheres;
        public Material material;
    }

    public HandState Left = new HandState();
    public HandState Right = new HandState();

    public class BodyState
    {
        public bool present;
        public bool visible;      // presente y con datos recientes
        public float lean;        // + = hacia adelante, - = hacia atras
        public float tilt;        // grados de costado
        public float armLeft = -1f, armRight = -1f;  // 0 = a la altura del hombro, 1 = bien arriba
        public bool cross;        // brazos cruzados en X
        public float[] raw = new float[99];
        public float lastDataTime = -10f;
    }

    public BodyState Body = new BodyState();

    OSCReceiver receiver;
    readonly object lockObj = new object();

    void Start()
    {
        Application.runInBackground = true;

        receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalPort = port;
        receiver.Bind("/hand/*/lm", OnLandmarks);
        receiver.Bind("/hand/*/open", OnOpen);
        receiver.Bind("/hand/*/present", OnPresent);
        receiver.Bind("/hand/*/fingers", OnFingers);
        receiver.Bind("/body/present", OnBodyPresent);
        receiver.Bind("/body/lean", OnBodyLean);
        receiver.Bind("/body/tilt", OnBodyTilt);
        receiver.Bind("/body/arms", OnBodyArms);
        receiver.Bind("/body/lm", OnBodyLandmarks);
        receiver.Bind("/body/cross", OnBodyCross);

        if (showDebugSpheres)
        {
            CreateSpheres(Left, leftColor);
            CreateSpheres(Right, rightColor);
        }
    }

    // En el editor usa el material por defecto (el que ya se veia bien).
    // En el celular prueba un shader "Unlit" y, si no lo encuentra, vuelve al por defecto.
    Material MakeHandMaterial(Material defaultMat, Color color)
    {
        Material mat = null;
        if (!Application.isEditor && unlitOnDevice)
        {
            Shader sh = Shader.Find("Universal Render Pipeline/Unlit");
            if (sh == null) sh = Shader.Find("Sprites/Default");
            if (sh == null) sh = Shader.Find("Unlit/Color");
            if (sh != null) mat = new Material(sh);
        }
        if (mat == null) mat = new Material(defaultMat);

        mat.color = color;
        Debug.Log("[Manos] shader de las esferas: " + mat.shader.name);
        return mat;
    }

    void CreateSpheres(HandState hand, Color color)
    {
        hand.material = null;
        hand.spheres = new Transform[21];
        for (int i = 0; i < 21; i++)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(go.GetComponent<Collider>());
            go.transform.SetParent(transform, false);
            go.transform.localScale = Vector3.one * sphereSize;

            var r = go.GetComponent<Renderer>();
            if (hand.material == null) hand.material = MakeHandMaterial(r.sharedMaterial, color);
            r.sharedMaterial = hand.material;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            r.receiveShadows = false;

            go.SetActive(false);
            hand.spheres[i] = go.transform;
        }
    }

    HandState StateFromAddress(string address)
    {
        // "/hand/Left/lm" -> ["", "hand", "Left", "lm"]
        var parts = address.Split('/');
        if (parts.Length < 3) return null;
        return parts[2] == "Left" ? Left : (parts[2] == "Right" ? Right : null);
    }

    void OnLandmarks(OSCMessage message)
    {
        var hand = StateFromAddress(message.Address);
        if (hand == null) return;

        lock (lockObj)
        {
            int n = Mathf.Min(63, message.Values.Count);
            for (int i = 0; i < n; i++)
                hand.raw[i] = message.Values[i].FloatValue;
            hand.hasData = true;
            hand.packets++;
        }
    }

    void OnOpen(OSCMessage message)
    {
        var hand = StateFromAddress(message.Address);
        if (hand == null || message.Values.Count == 0) return;
        lock (lockObj) hand.open = message.Values[0].IntValue != 0;
    }

    void OnFingers(OSCMessage message)
    {
        var hand = StateFromAddress(message.Address);
        if (hand == null || message.Values.Count == 0) return;
        lock (lockObj) hand.fingers = message.Values[0].IntValue;
    }

    /// <summary>Mano visible y cerrada (puño).</summary>
    public static bool IsFist(HandState h)
    {
        return h.visible && !h.open && (h.fingers < 0 || h.fingers == 0);
    }

    /// <summary>Mano visible con un solo dedo estirado.</summary>
    public static bool IsOneFinger(HandState h)
    {
        return h.visible && !h.open && h.fingers == 1;
    }

    /// <summary>Mano visible, abierta y levantada (la muñeca en la parte de arriba de la imagen de la camara).</summary>
    public static bool IsRaisedOpen(HandState h, float highY)
    {
        return h.visible && h.open && h.raw[1] < highY;
    }

    void OnPresent(OSCMessage message)
    {
        var hand = StateFromAddress(message.Address);
        if (hand == null || message.Values.Count == 0) return;
        lock (lockObj) hand.present = message.Values[0].IntValue != 0;
    }

    float bodyPackets, bodyPacketsSeen;
    float crossTimer;

    void OnBodyPresent(OSCMessage message)
    {
        if (message.Values.Count == 0) return;
        lock (lockObj) Body.present = message.Values[0].IntValue != 0;
    }

    void OnBodyLean(OSCMessage message)
    {
        if (message.Values.Count == 0) return;
        lock (lockObj) { Body.lean = message.Values[0].FloatValue; bodyPackets++; }
    }

    void OnBodyTilt(OSCMessage message)
    {
        if (message.Values.Count == 0) return;
        lock (lockObj) Body.tilt = message.Values[0].FloatValue;
    }

    void OnBodyArms(OSCMessage message)
    {
        if (message.Values.Count < 2) return;
        lock (lockObj)
        {
            Body.armLeft = message.Values[0].FloatValue;
            Body.armRight = message.Values[1].FloatValue;
        }
    }

    void OnBodyCross(OSCMessage message)
    {
        if (message.Values.Count == 0) return;
        lock (lockObj) Body.cross = message.Values[0].IntValue != 0;
    }

    void OnBodyLandmarks(OSCMessage message)
    {
        lock (lockObj)
        {
            int n = Mathf.Min(99, message.Values.Count);
            for (int i = 0; i < n; i++) Body.raw[i] = message.Values[i].FloatValue;
        }
    }

    void Update()
    {
        UpdateHand(Left, leftColor);
        UpdateHand(Right, rightColor);

        lock (lockObj)
        {
            if (bodyPackets != bodyPacketsSeen) { bodyPacketsSeen = bodyPackets; Body.lastDataTime = Time.time; }
            Body.visible = Body.present && Time.time - Body.lastDataTime < dataTimeout;
        }
        bodyVisible = Body.visible;

        // Brazos cruzados sostenidos: recargar la escena (vuelve todo al principio, cabrio incluido)
        crossTimer = crossToRestart && Body.visible && Body.cross ? crossTimer + Time.deltaTime : 0f;
        if (crossTimer >= crossHoldTime)
        {
            crossTimer = 0f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(gameObject.scene.buildIndex >= 0
                ? gameObject.scene.buildIndex : 0);
            return;
        }
        bodyLean = Body.lean;
        bodyTilt = Body.tilt;
        armLeft = Body.armLeft;
        armRight = Body.armRight;

        leftVisible = Left.visible;
        leftOpen = Left.open;
        rightVisible = Right.visible;
        rightOpen = Right.open;
    }

    void UpdateHand(HandState hand, Color baseColor)
    {
        bool present, open;
        int packets;
        lock (lockObj)
        {
            present = hand.present && hand.hasData;
            open = hand.open;
            packets = hand.packets;
            if (present)
            {
                for (int i = 0; i < 21; i++)
                {
                    float x = (hand.raw[i * 3] - 0.5f) * width;
                    float y = (0.5f - hand.raw[i * 3 + 1]) * height;
                    float z = -hand.raw[i * 3 + 2] * depthScale;
                    hand.points[i] = new Vector3(x, y, z);
                }
            }
        }

        // Detectar si llegaron datos nuevos (si no, la mano se oculta tras dataTimeout)
        if (packets != hand.lastPacketsSeen)
        {
            hand.lastPacketsSeen = packets;
            hand.lastDataTime = Time.time;
        }
        bool fresh = Time.time - hand.lastDataTime < dataTimeout;
        hand.visible = present && fresh;

        if (hand.spheres == null) return;

        if (hand.material != null)
            hand.material.color = open ? Color.green : baseColor;

        float t = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);
        for (int i = 0; i < 21; i++)
        {
            var s = hand.spheres[i];
            s.gameObject.SetActive(hand.visible);
            if (!hand.visible) continue;

            if (!hand.wasVisible) s.localPosition = hand.points[i];
            else s.localPosition = Vector3.Lerp(s.localPosition, hand.points[i], t);
        }
        hand.wasVisible = hand.visible;
    }
}

using UnityEngine;

/// <summary>
/// Visor VR casero para celular: crea dos camaras (ojo izquierdo y derecho, pantalla dividida)
/// y mueve la "cabeza" con el giroscopio. En el editor (sin giroscopio) se mira con el mouse
/// manteniendo apretado el boton derecho.
///
/// Controles en el celular:
///   - Tocar la pantalla con 1 dedo (o el boton del Cardboard): fija "adelante".
///   - Tocar con 2 dedos: alterna entre modo visor (2 ojos) y modo normal (pantalla completa).
/// En el editor: tecla V alterna el modo, tecla R recentra.
///
/// Ponelo en un objeto vacio llamado "Rig". La cabeza gira, el Rig NO gira:
/// las manos (objeto "Manos") van como hijas del Rig.
/// </summary>
public class StereoGyroRig : MonoBehaviour
{
    [Header("Modo")]
    [Tooltip("true = visor de dos ojos, false = una sola vista a pantalla completa")]
    public bool stereo = true;

    [Header("Estereo")]
    [Tooltip("Distancia entre ojos en metros")]
    public float ipd = 0.064f;
    public float fieldOfView = 75f;
    [Tooltip("Campo de vision vertical en modo normal")]
    public float monoFieldOfView = 65f;
    public float nearClip = 0.03f;
    public float farClip = 100f;
    public Color background = new Color(0.02f, 0.02f, 0.04f);

    [Header("Giroscopio")]
    [Tooltip("Segundos despues de empezar para fijar 'adelante' automaticamente")]
    public float recenterDelay = 2f;

    [Header("Editor (sin giroscopio)")]
    public float mouseSensitivity = 3f;

    [Header("Debug")]
    [Tooltip("Crea 4 cubos de colores alrededor para ver el giro de la cabeza")]
    public bool createDebugMarkers = true;

    Transform head;
    Camera leftEye, rightEye;
    bool useGyro;
    bool recentered;
    float yawOffset;
    float startTime;
    float lastToggle = -10f;
    float editorYaw, editorPitch;

    void Start()
    {
        // Desactivar la camara por defecto de la escena
        var main = Camera.main;
        if (main != null) main.gameObject.SetActive(false);

        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        head = new GameObject("Head").transform;
        head.SetParent(transform, false);
        head.gameObject.AddComponent<AudioListener>();

        leftEye = CreateEye("OjoIzquierdo", -ipd / 2f);
        rightEye = CreateEye("OjoDerecho", ipd / 2f);
        ApplyMode();

        useGyro = SystemInfo.supportsGyroscope;
        if (useGyro) Input.gyro.enabled = true;

        if (createDebugMarkers) CreateMarkers();

        startTime = Time.time;
    }

    Camera CreateEye(string eyeName, float xOffset)
    {
        var go = new GameObject(eyeName);
        go.transform.SetParent(head, false);
        go.transform.localPosition = new Vector3(xOffset, 0f, 0f);

        var cam = go.AddComponent<Camera>();
        cam.nearClipPlane = nearClip;
        cam.farClipPlane = farClip;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = background;
        return cam;
    }

    void ApplyMode()
    {
        if (stereo)
        {
            leftEye.rect = new Rect(0f, 0f, 0.5f, 1f);
            leftEye.fieldOfView = fieldOfView;
            leftEye.transform.localPosition = new Vector3(-ipd / 2f, 0f, 0f);
            rightEye.gameObject.SetActive(true);
            rightEye.rect = new Rect(0.5f, 0f, 0.5f, 1f);
            rightEye.fieldOfView = fieldOfView;
        }
        else
        {
            leftEye.rect = new Rect(0f, 0f, 1f, 1f);
            leftEye.fieldOfView = monoFieldOfView;
            leftEye.transform.localPosition = Vector3.zero;
            rightEye.gameObject.SetActive(false);
        }
    }

    void CreateMarkers()
    {
        Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow };
        for (int i = 0; i < 4; i++)
        {
            float angle = i * 90f * Mathf.Deg2Rad;
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Destroy(cube.GetComponent<Collider>());
            cube.name = "Marcador" + i;
            cube.transform.position = transform.position +
                new Vector3(Mathf.Sin(angle), 0f, Mathf.Cos(angle)) * 3f;
            cube.transform.localScale = Vector3.one * 0.4f;
            cube.GetComponent<Renderer>().material.color = colors[i];
        }
    }

    void Update()
    {
        // Alternar visor / pantalla completa: 2 dedos en el celular, tecla V en el editor
        bool twoFingers = Input.touchCount >= 2 && Input.GetTouch(1).phase == TouchPhase.Began;
        if ((twoFingers || Input.GetKeyDown(KeyCode.V)) && Time.time - lastToggle > 1f)
        {
            stereo = !stereo;
            ApplyMode();
            lastToggle = Time.time;
        }

        if (useGyro) UpdateGyro();
        else UpdateMouse();
    }

    void UpdateGyro()
    {
        Quaternion q = Input.gyro.attitude;
        Quaternion raw = Quaternion.Euler(90f, 0f, 0f) * new Quaternion(q.x, q.y, -q.z, -q.w);

        // Fijar "adelante" automaticamente una vez, y de nuevo al tocar con 1 dedo
        bool tapped = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
        if ((!recentered && Time.time - startTime > recenterDelay) || tapped)
        {
            yawOffset = raw.eulerAngles.y;
            recentered = true;
        }

        head.localRotation = Quaternion.Euler(0f, -yawOffset, 0f) * raw;
    }

    void UpdateMouse()
    {
        if (Input.GetMouseButton(1))
        {
            editorYaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            editorPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            editorPitch = Mathf.Clamp(editorPitch, -85f, 85f);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            editorYaw = 0f;
            editorPitch = 0f;
        }
        head.localRotation = Quaternion.Euler(editorPitch, editorYaw, 0f);
    }
}

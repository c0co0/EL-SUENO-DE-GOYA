using UnityEngine;
using UnityEngine.XR;

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
/// Celulares SIN giroscopio (ej. Samsung Galaxy A04): se usa el acelerometro.
///   - Mirar arriba / abajo: funciona igual (inclinar la cabeza adelante / atras).
///   - Girar a los costados: inclinar la cabeza hacia el hombro (como "manejar" la vista).
///     El acelerometro solo mide la gravedad, asi que no puede detectar el giro horizontal directo.
///
/// Con el plugin Google Cardboard XR activo (XR Plug-in Management), no se arman las dos camaras
/// a mano: el plugin dibuja los dos ojos con la distorsion de las lentes del visor y la rotacion
/// de la cabeza sale del seguimiento de Cardboard. Si el XR no esta activo (por ejemplo en el editor),
/// se usa el visor casero de siempre.
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
    [Tooltip("Distancia entre los centros de las lentes del visor en metros (0 = usar mitades de pantalla). " +
             "Si se ve doble, ajustar este valor: medir con una regla de centro a centro de las lentes.")]
    public float lensSeparation = 0.062f;
    public float fieldOfView = 75f;
    [Tooltip("Campo de vision vertical en modo normal")]
    public float monoFieldOfView = 65f;
    public float nearClip = 0.03f;
    public float farClip = 100f;
    public Color background = new Color(0.02f, 0.02f, 0.04f);

    [Header("Giroscopio")]
    [Tooltip("Segundos despues de empezar para fijar 'adelante' automaticamente")]
    public float recenterDelay = 2f;

    [Header("Celular sin giroscopio (acelerometro)")]
    [Tooltip("Usar el acelerometro si el celular no tiene giroscopio")]
    public bool accelerometerFallback = true;
    [Tooltip("Suavizado del acelerometro (mas alto = responde mas rapido, mas bajo = mas suave)")]
    public float accelSmoothing = 8f;
    [Tooltip("Grados de inclinacion lateral que se ignoran antes de empezar a girar")]
    public float tiltDeadZone = 12f;
    [Tooltip("Velocidad de giro (grados por segundo) con la cabeza inclinada 30 grados")]
    public float tiltTurnSpeed = 45f;
    [Tooltip("Tildar si al mirar arriba la vista baja (y al reves)")]
    public bool invertPitch = false;
    [Tooltip("Tildar si al inclinar la cabeza a la derecha la vista gira a la izquierda")]
    public bool invertTurn = false;

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
    bool useAccel;
    bool useXR;
    Vector3 gravity = new Vector3(-1f, 0f, 0f);
    float accelYaw;
    float pitchOffset, rollOffset;
    bool accelCalibrated;

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

        // Con Cardboard XR una sola camara alcanza: el plugin la renderiza para los dos ojos
        useXR = XRSettings.isDeviceActive;
        if (useXR)
        {
            leftEye = CreateEye("CamaraXR", 0f);
            leftEye.gameObject.tag = "MainCamera";
            stereo = true;
        }
        else
        {
            leftEye = CreateEye("OjoIzquierdo", -ipd / 2f);
            rightEye = CreateEye("OjoDerecho", ipd / 2f);
            ApplyMode();
        }

        useGyro = SystemInfo.supportsGyroscope;
        if (useGyro) Input.gyro.enabled = true;
        useAccel = !useGyro && accelerometerFallback && !Application.isEditor && SystemInfo.supportsAccelerometer;
        Debug.Log("[Visor] XR: " + useXR + " (" + XRSettings.loadedDeviceName + ") | giroscopio: " + useGyro +
                  " | acelerometro: " + useAccel);

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
            // Centro de cada ojo en la pantalla (0..1). Por defecto, el centro de cada mitad.
            float cx = 0.25f;
            float screenWidthM = Screen.dpi > 0f ? Screen.width / Screen.dpi * 0.0254f : 0f;
            if (lensSeparation > 0f && screenWidthM > 0f)
                cx = Mathf.Clamp(0.5f - lensSeparation * 0.5f / screenWidthM, 0.05f, 0.25f);
            // Cada vista se centra bajo su lente (si la pantalla es mas ancha que el visor, quedan bordes negros)
            float hw = cx;
            leftEye.rect = new Rect(cx - hw, 0f, 2f * hw, 1f);
            leftEye.fieldOfView = fieldOfView;
            leftEye.transform.localPosition = new Vector3(-ipd / 2f, 0f, 0f);
            rightEye.gameObject.SetActive(true);
            rightEye.rect = new Rect(1f - cx - hw, 0f, 2f * hw, 1f);
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
        if (!useXR && (twoFingers || Input.GetKeyDown(KeyCode.V)) && Time.time - lastToggle > 1f)
        {
            stereo = !stereo;
            ApplyMode();
            lastToggle = Time.time;
        }

        if (useGyro) UpdateGyro();
        else if (useAccel) UpdateAccel();
        else UpdateMouse();
    }

    // Sin giroscopio: el acelerometro da la direccion de la gravedad.
    // Con el celular acostado y la pantalla hacia la persona, la gravedad queda
    // casi toda sobre el eje x del celular (+x o -x segun para que lado se acueste).
    // Al empezar (y al tocar la pantalla) se toma la posicion actual como "derecho".
    void UpdateAccel()
    {
        float dt = Time.deltaTime;
        Vector3 a = Input.acceleration;
        if (a.sqrMagnitude > 0.01f)
            gravity = Vector3.Lerp(gravity, a, 1f - Mathf.Exp(-accelSmoothing * dt));

        // Arriba / abajo: cuanto apunta la gravedad hacia la pantalla
        float pitch = Mathf.Atan2(gravity.z, Mathf.Sqrt(gravity.x * gravity.x + gravity.y * gravity.y)) * Mathf.Rad2Deg;
        // Inclinacion hacia el hombro: angulo de la gravedad en el plano de la pantalla
        float roll = Mathf.Atan2(gravity.y, gravity.x) * Mathf.Rad2Deg;

        // Calibrar: la posicion actual pasa a ser "derecho y mirando adelante"
        bool tapped = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
        if ((!accelCalibrated && Time.time - startTime > recenterDelay) || tapped)
        {
            pitchOffset = pitch;
            rollOffset = roll;
            accelYaw = 0f;
            accelCalibrated = true;
        }
        pitch -= pitchOffset;
        roll = Mathf.DeltaAngle(rollOffset, roll);
        if (invertPitch) pitch = -pitch;
        if (invertTurn) roll = -roll;

        // La inclinacion lateral hace girar la vista, como un volante (solo despues de calibrar)
        float extra = Mathf.Abs(roll) - tiltDeadZone;
        if (accelCalibrated && extra > 0f)
            accelYaw += Mathf.Sign(roll) * Mathf.Min(extra / 30f, 1.5f) * tiltTurnSpeed * dt;

        head.localRotation = Quaternion.Euler(Mathf.Clamp(-pitch, -85f, 85f), accelYaw, 0f);
    }

    void UpdateGyro()
    {
        Quaternion raw;
        if (!useXR || !TryGetXRHeadRotation(out raw))
        {
            Quaternion q = Input.gyro.attitude;
            raw = Quaternion.Euler(90f, 0f, 0f) * new Quaternion(q.x, q.y, -q.z, -q.w);
        }

        // Fijar "adelante" automaticamente una vez, y de nuevo al tocar con 1 dedo
        bool tapped = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;
        if ((!recentered && Time.time - startTime > recenterDelay) || tapped)
        {
            yawOffset = raw.eulerAngles.y;
            recentered = true;
        }

        head.localRotation = Quaternion.Euler(0f, -yawOffset, 0f) * raw;
    }

    // Rotacion de la cabeza segun el seguimiento de Cardboard
    static bool TryGetXRHeadRotation(out Quaternion rotation)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.CenterEye);
        if (!device.isValid) device = InputDevices.GetDeviceAtXRNode(XRNode.Head);
        if (device.isValid && device.TryGetFeatureValue(CommonUsages.centerEyeRotation, out rotation))
            return true;
        if (device.isValid && device.TryGetFeatureValue(CommonUsages.deviceRotation, out rotation))
            return true;
        rotation = Quaternion.identity;
        return false;
    }

    void UpdateMouse()
    {
        if (Input.GetMouseButton(1))
        {
            editorYaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            editorPitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            editorPitch = Mathf.Clamp(editorPitch, -85f, 85f);
        }
        // Flechas izquierda / derecha: girar (adelante / atras camina, lo maneja GoyaGallery)
        if (Input.GetKey(KeyCode.LeftArrow)) editorYaw -= 90f * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow)) editorYaw += 90f * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.R))
        {
            editorYaw = 0f;
            editorPitch = 0f;
        }
        head.localRotation = Quaternion.Euler(editorPitch, editorYaw, 0f);
    }
}

using Google.XR.Cardboard;
using UnityEngine;

/// <summary>
/// Arranque del plugin Google Cardboard (basado en CardboardStartup del ejemplo HelloCardboard).
///  - La primera vez pide escanear el codigo QR del visor, para que la distorsion de las
///    lentes quede bien calibrada (si el visor no tiene QR, se puede saltear y usa valores por defecto).
///  - Boton del engranaje (arriba en la pantalla): volver a escanear el QR.
///  - Boton X: cerrar la app.
///
/// Ponelo en cualquier objeto de la escena (por ejemplo en "Rig").
/// Si el XR no esta activo (editor, o XR Plug-in Management sin Cardboard), no hace nada.
/// </summary>
public class CardboardSetup : MonoBehaviour
{
    [Tooltip("Pedir el QR del visor al abrir la app si todavia no se escaneo ninguno")]
    public bool scanOnStart = true;

    bool active;

    void Start()
    {
        active = UnityEngine.XR.XRSettings.isDeviceActive && !Application.isEditor;
        if (!active) return;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        Screen.brightness = 1f;

        if (scanOnStart && !Api.HasDeviceParams())
            Api.ScanDeviceParams();
    }

    void Update()
    {
        if (!active) return;

        if (Api.IsGearButtonPressed) Api.ScanDeviceParams();
        if (Api.IsCloseButtonPressed) Application.Quit();
        if (Api.HasNewDeviceParams()) Api.ReloadDeviceParams();

        Api.UpdateScreenParams();
    }
}

# Google Cardboard XR en El sueño de Goya

Rama: `cardboard-xr`. `main` sigue con el visor casero (`StereoGyroRig` armando dos cámaras a mano).

## Qué cambia con Cardboard

| | Visor casero (`main`) | Cardboard XR (esta rama) |
|---|---|---|
| Dos ojos | Dos cámaras con pantalla partida | El plugin renderiza una cámara para los dos ojos |
| Distorsión de las lentes | No hay | Corregida según el QR del visor |
| Seguimiento de cabeza | `Input.gyro` | Seguimiento de Cardboard (giroscopio + acelerómetro) |
| Botones | 1 toque recentra, 2 dedos cambia de modo | 1 toque recentra, ⚙ escanear QR, ✕ salir |

Las manos (Python → OSC → Unity), la galería y las partículas no cambian.

## Qué ya está hecho en el repo

- `Packages/manifest.json`: paquete `com.google.xr.cardboard` (v1.36.0). Unity baja solo XR Plug-in Management y sus dependencias.
- `Assets/StereoGyroRig.cs`: si el XR está activo, crea **una sola cámara** y toma la rotación de la cabeza de Cardboard. Si no, sigue igual que antes (editor con mouse, celular sin XR con dos cámaras).
- `Assets/CardboardSetup.cs` (nuevo, ya agregado al objeto **Rig**): pide el QR del visor la primera vez, botón ⚙ para volver a escanearlo y ✕ para salir.
- Player Settings (Android): orientación **Landscape Left**, gráficos **OpenGLES3** (sin Vulkan), punto de entrada **Activity** (sin GameActivity).
  Ya estaban bien: API mínima 26, IL2CPP, ARM64, Internet *Require*, Optimized Frame Pacing apagado.

## Qué hay que hacer en Unity (una sola vez)

Requiere tener **Git instalado** (Unity lo usa para bajar el paquete de GitHub).

1. Traer la rama:
   ```
   cd C:\Proyectos\EL-SUENO-DE-GOYA
   git fetch
   git checkout cardboard-xr
   ```
   Abrir el proyecto. La primera vez tarda más porque baja el paquete de Cardboard.
2. **XR Plug-in Management:** Edit → Project Settings → XR Plug-in Management → pestaña **Android** (ícono de Android) → tildar **Cardboard XR Plugin**. Dejar tildado *Initialize XR on Startup*.
3. **Plantillas de Gradle:** Edit → Project Settings → Player → Android → **Publishing Settings** → sección *Build* → tildar **Custom Main Gradle Template** y **Custom Gradle Properties Template**. Unity crea dos archivos en `Assets/Plugins/Android/`.
4. Abrir `Assets/Plugins/Android/mainTemplate.gradle` y, dentro del bloque `dependencies { ... }`, agregar:
   ```
   implementation 'androidx.appcompat:appcompat:1.6.1'
   implementation 'com.google.android.gms:play-services-vision:20.1.3'
   implementation 'com.google.android.material:material:1.12.0'
   implementation 'com.google.protobuf:protobuf-javalite:3.19.4'
   ```
5. Abrir `Assets/Plugins/Android/gradleTemplate.properties` y agregar al final:
   ```
   android.enableJetifier=true
   android.useAndroidX=true
   ```
6. Revisar en Player → Android → Other Settings:
   - *Target API Level*: **Automatic (highest installed)** o **35** o más. Si Unity avisa que falta el SDK, aceptar la actualización.
   - *Active Input Handling*: dejar **Both**. La guía de Google dice "New", pero los scripts del proyecto usan el sistema de input viejo (`Input.touchCount`, teclas de prueba) y con "New" dejarían de funcionar. Con "Both" el plugin anda igual.
7. Guardar (Ctrl+S y File → Save Project), hacer commit y push de los cambios (ver `docs/guia-git.md`).
8. File → Build Profiles → Android → **Build and Run**.

## Al abrir la app en el celular

- Pide escanear el **código QR del visor** (suele estar impreso en la caja o en el visor). Si el visor no tiene, se puede saltear y usa valores por defecto. Con el ⚙ se vuelve a escanear.
- Tocar la pantalla una vez con la vista al frente para recentrar.

## Aviso: Samsung Galaxy A04

El seguimiento de cabeza de Cardboard necesita **giroscopio** y el A04 no tiene. En ese caso el script sigue usando el modo acelerómetro (mirar arriba/abajo e inclinar la cabeza para girar), pero ahora con la distorsión de lentes de Cardboard. Para la experiencia completa hace falta un celular con giroscopio.

## Lo que no se pudo probar

Todo esto se armó sin poder abrir Unity ni compilar para el celular. Si algo falla, lo primero a mirar es la **Console** de Unity y mandar captura del primer error en rojo.

# El sueño de Goya

TP2 de Informática Aplicada 2 (Cátedra Causa): experiencia de realidad virtual con celular inspirada en Francisco de Goya.
Estudiantes: Josefina Soria y Pilar Gargiulo.

La persona entra al mundo de Goya con un visor en el celular y, con las manos (detectadas por una webcam), destapa los sueños del pintor: los cuadros se revelan, se deshacen en partículas y se vuelven a formar.

## Arquitectura

```
Webcam ──► tracker/hand_tracker.py (Python + MediaPipe) ──OSC UDP 5052──► Unity (celular Android en visor)
```

## Carpetas

- `Assets/`, `Packages/`, `ProjectSettings/`: proyecto de Unity (6000.6.1f1, URP). La escena es `Assets/Scenes/SampleScene.unity`.
- `tracker/`: detector de manos en Python que manda los datos por OSC.
- `blender/`: archivo fuente del macho cabrío (`aquelarre.fbx` en Unity).
- `docs/`: instrucciones originales y análisis del estado del TP.

## Cómo correrlo

1. Instalar Unity Hub y Unity **6000.6.1f1** con Android Build Support (OpenJDK, Android SDK y NDK).
2. En Unity Hub: Add → Add project from disk → elegir esta carpeta. La primera vez tarda porque genera la carpeta `Library`.
3. Abrir `Assets/Scenes/SampleScene`.
4. Con Python 3.10 a 3.12, en la carpeta `tracker`:
   ```
   pip install -r requirements.txt
   python hand_tracker.py --backend msmf
   ```
5. Darle Play en Unity.

Para el celular: File → Build Profiles → Android → Build and Run, y correr el tracker con la IP del celular (misma red wifi):
```
python hand_tracker.py --backend msmf --ip 192.168.1.35
```

### Pruebas sin manos (en el editor)

- Clic derecho + mouse: mirar. R: recentrar. V: visor / pantalla completa.
- I: deshacer el macho cabrío. T: deshacer los cuadros revelados.

## Trabajar entre varias compus

- Antes de empezar: **Pull** (traer lo último).
- Al terminar: **Commit** y **Push**.
- Evitar editar la escena (`SampleScene.unity`) las dos al mismo tiempo: es el archivo que más conflictos genera. Los scripts `.cs` se pueden editar en paralelo sin problema.

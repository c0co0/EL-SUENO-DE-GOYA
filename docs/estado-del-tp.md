# El sueño de Goya: qué hay hecho y qué falta

Actualizado el 19/10/2026 con el proyecto de Unity (`goyavrunity.zip`) y `aquelarrepruebauno.blend`.
El proyecto quedó descomprimido en `/mnt/project-files/proyecto-unity/goyavrunity/` (Unity 6000.6.1f1, URP, sin carpeta `Library`, que Unity regenera sola).

## 1. Arquitectura

```
Webcam ──► hand_tracker.py (Python + MediaPipe) ──OSC UDP 5052──► Unity (celular Android en visor)
```

## 2. Qué hace cada script de Unity (`Assets/`)

| Script | Objeto en la escena | Qué hace |
|---|---|---|
| `HandOSCReceiver.cs` | Manos | Recibe `/hand/Left|Right/present|open|lm` con la librería **extOSC**, convierte los 21 puntos a 3D y los muestra como esferitas (celeste/rosa, verdes si la mano está abierta). Deja el estado de cada mano para los demás scripts. |
| `StereoGyroRig.cs` | Rig | **Visor VR casero**: dos cámaras (pantalla partida), la cabeza gira con el giroscopio. 1 toque = recentrar, 2 dedos = alternar visor / pantalla completa (por eso en el video se ve una sola imagen). En el editor: clic derecho para mirar, V y R. Tiene el AudioListener en la cabeza. |
| `GoyaIntro.cs` | Galeria | Intro: el **macho cabrío** (`aquelarre.fbx`) flota y gira. Si lo apuntás con la mano abierta, se deshace en 6000 partículas y aparece la galería. Tecla I para probar sin manos. |
| `GoyaGallery.cs` | Galeria | 3 cuadros en arco cubiertos por un velo: centro *Una tercera de las Pinturas negras*, izquierda *Saturno*, derecha *El vuelo de las brujas*. La mano funciona como puntero (rayo cabeza → mano) y al pasarla el cuadro se revela. |
| `GoyaParticles.cs` | Galeria | Con la mano abierta sobre un cuadro revelado, el cuadro se deshace en 8000 partículas con sus colores, el viento se las lleva y después **vuelven a formar el mismo cuadro**. Tecla T para probar. |
| `GoyaAmbience.cs` | Galeria | Polvo flotante, aura cálida detrás de cada cuadro y sonido: ambiente *De Civitate Dei* (en loop) y efectos (whoosh al revelar y rehacer, guitarra shimmer al deshacer). |

El archivo `aquelarrepruebauno.blend` es de Blender 5.x. El `aquelarre.fbx` de Unity fue exportado de Blender 5.2 y está hecho con cubos y un material "brown", así que todo indica que el .blend es la fuente del macho cabrío.

## 3. Contra la consigna

| Columna | Estado |
|---|---|
| Interface | ✅ Captura óptica 2D (webcam + MediaPipe). Cambió respecto de la propuesta (decía cámara de profundidad): hay que contarlo en la bitácora. |
| Sistemas complejos | 🟡 Cada partícula tiene posición y velocidad, y se mueve por viento, ruido y rozamiento. Pero las partículas **no interactúan entre sí ni con la mano**, y el rehacerse es una interpolación (animación), no simulación. El anexo dice que no vale un efecto que solo imita. Es el punto más débil. |
| Entorno | ✅ VR con celular: estéreo + giroscopio. |
| Modelo 3D | 🟡 Existe (macho cabrío en Blender). La consigna pide modelar un **objeto real propio**: si se basaron en una figura real que tienen, sacarle fotos para la bitácora. |
| Sonido | 🟡 Los efectos suenan en la posición de cada cuadro (`PlayClipAtPoint` es 3D) y el oyente gira con la cabeza, así que **ya hay espacialización**. El ambiente es 2D (`spatialBlend = 0`). Para que sea claramente "audio inmersivo 3D" conviene sumar fuentes 3D en loop por cuadro y probarlo con auriculares. |
| Comunicación entre apps | ✅ OSC Python → Unity. |

## 4. Contra la idea

- ✅ Bienvenida del aquelarre (macho cabrío + cuadro central).
- ✅ Destapar los sueños con las manos.
- 🟡 Se desarman y se rearman, pero vuelven a ser **el mismo cuadro**, no un personaje o escena nueva.
- ❌ **Más movimiento → obras más oscuras**: no está. Solo hay 3 cuadros fijos.

## 5. Próximos pasos, en orden

1. **Abrir el proyecto y probarlo** en tu PC (Unity Hub → Add project from disk → carpeta `goyavrunity`; Unity 6000.6.1f1 con Android Build Support). Correr `python hand_tracker.py --backend msmf` y darle Play.
2. **Reforzar el sistema de partículas** (lo más riesgoso para la nota): que la mano empuje o atraiga las partículas mientras flotan, y que vuelvan con una fuerza tipo resorte en vez de interpolación.
3. **Progresión hacia lo oscuro**: medir cuánto se mueven las manos y, al pasar umbrales, cambiar los cuadros por Pinturas Negras más oscuras (más imágenes en `Assets`).
4. **Audio 3D**: ambiente o susurros en loop como AudioSource 3D en cada cuadro.
5. Fotos del objeto real del modelo, bitácora, video reel y video técnico.

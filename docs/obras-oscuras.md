# Obras que se oscurecen

Rama: `obras-oscuras` (sale de `cardboard-xr`).

## Cómo funciona

- **Oscuridad acumulada** (`darkness`, de 0 a 1, en el objeto **Galeria**): sube mientras las manos se mueven. Con unos 30 metros de movimiento de manos (más o menos medio minuto moviéndolas bastante) llega al máximo. Con las manos quietas baja muy despacio.
- **Rearmarse en otra obra:** cuando un cuadro se deshace en partículas, ya no vuelve el mismo. Las partículas se rearman en la obra siguiente de la lista **Darker Paintings**, cambiando de color en el camino. Si la oscuridad acumulada va más adelante en la lista, salta directo a una más oscura. Nunca muestra la misma obra en dos cuadros a la vez.
- **Agitación** (`agitation`, de 0 a 1): qué tan rápido se mueven las manos en este momento. Con las manos agitadas:
  - alcanza con menos tiempo de mano abierta para deshacer un cuadro;
  - el viento se lleva las partículas más fuerte;
  - si la agitación se mantiene alta (más de 0,75), los cuadros revelados se deshacen solos.
- **Todo se apaga con la oscuridad:** los cuadros se tiñen más oscuros, las auras pasan de dorado a rojo sangre, el polvo se vuelve gris y el sonido ambiente se pone más grave.

Si la lista **Darker Paintings** está vacía, todo funciona como antes (el cuadro se rearma igual).

## Qué hay que hacer en Unity (una vez)

1. Bajar las imágenes de las Pinturas Negras (por ejemplo de Wikimedia Commons o del sitio del Museo del Prado, son de dominio público). Sugerencia de orden, de menos a más oscura:
   1. *Dos viejos comiendo sopa*
   2. *Duelo a garrotazos*
   3. *El perro semihundido*
   4. *Átropos* (o *Las Parcas*)
   5. *Judith y Holofernes*
2. Crear la carpeta `Assets/Obras oscuras` y arrastrar ahí las imágenes.
3. Seleccionarlas todas en la ventana Project → en el Inspector, **Texture Type: Sprite (2D and UI)** → **Apply**. (Read/Write no hace falta.)
4. Seleccionar el objeto **Galeria** en la Hierarchy → componente **Goya Gallery** → **Darker Paintings**: arrastrar las imágenes en el orden de arriba.
5. Guardar la escena (Ctrl+S) y hacer commit y push.

## Probar en el editor sin manos

- **I**: saltear la intro de la cabra.
- Pasar el mouse no sirve para revelar: con el tracker de Python corriendo, apuntar con la mano a cada cuadro para revelarlo.
- **T**: deshacer todos los cuadros revelados → se rearman en otra obra.
- **O**: sumar oscuridad (+25 %). Mirar el valor *Darkness* en el Inspector de Galeria.
- **Backspace**: volver todo al principio (obras originales, sin oscuridad).

## Valores para ajustar (Inspector de Galeria y de Goya Particles)

| Valor | Dónde | Qué hace |
|---|---|---|
| Movement For Full Dark | Goya Gallery | Más alto = tarda más en oscurecerse |
| Darkness Recovery | Goya Gallery | 0 = la oscuridad nunca baja |
| Dark Tint | Goya Gallery | Color de los cuadros con oscuridad total |
| Auto Break Agitation | Goya Particles | Más alto = cuesta más que se deshagan solos (1 = nunca) |
| Agitation Wind | Goya Particles | Cuánto más fuerte sopla el viento con las manos agitadas |
| Dark Halo Color, Dark Ambient Pitch | Goya Ambience | Color de las auras y tono del sonido al final |

## Lo que no se pudo probar

Se escribió sin poder abrir Unity. Si algo falla, mandar captura del primer error rojo de la Console.

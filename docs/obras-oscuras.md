# Galería 360: de lo cotidiano a lo oscuro

Rama: `obras-oscuras` (sale de `cardboard-xr`).

## Cómo funciona

- **Anillo de 360°:** las obras se reparten alrededor de la persona (8 por defecto, *Ring Count*). La primera queda adelante y las siguientes alternan derecha e izquierda, así las primeras de la lista quedan más cerca de la vista inicial.
- **Una sola lista ordenada** (*Painting Sequence*, en el objeto **Galeria**): las primeras 8 llenan el anillo. Cada vez que una obra se desarma en partículas, se rearma como **la siguiente de la lista que todavía no apareció**. Así el anillo pasa de a poco de la vida cotidiana a las Pinturas Negras.
- **El movimiento acelera la caída:** cuanto más se mueven las manos, sube la oscuridad y el viento se lleva más fuerte las partículas. Con las manos muy agitadas puede saltear una obra de la lista (*Max Skip*).
- **Todo se apaga con el recorrido:** a medida que avanza la lista (o con mucho movimiento), los cuadros se tiñen más oscuros, las auras pasan de dorado a rojo, el polvo se vuelve gris y el sonido ambiente se pone más grave.
- **Cuando se termina la lista,** los cuadros siguen rotando entre las últimas (las más oscuras).

## Gestos

| Gesto | Qué hace |
|---|---|
| Apuntar a la cabra con el **puño cerrado** y después **abrir la mano** sobre ella (un segundo) | Se deshace la cabra y aparecen las obras |
| **Apuntar** una obra con cualquier mano (un segundo) | Se revela |
| **Barrer** una obra revelada con la **mano abierta** (moverla rápido de lado a lado, el cursor se pone naranja) | Se desarma y vuelve como la siguiente |
| **Apuntar una obra y cerrar el puño** (mantenerlo) | Te acercás a esa obra mientras el puño siga cerrado; al soltarlo te quedás ahí |
| **Dos puños cerrados** | Te alejás |

Las manos y el puntero giran con la cabeza, así se pueden tocar las obras de atrás. No se puede salir del anillo ni acercarse a menos de 70 cm de las obras (*Min Distance To Paintings*).

Colores del cursor: blanco o color de la mano = apuntando, verde = mano abierta, naranja = barriendo.

## Las obras ya cargadas

Las 28 obras están en `Assets/Obras`, numeradas en el orden del recorrido y ya cargadas en **Galeria → Painting Sequence**. Las primeras 8 llenan el anillo al empezar.

**Vida cotidiana:** 01 La gallina ciega · 02 La cometa · 03 El cacharrero · 04 Manuel Osorio Manrique de Zúñiga · 05 La nevada · 06 La lechera de Burdeos · 07 La maja vestida · 08 Majas en el balcón

**Lo cortesano y lo inquietante:** 09 La maja desnuda · 10 La familia de Carlos IV · 11 Aníbal vencedor · 12 El pelele · 13 Riña de gatos · 14 Milagro de San Antonio · 15 El entierro de la sardina · 16 Ciudad sobre una roca

**Brujas y sueños:** 17 El aquelarre · 18 Vuelo de brujas · 19 Modo de volar (Disparates) · 20 Las viejas

**La guerra:** 21 El coloso · 22 El 3 de mayo de 1808 · 23 Tampoco (Desastres de la guerra)

**Pinturas Negras:** 24 Dos mujeres y un hombre · 25 Dos viejos comiendo sopa · 26 Duelo a garrotazos · 27 El perro · 28 Saturno devorando a su hijo

### Cambiar el orden o sumar obras

- **Reordenar:** en *Painting Sequence*, arrastrar cada elemento desde las dos rayitas `=` de la izquierda.
- **Sumar una obra:** ponerla en `Assets/Obras` → Inspector → **Texture Type: Sprite (2D and UI)** → **Apply**, y arrastrarla a la lista en el lugar que corresponda (Read/Write no hace falta).
- **Sacar una:** clic derecho sobre el elemento de la lista → *Delete Array Element*.
- Guardar la escena (Ctrl+S) y hacer commit y push.

Si *Painting Sequence* queda vacía, la galería vuelve a los 3 cuadros de adelante.

## Probar en la PC (sin tracker ni celular)

| Tecla / mouse | Qué hace |
|---|---|
| **I** | Saltear la intro de la cabra |
| **Clic derecho + mover el mouse** | Mirar alrededor |
| **W / S** o **flechas ↑ / ↓** | Acercarse / alejarse (con clic izquierdo sobre una obra, ↑ va hacia esa obra) |
| **Flechas ← / →** | Girar |
| **Mantener clic izquierdo** sobre una obra | Revelarla (apunta al centro de la pantalla) |
| **Mantener E** sobre una obra revelada | Barrerla: se desarma y vuelve como la siguiente |
| **T** | Desarmar todas las obras reveladas a la vez |
| **O** | Sumar oscuridad (+25 %) |
| **V** | Cambiar entre pantalla partida (visor) y una sola cámara |
| **R** | Recentrar la vista |
| **Backspace** | Volver todo al principio |

Con el tracker de Python corriendo, las manos funcionan además del mouse.

## Valores para ajustar

| Valor | Componente | Qué hace |
|---|---|---|
| Ring Count | Goya Gallery | Cuántas obras hay en el anillo |
| Distance, Painting Max Size | Goya Gallery | Radio del anillo y tamaño de las obras |
| Max Skip | Goya Gallery | Cuántas obras puede saltear si las manos se mueven mucho (0 = nunca) |
| Move Speed, Fist Hold Time | Goya Gallery | Velocidad al acercarse y cuánto hay que sostener el puño antes de moverse |
| Movement For Full Dark, Dark Tint | Goya Gallery | Cuánto movimiento oscurece del todo y a qué color |
| Touch Hold Time, Require Fist First | Goya Intro | Cuánto hay que sostener la mano abierta sobre la cabra, y si hace falta el puño antes |
| Swipe Speed, Swipe Time | Goya Gallery | Qué tan rápido y cuánto tiempo hay que barrer una obra para desarmarla |
| Auto Break, Auto Break Agitation | Goya Particles | Que las obras se desarmen solas con las manos muy agitadas (apagado por defecto) |
| Particle Count | Goya Particles | Bajarlo (por ejemplo a 4000) si el celular anda lento con 8 obras |

## Lo que no se pudo probar

Se escribió sin poder abrir Unity. Si algo falla, mandar captura del primer error rojo de la Console.

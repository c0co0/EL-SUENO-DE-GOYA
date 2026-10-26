# Galería 360: de lo cotidiano a lo oscuro

Rama: `obras-oscuras` (sale de `cardboard-xr`).

## Cómo funciona

- **Bienvenida:** el cabrío flota adelante y arriba de él aparecen, una por una, las reglas (campo *Rules* de **Goya Intro**, se pueden editar en el Inspector). Hasta que no termina de explicar no se lo puede deshacer (*Wait For Rules*). Se le puede poner voz a cada frase en *Narration*. La tecla **I** saltea todo.
- **Las obras están en el aire:** después de la cabra, las obras son polvo casi invisible, desparramado y mezclado con el aire (*Assemble From Dust* en **Goya Gallery**). Al barrer cualquiera con la mano abierta, se arman todas, al revés de cuando se desarman.
- **Tandas:** con las obras armadas, un barrido sobre cualquiera las desarma **todas juntas**: las partículas vuelan, se desvanecen, y la tanda siguiente se arma desde el aire (*Change All Together*). Con 28 obras y 7 por tanda (*Batch Size*) son 4 tandas, de la vida cotidiana a las Pinturas Negras. La última tanda ya no se desarma (Backspace vuelve a empezar).

- **Anillo de 360°:** las obras se reparten alrededor de la persona (7 por defecto, *Batch Size*). La primera queda adelante y las siguientes alternan derecha e izquierda, así las primeras de la lista quedan más cerca de la vista inicial.
- **Una sola lista ordenada** (*Painting Sequence*, en el objeto **Galeria**): se recorre de a una tanda. Si se apaga *Change All Together*, vuelve el modo anterior: cada obra se desarma sola y se rearma como la siguiente de la lista que todavía no apareció.
- **El movimiento acelera la caída:** cuanto más se mueven las manos, sube la oscuridad y el viento se lleva más fuerte las partículas. En el modo de a una obra, con las manos muy agitadas puede saltear una de la lista (*Max Skip*).
- **Todo se apaga con el recorrido:** a medida que avanza la lista (o con mucho movimiento), los cuadros se tiñen más oscuros, las auras pasan de dorado a rojo, el polvo se vuelve gris y el sonido ambiente se pone más grave.
- **Cuando se termina la lista,** los cuadros siguen rotando entre las últimas (las más oscuras).

## Gestos

| Gesto | Qué hace |
|---|---|
| Apuntar a la cabra con el **puño cerrado** y después **abrir la mano** sobre ella (un segundo) | Se deshace la cabra y aparecen las obras |
| **Barrer** donde está una obra con la **mano abierta** (moverla de lado a lado, el cursor se pone naranja) | Se arman todas las obras de la tanda |
| **Barrer** una obra armada | Se desvanecen todas y se arma la tanda siguiente |
| **Apuntar una obra y cerrar el puño** (mantenerlo) | Te acercás a esa obra mientras el puño siga cerrado; al soltarlo te quedás ahí |
| **Dos puños cerrados** | Te alejás |

Las manos y el puntero giran con la cabeza, así se pueden tocar las obras de atrás. No se puede salir del anillo ni acercarse a menos de 70 cm de las obras (*Min Distance To Paintings*).

Colores del cursor: blanco o color de la mano = apuntando, verde = mano abierta, naranja = barriendo.

## Las obras ya cargadas

Las 28 obras están en `Assets/Obras`, numeradas en el orden del recorrido y ya cargadas en **Galeria → Painting Sequence**. Van de a 7 por tanda: 1 a 7, 8 a 14, 15 a 21 y 22 a 28.

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
| **Mantener E** mirando una obra | Barrerla (apunta al centro de la pantalla): arma la tanda, o la desarma y pasa a la siguiente |
| **Mantener clic izquierdo** sobre una obra + **↑** | Acercarse a esa obra |
| **T** | Lo mismo que barrer, sin apuntar |
| **O** | Sumar oscuridad (+25 %) |
| **V** | Cambiar entre pantalla partida (visor) y una sola cámara |
| **R** | Recentrar la vista |
| **Backspace** | Volver todo al principio |

Con el tracker de Python corriendo, las manos funcionan además del mouse.

## Valores para ajustar

| Valor | Componente | Qué hace |
|---|---|---|
| Batch Size | Goya Gallery | Cuántas obras hay en el anillo (una tanda) |
| Change All Together | Goya Gallery | Si un barrido cambia todas las obras juntas o de a una |
| Distance, Painting Max Size | Goya Gallery | Radio del anillo y tamaño de las obras |
| Max Skip | Goya Gallery | Cuántas obras puede saltear si las manos se mueven mucho (0 = nunca) |
| Move Speed, Fist Hold Time | Goya Gallery | Velocidad al acercarse y cuánto hay que sostener el puño antes de moverse |
| Movement For Full Dark, Dark Tint | Goya Gallery | Cuánto movimiento oscurece del todo y a qué color |
| Touch Hold Time, Require Fist First | Goya Intro | Cuánto hay que sostener la mano abierta sobre la cabra, y si hace falta el puño antes |
| Swipe Speed, Swipe Distance | Goya Gallery | Qué tan rápido y cuánto recorrido de mano abierta hace falta para desarmar una obra (más bajo = más fácil) |
| Auto Break, Auto Break Agitation | Goya Particles | Que las obras se desarmen solas con las manos muy agitadas (apagado por defecto) |
| Particle Count | Goya Particles | Partículas por obra (4000). Bajarlo (por ejemplo a 2500) si el celular anda lento con 8 obras flotando |
| Air Spread, Air Depth, Air Gray, Air Alpha, Air Size | Goya Particles | Qué tan desparramado, profundo, gris, visible y grande es el polvo antes de armarse (Air Alpha 0 = invisible) |
| Rules, Rule Duration, Wait For Rules | Goya Intro | Frases del cabrío, segundos por frase, y si hay que esperar a que termine |

## Lo que no se pudo probar

Se escribió sin poder abrir Unity. Si algo falla, mandar captura del primer error rojo de la Console.

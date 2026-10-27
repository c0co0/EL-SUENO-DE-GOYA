# Galería 360: de lo cotidiano a lo oscuro

Rama: `obras-oscuras` (sale de `cardboard-xr`).

## Cómo funciona

- **Bienvenida:** el macho cabrío flota adelante con la frase "Levantá una mano para empezar" (*Start Line*), así nadie se pierde las reglas mientras se pone el visor. Al levantar una mano, arriba de él aparecen, una por una, frases cortas con las reglas (campo *Lines* de **Goya Intro**, se pueden editar en el Inspector). Hasta que no termina de explicar no se lo puede deshacer (*Wait For Rules*). Se le puede poner voz a cada frase en *Narration*.
- **El cabrío arde:** con un brazo levantado y la mano abierta, se prende fuego, se carboniza y se dispersa en brasas.
- **Una obra a la vez:** la obra empieza como polvo casi invisible en el aire. Con los dos brazos levantados y las manos abiertas, el polvo se arma. Con los dos puños cerrados, se desarma en un remolino de brasas que viaja hasta otro lugar al azar de la galería y se arma como la obra siguiente.
- **Galería de 180°:** las obras aparecen solo adelante y a los costados, nunca atrás (*Arc Degrees*, 5 lugares posibles en *Arc Slots*). La primera aparece adelante.
- **Lista ordenada por épocas** (*Painting Sequence*, en el objeto **Galeria**). La última obra ya no se desarma (Backspace vuelve a empezar).
- **Todo se apaga con el recorrido:** a medida que avanza la lista (o con mucho movimiento de manos), los cuadros se tiñen más oscuros, las auras pasan de dorado a rojo y el sonido ambiente se pone más grave.

## Gestos

| Gesto | Qué hace |
|---|---|
| **Levantar una mano** al principio | El cabrío empieza a explicar las reglas |
| **Un brazo levantado con la mano abierta** (sostenerlo un momento), cuando el cabrío terminó de hablar (si es la misma mano del principio, hay que bajarla antes) | El cabrío se prende fuego y aparece la galería |
| **Los dos brazos levantados con las manos abiertas** | Se arma la obra |
| **Los dos puños cerrados** (a cualquier altura) | Se desarma la obra y se arma la siguiente en otro lugar |
| **Un dedo levantado** (cualquier mano) | Te acercás a la obra que estás mirando |
| **Un dedo levantado en cada mano** | Te alejás |
| **Cruzar los brazos en X** delante del pecho (sostenerlo un segundo y medio) | Vuelve todo a empezar, desde el cabrío. Necesita el tracker con el cuerpo; en la ventana dice "BRAZOS CRUZADOS" |

Los gestos de armar y desarmar hay que sostenerlos un momento (*Gesture Hold Time*) y soltarlos antes de repetirlos. "Levantado" quiere decir que la mano está en la parte de arriba de la imagen de la cámara (*Hand High Y*); si el tracker también ve el cuerpo, alcanza con que el brazo esté arriba del hombro.

En la ventana del tracker, al lado de cada mano dice si está abierta o cerrada y cuántos dedos ve entre paréntesis: 0 = puño, 1 = un dedo, 3 o más = abierta.

Los gestos viejos (barrer, puño para acercarse, inclinarse) quedaron apagados y se pueden volver a prender en **Goya Gallery**: *Swipe Gestures*, *Fist Walk* y *Lean Walk*.

## Las obras ya cargadas

Las obras están en `Assets/Obras` y ya cargadas en **Galeria → Painting Sequence**, agrupadas por época (*Epoch Sizes* = 6, 7, 5, 4, 5). Los números del archivo son de antes y no indican el orden.

| Época | Tanda 1 | Tanda 2 |
|---|---|---|
| **Cartones para tapices** (1778-1792) | La gallina ciega · La cometa · El cacharrero | La nevada · El pelele · Riña de gatos |
| **Pintor de la corte** (1771-1801) | Aníbal vencedor · Manuel Osorio · Milagro de San Antonio | Majas en el balcón · La maja vestida · La maja desnuda · La familia de Carlos IV |
| **Brujas y disparates** (1797-1823) | El aquelarre · Vuelo de brujas | El entierro de la sardina · Modo de volar · Ciudad sobre una roca |
| **La guerra** (1808-1814) | El coloso · El 3 de mayo de 1808 | Tampoco · Las viejas |
| **Pinturas Negras** (1819-1823) | Dos mujeres y un hombre · Dos viejos comiendo sopa | Duelo a garrotazos · El perro · Saturno devorando a su hijo |

*La lechera de Burdeos* (1827) quedó afuera porque no encaja en ninguna época: es luminosa pero posterior a las Pinturas Negras.

### Cambiar el orden o sumar obras

- **Reordenar:** en *Painting Sequence*, arrastrar cada elemento desde las dos rayitas `=` de la izquierda. Si cambia cuántas obras tiene una época, actualizar *Epoch Sizes*.
- **Sumar una obra:** ponerla en `Assets/Obras` → Inspector → **Texture Type: Sprite (2D and UI)** → **Apply**, y arrastrarla a la lista en el lugar que corresponda (Read/Write no hace falta).
- **Sacar una:** clic derecho sobre el elemento de la lista → *Delete Array Element*.
- Guardar la escena (Ctrl+S) y hacer commit y push.

Si *Painting Sequence* queda vacía, la galería vuelve a los 3 cuadros de adelante.

## Probar en la PC (sin tracker ni celular)

| Tecla / mouse | Qué hace |
|---|---|
| **Espacio** | Que el cabrío empiece a explicar (en lugar de levantar la mano) |
| **I** | Prender fuego al cabrío sin el gesto |
| **Clic derecho + mover el mouse** | Mirar alrededor |
| **W / S** o **flechas ↑ / ↓** | Acercarse / alejarse |
| **Flechas ← / →** | Girar |
| **E** | Armar la obra, o desarmarla y pasar a la siguiente |
| **T** | Lo mismo que E |
| **O** | Sumar oscuridad (+25 %) |
| **V** | Cambiar entre pantalla partida (visor) y una sola cámara |
| **R** | Recentrar la vista |
| **Backspace** | Volver todo al principio |

Con el tracker de Python corriendo, las manos funcionan además del teclado.

## Valores para ajustar

| Valor | Componente | Qué hace |
|---|---|---|
| Epoch Sizes, Batches Per Epoch | Goya Gallery | Cuántas obras tiene cada época y en cuántas tandas se reparte |
| Batch Spacing | Goya Gallery | Grados entre las obras de una tanda |
| Change All Together | Goya Gallery | Si un barrido cambia todas las obras juntas o de a una |
| Break Wave Time, Scatter Time, Reform Time, Reform Spread | Goya Particles | Qué tan lento se desarma, cuánto flota el humo y qué tan lento se arma la tanda siguiente |
| One At A Time, Arc Degrees, Arc Slots | Goya Gallery | Una obra a la vez, cuántos grados tiene la galería y en cuántos lugares puede aparecer |
| Hand High Y, Gesture Hold Time | Goya Gallery | Qué tan arriba tiene que estar la mano para contar como levantada, y cuánto sostener los gestos |
| Ember Smoke, Ember Alpha, Swirl, Swirl Pause | Goya Particles | Color y visibilidad de las brasas, cuánto gira el remolino y cuánto tiempo extra gira antes de viajar |
| Travel, Travel Arc | Goya Particles | Que las brasas viajen hasta la obra siguiente, y cuánto suben en el camino |
| Burn, Burn Time, Fire Rate, Fire Color, Ember Color | Goya Intro | Fuego del cabrío: si arde, cuánto tiempo, cuántas llamas y sus colores |
| Distance, Painting Max Size | Goya Gallery | Radio del anillo y tamaño de las obras |
| Max Skip | Goya Gallery | Cuántas obras puede saltear si las manos se mueven mucho (0 = nunca) |
| Move Speed, Fist Hold Time | Goya Gallery | Velocidad al acercarse y cuánto hay que sostener el puño antes de moverse |
| Movement For Full Dark, Dark Tint | Goya Gallery | Cuánto movimiento oscurece del todo y a qué color |
| One Arm Gesture, Hand High Y, Arms Up Level, Touch Hold Time | Goya Intro | Gesto para prender fuego al cabrío: qué tan arriba la mano o el brazo, y cuánto sostenerlo. *Hand Gesture* vuelve a activar el gesto viejo del puño |
| Swipe Speed, Swipe Distance | Goya Gallery | Qué tan rápido y cuánto recorrido de mano abierta hace falta para desarmar una obra (más bajo = más fácil) |
| Auto Break, Auto Break Agitation | Goya Particles | Que las obras se desarmen solas con las manos muy agitadas (apagado por defecto) |
| Particle Count | Goya Particles | Partículas por obra (4000). Bajarlo (por ejemplo a 2500) si el celular anda lento con 8 obras flotando |
| Air Spread, Air Depth, Air Gray, Air Alpha, Air Size | Goya Particles | Qué tan desparramado, profundo, gris, visible y grande es el polvo antes de armarse (Air Alpha 0 = invisible) |
| Lean Walk, Lean Forward, Lean Back | Goya Gallery | Moverse inclinándose (apagado), y cuánto hay que inclinarse |
| Lines, Rule Duration, Wait For Rules | Goya Intro | Frases del cabrío, segundos por frase, y si hay que esperar a que termine |

## Lo que no se pudo probar

Se escribió sin poder abrir Unity. Si algo falla, mandar captura del primer error rojo de la Console.

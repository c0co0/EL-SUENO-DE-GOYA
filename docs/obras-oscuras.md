# Galería 360: de lo cotidiano a lo oscuro

Rama: `obras-oscuras` (sale de `cardboard-xr`).

## Cómo funciona

- **Bienvenida:** el cabrío flota adelante y arriba de él aparecen, una por una, las reglas (campo *Rules* de **Goya Intro**, se pueden editar en el Inspector). Hasta que no termina de explicar no se lo puede deshacer (*Wait For Rules*). Se le puede poner voz a cada frase en *Narration*. La tecla **I** saltea todo.
- **Las obras están en el aire:** después de la cabra, las obras son polvo casi invisible, desparramado y mezclado con el aire (*Assemble From Dust* en **Goya Gallery**). Al barrer cualquiera con la mano abierta, se arman todas, al revés de cuando se desarman.
- **Una obra a la vez** (*One At A Time* en **Goya Gallery**): se ve una sola obra. Al barrerla (o con el conjuro de los brazos) se desarma en un remolino de brasas rojas, y las brasas viajan por el aire, rodeándote, hasta un lugar al azar del anillo de 360° (*Ring Slots*, 8 lugares), donde se arman como la obra siguiente de la lista. Las obras siguen el orden de las épocas. La última ya no se desarma (Backspace vuelve a empezar).
- **Tandas por época** (si se apaga *One At A Time*): un barrido desarma todas las obras de la tanda juntas y la tanda siguiente aparece alrededor de donde estás mirando. Cada época se reparte en 2 tandas (*Batches Per Epoch*) y nunca se mezclan.
- **Una sola lista ordenada** (*Painting Sequence*, en el objeto **Galeria**), agrupada por época. *Epoch Sizes* dice cuántas obras tiene cada época, en orden. Si se apaga *Change All Together*, vuelve el modo anterior: anillo de 360° donde cada obra se desarma sola y se rearma como la siguiente.
- **El movimiento acelera la caída:** cuanto más se mueven las manos, sube la oscuridad y el viento se lleva más fuerte las partículas. En el modo de a una obra, con las manos muy agitadas puede saltear una de la lista (*Max Skip*).
- **Todo se apaga con el recorrido:** a medida que avanza la lista (o con mucho movimiento), los cuadros se tiñen más oscuros, las auras pasan de dorado a rojo, el polvo se vuelve gris y el sonido ambiente se pone más grave.

## Gestos

| Gesto | Qué hace |
|---|---|
| **Levantar los dos brazos e inclinarse un poco hacia adelante** (sostenerlo un segundo), cuando el cabrío terminó de hablar | El cabrío se prende fuego, se carboniza y se dispersa en brasas; después aparece la primera obra |
| **Barrer** donde está la obra con la **mano abierta** (moverla de lado a lado, el cursor se pone naranja) | El polvo se arma en la obra |
| **Barrer** la obra armada | Se hace brasa, viaja por el aire y se arma la siguiente en otro lugar |
| **Apuntar una obra y cerrar el puño** (mantenerlo) | Te acercás a esa obra mientras el puño siga cerrado; al soltarlo te quedás ahí |
| **Dos puños cerrados** | Te alejás |
| **Inclinarte hacia adelante** (hacia la cámara) | Avanzás hacia donde mirás; cuanto más te inclinás, más rápido |
| **Echarte hacia atrás** | Retrocedés |
| **Levantar los dos brazos** y sostenerlos un segundo | Conjuro: arma la tanda, o la desvanece y llega la siguiente (igual que barrer). Hay que bajarlos para hacer otro |

### Cuerpo

El tracker ahora sigue también el cuerpo con la misma cámara. Al arrancar, quedate derecho unos segundos frente a la cámara: toma esa postura como punto de partida (en la ventana dice "tomando postura..."). Si te movés de lugar, apretá **C** en la ventana del tracker para tomarla de nuevo. La inclinación se mide por cuánto más grandes se ven los hombros, así que la cámara tiene que verte por lo menos de la cintura para arriba.

Si la computadora va lenta con manos y cuerpo, se puede correr solo con manos: `py -3.12 hand_tracker.py --no-body`.

Las manos y el puntero giran con la cabeza, así se pueden tocar las obras de atrás. No se puede salir del anillo ni acercarse a menos de 70 cm de las obras (*Min Distance To Paintings*).

Colores del cursor: blanco o color de la mano = apuntando, verde = mano abierta, naranja = barriendo.

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
| Epoch Sizes, Batches Per Epoch | Goya Gallery | Cuántas obras tiene cada época y en cuántas tandas se reparte |
| Batch Spacing | Goya Gallery | Grados entre las obras de una tanda |
| Change All Together | Goya Gallery | Si un barrido cambia todas las obras juntas o de a una |
| Break Wave Time, Scatter Time, Reform Time, Reform Spread | Goya Particles | Qué tan lento se desarma, cuánto flota el humo y qué tan lento se arma la tanda siguiente |
| One At A Time, Ring Slots | Goya Gallery | Una obra a la vez y en cuántos lugares del anillo puede aparecer |
| Ember Smoke, Ember Alpha, Swirl, Swirl Pause | Goya Particles | Color y visibilidad de las brasas, cuánto gira el remolino y cuánto tiempo extra gira antes de viajar |
| Travel, Travel Arc | Goya Particles | Que las brasas viajen hasta la obra siguiente, y cuánto suben en el camino |
| Burn, Burn Time, Fire Rate, Fire Color, Ember Color | Goya Intro | Fuego del cabrío: si arde, cuánto tiempo, cuántas llamas y sus colores |
| Distance, Painting Max Size | Goya Gallery | Radio del anillo y tamaño de las obras |
| Max Skip | Goya Gallery | Cuántas obras puede saltear si las manos se mueven mucho (0 = nunca) |
| Move Speed, Fist Hold Time | Goya Gallery | Velocidad al acercarse y cuánto hay que sostener el puño antes de moverse |
| Movement For Full Dark, Dark Tint | Goya Gallery | Cuánto movimiento oscurece del todo y a qué color |
| Body Gesture, Arms Up Level, Lean Forward, Touch Hold Time | Goya Intro | Gesto para deshacer al cabrío: qué tan arriba los brazos, cuánto inclinarse y cuánto sostenerlo. *Hand Gesture* vuelve a activar el gesto viejo del puño |
| Swipe Speed, Swipe Distance | Goya Gallery | Qué tan rápido y cuánto recorrido de mano abierta hace falta para desarmar una obra (más bajo = más fácil) |
| Auto Break, Auto Break Agitation | Goya Particles | Que las obras se desarmen solas con las manos muy agitadas (apagado por defecto) |
| Particle Count | Goya Particles | Partículas por obra (4000). Bajarlo (por ejemplo a 2500) si el celular anda lento con 8 obras flotando |
| Air Spread, Air Depth, Air Gray, Air Alpha, Air Size | Goya Particles | Qué tan desparramado, profundo, gris, visible y grande es el polvo antes de armarse (Air Alpha 0 = invisible) |
| Lean To Walk, Lean Forward, Lean Back | Goya Gallery | Moverse inclinándose, y cuánto hay que inclinarse para avanzar o retroceder |
| Arms Conjure, Arms Up Level, Arms Hold Time | Goya Gallery | Conjuro con los brazos, qué tan arriba y cuánto tiempo sostenerlos |
| Rules, Rule Duration, Wait For Rules | Goya Intro | Frases del cabrío, segundos por frase, y si hay que esperar a que termine |

## Lo que no se pudo probar

Se escribió sin poder abrir Unity. Si algo falla, mandar captura del primer error rojo de la Console.

# Galería 360: de lo cotidiano a lo oscuro

Rama: `obras-oscuras` (sale de `cardboard-xr`).

## Cómo funciona

- **Bienvenida:** el macho cabrío flota adelante con la frase "Levantá una mano para empezar" (*Start Line*), así nadie se pierde las reglas mientras se pone el visor. Al levantar una mano, arriba de él aparecen, una por una, frases cortas con las reglas (campo *Lines* de **Goya Intro**, se pueden editar en el Inspector). Hasta que no termina de explicar no se lo puede deshacer (*Wait For Rules*). Se le puede poner voz a cada frase en *Narration*.
- **El cabrío arde:** con un brazo levantado y la mano abierta, se prende fuego, se carboniza y se dispersa en brasas.
- **Un camino para recorrer** (*Path Mode* en **Goya Gallery**): las obras están a los costados de un camino, alternando izquierda y derecha, algunas más cerca y otras más atrás. El camino tiene cinco zonas, una por época y en orden: cartones para tapices, la corte, brujas y disparates, la guerra y las Pinturas Negras (*Zone Length*, 9 m cada una). Caminar hacia adelante es adentrarse en lo oscuro.
- **Un paisaje como el fondo de La gallina ciega** (**Goya Environment**, se agrega solo): cielo azul con nubes doradas, montañas azuladas a lo lejos, cerros, un lago plateado alrededor y tierra ocre con pasto oscuro por donde va el camino. Todo se genera al empezar, sin imágenes. Zona por zona el paisaje se tiñe (*Zones → Tint*): dorado en la corte, noche azul en las brujas, rojizo en la guerra y casi negro en las Pinturas Negras. Al pasar de una zona a otra cambia de a poco, y lo lejano se pierde en la niebla. Las paredes hechas con pinturas de la versión anterior se pueden volver a prender con *Walls* y *Ground*.
- **Las obras empiezan invisibles** (no hay polvo ni partículas flotando en el aire; para volver a verlo, subir Air Alpha y prender Dust en Goya Ambience). Mirando hacia donde está una obra, con los dos brazos levantados y las manos abiertas se arma. Con los dos puños cerrados se desarma en un remolino de brasas y vuelve a ser polvo, para armarla otra vez si se quiere.
- **Todo se apaga al avanzar:** cuanto más adentro del camino, más oscuros se tiñen los cuadros, las auras pasan de dorado a rojo y el sonido ambiente se pone más grave.
- Si se apaga *Path Mode*, vuelve la galería de 180° con una obra a la vez (*One At A Time*, *Arc Degrees*, *Arc Slots*).

## Gestos

| Gesto | Qué hace |
|---|---|
| **Levantar una mano** al principio | El cabrío empieza a explicar las reglas |
| **Un brazo levantado con la mano abierta** (sostenerlo un momento), cuando el cabrío terminó de hablar (si es la misma mano del principio, hay que bajarla antes) | El cabrío se prende fuego y aparece la galería |
| **Los dos brazos levantados con las manos abiertas**, mirando una obra | Se arma esa obra |
| **Los dos puños cerrados** (a cualquier altura), mirando una obra | Se desarma en brasas y vuelve a quedar invisible |
| **Un dedo levantado** (cualquier mano) | Caminás hacia adelante, hacia donde mirás |
| **Un dedo levantado en cada mano** | Caminás hacia atrás |
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

## Pinturas negras por dentro (dioramas)

Al salir de la cueva, cuatro Pinturas Negras se recorren por dentro, una detrás de otra: primero *Saturno devorando a su hijo* (es lo que se ve desde la salida), después *El gran cabrón* (el aquelarre), *Las Parcas* y *El perro*. Cada una está separada en capas a distintas distancias (el fondo lejos, a 16 m, y las figuras más cerca). Desde la entrada de cada una se ve como la pintura, pero el fondo sigue más allá del cuadro y se funde con la oscuridad, así que no hay recuadro. Al caminar las capas se separan y se pasa entre las figuras, que respiran apenas. Al atravesar el fondo de una se entra en la siguiente; el de El perro es el final.

| Pintura | Capas (de atrás hacia adelante) |
|---|---|
| El gran cabrón | la multitud; el grupo de la derecha y el de la izquierda; el macho cabrío; la muchacha de blanco |
| Las Parcas | el paisaje con el río; las dos de la izquierda; la de adelante con el hombre atado; los arbustos oscuros |
| El perro | el campo ocre vacío; el perro; la ladera (el perro asoma detrás) |
| Saturno | el fondo oscuro; el cuerpo; la cabeza y el hombro; el hijo con las manos |

- No se ven desde el principio: aparecen de la oscuridad recién al llegar a la salida de la cueva.
- Las capas están en `Assets/Resources/Dioramas/`, del mismo tamaño que la pintura. Las de El gran cabrón, Las Parcas y El perro se generan con `tools/paisaje/dioramas.py` (la imagen de Las Parcas está en `tools/paisaje/fuentes`). *El gran cabrón* sale de una imagen chica (960 px de ancho), así que se ve más blanda que las otras.
- `GoyaDiorama` (lo agrega solo `GoyaGallery`) tiene la lista: nombre, obra que reemplaza (El perro ya no está en la cueva), alto, y cada capa con su imagen y distancia. Entre una pintura y la siguiente hay 20 m (`spacing`).

## El piso y la profundidad

`GoyaTerrain` (lo agrega solo el ambiente) arma un terreno de verdad: plano en el camino, con lomas a los costados que tapan lo que está detrás, y que baja hasta el lago a lo lejos. Tiene las pinceladas del suelo de *Duelo a garrotazos* y los colores del camino de *La gallina ciega* y la tierra de *La cometa*. Hay matas de pasto: muchas y chicas cerca del camino, menos y más grandes lejos, alrededor de los árboles y al pie de cada obra, que quedan un poco enterrados. Lo lejano se aclara con bruma (perspectiva aérea) y todo se oscurece con cada época.

## Las obras son parte del lugar (relieve)

Las obras del camino (antes de la cueva) ya no flotan: están apoyadas en el piso, un poco enterradas entre el pasto, con los bordes gastados y en relieve (`GoyaRelief`, se agrega solo). Las figuras de cada obra (los recortes de `Assets/Resources/Cortes`) sobresalen, el resto es una lámina gruesa que se afina hacia los bordes, con luz y sombra en las pinceladas. Se ajusta en el componente Goya Relief: cuánto sobresalen las figuras (`figureDepth`), el grosor (`baseDepth`), las pinceladas (`detailDepth`), cuánto se comen los bordes (`roughEdge`) y cuánto quedan enterradas (`sink`). Las obras animadas quedan planas (`flat`), porque sus capas van encima.

## Pinturas animadas: El pelele y Vuelo de brujas

- Cuando *El pelele* está armado, las mujeres lo tiran con la manta: sale por arriba del cuadro dando vueltas, cae y la manta se hunde (`GoyaAnimated`). Capas generadas con `tools/paisaje/pelele.py`.
- Cuando *Vuelo de brujas* está armado, el grupo de brujas que lleva al hombre flota en el cielo: sube y baja despacio, se mece y gira (`GoyaVuelo`). Capas generadas con `tools/paisaje/vuelo.py` a partir del recorte de `Cortes`.
- Las capas están en `Assets/Resources/Animadas`. Los dos componentes se agregan solos y se ajustan en el Inspector (velocidad, altura, giro).

## De dónde nacen las obras

En el paisaje solo hay árboles recortados de pinturas de Goya, con volumen (un relieve inflado, no una lámina plana): el de *La cometa*, el doblado por el viento de *La nevada* y el tronco de *La gallina ciega* (`GoyaOrigins`, se agrega solo). Tres de ellos están donde va una obra (uno por época, en las tres primeras), y otros seis están sueltos en las lomas, algunos espejados para que no se repitan iguales.

Al armar una obra que tiene árbol, el árbol se deshace en partículas de sus propios colores, que giran en un remolino alrededor de la mano con cintas doradas y forman la pintura: el árbol se transforma en la obra. Al desarmarla vuelve a ser el árbol. Las demás obras nacen del suelo, con color de tierra.

- Los recortes están en `Assets/Resources/Paisaje` (se generan con `tools/paisaje/recortar.py`). En el componente Goya Origins, `Props` son los árboles que van con una obra y `Trees` los sueltos: se puede cambiar el tamaño, la época, el lugar, el grosor y el espejado de cada uno.
- Ajustes en Goya Particles, sección Remolino: Vortex (prender/apagar), Vortex Size, Vortex Speed, Toward Hand, Vortex Ribbons, Streak.
- Las manos ya no se ven como esferas de colores: `GoyaHands` las dibuja como puntitos dorados, con cintas que giran cuando la mano está abierta y levantada.
- La música de fondo en loop se sacó. La animación de entrar a las obras (`GoyaPortal`) está apagada: se prende con Portals en ese componente.

## La cueva (mitad de la zona de la guerra)

Desde la mitad de la zona roja hasta Saturno el camino entra en una cueva (`GoyaCave.cs`, se arma sola en modo camino). La cueva está adentro de una montaña grande de roca ocre (los colores de *Ciudad sobre una roca*), con salientes y grietas: el camino llega por un valle a una boca grande con una pared de roca alrededor, que se angosta hacia adentro. Del otro lado se sale por otra boca hacia Saturno. Las paredes tienen el fondo ocre de *El perro* (`Assets/Resources/Paisaje/cueva_pared.jpg`, sale de `tools/paisaje/texturas.py`).

- En la entrada hay una antorcha apagada apoyada en una piedra, con un cartel. Con el mismo gesto que arma una obra (los dos brazos arriba con las manos abiertas, o **E** en la PC) cerca de la antorcha, se enciende y queda en la mano (si no hay manos, abajo a la derecha de la vista).
- Adentro está oscuro: las obras ya están colgadas en las paredes y solo se ven donde llega la luz de la antorcha. No se arman ni se desarman con gestos.
- Las Pinturas Negras (zona 4) no están al principio: aparecen de a poco en la pared cuando se las alumbra de cerca (`appearDistance`, `appearTime`).
- Al salir de la cueva está Saturno.
- La antorcha alumbra poco (unos 3 m), así que hay que acercarse a cada obra.
- Se ajusta en el componente `GoyaCave`: dónde empieza (`startZone`, `startAt`), ancho y alto, tamaño de la boca (`mouthScale`, `flareLength`), alto y ancho de la montaña (`mountainHeight`, `mountainWidth`), alcance y color de la luz (`torchRange`, `torchColor`), y cuánta luz hay sin antorcha (`ambient`).

## Sonido

`GoyaSound` (se agrega solo) arma un campo de sonido envolvente que cambia con cada zona del camino:

| Zona | Ambiente alrededor | Sonidos sueltos |
|---|---|---|
| Cartones | viento y hojas | pájaros arriba |
| La corte | murmullo de gente en un salón | clave desde otro salón, copas |
| Brujas | viento nocturno que aúlla, grillos | susurros muy cerca, búho |
| La guerra | retumbe grave | cañones y disparos lejos en el horizonte, tambores |
| Pinturas negras | drone grave y una respiración | gemidos, latido pegado a vos, Saturno masticando cerca |

- El ambiente sale de 4 fuentes alrededor tuyo (adelante, atrás y costados) que caminan con vos pero no giran con la cabeza: al girar, el sonido queda en su lugar. El cambio de zona se nota en pocos pasos (`Zone Blend`), por ejemplo al entrar a la zona roja.
- En la cueva el ambiente de afuera baja (`Inside Cave`) y suenan eco, gotas y la antorcha encendida.
- Pasos en la tierra cada 0,7 m que caminás, y el fuego cuando se quema el cabrío.
- Durante la introducción el ambiente suena más bajo.
- Los sonidos están sintetizados (`tools/sonidos/generar.py`) en `Assets/Resources/Sonidos`. Para usar grabaciones, reemplazar el `.ogg` por otro con el mismo nombre.
- Volúmenes en el componente Goya Sound (aparece en Play sobre el objeto de la galería): Master, Ambience, Events, Steps, Fire, Intro Ambience.

## Probar en la PC (sin tracker ni celular)

| Tecla / mouse | Qué hace |
|---|---|
| **Espacio** | Que el cabrío empiece a explicar (en lugar de levantar la mano) |
| **I** | Prender fuego al cabrío sin el gesto |
| **Clic derecho + mover el mouse** | Mirar alrededor |
| **W / S** o **flechas ↑ / ↓** | Caminar hacia adelante (hacia donde mirás) / hacia atrás |
| **Flechas ← / →** | Girar |
| **E** o **T** | Armar la obra que estás mirando, o desarmarla si ya está armada |
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
| Path Mode, Zone Length, Corridor Half Width, Path Move Speed, Gesture Distance | Goya Gallery | El camino: largo de cada zona, ancho por donde se camina, velocidad y hasta qué distancia alcanzan los gestos |
| Zones (Sky, Fog, Fog Density, Tint), Sky Top, Sky Horizon, Cloud Light, Cloud Gold, Cloud Amount, Far Mountains, Near Hills, Lake, Earth, Grass | Goya Environment | Colores del paisaje y de cada zona. Los colores del paisaje se aplican al empezar; los de las zonas se pueden ajustar con Play |
| Visible Distance | Goya Particles | El polvo de las obras más lejanas que esto no se dibuja (bajarlo si el celular anda lento) |
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

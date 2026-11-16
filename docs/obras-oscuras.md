# Galería 360: de lo cotidiano a lo oscuro

Rama: `obras-oscuras` (sale de `cardboard-xr`).

## Cómo funciona

- **Bienvenida:** el macho cabrío flota adelante con la frase "Levantá una mano para empezar" (*Start Line*), así nadie se pierde las reglas mientras se pone el visor. Al levantar una mano, arriba de él aparecen, una por una, frases cortas con las reglas (campo *Lines* de **Goya Intro**, se pueden editar en el Inspector). Hasta que no termina de explicar no se lo puede deshacer (*Wait For Rules*). Se le puede poner voz a cada frase en *Narration*.
- **El cabrío arde:** con un brazo levantado y la mano abierta, se prende fuego, se carboniza y se dispersa en brasas.
- **Un camino para recorrer** (*Path Mode* en **Goya Gallery**): afuera hay una obra por etapa, elegida al azar en cada recorrido, colgada de un árbol al costado del camino. El camino tiene cinco zonas, una por época y en orden: cartones para tapices, la corte, brujas y disparates, la guerra y las Pinturas Negras (*Zone Length*, 16 m cada una). Caminar hacia adelante es adentrarse en lo oscuro.
- **Un paisaje como el fondo de La gallina ciega** (**Goya Environment**, se agrega solo): cielo azul con nubes doradas, montañas azuladas a lo lejos, cerros, un lago plateado alrededor y tierra ocre con pasto oscuro por donde va el camino. Todo se genera al empezar, sin imágenes. Zona por zona el paisaje se tiñe (*Zones → Tint*): dorado en la corte, noche azul en las brujas, rojizo en la guerra y casi negro en las Pinturas Negras. Al pasar de una zona a otra cambia de a poco, y lo lejano se pierde en la niebla. Las paredes hechas con pinturas de la versión anterior se pueden volver a prender con *Walls* y *Ground*.
- **Las obras empiezan invisibles** (no hay polvo ni partículas flotando en el aire; para volver a verlo, subir Air Alpha y prender Dust en Goya Ambience). Mirando hacia donde está una obra, con los dos brazos levantados y las manos abiertas se arma. Con los dos puños cerrados se desarma en un remolino de brasas y vuelve a ser polvo, para armarla otra vez si se quiere.
- **Todo se apaga al avanzar:** cuanto más adentro del camino, más oscuros se tiñen los cuadros, las auras pasan de dorado a rojo y el sonido ambiente se pone más grave.
- Si se apaga *Path Mode*, vuelve la galería de 180° con una obra a la vez (*One At A Time*, *Arc Degrees*, *Arc Slots*).

## Las rocas de la cueva (simulación física, apagadas)

**Están apagadas** (en **Goya Cave**, *Falling Rocks*). Cuando se prenden: adentro de la cueva, con la antorcha prendida, se desprenden rocas del techo. Están hechas con la **física de Unity (PhysX)**: cada roca es un cuerpo rígido con masa (según su tamaño), gravedad, rebote y rozamiento, y choca con el piso, las paredes, las otras rocas y con vos (una cápsula invisible que te sigue). Nada está animado:

- Primero se oye un retumbe y cae polvo del techo (el aviso). Después se sueltan de 1 a 4 rocas juntas, que se chocan entre sí mientras caen.
- Si una roca pega contra el piso rápido (más de 3,5 m/s), **se parte** en 3 a 6 pedazos que salen despedidos con la energía que traía, con una nube de polvo.
- Los pedazos quedan como escombros: ruedan, se amontonan y **los empujás al caminar**. Los más viejos se hunden en el piso para que no se acumulen de más (*Max Rubble*).
- Las rocas se desprenden donde vas a estar (adelante, más lejos si corrés). Para esquivarlas hay que frenar, correr o ir hacia un costado.
- **Si una roca te pega, la llama de la antorcha se achica** (ilumina menos y más cerca). Con 3 golpes se apaga: levantando los dos brazos se vuelve a prender, pero más chica.
- Se ajusta en **Goya Cave**, sección *Rocas que caen*: cada cuánto, cuántas por derrumbe, tamaño, a qué velocidad se parten, cuántos pedazos, el rebote y cuánto achica la llama cada golpe.

## El reloj (apagado por ahora)

`GoyaGame` tiene un reloj para encontrar la mayor cantidad de obras en 5 minutos, con el contador de obras arriba de la vista. **Está apagado**: para prenderlo, en **Goya Game** marcar *Timer* (se agrega solo al darle Play; para cambiarlo hay que hacerlo en Play o cambiar el valor por defecto en el script). Con el reloj prendido, cada roca que te pega además resta 15 segundos.

## Sistema complejo: la bandada de murciélagos

Los murciélagos y búhos de *El sueño de la razón produce monstruos* vuelan en bandada (`GoyaFlock`, se agrega solo), **lejos, en el cielo**: dan vueltas a unos 24 m al costado del camino y 9 m de alto, y te acompañan de lejos mientras caminás. No se cruzan en el camino ni tapan las obras.

Es una simulación de **flocking** (los *boids* de Craig Reynolds, 1987): no hay ninguna animación de la bandada; cada murciélago tiene posición, velocidad y miedo, mira solo a sus vecinos cercanos (0,8 m) y sigue reglas en cada cuadro:

1. **Separación**: se aleja de los que están demasiado cerca.
2. **Alineación**: copia la dirección promedio de sus vecinos.
3. **Cohesión**: va hacia el centro de sus vecinos.
4. **Ronda**: gira alrededor de un punto en el cielo (hay dos rondas que avanzan con vos).
5. **Miedo y contagio**: si algo los asusta, el miedo pasa a sus vecinos un poco más débil (82 %) y se propaga como una ola.

Lo que se ve (remolinos, la bandada que se estira, se divide y se reagrupa) **emerge** de esas reglas. Sonido 3D: el aleteo sale del centro de la bandada.

Se ajusta en **Goya Flock**: cantidad (160), `Far Distance`, `Far Height` y `Far Size` (dónde y qué tan grande es la ronda). Apagando `Far Away` vuelve el modo anterior, donde la bandada anidaba delante de las obras ocultas, las tapaba y había que espantarla con la mano abierta para poder armarlas.

## Gestos

| Gesto | Qué hace |
|---|---|
| **Levantar una mano** al principio | El cabrío empieza a explicar las reglas |
| **Un brazo levantado con la mano abierta** (sostenerlo un momento), cuando el cabrío terminó de hablar (si es la misma mano del principio, hay que bajarla antes) | El cabrío se prende fuego y aparece la galería |
| **Los dos brazos levantados con las manos abiertas**, mirando una obra | Se arma esa obra |
| **Los dos puños cerrados** (a cualquier altura), mirando una obra | Se desarma en brasas y vuelve a quedar invisible |
| **Un dedo levantado** (cualquier mano) | Caminás hacia adelante, hacia donde mirás |
| **Un dedo levantado en cada mano** | Caminás hacia atrás |
| **Dos dedos levantados** (la V, en una mano) | Corrés hacia donde mirás (en la PC: Shift + ↑) |
| **Tres dedos levantados en una mano** (en la cueva) | Levantás el cuadro tirado más cercano (en la PC: G) |
| **Una V en cada mano** (en la cueva, llevando un cuadro) | Lo colgás en su marca de la pared, si estás cerca (en la PC: F) |
| **Los dos puños cerrados** (en la cueva, junto a un cuadro que colgaste) | Se deshace en partículas que vienen hacia vos: lo coleccionás (en la PC: C) |
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

## Las obras se forman juntando polvo

**Ahora no hay polvo flotando** (`Loose Dust`, apagado): las motas sueltas son invisibles. Aparecen recién cuando la mano las junta, y se ven volar de la mano a la tela, donde nace el caminante. Prendiendo `Loose Dust` vuelve la nube de motas.

Inspirado en *Dust* de Robert Hodgin (motas de polvo iluminadas que se arremolinan cuando la mano mueve el aire). Cada obra oculta es una **nube de motas de luz** que flota donde va a estar la obra (`GoyaParticles`, *Gather Dust*):

- Cada mota es una partícula con velocidad. Sueltas, las mecen corrientes suaves y vuelven despacio a su lugar en la nube. Titilan, y algunas son grandes y borrosas, como fuera de foco.
- **La mano mueve el aire**: señalando la nube (o con E en la PC), el polvo cercano se arremolina y va hacia la mano. Cuanto más rápido movés la mano, más aire.
- Las motas que quedan muy cerca de la mano **se atrapan**: un resorte las lleva a su lugar en la obra y de a poco toman el color de la pintura. Así vas acomodando el polvo y la obra aparece por partes, donde pasaste la mano.
- Cada mota que llega a la tela se apaga y da **una pincelada nueva** (un caminante, ver abajo). Las pinceladas se reparten por toda la tela, donde falta pintar, así la obra aparece pareja y no como una mancha que crece desde la mano.
- Cuando juntaste el 70 % (*Form At*), el resto se junta solo y la obra queda formada (y viva, ver abajo).
- El gesto de los dos brazos arriba sigue armándola de golpe.
- Se ajusta en **Goya Particles**, sección *Polvo que se junta*: color y brillo de las motas, tamaño de la nube, fuerza y alcance del aire de la mano, distancia a la que se atrapan, el resorte y cuánto hay que juntar.

## Las obras se pintan sobre telas colgadas de los árboles (caminantes)

### Obras al azar

Afuera de la cueva hay una obra en cada etapa (1 cartones para tapices, 2 la corte, 3 brujas y disparates), **elegida al azar entre las de esa etapa** cada vez que empieza el recorrido (`Random Paintings` en **Goya Gallery**). Así cada recorrido es distinto. Si sale *El pelele* o *Vuelo de brujas*, se pintan animadas. Las de la cueva y las Pinturas Negras siguen siendo siempre las mismas.

Para elegir obras fijas, apagá `Random Paintings` y escribilas en `Only Paintings`. La bifurcación en tres caminos quedó apagada (`Fork At Start`). La caminata es más rápida (`Path Move Speed` 1,6).


Las obras de afuera son grandes (hasta 2,7 m, `Outside Scale` 1,7 en **Goya Gallery**) y están a unos 4 m del centro del camino (`Outside Distance`), así se ven enteras y con más detalle. Cada obra es una **tela colgada de la rama baja de un árbol** fractal que crece al costado, más o menos a la altura de la vista. La rama es parte del árbol: sale de donde termina el tronco, pasa justo detrás del borde de arriba de la tela y se mece con el árbol, y el borde de arriba de la tela sigue ese vaivén. La tela empieza en blanco (lino viejo con los bordes gastados) y tiene un borde sin pintar alrededor de la obra.

Cuando juntás el polvo con la mano, cada mota que llega a la tela se apaga y **nace un caminante** en una parte de la tela que todavía falta pintar, como en el video de Processing: una partícula con posición, velocidad y aceleración que toma el color de la pintura, deja una pincelada y se achica hasta morir. Los caminantes **siguen los contornos** de la pintura (van a lo largo de los bordes de las figuras, no los cruzan) y llevan su pintura, que mezclan de a poco con la del lugar; si cruzan a un color muy distinto, el trazo termina. Por eso dejan pinceladas largas que siguen las formas, como un pincel, y no manchas redondas.

**Definición:** la tela tiene 512 píxeles de alto y los caminantes leen la pintura en alta resolución. Pintan de lo grueso a lo fino: las primeras pinceladas son grandes y, a medida que la tela se cubre, nacen más chicas. Al terminar, lo pintado se afina hacia la pintura (se ven las pinceladas pero con definición) y después siguen naciendo caminantes chiquitos que afinan los detalles mientras la mirás (`Detail Walkers`).

Las rocas están apagadas (`Use Rocks`). La tela se mece con el aire y con la mano.

Código: `Assets/GoyaCanvas.cs` (la tela y los caminantes) y `GoyaParticles` (sección **Obras pintadas sobre telas y rocas**). El árbol con su rama lo planta `GoyaFractalTrees.Plant`.

| Valor | Qué hace |
|---|---|
| `Paint On Surfaces` | Apagado: vuelve a la obra de partículas flotando, sin tela |
| `Max Walkers` | Caminantes a la vez en cada obra (más = se pinta más rápido) |
| `Walker Size` | Tamaño de la pincelada al nacer |
| `Walker Shrink` | Cuánto se achica en cada paso (más = trazos más cortos) |
| `Walker Steps` | Pasos por cuadro (más = pinta más rápido) |
| `Finish After` | Segundos máximos para terminar de pintarse sola después de juntar el polvo |
| `Detail Walkers` | Caminantes chiquitos por cuadro que siguen afinando la obra terminada |
| `Cloth Margin` | Borde de tela sin pintar alrededor de la obra |
| `Use Rocks` | Una y una en roca en vez de tela |
| `Bokeh` | Parte de las motas grandes y borrosas (casi ninguna) |

Cada zona del camino mide 16 m (`Zone Length` en Goya Gallery).

## Las obras vivas (hechas de partículas)

Cuando una obra del camino se arma, **ya no vuelve a ser un cuadro plano**: queda hecha de sus partículas (`GoyaParticles`, *Living Paintings*). Flotan a la altura de la vista, sin marco, y se mezclan con el aire del lugar:

- **Volumen**: cada partícula tiene su lugar en la obra, y lo claro sale hacia adelante y lo oscuro se hunde (*Living Depth*). Al caminar alrededor se nota la profundidad.
- **La imagen la lleva una tela de partículas**: una grilla de puntos (*Sheet Resolution*, 56 a lo alto) con la pintura pegada encima, así se ve con toda su definición. Cada punto es una partícula unida a su lugar y a sus 4 vecinas por resortes (*Neighbor Spring*): por eso un empujón de la mano se propaga como una onda por la obra.
- **Sin marco**: el borde de la tela se vuelve transparente de forma irregular, y ahí siguen las partículas sueltas, más chicas, alejándose como humo (*Edge Dissolve*, *Edge Drift*, *Edge Particle Size*).
- **Simulación**: cada partícula es un punto con velocidad, atado a su lugar por un resorte con freno (*Spring*, *Damping*). Encima actúa un campo de corrientes de aire que cambia con el tiempo (*Flow*, más fuerte en los bordes y cuando las manos se agitan).
- **La mano las aparta**: donde la mano (o el puntero) toca la obra, las partículas se corren (*Hand Push*, *Hand Radius*). Cuando la mano se va, el resorte las devuelve y la obra ondula hasta quedarse quieta.
- **Las figuras quedan paradas en el paisaje**: con el recorte de cada obra (`Assets/Resources/Cortes`), el fondo de la pintura queda transparente (*Background Alpha*, 30 %) y las figuras enteras. Así no se ve un rectángulo flotando.
- Toman el color de la época (más oscura con el movimiento) y la niebla del lugar, igual que antes.
- Los dos puños la desarman como siempre. Las animadas (*El pelele*, *Vuelo de brujas*) quedan como cuadro (*Keep Flat*), porque sus capas van encima.
- Si el celular anda lento: bajar *Sheet Resolution*, *Particle Count* o *Living Distance* (las obras más lejanas que eso no se simulan).
- Para volver a las obras en relieve apoyadas en el piso (`GoyaRelief`): destildar *Living Paintings*.

## Árboles fractales

Los árboles sueltos del paisaje ya no son recortes de las pinturas: se generan por **recursión** (`GoyaFractalTrees`, se agrega solo), como el árbol de la Unidad 5:

1. Dibujar una rama.
2. Al final de la rama abrir 2 ramas (a veces 3) más cortas y más finas, giradas un ángulo.
3. Repetir hasta la profundidad elegida (5 a 7 generaciones) o hasta que las ramas sean muy cortas.

Es un **fractal estocástico**: las reglas son fijas, pero el ángulo, el largo y la cantidad de ramas tienen azar, así que no hay dos árboles iguales. Además las ramas se reparten alrededor de su madre cada 137,5° (como las hojas en las plantas), para que el árbol tenga volumen, y buscan la luz (hacia arriba) y el viento (hacia un costado).

Copian la paleta y el porte de los árboles que ya teníamos:

| Tipo | Como en | Reglas |
|---|---|---|
| Cometa | *La cometa* | Tronco claro y torcido, matas de hojas verde oliva |
| Gallina | *La gallina ciega* | Tronco grueso ocre, copa alta y rala, verde pálido |
| Nevada | *La nevada* | Pelado, gris, doblado por el viento, muchas ramitas finas |

- **Crecen cuando te acercás**: cada generación de ramas aparece después de la anterior, así se ve la recursión.
- **Se mecen**: cada rama suma el vaivén de su madre, por eso las puntas se mueven más que el tronco.
- **Las hojas son un sistema de partículas** (como en la Unidad 5: posición, velocidad, aceleración y vida). El viento a veces arranca una hoja, que cae con gravedad, rozamiento y vaivén, se desvanece y después vuelve a brotar en su rama. Con las manos agitadas sopla más fuerte.
- Son **todos** los árboles: los 6 sueltos de **Goya Origins**, los que estaban detrás de las obras (*La cometa*, *La gallina ciega*, *La nevada*), ahora fractales y un poco atrás, y 2 más al azar por zona (*Extra Per Zone*).
- Para volver a los árboles recortados: destildar *Fractal Trees* en **Goya Fractal Trees**.

## Pinturas animadas: El pelele y Vuelo de brujas

- Cuando *El pelele* está armado, las mujeres lo tiran con la manta: sale por arriba del cuadro dando vueltas, cae y la manta se hunde (`GoyaAnimated`). Capas generadas con `tools/paisaje/pelele.py`.
- Cuando *Vuelo de brujas* está armado, el grupo de brujas que lleva al hombre flota en el cielo: sube y baja despacio, se mece y gira (`GoyaVuelo`). Capas generadas con `tools/paisaje/vuelo.py` a partir del recorte de `Cortes`.
- Las capas están en `Assets/Resources/Animadas`. Los dos componentes se agregan solos y se ajustan en el Inspector (velocidad, altura, giro).
- **En tela o roca la animación la pintan los caminantes.** Las capas no se muestran: la animación corre igual, y cada caminante toma el color de cómo se ve la obra en ese momento (el fondo, la manta y la figura en su lugar actual). Cuando la obra ya está pintada, siguen naciendo caminantes rápidos de pinceladas cortas en la parte que se mueve, así que la figura se mueve pintada y deja una estela de pinceladas que los demás tapan enseguida. En **Goya Particles**: `Anim Walkers` (caminantes nuevos por cuadro; más = movimiento más nítido) y `Anim Brush` (tamaño de esas pinceladas).

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
- Al salir de la cueva aparece un cartel: con el mismo gesto (o **E**) se suelta la antorcha, que cae al piso y se apaga de a poco. Ahí está Saturno.
- La antorcha alumbra poco (unos 3 m), así que hay que acercarse a cada obra.
- **Los cuadros se caen** (`fallingPaintings`): al encender la antorcha tiembla todo, no se puede caminar unos segundos, la cueva se ilumina un poco y los cuadros tiemblan y se caen al piso uno tras otro (`fallInterval`). En la pared queda un rectángulo más oscuro donde estaba cada uno. Con tres dedos de una mano (o **G**) se levanta el cuadro tirado más cercano (`pickDistance`) y se lleva delante de la vista, más chico (`carryScale`). Con una V en cada mano (o **F**) cerca de su marca (`hangDistance`) vuelve a colgarse; mientras se lleva un cuadro, su lugar en la pared brilla al acercarse. Las Pinturas Negras tiradas solo se ven, y se pueden levantar, cuando se las alumbra. Una vez colgado, con los dos puños cerrados (o **C**) el cuadro se deshace en partículas con sus colores, de arriba hacia abajo, que vuelan hacia la mano: queda coleccionado y la pared vacía (`collectParticles`, `collectTime`). Junto a un cuadro colgado, los dos puños no te hacen caminar hacia atrás. En el juego, las obras de la cueva cuentan como encontradas al coleccionarlas.
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
| **Z** / **X** (o **Av Pág** / **Re Pág**) | Mirar para abajo / para arriba (sin celular). **R** vuelve la vista al frente |
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

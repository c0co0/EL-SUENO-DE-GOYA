# Galería 360: de lo cotidiano a lo oscuro

Rama: `obras-oscuras` (sale de `cardboard-xr`).

## Cómo funciona

- **Anillo de 360°:** las obras se reparten alrededor de la persona (8 por defecto, *Ring Count*). La primera queda adelante y las siguientes alternan derecha e izquierda, así las primeras de la lista quedan más cerca de la vista inicial.
- **Una sola lista ordenada** (*Painting Sequence*, en el objeto **Galeria**): las primeras 8 llenan el anillo. Cada vez que una obra se desarma en partículas, se rearma como **la siguiente de la lista que todavía no apareció**. Así el anillo pasa de a poco de la vida cotidiana a las Pinturas Negras.
- **El movimiento acelera la caída:** cuanto más se mueven las manos, sube la oscuridad. Con las manos muy agitadas puede saltear una obra de la lista (*Max Skip*), los cuadros se desarman con menos toque y, si la agitación sigue, se desarman solos.
- **Todo se apaga con el recorrido:** a medida que avanza la lista (o con mucho movimiento), los cuadros se tiñen más oscuros, las auras pasan de dorado a rojo, el polvo se vuelve gris y el sonido ambiente se pone más grave.
- **Cuando se termina la lista,** los cuadros siguen rotando entre las últimas (las más oscuras).

## Gestos

| Gesto | Qué hace |
|---|---|
| Apuntar a la cabra con el **puño cerrado** y después **abrir la mano** sobre ella (un segundo) | Se deshace la cabra y aparecen las obras |
| Pasar la mano (cualquier forma) por una obra | Se revela |
| **Mano abierta** sobre una obra revelada | Se desarma y vuelve como la siguiente |
| **Un puño cerrado** sostenido | Caminar hacia donde mirás |
| **Dos puños cerrados** | Caminar hacia atrás |

Las manos y el puntero giran con la cabeza, así se pueden tocar las obras de atrás. No se puede salir del anillo ni acercarse a menos de 70 cm de las obras (*Min Distance To Paintings*).

## Cómo ordenar las obras

1. Crear la carpeta `Assets/Obras` y arrastrar ahí todas las imágenes.
2. Para que sea fácil, ponerles un número adelante del nombre según el orden: `01 El quitasol.jpg`, `02 La vendimia.jpg`, etc. Unity las muestra ordenadas por nombre.
3. Seleccionarlas todas en la ventana Project → Inspector → **Texture Type: Sprite (2D and UI)** → **Apply**. (Read/Write no hace falta.)
4. Seleccionar **Galeria** en la Hierarchy → componente **Goya Gallery** → **Painting Sequence**. Con la lista **cerrada** (flechita a la izquierda del nombre), seleccionar todas las imágenes en Project (clic en la primera, Shift + clic en la última) y arrastrarlas juntas sobre el nombre *Painting Sequence*. Se cargan en el orden del nombre.
5. Para cambiar el orden después, arrastrar cada elemento de la lista desde las dos rayitas `=` que tiene a la izquierda.
6. Guardar la escena (Ctrl+S) y hacer commit y push.

Si *Painting Sequence* queda vacía, la galería sigue como antes (los 3 cuadros de adelante).

## Orden sugerido (todas son de dominio público)

**Vida cotidiana** (cartones para tapices, 1777–1792). Estas 8 llenan el anillo al empezar:
1. *El quitasol*
2. *La vendimia*
3. *Las floreras*
4. *La gallina ciega*
5. *El cacharrero*
6. *Niños inflando una vejiga*
7. *La pradera de San Isidro*
8. *El pelele* (ya tiene algo inquietante: la muñeca manteada)

**Lo extraño** (1790s):

9. *Corral de locos*
10. *El sueño de la razón produce monstruos* (Capricho 43)
11. *El aquelarre* (1798, Museo Lázaro Galdiano)
12. *El vuelo de las brujas* (ya está en el proyecto)

**La guerra:**

13. *El 2 de mayo de 1808 en Madrid*
14. *El 3 de mayo de 1808 en Madrid*

**Pinturas Negras** (1819–1823):

15. *Dos viejos comiendo sopa*
16. *Duelo a garrotazos*
17. *El perro semihundido*
18. *Átropos (Las Parcas)*
19. *Una tercera de las Pinturas negras* (ya está en el proyecto)
20. *Saturno devorando a su hijo* (ya está en el proyecto, el final)

Se pueden bajar de Wikimedia Commons o del sitio del Museo del Prado. Con unas 20 obras, el recorrido completo lleva unas 12 desarmadas.

## Probar en el editor

- **I**: saltear la intro de la cabra.
- **Clic derecho + mouse**: mirar alrededor. **W / S**: caminar adelante / atrás.
- **T**: desarmar todos los cuadros revelados (para revelarlos hace falta el tracker de Python).
- **O**: sumar oscuridad (+25 %).
- **Backspace**: volver todo al principio.

## Valores para ajustar

| Valor | Componente | Qué hace |
|---|---|---|
| Ring Count | Goya Gallery | Cuántas obras hay en el anillo |
| Distance, Painting Max Size | Goya Gallery | Radio del anillo y tamaño de las obras |
| Max Skip | Goya Gallery | Cuántas obras puede saltear si las manos se mueven mucho (0 = nunca) |
| Move Speed, Fist Hold Time | Goya Gallery | Velocidad al caminar y cuánto hay que sostener el puño |
| Movement For Full Dark, Dark Tint | Goya Gallery | Cuánto movimiento oscurece del todo y a qué color |
| Touch Hold Time, Require Fist First | Goya Intro | Cuánto hay que sostener la mano abierta sobre la cabra, y si hace falta el puño antes |
| Auto Break Agitation | Goya Particles | Más alto = cuesta más que se desarmen solos (1 = nunca) |
| Particle Count | Goya Particles | Bajarlo (por ejemplo a 4000) si el celular anda lento con 8 obras |

## Lo que no se pudo probar

Se escribió sin poder abrir Unity. Si algo falla, mandar captura del primer error rojo de la Console.

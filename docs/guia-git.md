# Guía: trabajar con el repositorio de GitHub

Repo: https://github.com/c0co0/EL-SUENO-DE-GOYA

La idea: GitHub guarda la versión compartida del proyecto. Cada una tiene una copia en su compu (un **clon**). Cuando terminás de trabajar, **subís** tus cambios (*commit* + *push*). Antes de empezar, **traés** los cambios de la otra (*pull*).

---

## Parte 1 · Pasar el proyecto de Pilar al repositorio (se hace una sola vez)

### 1. Darle acceso a Pilar (lo hace Jose)
1. En https://github.com/c0co0/EL-SUENO-DE-GOYA → **Settings** → **Collaborators** → **Add people**.
2. Escribir el usuario de GitHub de Pilar y enviar la invitación.
3. Pilar acepta la invitación desde el mail que le llega (o desde https://github.com/notifications).

Si Pilar no tiene cuenta de GitHub, la crea gratis en https://github.com/signup.

### 2. Guardar lo que tiene ahora
1. En Unity: guardar la escena (**Ctrl+S**) y **File → Save Project**. Cerrar Unity.
2. **No borrar** la carpeta vieja del proyecto: queda como respaldo hasta confirmar que todo anda.

### 3. Clonar el repo (elegir A o B)

**A · Con GitHub Desktop**
1. Instalar desde https://desktop.github.com e iniciar sesión con su cuenta de GitHub.
2. **File → Clone repository** → pestaña **GitHub.com** → elegir `c0co0/EL-SUENO-DE-GOYA`.
3. En *Local path* poner `C:\Proyectos` → **Clone**.

**B · Sin GitHub Desktop (consola)**
1. Instalar Git desde https://git-scm.com/download/win (opciones por defecto).
2. Abrir PowerShell y escribir, una línea por vez:
   ```
   cd C:\
   mkdir Proyectos
   cd Proyectos
   git clone https://github.com/c0co0/EL-SUENO-DE-GOYA.git
   cd EL-SUENO-DE-GOYA
   ```

Queda el proyecto en `C:\Proyectos\EL-SUENO-DE-GOYA`.

### 3b. Decirle a Git quién sos (una sola vez por computadora)
Sin esto, `git commit` falla con *"Author identity unknown"* y el `push` dice *"Everything up-to-date"* sin subir nada. Para `pull` no hace falta, pero conviene hacerlo de entrada.

En PowerShell, con **tus datos reales** (no copiar los ejemplos tal cual):
```
git config --global user.name "Pilar Gargiulo"
git config --global user.email "el-mail-de-tu-cuenta-de-github"
```
- El repositorio es público: ese mail queda visible en el historial. Si preferís ocultarlo, usá el que aparece en GitHub → **Settings → Emails** (`...@users.noreply.github.com`).
- Si te equivocaste, volvé a correr los dos comandos: el valor nuevo pisa al anterior.
- Con GitHub Desktop se configura en **File → Options → Git**.

**Importante para Unity:** la rama `cardboard-xr` baja el plugin de Cardboard con Git. Si instalaste Git con Unity Hub abierto, cerrá Unity Hub del todo (también el ícono al lado del reloj) o reiniciá la PC; si no, Unity dice *"No 'git' executable was found"*.

### 4. Pasar los cambios que Pilar hizo después de mandar el zip
El repo tiene el zip que mandó Pilar hoy más los arreglos del visor (`StereoGyroRig.cs`). Si después de eso Pilar cambió algo en su carpeta vieja (por ejemplo, valores en el Inspector o un script), hay que copiar esos archivos a la carpeta nueva:

- Scripts: `Assets\*.cs` (¡**no** copiar `StereoGyroRig.cs`, el del repo es el más nuevo!).
- Escena: `Assets\Scenes\SampleScene.unity` (ahí se guardan los valores del Inspector).
- Cualquier archivo nuevo que haya agregado (imágenes, sonidos, modelos), **junto con su `.meta`**.

Si no cambió nada desde que mandó el zip, saltear este paso.

### 5. Abrir el proyecto nuevo en Unity
1. Unity Hub → **Projects** → **Add** → **Add project from disk** → `C:\Proyectos\EL-SUENO-DE-GOYA`.
2. La primera vez tarda (Unity arma la carpeta `Library`).
3. Abrir `Assets/Scenes/SampleScene` y probar con Play.
4. Si copió cambios en el paso 4, subirlos (ver Parte 2, "Al terminar").

Desde ahora Pilar trabaja **siempre** en `C:\Proyectos\EL-SUENO-DE-GOYA`, no en la carpeta vieja.

---

## Parte 2 · El día a día

### Antes de empezar a trabajar: traer lo último (*pull*)
Conviene hacerlo con Unity cerrado (o al menos con todo guardado).

- **GitHub Desktop:** botón **Fetch origin** → si aparece **Pull origin**, tocarlo.
- **Consola:**
  ```
  cd C:\Proyectos\EL-SUENO-DE-GOYA
  git pull
  ```

Después abrir Unity: toma los cambios solo.

### Al terminar: subir tus cambios (*commit* + *push*)
1. En Unity: **Ctrl+S** (escena) y **File → Save Project**.
2. Subir:
   - **GitHub Desktop:** a la izquierda aparecen los archivos cambiados. Abajo, en *Summary*, escribir qué cambiaste (ej. "agrego sonido 3D a los cuadros") → **Commit to main** → **Push origin**.
   - **Consola:**
     ```
     cd C:\Proyectos\EL-SUENO-DE-GOYA
     git add .
     git commit -m "qué cambié"
     git push
     ```
3. Avisarle a la otra que subiste algo, así hace *pull*.

### Si el push no te deja ("rejected" / "fetch first")
Significa que la otra subió algo mientras trabajabas. Primero traer, después subir:
- **GitHub Desktop:** **Fetch origin** → **Pull origin** → **Push origin**.
- **Consola:** `git pull` y después `git push`.

### Si aparece un conflicto
Pasa cuando las dos cambiaron **el mismo archivo**. El que más problemas da es la escena (`SampleScene.unity`).
- Para evitarlo: **no editen la escena las dos al mismo tiempo**. Avísense quién la está tocando. Los scripts `.cs` distintos se pueden editar en paralelo sin problema.
- Si igual pasa con la escena, lo más simple es quedarse con una de las dos versiones:
  - **GitHub Desktop:** en el archivo en conflicto, clic derecho → *Use the version from main* (la de GitHub) o *Use the modified file* (la tuya) → **Commit merge** → **Push origin**.
  - **Consola:** `git checkout --theirs Assets/Scenes/SampleScene.unity` (la de GitHub) o `--ours` (la tuya), después `git add .`, `git commit -m "resuelvo conflicto"` y `git push`.
  - Después rehacer a mano los cambios de la versión que se descartó.

---

## Resumen rápido

| Momento | GitHub Desktop | Consola |
|---|---|---|
| Empezar | Fetch origin → Pull origin | `git pull` |
| Terminar | Summary → Commit to main → Push origin | `git add .` · `git commit -m "..."` · `git push` |
| No me deja subir | Pull origin → Push origin | `git pull` · `git push` |

Regla de oro: **pull al empezar, push al terminar, y la escena de a una.**

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sistema complejo del sueño: una bandada de murcielagos (los de "El sueño de la razon produce monstruos")
/// simulada con reglas de flocking (Reynolds, "boids"). Cada murcielago solo conoce a sus vecinos cercanos y sigue
/// reglas simples; el comportamiento de la bandada (remolinos, olas de panico, reagruparse) no esta animado,
/// sale de la suma de esas reglas.
///
/// Reglas de cada murcielago, por cuadro:
///   1. Separacion: se aleja de los vecinos que estan demasiado cerca.
///   2. Alineacion: copia la direccion promedio de sus vecinos.
///   3. Cohesion: va hacia el centro de sus vecinos.
///   4. Nido: vuela alrededor de una obra todavia oculta y la tapa (cada obra oculta cercana es un nido;
///      los murcielagos se reparten entre los nidos segun cuantos hay en cada uno).
///   5. Miedo: huye de la mano que barre la obra, de las palmas y del fuego de la antorcha.
///   6. Contagio: el miedo de un murcielago pasa a sus vecinos (un poco mas debil), asi el susto se propaga
///      como una ola por la bandada. El miedo se calma solo con el tiempo y entonces vuelven al nido.
///
/// Juego: mientras una obra esta tapada por la bandada no se puede armar. Hay que espantarlos con la mano abierta
/// (barriendo la obra) y armarla (dos brazos arriba) antes de que vuelvan. Si se hace el gesto de armar con la obra
/// tapada, los murcielagos se asustan pero la obra no se arma.
/// En la PC: mantener E sobre la obra los espanta. Se agrega solo y funciona en modo camino.
/// </summary>
public class GoyaFlock : MonoBehaviour
{
    [Header("Bandada")]
    public bool flock = true;
    [Tooltip("Cantidad de murcielagos. Si el celular anda lento, bajarlo")]
    public int count = 160;
    [Tooltip("Envergadura de las alas (metros)")]
    public float wingspan = 0.22f;
    public Color color = new Color(0.07f, 0.055f, 0.06f, 1f);

    [Header("Reglas (pesos)")]
    [Tooltip("Hasta donde ve a sus vecinos (metros)")]
    public float neighborRadius = 0.8f;
    [Tooltip("Distancia minima a otro murcielago")]
    public float separationRadius = 0.3f;
    public float separation = 3.5f;
    public float alignment = 1.0f;
    public float cohesion = 0.8f;
    [Tooltip("Fuerza con la que vuelve a su obra")]
    public float roost = 2.2f;
    [Tooltip("Fuerza con la que gira alrededor de la obra")]
    public float orbit = 1.2f;
    public float flee = 9f;
    public float minSpeed = 0.7f;
    public float maxSpeed = 1.6f;
    [Tooltip("Velocidad maxima con todo el miedo")]
    public float panicSpeed = 4.2f;
    public float maxForce = 6f;

    [Header("Miedo")]
    [Tooltip("Radio del susto de la mano que barre una obra (metros)")]
    public float handRadius = 1.1f;
    [Tooltip("Radio del susto del fuego de la antorcha")]
    public float torchRadius = 2.4f;
    [Tooltip("Cuanto del miedo de un vecino se contagia (0 = nada, 1 = todo)")]
    [Range(0f, 1f)] public float contagion = 0.82f;
    [Tooltip("Velocidad del contagio (por segundo)")]
    public float contagionSpeed = 5f;
    [Tooltip("Cuanto miedo se pierde por segundo (cuando baja, vuelven a la obra)")]
    public float calm = 0.2f;

    [Header("Nidos (obras ocultas)")]
    [Tooltip("Obras ocultas a menos de esta distancia se vuelven nidos")]
    public float roostRange = 15f;
    [Tooltip("Cuantas obras ocultas tapa la bandada a la vez")]
    public int maxRoosts = 3;
    [Tooltip("Murcielagos tranquilos delante de la obra que alcanzan para taparla")]
    public int coverMin = 8;

    public static GoyaFlock Instance { get; private set; }

    class Roost
    {
        public GoyaGallery.Painting p;
        public Vector3 center, normal, right, up;
        public int members, covering;
    }

    GoyaGallery gallery;
    Vector3[] pos, vel, prevVel;
    float[] panic, prevPanic, flap, flapRate, homeAngle, homeRadius, homeSpeed;
    int[] roostOf;
    readonly List<Roost> roosts = new List<Roost>();
    readonly List<Vector4> scares = new List<Vector4>();   // xyz = lugar, w = radio; la fuerza va en scareForce
    readonly List<float> scareForce = new List<float>();
    float roostTimer;
    bool built;

    Mesh mesh;
    Vector3[] verts;
    Color[] colors;
    MeshRenderer meshRenderer;
    AudioSource flutter;
    AudioClip[] screeches;
    float screechWait;
    GoyaEnvironment environment;

    // Celdas para buscar vecinos rapido (asi no compara cada murcielago con todos)
    readonly Dictionary<long, List<int>> grid = new Dictionary<long, List<int>>();
    readonly Stack<List<int>> spare = new Stack<List<int>>();

    const int VertsPerBat = 14;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Auto()
    {
        var g = FindFirstObjectByType<GoyaGallery>();
        if (g != null && g.GetComponent<GoyaFlock>() == null) g.gameObject.AddComponent<GoyaFlock>();
    }

    void Awake() { Instance = this; }

    /// <summary>Cuantos murcielagos tranquilos hay delante de la obra (0 si no es un nido).</summary>
    public int Cover(GoyaGallery.Painting p)
    {
        if (!flock || !built) return 0;
        foreach (var r in roosts) if (r.p == p) return r.covering;
        return 0;
    }

    /// <summary>La bandada tapa la obra: no se puede armar.</summary>
    public bool Covered(GoyaGallery.Painting p) { return Cover(p) >= coverMin; }

    /// <summary>Un susto de golpe (por ejemplo, el gesto de armar con la obra tapada).</summary>
    public void Scare(Vector3 at, float radius, float amount)
    {
        if (!built) return;
        for (int i = 0; i < pos.Length; i++)
        {
            float d = Vector3.Distance(pos[i], at);
            if (d < radius) panic[i] = Mathf.Clamp01(panic[i] + amount * (1f - d / radius));
        }
        Screech(at);
    }

    void Build()
    {
        int n = Mathf.Max(1, count);
        pos = new Vector3[n]; vel = new Vector3[n]; prevVel = new Vector3[n];
        panic = new float[n]; prevPanic = new float[n];
        flap = new float[n]; flapRate = new float[n];
        homeAngle = new float[n]; homeRadius = new float[n]; homeSpeed = new float[n];
        roostOf = new int[n];
        Vector3 start = gallery.RigPosition + Vector3.up * 3f + Vector3.forward * 6f;
        for (int i = 0; i < n; i++)
        {
            pos[i] = start + Random.insideUnitSphere * 2.5f;
            vel[i] = Random.onUnitSphere * minSpeed;
            flap[i] = Random.value * 10f;
            flapRate[i] = Random.Range(9f, 14f);
            homeAngle[i] = Random.value * Mathf.PI * 2f;
            homeRadius[i] = Mathf.Sqrt(Random.value);          // repartidos parejo sobre la obra
            homeSpeed[i] = Random.Range(0.4f, 0.9f) * (Random.value < 0.5f ? -1f : 1f);
            roostOf[i] = -1;
        }

        var go = new GameObject("Bandada");
        go.transform.SetParent(transform, false);
        go.transform.position = Vector3.zero;   // los vertices van en coordenadas del mundo
        mesh = new Mesh { name = "Murcielagos" };
        mesh.MarkDynamic();
        verts = new Vector3[n * VertsPerBat];
        colors = new Color[verts.Length];
        var tris = new int[n * 8 * 3];
        // Cuerpo (0-3): cabeza, ala izq, cola, ala der. Cada ala (4 a 8 y 9 a 13): hombro adelante, hombro atras,
        // codo, punta y la muesca del borde de atras.
        int[] one = { 0, 1, 2, 0, 2, 3, 4, 6, 8, 4, 8, 5, 6, 7, 8, 9, 11, 13, 9, 13, 10, 11, 12, 13 };
        for (int b = 0; b < n; b++)
            for (int k = 0; k < one.Length; k++) tris[b * one.Length + k] = b * VertsPerBat + one[k];
        mesh.vertices = verts;
        mesh.colors = colors;
        mesh.triangles = tris;
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100000f);
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        meshRenderer = go.AddComponent<MeshRenderer>();
        var shader = Shader.Find("Goya/Terreno");
        var mat = new Material(shader != null ? shader : Shader.Find("Sprites/Default"));
        mat.mainTexture = Texture2D.whiteTexture;
        meshRenderer.sharedMaterial = mat;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;

        // Sonido 3D: el aleteo sale del centro de la bandada; los chillidos, de donde se asustan
        var clip = Resources.Load<AudioClip>("Sonidos/bandada_aleteo");
        if (clip != null)
        {
            var s = new GameObject("Bandada_Aleteo");
            s.transform.SetParent(transform, false);
            flutter = s.AddComponent<AudioSource>();
            flutter.clip = clip;
            flutter.loop = true;
            flutter.spatialBlend = 1f;
            flutter.rolloffMode = AudioRolloffMode.Linear;
            flutter.minDistance = 1f;
            flutter.maxDistance = 14f;
            flutter.volume = 0f;
            flutter.Play();
        }
        var list = new List<AudioClip>();
        for (int i = 1; i <= 3; i++)
        {
            var c = Resources.Load<AudioClip>("Sonidos/murcielago_chillido_" + i);
            if (c != null) list.Add(c);
        }
        screeches = list.ToArray();
        environment = gallery.GetComponent<GoyaEnvironment>();
        built = true;
    }

    void Update()
    {
        if (gallery == null) gallery = GetComponent<GoyaGallery>();
        if (gallery == null || !gallery.pathMode || gallery.PathLength <= 0f) return;
        if (!flock)
        {
            if (meshRenderer != null) meshRenderer.enabled = false;
            if (flutter != null) flutter.volume = 0f;
            return;
        }
        if (!built) Build();

        float dt = Mathf.Min(Time.deltaTime, 0.05f);
        roostTimer -= dt;
        if (roostTimer <= 0f) { roostTimer = 0.5f; UpdateRoosts(); }
        UpdateScares();
        Simulate(dt);
        Draw();

        bool show = gallery.appear > 0.5f;
        meshRenderer.enabled = show;
        if (flutter != null)
        {
            Vector3 mid = Vector3.zero; float fear = 0f;
            for (int i = 0; i < pos.Length; i++) { mid += pos[i]; fear += panic[i]; }
            mid /= pos.Length; fear /= pos.Length;
            flutter.transform.position = mid;
            flutter.volume = show ? Mathf.Lerp(0.35f, 1f, fear) : 0f;
            flutter.pitch = Mathf.Lerp(1f, 1.3f, fear);
        }
        screechWait -= dt;
    }

    // Las obras ocultas cercanas (todavia sin armar) son los nidos. Los murcielagos se reparten entre ellos.
    void UpdateRoosts()
    {
        Vector3 rig = gallery.RigPosition;
        var keep = new List<GoyaGallery.Painting>();
        var candidates = new List<GoyaGallery.Painting>();
        foreach (var p in gallery.paintings)
        {
            if (!p.active || p.revealed || p.locked || p.root == null) continue;
            float d = Vector3.Distance(p.root.position, rig);
            if (d > roostRange || p.root.position.z < rig.z - 1.5f) continue;   // las que quedaron atras no
            candidates.Add(p);
        }
        candidates.Sort((a, b) => Vector3.Distance(a.root.position, rig).CompareTo(Vector3.Distance(b.root.position, rig)));
        if (candidates.Count > maxRoosts) candidates.RemoveRange(maxRoosts, candidates.Count - maxRoosts);

        // Los nidos que siguen valen; los murcielagos de un nido que se fue buscan otro
        var old = new List<Roost>(roosts);
        roosts.Clear();
        var remap = new int[old.Count];
        for (int k = 0; k < old.Count; k++) remap[k] = -1;
        foreach (var p in candidates)
        {
            int was = old.FindIndex(r => r.p == p);
            var r = was >= 0 ? old[was] : new Roost { p = p };
            if (was >= 0) remap[was] = roosts.Count;
            Vector3 toRig = rig - p.root.position; toRig.y = 0f;
            r.center = p.root.position;
            r.normal = Vector3.Dot(p.root.forward, toRig) >= 0f ? p.root.forward : -p.root.forward;   // la cara que mira a la persona
            r.right = p.root.right; r.up = p.root.up;
            r.members = 0;
            roosts.Add(r);
        }
        for (int i = 0; i < pos.Length; i++)
        {
            roostOf[i] = roostOf[i] >= 0 && roostOf[i] < remap.Length ? remap[roostOf[i]] : -1;
            if (roostOf[i] >= 0) roosts[roostOf[i]].members++;
        }
        if (roosts.Count == 0) return;
        // Los que no tienen nido (o sobran en uno muy lleno) van al que tenga menos, con algo de preferencia por el cercano
        int fair = Mathf.CeilToInt((float)pos.Length / roosts.Count);
        for (int i = 0; i < pos.Length; i++)
        {
            if (roostOf[i] >= 0 && roosts[roostOf[i]].members <= fair + 2) continue;
            if (roostOf[i] >= 0) roosts[roostOf[i]].members--;
            int best = 0; float bestScore = float.MaxValue;
            for (int k = 0; k < roosts.Count; k++)
            {
                float score = roosts[k].members / (float)fair + Vector3.Distance(pos[i], roosts[k].center) * 0.05f;
                if (score < bestScore) { bestScore = score; best = k; }
            }
            roostOf[i] = best;
            roosts[best].members++;
        }
    }

    // Lo que asusta: la mano que barre una obra, las palmas y el fuego de la antorcha
    void UpdateScares()
    {
        scares.Clear(); scareForce.Clear();
        float hand = 0.6f + gallery.agitation * 2.5f;
        foreach (var p in gallery.paintings)
            if (p.active && p.hovered && p.openHandOver) Add(p.hitPoint, handRadius, hand);
        Vector3 palm;
        if (GoyaHands.Palm(out palm)) Add(palm, 0.7f, 0.5f + gallery.agitation * 2f);
        var cave = gallery.Cave;
        Vector3 fire;
        if (cave != null && cave.TorchFire(out fire)) Add(fire, torchRadius, 1.2f);
        if (gallery.Head != null) Add(gallery.Head.position, 0.6f, 0.4f);   // no se te meten en la cara
    }

    void Add(Vector3 at, float radius, float force)
    {
        scares.Add(new Vector4(at.x, at.y, at.z, radius));
        scareForce.Add(force);
    }

    long Key(Vector3 p)
    {
        float s = Mathf.Max(0.1f, neighborRadius);
        long x = Mathf.FloorToInt(p.x / s), y = Mathf.FloorToInt(p.y / s), z = Mathf.FloorToInt(p.z / s);
        return (x & 0x1FFFFF) | ((y & 0x1FFFFF) << 21) | ((z & 0x1FFFFF) << 42);
    }

    void Simulate(float dt)
    {
        int n = pos.Length;
        foreach (var l in grid.Values) { l.Clear(); spare.Push(l); }
        grid.Clear();
        for (int i = 0; i < n; i++)
        {
            long k = Key(pos[i]);
            List<int> l;
            if (!grid.TryGetValue(k, out l)) { l = spare.Count > 0 ? spare.Pop() : new List<int>(); grid[k] = l; }
            l.Add(i);
        }
        System.Array.Copy(vel, prevVel, n);
        System.Array.Copy(panic, prevPanic, n);

        float s = Mathf.Max(0.1f, neighborRadius);
        float r2 = neighborRadius * neighborRadius, sep2 = separationRadius * separationRadius;
        float time = Time.time;
        foreach (var r in roosts) r.covering = 0;

        for (int i = 0; i < n; i++)
        {
            Vector3 p = pos[i];
            Vector3 sep = Vector3.zero, ali = Vector3.zero, coh = Vector3.zero;
            int near = 0;
            float neighborFear = 0f;
            int cx = Mathf.FloorToInt(p.x / s), cy = Mathf.FloorToInt(p.y / s), cz = Mathf.FloorToInt(p.z / s);
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    for (int dz = -1; dz <= 1; dz++)
                    {
                        long k = ((long)(cx + dx) & 0x1FFFFF) | (((long)(cy + dy) & 0x1FFFFF) << 21) | (((long)(cz + dz) & 0x1FFFFF) << 42);
                        List<int> l;
                        if (!grid.TryGetValue(k, out l)) continue;
                        for (int m = 0; m < l.Count; m++)
                        {
                            int j = l[m];
                            if (j == i) continue;
                            Vector3 off = p - pos[j];
                            float d2 = off.sqrMagnitude;
                            if (d2 > r2) continue;
                            near++;
                            ali += prevVel[j];
                            coh += pos[j];
                            if (d2 < sep2 && d2 > 1e-6f) sep += off / d2;   // cuanto mas cerca, mas empuja
                            if (prevPanic[j] > neighborFear) neighborFear = prevPanic[j];
                        }
                    }

            // 6. Contagio y calma
            float fear = panic[i];
            float caught = neighborFear * contagion;
            if (caught > fear) fear = Mathf.MoveTowards(fear, caught, contagionSpeed * dt);
            fear -= calm * dt;

            Vector3 force = Vector3.zero;
            if (near > 0)
            {
                force += sep * separation * 0.1f;
                force += (ali / near - vel[i]) * alignment;
                force += (coh / near - p) * cohesion;
            }

            // 5. Miedo: huir de lo que asusta (y asustarse mas cuanto mas cerca)
            for (int k = 0; k < scares.Count; k++)
            {
                Vector4 sc = scares[k];
                Vector3 off = p - new Vector3(sc.x, sc.y, sc.z);
                float d = off.magnitude;
                if (d >= sc.w) continue;
                float w = 1f - d / sc.w;
                fear += scareForce[k] * w * dt * 3f;
                force += (d > 1e-4f ? off / d : Random.onUnitSphere) * flee * w;
            }
            fear = Mathf.Clamp01(fear);
            panic[i] = fear;

            // 4. Nido: girar alrededor de su obra, delante de ella
            int ro = roostOf[i];
            float calmness = (1f - fear) * (1f - fear);
            if (ro >= 0 && ro < roosts.Count)
            {
                var r = roosts[ro];
                homeAngle[i] += homeSpeed[i] * dt;
                float hw = r.p.halfW * 0.95f, hh = r.p.halfH * 0.95f;
                Vector3 home = r.center + r.normal * 0.35f
                             + r.right * (Mathf.Cos(homeAngle[i]) * homeRadius[i] * hw)
                             + r.up * (Mathf.Sin(homeAngle[i]) * homeRadius[i] * hh);
                Vector3 toHome = home - p;
                force += Vector3.ClampMagnitude(toHome, 1.5f) * roost * calmness;
                Vector3 around = Vector3.Cross(r.normal, p - r.center);
                force += around.normalized * orbit * calmness * Mathf.Sign(homeSpeed[i]);
                // Asustados: se van para arriba y lejos de la obra
                force += ((p - r.center).normalized + Vector3.up) * fear * 3f;

                // Esta tapando la obra?
                Vector3 local = p - r.center;
                if (fear < 0.5f && Mathf.Abs(Vector3.Dot(local, r.right)) < r.p.halfW && Mathf.Abs(Vector3.Dot(local, r.up)) < r.p.halfH
                    && Mathf.Abs(Vector3.Dot(local, r.normal)) < 1.0f)
                    r.covering++;
            }
            else
            {
                // Sin obras ocultas cerca: revolotean arriba y adelante de la persona
                Vector3 wait = gallery.RigPosition + Vector3.forward * 7f + Vector3.up * 3.2f;
                force += Vector3.ClampMagnitude(wait - p, 3f) * 0.6f;
                force += Vector3.Cross(Vector3.up, p - wait).normalized * 0.8f;
            }

            // Ni el piso ni muy alto
            float floor = gallery.RigPosition.y + 0.25f;
            if (p.y < floor + 0.3f) force += Vector3.up * (floor + 0.3f - p.y) * 12f;
            if (p.y > gallery.RigPosition.y + 7f) force += Vector3.down * 3f;
            // Un poco de azar, para que no se vean como maquinas
            force += new Vector3(Mathf.PerlinNoise(i * 0.37f, time * 0.6f) - 0.5f,
                                 Mathf.PerlinNoise(i * 0.53f + 7f, time * 0.6f) - 0.5f,
                                 Mathf.PerlinNoise(i * 0.71f + 13f, time * 0.6f) - 0.5f) * 1.5f;

            force = Vector3.ClampMagnitude(force, maxForce * (1f + fear));
            Vector3 v = vel[i] + force * dt;
            float speed = v.magnitude, top = Mathf.Lerp(maxSpeed, panicSpeed, fear);
            if (speed > top) v *= top / speed;
            else if (speed < minSpeed) v = (speed > 1e-4f ? v / speed : Random.onUnitSphere) * minSpeed;
            vel[i] = v;
            pos[i] = p + v * dt;
            flap[i] += dt * flapRate[i] * (1f + fear);
        }

        // El primero que se asusta mucho chilla
        if (screechWait <= 0f)
            for (int i = 0; i < n; i++)
                if (panic[i] > 0.7f && prevPanic[i] <= 0.7f) { Screech(pos[i]); break; }
    }

    void Screech(Vector3 at)
    {
        if (screeches == null || screeches.Length == 0 || screechWait > 0f || gallery.appear < 0.5f) return;
        screechWait = 1.2f;
        AudioSource.PlayClipAtPoint(screeches[Random.Range(0, screeches.Length)], at, 0.7f);
    }

    void Draw()
    {
        float span = wingspan * 0.5f;
        for (int i = 0; i < pos.Length; i++)
        {
            Vector3 fwd = vel[i].sqrMagnitude > 1e-6f ? vel[i].normalized : Vector3.forward;
            Vector3 right = Vector3.Cross(Vector3.up, fwd);
            if (right.sqrMagnitude < 1e-4f) right = Vector3.right;
            right.Normalize();
            // Se inclina en las curvas
            Vector3 turn = vel[i] - prevVel[i];
            float bank = Mathf.Clamp(Vector3.Dot(turn, right) * 6f, -0.8f, 0.8f);
            Vector3 up = Vector3.Cross(fwd, right);
            up = (up + right * bank).normalized;
            right = Vector3.Cross(up, fwd).normalized;

            float wing = Mathf.Sin(flap[i]);
            float elbowY = wing * 0.35f, tipY = Mathf.Sin(flap[i] - 0.7f) * 0.7f;
            Vector3 c = pos[i];
            int b = i * VertsPerBat;
            verts[b + 0] = c + fwd * span * 0.45f;
            verts[b + 1] = c - right * span * 0.12f;
            verts[b + 2] = c - fwd * span * 0.5f;
            verts[b + 3] = c + right * span * 0.12f;
            for (int sgn = -1, w = 0; w < 2; w++, sgn += 2)
            {
                int o = b + 4 + w * 5;
                Vector3 side = right * sgn;
                verts[o + 0] = c + side * span * 0.08f + fwd * span * 0.18f;
                verts[o + 1] = c + side * span * 0.08f - fwd * span * 0.25f;
                verts[o + 2] = c + side * span * 0.55f + fwd * span * 0.2f + up * span * elbowY;
                verts[o + 3] = c + side * span * 1.0f - fwd * span * 0.15f + up * span * tipY;
                verts[o + 4] = c + side * span * 0.6f - fwd * span * 0.35f + up * span * (elbowY + tipY) * 0.4f;
            }
            Color col = color;
            if (environment != null) col = environment.Fogged(col, c);
            for (int k = 0; k < VertsPerBat; k++) colors[b + k] = col;
        }
        mesh.vertices = verts;
        mesh.colors = colors;
    }
}

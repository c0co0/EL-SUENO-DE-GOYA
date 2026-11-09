using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Arboles fractales del paisaje (Unidad 5: fractales y sistemas de particulas). Cada arbol se genera por recursion:
///   1. Dibujar una rama.
///   2. Al final de la rama, abrir 2 (a veces 3) ramas mas cortas y mas finas, giradas un angulo al azar.
///   3. Repetir hasta llegar a la profundidad elegida o a ramas muy cortas.
/// Es un fractal estocastico: las reglas son fijas pero el angulo, el largo y la cantidad de ramas tienen azar,
/// asi que no hay dos arboles iguales. Copian la paleta y el porte de los arboles de las pinturas de Goya
/// (La cometa, La gallina ciega, La nevada).
///
/// Los arboles CRECEN cuando te acercas: cada generacion de ramas aparece despues de la anterior (se ve la recursion).
/// Las ramas se mecen con el viento (cuanto mas finas, mas se mueven) y las hojas son un sistema de particulas:
/// cada hoja tiene posicion, velocidad y vida; el viento a veces las arranca, caen girando y se desvanecen,
/// y despues vuelven a brotar en su rama. Con las manos agitadas sopla mas fuerte.
///
/// Reemplaza a los arboles recortados sueltos de GoyaOrigins (los arboles de los que nacen obras siguen igual).
/// Se agrega solo y funciona en modo camino.
/// </summary>
public class GoyaFractalTrees : MonoBehaviour
{
    [Tooltip("Arboles fractales en lugar de los arboles recortados sueltos")]
    public bool fractalTrees = true;
    [Tooltip("Arboles extra por zona (ademas de los de Goya Origins)")]
    public int extraPerZone = 2;
    [Tooltip("Zonas con arboles (0 cartones, 1 corte, 2 brujas)")]
    public int zones = 3;

    [Header("Crecimiento")]
    [Tooltip("Empiezan a crecer cuando estas a menos de esta distancia (metros)")]
    public float growDistance = 16f;
    [Tooltip("Generaciones de ramas por segundo")]
    public float growSpeed = 1.3f;

    [Header("Viento y hojas")]
    public float sway = 0.05f;
    [Tooltip("Probabilidad por segundo de que el viento arranque una hoja")]
    public float leafFall = 0.015f;
    [Tooltip("Segundos de vida de una hoja que cae")]
    public float leafLife = 6f;
    [Tooltip("Los arboles mas lejanos que esto no se animan")]
    public float animateDistance = 26f;

    public enum Kind { Cometa, Gallina, Nevada }

    // Reglas de cada tipo de arbol (copiadas a ojo de los arboles de las pinturas)
    class Style
    {
        public int depth;            // generaciones de ramas
        public float trunk;          // largo del tronco (proporcion de la altura)
        public float ratio;          // cada rama mide esto de su madre
        public float angle;          // apertura de las ramas (grados)
        public float jitter;         // azar en angulo y largo
        public float third;          // probabilidad de una tercera rama
        public float radius;         // radio del tronco (proporcion de la altura)
        public float radiusRatio;    // cada rama es esto de gruesa que su madre
        public float lean;           // inclinacion hacia un costado (el viento de La nevada)
        public float up;             // cuanto buscan las ramas ir hacia arriba
        public Color barkLow, barkHigh;
        public bool leaves;
        public int leavesPerTip;
        public float leafSize;
        public Color leafA, leafB, leafLight;
        public int leafFromGen;      // desde que generacion hay hojas
    }

    static Style Get(Kind k)
    {
        switch (k)
        {
            case Kind.Gallina:   // tronco grueso ocre, copa alta y rala, verde palido
                return new Style
                {
                    depth = 5, trunk = 0.55f, ratio = 0.62f, angle = 22f, jitter = 0.35f, third = 0.2f, radius = 0.05f, radiusRatio = 0.62f,
                    lean = 0.08f, up = 0.5f, barkLow = new Color(0.42f, 0.37f, 0.33f), barkHigh = new Color(0.67f, 0.66f, 0.58f),
                    leaves = true, leavesPerTip = 5, leafSize = 0.4f, leafA = new Color(0.55f, 0.6f, 0.5f), leafB = new Color(0.7f, 0.75f, 0.68f),
                    leafLight = new Color(0.82f, 0.84f, 0.72f), leafFromGen = 3
                };
            case Kind.Nevada:    // pelado, gris oscuro, doblado por el viento, ramitas finas
                return new Style
                {
                    depth = 7, trunk = 0.35f, ratio = 0.72f, angle = 24f, jitter = 0.45f, third = 0.35f, radius = 0.022f, radiusRatio = 0.6f,
                    lean = 0.5f, up = 0.15f, barkLow = new Color(0.33f, 0.33f, 0.36f), barkHigh = new Color(0.48f, 0.49f, 0.52f),
                    leaves = false
                };
            default:             // La cometa: tronco claro y torcido, matas de hojas verde oliva
                return new Style
                {
                    depth = 6, trunk = 0.45f, ratio = 0.68f, angle = 26f, jitter = 0.4f, third = 0.3f, radius = 0.032f, radiusRatio = 0.64f,
                    lean = 0.25f, up = 0.4f, barkLow = new Color(0.26f, 0.23f, 0.18f), barkHigh = new Color(0.8f, 0.78f, 0.72f),
                    leaves = true, leavesPerTip = 4, leafSize = 0.42f, leafA = new Color(0.34f, 0.38f, 0.28f), leafB = new Color(0.47f, 0.51f, 0.4f),
                    leafLight = new Color(0.62f, 0.65f, 0.47f), leafFromGen = 3
                };
        }
    }

    class Node
    {
        public int parent, gen;
        public Vector3 dir, u, v;    // direccion de la rama y dos ejes para el cilindro
        public float length, r0, r1, phase;
        public Vector3 start, end, swayOff;
    }

    class Leaf
    {
        public int node;
        public Vector3 offset, pos, vel;
        public Color color;
        public float size, life, spin;   // life: 1 = en la rama, < 1 cayendo
        public bool falling;
    }

    class Tree
    {
        public Kind kind;
        public Style style;
        public Vector3 root;
        public float height, grow;
        public bool grounded;
        public Color tint = Color.white;
        public List<Node> nodes = new List<Node>();
        public List<Leaf> leaves = new List<Leaf>();
        public Mesh mesh;
        public Vector3[] verts;
        public Color[] colors;
        public Color[] baseColors;
        public MeshRenderer renderer;
        public ParticleSystem ps;
        public ParticleSystem.Particle[] parts;
    }

    const int Sides = 4;    // ramas de 4 caras (pinceladas): liviano para el celular

    GoyaGallery gallery;
    GoyaEnvironment environment;
    GoyaTerrain terrain;
    readonly List<Tree> trees = new List<Tree>();
    Material barkMat, leafMat;
    float floorY;
    bool built;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Auto()
    {
        var g = FindFirstObjectByType<GoyaGallery>();
        if (g != null && g.GetComponent<GoyaFractalTrees>() == null) g.gameObject.AddComponent<GoyaFractalTrees>();
    }

    /// <summary>GoyaOrigins pregunta si tiene que hacer los arboles recortados sueltos.</summary>
    public static bool Replaces(GameObject go)
    {
        var f = go.GetComponent<GoyaFractalTrees>();
        return f == null || f.fractalTrees;   // se agrega solo: si todavia no esta, igual los va a hacer
    }

    void Update()
    {
        if (gallery == null) gallery = GetComponent<GoyaGallery>();
        if (gallery == null || !gallery.pathMode || gallery.PathLength <= 0f || !fractalTrees) return;
        if (!built) { Build(); if (!built) return; }

        float dt = Mathf.Min(Time.deltaTime, 0.05f);
        Vector3 rig = gallery.RigPosition;
        float windBoost = 1f + gallery.agitation * 2f;
        bool show = gallery.appear > 0.01f;
        foreach (var t in trees)
        {
            if (!t.grounded && terrain != null && terrain.Ready)
            {
                t.root.y = floorY + terrain.Height(t.root.x, t.root.z) - 0.1f;
                t.grounded = true;
            }
            float d = Vector3.Distance(new Vector3(t.root.x, rig.y, t.root.z), rig);
            if (show && d < growDistance && t.grow < t.style.depth + 1.5f) t.grow += dt * growSpeed;
            t.renderer.enabled = show && t.grow > 0f;
            if (d > animateDistance && t.grow >= t.style.depth + 1.5f) continue;   // lejos y ya crecido: queda quieto
            Shape(t, Time.time, windBoost);
            Paint(t);
            UpdateLeaves(t, dt, windBoost, show);
        }
    }

    void Build()
    {
        if (gallery.ZoneCount == 0) return;
        environment = GetComponent<GoyaEnvironment>();
        terrain = GetComponent<GoyaTerrain>();
        floorY = gallery.RigPosition.y - (environment != null ? environment.eyeHeight : 1.6f);
        var tex = Resources.Load<Texture2D>("Paisaje/pinceladas");
        var sh = Resources.Load<Shader>("Shaders/GoyaTerreno");
        barkMat = new Material(sh != null ? sh : Shader.Find("Sprites/Default")) { mainTexture = tex };
        leafMat = new Material(Shader.Find("Sprites/Default")) { mainTexture = LeafTexture() };

        var c = gallery.Center;
        var rnd = new System.Random(1746);   // siempre el mismo bosque
        var origins = gallery.Origins;
        if (origins != null)
            foreach (var o in origins.trees)
            {
                if (o == null || o.zone >= gallery.ZoneCount) continue;
                Kind k = o.image.Contains("gallina") ? Kind.Gallina : o.image.Contains("nevada") ? Kind.Nevada : Kind.Cometa;
                float z = c.z + (o.zone + o.along) * gallery.zoneLength;
                Add(k, new Vector3(c.x + o.side, floorY, z), Mathf.Max(3f, o.height), rnd);
            }
        // Mas arboles, al azar a los costados del camino
        for (int zone = 0; zone < Mathf.Min(zones, gallery.ZoneCount); zone++)
            for (int i = 0; i < extraPerZone; i++)
            {
                float side = (rnd.NextDouble() < 0.5 ? -1f : 1f) * (5f + (float)rnd.NextDouble() * 5f);
                float z = c.z + (zone + (float)rnd.NextDouble()) * gallery.zoneLength;
                Kind k = (Kind)rnd.Next(0, 3);
                if (zone == 2 && rnd.NextDouble() < 0.5) k = Kind.Nevada;   // en la noche de las brujas, mas arboles pelados
                Add(k, new Vector3(c.x + side, floorY, z), 4f + (float)rnd.NextDouble() * 2.5f, rnd);
            }
        built = true;
    }

    void Add(Kind kind, Vector3 root, float height, System.Random rnd)
    {
        var cave = gallery.Cave;
        if (cave != null && cave.Footprint(root.x, root.z)) return;
        var t = new Tree { kind = kind, style = Get(kind), root = root, height = height };
        // El viento de La nevada sopla siempre hacia el mismo lado; los demas, hacia el camino o hacia afuera
        float side = Mathf.Sign(root.x - gallery.Center.x);
        Vector3 leanDir = kind == Kind.Nevada ? new Vector3(1f, 0f, 0.3f).normalized : new Vector3(-side * ((float)rnd.NextDouble() - 0.3f), 0f, (float)rnd.NextDouble() - 0.5f).normalized;
        float h = height * 0.7f;   // la suma de las ramas hace al arbol mas alto que el tronco solo
        Grow(t, -1, root, (Vector3.up + leanDir * t.style.lean * 0.6f).normalized, h * t.style.trunk, height * t.style.radius, 0, leanDir, rnd);
        BuildMesh(t);
        if (t.style.leaves) BuildLeaves(t, rnd);
        trees.Add(t);
    }

    // La recursion: una rama y, al final, sus hijas mas cortas y mas finas
    void Grow(Tree t, int parent, Vector3 start, Vector3 dir, float length, float radius, int gen, Vector3 leanDir, System.Random rnd)
    {
        var s = t.style;
        var n = new Node { parent = parent, gen = gen, dir = dir, length = length, r0 = radius, r1 = radius * s.radiusRatio, phase = (float)rnd.NextDouble() * 6.28f };
        n.u = Vector3.Cross(dir, Mathf.Abs(dir.y) < 0.95f ? Vector3.up : Vector3.right).normalized;
        n.v = Vector3.Cross(dir, n.u).normalized;
        int me = t.nodes.Count;
        t.nodes.Add(n);
        if (gen >= s.depth || length < 0.08f || t.nodes.Count > 500) return;

        int children = rnd.NextDouble() < s.third ? 3 : 2;
        float roll = (float)rnd.NextDouble() * 360f;
        for (int i = 0; i < children; i++)
        {
            float a = s.angle * (1f + s.jitter * ((float)rnd.NextDouble() * 2f - 1f));
            // Giradas alrededor de la madre (como las hojas: 137,5 grados entre una y otra) para que el arbol tenga volumen
            Quaternion around = Quaternion.AngleAxis(roll + i * 137.5f, dir);
            Vector3 axis = around * n.u;
            Vector3 d = Quaternion.AngleAxis(a * (i == 2 ? 0.4f : 1f), axis) * dir;
            // Tropismos: hacia arriba (la luz) y hacia donde sopla el viento
            d = (d + Vector3.up * s.up * 0.3f + leanDir * s.lean * 0.25f).normalized;
            float len = length * s.ratio * (1f + s.jitter * 0.5f * ((float)rnd.NextDouble() * 2f - 1f));
            Grow(t, me, Vector3.zero, d, len, n.r1, gen + 1, leanDir, rnd);
        }
    }

    void BuildMesh(Tree t)
    {
        int n = t.nodes.Count;
        t.verts = new Vector3[n * Sides * 2];
        t.colors = new Color[t.verts.Length];
        t.baseColors = new Color[t.verts.Length];
        var uv = new Vector2[t.verts.Length];
        var tris = new int[n * Sides * 6];
        int ti = 0;
        float maxGen = Mathf.Max(1, t.style.depth);
        for (int k = 0; k < n; k++)
        {
            var nd = t.nodes[k];
            for (int s = 0; s < Sides; s++)
            {
                int a = k * Sides * 2 + s, b = a + Sides;
                // Luz de costado (como en las pinturas): una cara clara y la de atras oscura
                float light = 0.65f + 0.35f * Mathf.Cos(s * Mathf.PI * 2f / Sides - 0.8f);
                float h = Mathf.Clamp01((nd.gen + 0.5f) / maxGen);
                Color c0 = Color.Lerp(t.style.barkLow, t.style.barkHigh, Mathf.Clamp01(nd.gen / maxGen)) * light;
                Color c1 = Color.Lerp(t.style.barkLow, t.style.barkHigh, h) * light;
                c0.a = c1.a = 1f;
                t.baseColors[a] = c0; t.baseColors[b] = c1;
                uv[a] = new Vector2((float)s / Sides * 0.3f + nd.phase, 0f);
                uv[b] = new Vector2((float)s / Sides * 0.3f + nd.phase, nd.length * 0.8f);
                int s2 = (s + 1) % Sides;
                int a2 = k * Sides * 2 + s2, b2 = a2 + Sides;
                tris[ti++] = a; tris[ti++] = b; tris[ti++] = a2;
                tris[ti++] = a2; tris[ti++] = b; tris[ti++] = b2;
            }
        }
        var go = new GameObject("ArbolFractal_" + t.kind);
        go.transform.SetParent(transform, false);
        go.transform.position = Vector3.zero;
        t.mesh = new Mesh { name = "Arbol fractal" };
        t.mesh.MarkDynamic();
        Shape(t, 0f, 1f);
        t.mesh.vertices = t.verts;
        t.mesh.uv = uv;
        t.mesh.colors = t.baseColors;
        t.mesh.triangles = tris;
        t.mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100000f);
        go.AddComponent<MeshFilter>().sharedMesh = t.mesh;
        t.renderer = go.AddComponent<MeshRenderer>();
        t.renderer.sharedMaterial = barkMat;
        t.renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        t.renderer.enabled = false;
    }

    // Donde esta cada rama ahora: crece por generaciones y se mece (cada rama suma el vaiven de su madre)
    void Shape(Tree t, float time, float windBoost)
    {
        float swayAmp = sway * windBoost;
        for (int k = 0; k < t.nodes.Count; k++)
        {
            var n = t.nodes[k];
            float g = Mathf.Clamp01(t.grow - n.gen);
            g = g * g * (3f - 2f * g);
            Vector3 start = n.parent < 0 ? t.root : t.nodes[n.parent].end;
            Vector3 parentSway = n.parent < 0 ? Vector3.zero : t.nodes[n.parent].swayOff;
            float flex = (n.gen + 1f) * n.length * swayAmp;
            Vector3 wind = new Vector3(Mathf.Sin(time * 1.1f + n.phase) + 0.6f * Mathf.Sin(time * 0.37f + t.root.z),
                                       0.3f * Mathf.Sin(time * 1.7f + n.phase * 2f),
                                       0.6f * Mathf.Cos(time * 0.9f + n.phase)) * flex;
            n.swayOff = parentSway + wind * g;
            n.start = start;
            n.end = start + n.dir * (n.length * g) + (n.swayOff - parentSway);
            float r0 = n.r0 * Mathf.Max(0.05f, g), r1 = n.r1 * g;
            Vector3 axis = (n.end - n.start);
            Vector3 u = n.u, v = n.v;
            for (int s = 0; s < Sides; s++)
            {
                float ang = s * Mathf.PI * 2f / Sides;
                Vector3 ring = u * Mathf.Cos(ang) + v * Mathf.Sin(ang);
                int a = k * Sides * 2 + s;
                t.verts[a] = n.start + ring * r0;
                t.verts[a + Sides] = n.end + ring * r1;
            }
        }
        if (t.mesh != null) t.mesh.vertices = t.verts;
    }

    // Color de la epoca y niebla (con la niebla solo se puede oscurecer: el shader multiplica)
    void Paint(Tree t)
    {
        Color tint = Color.white;
        if (environment != null)
        {
            Color zone = environment.ZoneTint(t.root);
            tint = Color.Lerp(Color.white, zone, 0.7f);
            Color fa = environment.Fogged(Color.black, t.root + Vector3.up * t.height * 0.5f);
            Color fb = environment.Fogged(Color.white, t.root + Vector3.up * t.height * 0.5f);
            tint = new Color(tint.r * (fb.r - fa.r) + fa.r, tint.g * (fb.g - fa.g) + fa.g, tint.b * (fb.b - fa.b) + fa.b, 1f);
        }
        for (int i = 0; i < t.colors.Length; i++)
        {
            Color b = t.baseColors[i];
            t.colors[i] = new Color(b.r * tint.r, b.g * tint.g, b.b * tint.b, 1f);
        }
        t.mesh.colors = t.colors;
        t.tint = tint;
    }

    // ---------------------------------------------------------------- hojas (sistema de particulas)

    void BuildLeaves(Tree t, System.Random rnd)
    {
        var s = t.style;
        for (int k = 0; k < t.nodes.Count; k++)
        {
            var n = t.nodes[k];
            if (n.gen < s.leafFromGen) continue;
            bool tip = n.gen >= s.depth;
            int count = tip ? s.leavesPerTip : 1;
            for (int i = 0; i < count; i++)
            {
                float r = n.length * 0.9f + 0.15f;
                var l = new Leaf
                {
                    node = k,
                    offset = new Vector3((float)rnd.NextDouble() * 2f - 1f, ((float)rnd.NextDouble() * 2f - 1f) * 0.6f, (float)rnd.NextDouble() * 2f - 1f) * r,
                    size = s.leafSize * (0.6f + (float)rnd.NextDouble() * 0.8f) * t.height / 5.5f,
                    life = 1f,
                    spin = (float)rnd.NextDouble() * 360f
                };
                float m = (float)rnd.NextDouble();
                l.color = m < 0.2f ? s.leafLight : Color.Lerp(s.leafA, s.leafB, (float)rnd.NextDouble());
                l.color.a = 0.95f;
                t.leaves.Add(l);
            }
        }
        var go = new GameObject("Hojas_" + t.kind);
        go.transform.SetParent(transform, false);
        t.ps = go.AddComponent<ParticleSystem>();
        t.ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = t.ps.main;
        main.loop = false;
        main.playOnAwake = false;
        main.maxParticles = t.leaves.Count;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = 1000f;
        main.startSpeed = 0f;
        var em = t.ps.emission; em.enabled = false;
        var sh = t.ps.shape; sh.enabled = false;
        var rend = go.GetComponent<ParticleSystemRenderer>();
        rend.sharedMaterial = leafMat;
        rend.renderMode = ParticleSystemRenderMode.Billboard;
        rend.sortingOrder = 3;
        t.parts = new ParticleSystem.Particle[t.leaves.Count];
        t.ps.Play();
    }

    // Cada hoja: pegada a su rama (y se mece con ella) o, si el viento la arranco, cae con gravedad, aire y vaiven,
    // y se desvanece; despues vuelve a brotar en la rama
    void UpdateLeaves(Tree t, float dt, float windBoost, bool show)
    {
        if (t.ps == null) return;
        Vector3 gravity = new Vector3(0f, -0.9f, 0f);
        float time = Time.time;
        int n = 0;
        for (int i = 0; i < t.leaves.Count; i++)
        {
            var l = t.leaves[i];
            var node = t.nodes[l.node];
            float grown = Mathf.Clamp01(t.grow - node.gen - 0.5f);
            Vector3 home = node.end + l.offset * grown;
            if (!l.falling)
            {
                if (l.life < 1f) l.life = Mathf.MoveTowards(l.life, 1f, dt * 0.3f);    // brota de nuevo
                l.pos = home;
                if (grown >= 1f && Random.value < leafFall * windBoost * dt)
                {
                    l.falling = true;
                    l.vel = node.swayOff * 2f + Random.insideUnitSphere * 0.3f;
                }
            }
            else
            {
                // Particula libre: aceleracion = gravedad + aire (vaiven) - rozamiento; la vida baja hasta desaparecer
                Vector3 flutter = new Vector3(Mathf.Sin(time * 3f + l.spin), 0f, Mathf.Cos(time * 2.3f + l.spin)) * 1.5f * windBoost;
                Vector3 acc = gravity + flutter - l.vel * 1.8f;
                l.vel += acc * dt;
                l.pos += l.vel * dt;
                float ground = t.root.y + 0.05f;
                if (l.pos.y < ground) { l.pos.y = ground; l.vel = Vector3.zero; }
                l.spin += dt * 120f;
                l.life -= dt / Mathf.Max(0.5f, leafLife);
                if (l.life <= 0f) { l.falling = false; l.life = 0f; }   // vuelve a la rama (y brota)
            }
            float alpha = (l.falling ? Mathf.Clamp01(l.life * 2f) : l.life) * grown * (show ? gallery.appear : 0f);
            if (alpha <= 0.01f) continue;
            Color c = new Color(l.color.r * t.tint.r, l.color.g * t.tint.g, l.color.b * t.tint.b, l.color.a * alpha);
            t.parts[n].position = l.pos;
            t.parts[n].startColor = c;
            t.parts[n].startSize = l.size * (l.falling ? 0.45f : 1f);
            t.parts[n].rotation = l.spin;
            t.parts[n].startLifetime = 1000f;
            t.parts[n].remainingLifetime = 1000f;
            n++;
        }
        t.ps.SetParticles(t.parts, n);
    }

    // Mancha de hojas pintada: un borron irregular (no un circulo perfecto)
    static Texture2D LeafTexture()
    {
        const int n = 64;
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float dx = (x + 0.5f) / n * 2f - 1f, dy = (y + 0.5f) / n * 2f - 1f;
                float ang = Mathf.Atan2(dy, dx);
                float edge = 0.72f + 0.2f * Mathf.PerlinNoise(Mathf.Cos(ang) * 2f + 5f, Mathf.Sin(ang) * 2f + 5f) + 0.08f * Mathf.Sin(ang * 7f);
                float d = Mathf.Sqrt(dx * dx + dy * dy) / edge;
                float a = Mathf.Clamp01((1f - d) * 4f);
                float shade = 0.75f + 0.25f * Mathf.PerlinNoise(x * 0.15f, y * 0.15f) - 0.15f * dy;   // mas claro arriba
                tex.SetPixel(x, y, new Color(shade, shade, shade, a));
            }
        tex.Apply();
        return tex;
    }
}

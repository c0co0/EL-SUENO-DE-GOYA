using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// La cueva: empieza en la mitad de la zona de la guerra y llega hasta Saturno, adentro de una montaña de roca
/// con una boca grande a cada lado. Adentro esta oscuro.
/// En la entrada hay una antorcha apagada: con el mismo gesto que arma las obras (los dos brazos arriba con
/// las manos abiertas, o E en la PC) se enciende y queda en la mano. Las obras ya estan colgadas en las paredes
/// y solo se ven donde llega la luz. Las Pinturas Negras no estan al principio: aparecen de a poco en la pared
/// cuando se las alumbra de cerca (como cuando Goya las pinto en las paredes de su casa).
/// Las paredes tienen el fondo ocre de El perro. GoyaGallery lo agrega solo en modo camino.
/// </summary>
public class GoyaCave : MonoBehaviour
{
    [Header("Lugar")]
    [Tooltip("Zona donde empieza (3 = la guerra) y en que parte de esa zona (0.5 = la mitad)")]
    public int startZone = 3;
    [Range(0f, 1f)] public float startAt = 0.5f;
    [Tooltip("Zona cuyas obras no estan al principio: aparecen al alumbrarlas (4 = Pinturas Negras)")]
    public int hiddenZone = 4;

    [Header("Forma")]
    public float halfWidth = 2.6f;
    public float wallHeight = 2.4f;
    public float vaultHeight = 1.3f;
    [Tooltip("Que tan rugosa es la roca (metros)")]
    public float roughness = 0.35f;
    public Color wallColor = new Color(0.85f, 0.75f, 0.62f);
    public Color outsideRock = new Color(0.42f, 0.30f, 0.22f);
    [Tooltip("Cuanto mas grande es la boca que el tunel")]
    public float mouthScale = 1.7f;
    [Tooltip("Metros en que la boca se angosta hasta el tunel")]
    public float flareLength = 4f;

    [Header("Montaña")]
    public float mountainHeight = 17f;
    [Tooltip("Mitad del ancho de la montaña")]
    public float mountainWidth = 26f;
    [Tooltip("Metros de ladera antes de la entrada y despues de la salida")]
    public float mountainFoot = 7f;
    public Color rockLight = new Color(0.66f, 0.55f, 0.40f);   // ocre de Ciudad sobre una roca
    public Color rockDark = new Color(0.30f, 0.22f, 0.16f);
    public Color rockGreen = new Color(0.36f, 0.35f, 0.20f);

    [Header("Antorcha")]
    public float torchRange = 3.2f;
    public Color torchColor = new Color(1.6f, 1.0f, 0.5f);
    [Tooltip("Luz que hay en la cueva sin antorcha")]
    public Color ambient = new Color(0.025f, 0.022f, 0.02f);
    [Tooltip("A que distancia de la antorcha hay que estar para encenderla (metros)")]
    public float reach = 3.5f;
    [Tooltip("Distancia a la que hay que alumbrar una Pintura Negra para que aparezca, y cuanto tarda")]
    public float appearDistance = 3.2f;
    public float appearTime = 4f;

    [Header("Rocas que caen (simulacion fisica)")]
    [Tooltip("Adentro de la cueva se desprenden rocas del techo. Caen con gravedad, rebotan, chocan entre si, se parten al pegar contra el piso y quedan como escombros que podes patear")]
    public bool fallingRocks = true;
    [Tooltip("Segundos entre un derrumbe y el siguiente (al azar entre los dos)")]
    public Vector2 rockInterval = new Vector2(4f, 8f);
    [Tooltip("Rocas por derrumbe (al azar entre los dos)")]
    public Vector2Int rocksPerFall = new Vector2Int(1, 4);
    [Tooltip("Segundos de aviso (retumbe y polvo) antes de que se suelten")]
    public float rockWarning = 1.4f;
    [Tooltip("Tamaño de las rocas (metros, al azar entre los dos)")]
    public Vector2 rockSize = new Vector2(0.3f, 0.6f);
    [Tooltip("Velocidad del golpe contra el piso a partir de la cual una roca se parte (m/s)")]
    public float breakSpeed = 3.5f;
    [Tooltip("Pedazos en que se parte")]
    public Vector2Int fragments = new Vector2Int(3, 6);
    [Tooltip("Rebote (0 = nada, 1 = sin perder velocidad)")]
    [Range(0f, 1f)] public float bounce = 0.25f;
    [Tooltip("Escombros que quedan en el piso como maximo (los mas viejos se hunden)")]
    public int maxRubble = 40;
    [Tooltip("Cuanto achica la llama de la antorcha cada golpe (1 = la apaga)")]
    [Range(0f, 1f)] public float hitFlame = 0.34f;
    [Tooltip("Si el reloj (GoyaGame) esta prendido, segundos que se pierden por golpe")]
    public float rockPenalty = 15f;

    class Rock
    {
        public Rigidbody body;
        public float size, age;
        public Vector3 lastVel;
        public bool fragment, hit;
    }
    class Crack
    {
        public Vector3 at;
        public float time;
        public int count;
        public ParticleSystem dust;
    }
    readonly List<Rock> rocks = new List<Rock>();
    readonly List<Crack> cracks = new List<Crack>();
    Rigidbody body;              // la persona: una capsula que empuja los escombros
    PhysicsMaterial stoneMat;
    float flame = 1f;            // vida de la llama: cada roca que te pega la achica; en 0 se apaga
    readonly HashSet<GoyaGallery.Painting> found = new HashSet<GoyaGallery.Painting>();
    float rockTimer = 3f;
    bool colliders;
    Material rockMat, dustMat;
    AudioClip[] rumbleClips, hitClips;

    GoyaGallery gallery;
    float z0, z1, floorY, cx;
    bool built, lit, dropped;
    float dropFade = 1f;
    Vector3 dropAt;
    TextMesh dropHint;
    int lastGestures;
    readonly List<GoyaGallery.Painting> inside = new List<GoyaGallery.Painting>();
    readonly List<GoyaGallery.Painting> hidden = new List<GoyaGallery.Painting>();
    Transform torch, fireAt, stand;
    ParticleSystem fire;
    AudioSource fireSound;
    TextMesh hint;
    Vector3 standTop;
    float flicker;
    GoyaEnvironment environment;
    Mesh outside;
    Vector3[] outsidePos;
    Color[] outsideBase, outsideNow;
    float colorTimer;
    readonly List<Vector3> ringIn = new List<Vector3>(), ringOut = new List<Vector3>();

    public bool Built { get { return built; } }
    /// <summary>Obras de la cueva (para el contador del juego).</summary>
    public int PaintingCount { get { return inside.Count; } }
    /// <summary>Obras de la cueva que ya se alumbraron (y, las negras, que ya aparecieron).</summary>
    public int FoundCount { get { return found.Count; } }
    /// <summary>Donde esta el fuego de la antorcha, si esta prendida y en la mano (la bandada de murcielagos le huye).</summary>
    public bool TorchFire(out Vector3 at)
    {
        at = fireAt != null ? fireAt.position : Vector3.zero;
        return built && lit && !dropped && fireAt != null;
    }

    /// <summary>Si un punto esta adentro de la cueva.</summary>
    public bool Inside(Vector3 p)
    {
        return built && Mathf.Abs(p.x - cx) < halfWidth && p.z > z0 && p.z < z1;
    }

    /// <summary>Si un punto del piso cae debajo de la cueva, su boca o la montaña (ahi no va pasto ni arboles).</summary>
    public bool Footprint(float x, float z)
    {
        if (!built) return false;
        if (z > z0 - 0.5f && z < z1 + 0.5f && Mathf.Abs(x - cx) < halfWidth * Flare(z) + 0.6f) return true;
        return MountainAt(x, z) * Cut(x, z) > 0.8f;   // debajo de la montaña
    }

    public void Build(GoyaGallery g)
    {
        gallery = g;
        Vector3 c = g.Center;
        cx = c.x;
        z0 = c.z + (startZone + startAt) * g.zoneLength;
        z1 = c.z + g.PathLength;
        if (z1 - z0 < 4f || startZone >= g.ZoneCount) return;
        floorY = g.RigPosition.y - 1.6f;
        float eyeY = g.RigPosition.y;

        var spriteShader = Resources.Load<Shader>("Shaders/GoyaCuevaSprite");
        var wallShader = Resources.Load<Shader>("Shaders/GoyaCueva");
        var rockShader = Resources.Load<Shader>("Shaders/GoyaTerreno");
        if (spriteShader == null || wallShader == null) { Debug.LogWarning("[Cueva] Faltan los shaders de la cueva"); return; }
        var spriteMat = new Material(spriteShader);

        // Las obras que caen adentro van a las paredes
        foreach (var p in g.paintings)
        {
            float pz = p.root.position.z;
            if (pz < z0 + 0.8f || pz > z1 - 0.3f) continue;
            float side = p.root.position.x < cx ? -1f : 1f;
            p.root.position = new Vector3(cx + side * (halfWidth * Flare(pz) - 0.1f), eyeY + 0.1f, pz);
            p.root.rotation = Quaternion.LookRotation(new Vector3(side, 0f, 0f));
            p.locked = true;
            p.revealed = true;
            p.reveal = 1f;
            p.renderer.sharedMaterial = spriteMat;
            inside.Add(p);
            if (p.zone >= hiddenZone) { p.visibility = 0f; hidden.Add(p); }
        }

        var wallTex = Resources.Load<Texture2D>("Paisaje/cueva_pared");
        var wallMat = new Material(wallShader) { mainTexture = wallTex };
        BuildTunnel(wallMat);
        BuildFloor(new Material(wallShader) { mainTexture = Resources.Load<Texture2D>("Paisaje/pinceladas") });
        if (rockShader != null)
        {
            var rockMat = new Material(rockShader) { mainTexture = wallTex };
            BuildMountain(rockMat);
        }
        environment = g.GetComponent<GoyaEnvironment>();
        BuildTorch(c);
        built = true;
        lastGestures = g.FormGestures;
        SetLight(Vector3.zero, 0f);
    }

    // ---------------------------------------------------------------- forma

    // Cuanto mas grande es el tunel en ese punto: la boca es grande y se angosta hacia adentro (y en la salida)
    float Flare(float z)
    {
        float a = 1f - Mathf.SmoothStep(0f, 1f, (z - z0) / flareLength);
        float b = 1f - Mathf.SmoothStep(0f, 1f, (z1 - z) / (flareLength * 0.6f));
        return 1f + (mouthScale - 1f) * Mathf.Max(a, b * 0.6f);
    }

    // Perfil del tunel: paredes rectas y boveda, de derecha a izquierda
    List<Vector2> Profile()
    {
        var pts = new List<Vector2>();
        for (int i = 0; i <= 4; i++) pts.Add(new Vector2(halfWidth, wallHeight * i / 4f));
        for (int a = 1; a < 18; a++)
        {
            float ang = a * Mathf.PI / 18f;
            pts.Add(new Vector2(Mathf.Cos(ang) * halfWidth, wallHeight + Mathf.Sin(ang) * vaultHeight));
        }
        for (int i = 4; i >= 0; i--) pts.Add(new Vector2(-halfWidth, wallHeight * i / 4f));
        return pts;
    }

    // Hacia afuera de la cueva desde un punto del perfil
    Vector2 Outward(Vector2 p)
    {
        if (p.y <= wallHeight) return new Vector2(Mathf.Sign(p.x), 0f);
        return new Vector2(p.x / halfWidth, (p.y - wallHeight) / vaultHeight).normalized;
    }

    void BuildTunnel(Material mat)
    {
        var prof = Profile();
        int np = prof.Count;
        int nz = Mathf.CeilToInt((z1 - z0) / 0.5f) + 1;
        var verts = new List<Vector3>(); var uvs = new List<Vector2>(); var cols = new List<Color>(); var tris = new List<int>();
        for (int j = 0; j < nz; j++)
        {
            float z = Mathf.Lerp(z0, z1, (float)j / (nz - 1));
            float k = Flare(z);
            float arc = 0f;
            for (int i = 0; i < np; i++)
            {
                if (i > 0) arc += Vector2.Distance(prof[i], prof[i - 1]) * k;
                Vector2 p = prof[i];
                float n = Mathf.PerlinNoise(z * 0.7f + 3f, i * 0.45f) * 0.7f + Mathf.PerlinNoise(z * 2.1f, i * 1.3f + 9f) * 0.3f;
                float push = roughness * n * (p.y < 0.2f ? 0.3f : 1f);   // siempre hacia afuera: no tapa las obras
                Vector2 q = (p + Outward(p) * push) * k;
                var at = new Vector3(cx + q.x, floorY + q.y, z);
                verts.Add(at);
                if (j == 0) ringIn.Add(at);
                if (j == nz - 1) ringOut.Add(at);
                uvs.Add(new Vector2(arc / 2.5f, z / 2.5f));
                float shade = 0.65f + 0.45f * (1f - n);   // lo hundido mas oscuro
                cols.Add(wallColor * shade);
            }
        }
        for (int j = 0; j < nz - 1; j++)
            for (int i = 0; i < np - 1; i++)
            {
                int a = j * np + i, b = a + np;
                tris.AddRange(new[] { a, a + 1, b, a + 1, b + 1, b });
            }
        Make("Cueva_Paredes", verts, uvs, cols, tris, mat);
    }

    void BuildFloor(Material mat)
    {
        var verts = new List<Vector3>(); var uvs = new List<Vector2>(); var cols = new List<Color>(); var tris = new List<int>();
        int nz = Mathf.CeilToInt((z1 - z0) / 1f) + 1;
        for (int j = 0; j < nz; j++)
        {
            float z = Mathf.Lerp(z0, z1, (float)j / (nz - 1));
            for (int i = 0; i <= 4; i++)
            {
                float hw = halfWidth * Flare(z) + 0.2f;
                float x = cx - hw + hw * 2f * i / 4f;
                verts.Add(new Vector3(x, floorY + 0.015f, z));
                uvs.Add(new Vector2(x / 4f, z / 2f));
                cols.Add(new Color(0.45f, 0.36f, 0.28f) * (0.8f + 0.4f * Mathf.PerlinNoise(x, z)));
            }
        }
        for (int j = 0; j < nz - 1; j++)
            for (int i = 0; i < 4; i++)
            {
                int a = j * 5 + i, b = a + 5;
                tris.AddRange(new[] { a, b, a + 1, a + 1, b, b + 1 });
            }
        Make("Cueva_Piso", verts, uvs, cols, tris, mat);
    }

    // ---------------------------------------------------------------- la montaña

    /// <summary>Alto de la montaña (sobre el piso) en un punto, sin el corte del camino.</summary>
    float MountainAt(float x, float z)
    {
        float ax = Mathf.Abs(x - cx);
        float core = halfWidth * mouthScale + 3f;
        float across = 1f - Mathf.SmoothStep(0f, 1f, (ax - core) / Mathf.Max(1f, mountainWidth - core));
        float along = Mathf.SmoothStep(0f, 1f, (z - (z0 - mountainFoot)) / (mountainFoot + 1.5f))
                    * (1f - Mathf.SmoothStep(0f, 1f, (z - (z1 - 1.5f)) / (mountainFoot + 1.5f)));
        float big = 0.7f + 0.6f * Mathf.PerlinNoise(x * 0.06f + 5f, z * 0.06f);
        float crag = (Mathf.PerlinNoise(x * 0.22f, z * 0.22f + 30f) - 0.5f) * 3f + (Mathf.PerlinNoise(x * 0.7f + 9f, z * 0.7f) - 0.5f) * 0.8f;
        float h = mountainHeight * across * along * big + crag * across * along;
        return h - (1f - across) * 4f;   // los bordes bajan hasta debajo del terreno (y del lago)
    }

    // Afuera de la cueva el camino corta la montaña: un valle que lleva a la boca (y a Saturno al salir)
    float Cut(float x, float z)
    {
        if (z >= z0 && z <= z1) return 1f;
        float hw = halfWidth * (z < z0 ? mouthScale : 1f + (mouthScale - 1f) * 0.6f) + 0.4f;
        return Mathf.SmoothStep(0f, 1f, (Mathf.Abs(x - cx) - hw) / 4f);
    }

    void BuildMountain(Material mat)
    {
        var verts = new List<Vector3>(); var uvs = new List<Vector2>(); var cols = new List<Color>(); var tris = new List<int>();

        // Lineas de la grilla: pasan justo por la entrada y la salida
        var zs = new List<float>();
        void Span(float from, float to, float step) { int cnt = Mathf.Max(1, Mathf.CeilToInt((to - from) / step)); for (int k = 0; k < cnt; k++) zs.Add(Mathf.Lerp(from, to, (float)k / cnt)); }
        Span(z0 - mountainFoot - 1f, z0, 1f);
        Span(z0, z1, 1f);
        Span(z1, z1 + mountainFoot + 1f, 1f);
        zs.Add(z1 + mountainFoot + 1f);
        int nx = Mathf.CeilToInt(mountainWidth * 2f) + 1, nz = zs.Count;
        var light = new Vector3(-0.5f, 1f, -0.35f).normalized;
        for (int j = 0; j < nz; j++)
            for (int i = 0; i < nx; i++)
            {
                float x = cx - mountainWidth + i * (mountainWidth * 2f) / (nx - 1), z = zs[j];
                float h = MountainAt(x, z) * Cut(x, z);
                verts.Add(new Vector3(x, floorY + h - 0.3f, z));
                uvs.Add(new Vector2(x / 3f, (h + z) / 3f));
                float dx = MountainAt(x + 0.7f, z) * Cut(x + 0.7f, z) - MountainAt(x - 0.7f, z) * Cut(x - 0.7f, z);
                float dz = MountainAt(x, z + 0.7f) * Cut(x, z + 0.7f) - MountainAt(x, z - 0.7f) * Cut(x, z - 0.7f);
                Vector3 nrm = new Vector3(-dx / 1.4f, 1f, -dz / 1.4f).normalized;
                float steep = 1f - nrm.y;
                float patch = Mathf.PerlinNoise(x * 0.15f + 2f, z * 0.15f + 7f);
                Color c = Color.Lerp(rockDark, rockLight, Mathf.Clamp01(patch * 0.8f + Mathf.Clamp01(h / mountainHeight) * 0.4f));
                c = Color.Lerp(c, rockGreen, Mathf.Clamp01(1f - steep * 3f) * 0.6f * (1f - Mathf.Clamp01(h / mountainHeight)));   // pasto donde es plano
                c *= 0.55f + 0.65f * Mathf.Max(0f, Vector3.Dot(nrm, light));
                c.a = 1f;
                cols.Add(c);
            }
        float slit = halfWidth * mouthScale + 0.6f;
        for (int j = 0; j < nz - 1; j++)
            for (int i = 0; i < nx - 1; i++)
            {
                // La pared que cruzaria la boca (de la entrada o de la salida) no va: ahi estan las fachadas
                float mx = Mathf.Abs(cx - mountainWidth + (i + 0.5f) * (mountainWidth * 2f) / (nx - 1) - cx);
                bool across = (zs[j] < z0 && zs[j + 1] >= z0) || (zs[j] <= z1 && zs[j + 1] > z1);
                if (across && mx < slit) continue;
                int a = j * nx + i, b = a + nx;
                tris.AddRange(new[] { a, b, a + 1, a + 1, b, b + 1 });
            }

        Facade(verts, uvs, cols, tris, ringIn, z0, -1f);
        Facade(verts, uvs, cols, tris, ringOut, z1, 1f);

        outside = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        outsidePos = verts.ToArray();
        outsideBase = cols.ToArray();
        outsideNow = new Color[outsidePos.Length];
        outside.SetVertices(verts); outside.SetUVs(0, uvs); outside.SetColors(cols); outside.SetTriangles(tris, 0);
        outside.RecalculateBounds();
        var go = new GameObject("Cueva_Montaña");
        go.transform.SetParent(transform, false);
        go.AddComponent<MeshFilter>().sharedMesh = outside;
        var r = go.AddComponent<MeshRenderer>();
        r.sharedMaterial = mat;
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        r.receiveShadows = false;
    }

    // La pared de roca alrededor de la boca: del borde del tunel hacia afuera, con salientes
    void Facade(List<Vector3> verts, List<Vector2> uvs, List<Color> cols, List<int> tris, List<Vector3> ring, float z, float facing)
    {
        if (ring.Count < 2) return;
        float hw = halfWidth * mouthScale + 3.2f;
        Vector3 center = new Vector3(cx, floorY + wallHeight * 0.45f, z);
        int start = verts.Count, n = ring.Count;
        var light = new Vector3(-0.5f, 0.8f, -0.4f * -facing).normalized;
        for (int i = 0; i < n; i++)
        {
            Vector3 inner = ring[i];
            Vector3 dir = inner - center; dir.z = 0f;
            if (dir.sqrMagnitude < 0.01f) dir = Vector3.up;
            dir.Normalize();
            // Hasta el borde: a los costados el ancho de la fachada, arriba el alto de la montaña en la boca
            float tx = Mathf.Abs(dir.x) > 0.001f ? hw / Mathf.Abs(dir.x) : 1e4f;
            float topY = floorY + MountainAt(cx + dir.x * 2f, z - facing * 0.5f) + 0.8f;
            float ty = dir.y > 0.001f ? (topY - center.y) / dir.y : (dir.y < -0.001f ? (floorY - 0.4f - center.y) / dir.y : 1e4f);
            float t = Mathf.Min(tx, ty);
            Vector3 outer = center + dir * t;
            float nz = Mathf.PerlinNoise(i * 0.35f + z, 3f);
            float nz2 = Mathf.PerlinNoise(i * 0.8f, z + 11f);
            Vector3 mid = Vector3.Lerp(inner, outer, 0.3f + 0.15f * nz2);
            mid.z = z + facing * (0.9f + 1.4f * nz);          // saliente de roca hacia afuera
            Vector3 lip = Vector3.Lerp(inner, mid, 0.35f);
            lip.z = z + facing * 0.5f * nz;
            outer.z = z + facing * (0.2f + 1.2f * nz2);
            Vector3[] row = { inner, lip, mid, outer };
            for (int k = 0; k < 4; k++)
            {
                verts.Add(row[k]);
                uvs.Add(new Vector2(row[k].x / 2.5f, row[k].y / 2.5f));
                float shade = k == 0 ? 0.45f : (k == 1 ? 0.7f : 0.85f + 0.3f * nz);
                Color c = Color.Lerp(rockDark, rockLight, 0.35f + 0.5f * nz2) * shade;
                c.a = 1f;
                cols.Add(c);
            }
        }
        for (int i = 0; i < n - 1; i++)
            for (int k = 0; k < 3; k++)
            {
                int a = start + i * 4 + k, b = a + 4;
                if (facing < 0f) tris.AddRange(new[] { a, a + 1, b, b, a + 1, b + 1 });
                else tris.AddRange(new[] { a, b, a + 1, b, b + 1, a + 1 });
            }
    }

    // Tinte de la epoca y niebla sobre la montaña (como el terreno)
    void UpdateOutsideColors()
    {
        if (outside == null) return;
        Vector3 eye = gallery.RigPosition;
        float appear = gallery.appear;
        for (int i = 0; i < outsidePos.Length; i++)
        {
            Color c = outsideBase[i];
            if (environment != null)
            {
                Color tint = environment.ZoneTint(outsidePos[i]);
                c = new Color(c.r * tint.r, c.g * tint.g, c.b * tint.b, 1f);
                c = environment.Fogged(c, outsidePos[i]);
            }
            outsideNow[i] = c * appear;
        }
        outside.colors = outsideNow;
    }

    void Make(string label, List<Vector3> v, List<Vector2> uv, List<Color> c, List<int> t, Material mat)
    {
        var mesh = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        mesh.SetVertices(v); mesh.SetUVs(0, uv); mesh.SetColors(c); mesh.SetTriangles(t, 0);
        mesh.RecalculateBounds();
        var go = new GameObject(label);
        go.transform.SetParent(transform, false);
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        var r = go.AddComponent<MeshRenderer>();
        r.sharedMaterial = mat;
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        r.receiveShadows = false;
    }

    // ---------------------------------------------------------------- antorcha

    void BuildTorch(Vector3 c)
    {
        var rockShader = Resources.Load<Shader>("Shaders/GoyaTerreno");
        var mat = new Material(rockShader != null ? rockShader : Shader.Find("Sprites/Default")) { mainTexture = Resources.Load<Texture2D>("Paisaje/pinceladas") };

        // Una piedra en la entrada, con la antorcha apoyada arriba
        Vector3 at = new Vector3(cx + 1.1f, floorY, z0 - 1.2f);
        stand = Box("Antorcha_Piedra", at + Vector3.up * 0.45f, new Vector3(0.45f, 0.9f, 0.4f), outsideRock * 1.1f, mat);
        standTop = at + Vector3.up * 0.9f;

        torch = new GameObject("Antorcha").transform;
        torch.SetParent(transform, false);
        var handle = Box("Mango", Vector3.zero, new Vector3(0.04f, 0.55f, 0.04f), new Color(0.35f, 0.24f, 0.15f), mat);
        handle.SetParent(torch, false);
        handle.localPosition = Vector3.up * 0.275f;
        var head = Box("Trapo", Vector3.zero, new Vector3(0.08f, 0.12f, 0.08f), new Color(0.18f, 0.14f, 0.12f), mat);
        head.SetParent(torch, false);
        head.localPosition = Vector3.up * 0.58f;
        fireAt = new GameObject("Fuego").transform;
        fireAt.SetParent(torch, false);
        fireAt.localPosition = Vector3.up * 0.66f;
        torch.position = standTop;
        torch.rotation = Quaternion.Euler(0f, 0f, 12f);

        fire = MakeFire(fireAt);

        // Cartel: que hacer con la antorcha
        var t = new GameObject("Antorcha_Cartel");
        t.transform.SetParent(transform, false);
        t.transform.position = standTop + Vector3.up * 1.0f;
        hint = t.AddComponent<TextMesh>();
        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        hint.font = font;
        t.GetComponent<MeshRenderer>().sharedMaterial = font.material;
        hint.text = "Levantá los dos brazos\ncon las manos abiertas\npara encender la antorcha.";
        hint.fontSize = 48;
        hint.characterSize = 0.012f;
        hint.anchor = TextAnchor.MiddleCenter;
        hint.alignment = TextAlignment.Center;
        hint.color = new Color(1f, 0.9f, 0.75f, 0f);

        // Cartel para soltarla al salir (va delante de la vista)
        var d = new GameObject("Antorcha_CartelSoltar");
        d.transform.SetParent(transform, false);
        dropHint = d.AddComponent<TextMesh>();
        dropHint.font = font;
        d.GetComponent<MeshRenderer>().sharedMaterial = font.material;
        dropHint.text = "Levantá los dos brazos\ncon las manos abiertas\npara soltar la antorcha.";
        dropHint.fontSize = 48;
        dropHint.characterSize = 0.008f;
        dropHint.anchor = TextAnchor.MiddleCenter;
        dropHint.alignment = TextAlignment.Center;
        dropHint.color = new Color(1f, 0.9f, 0.75f, 0f);
    }

    Transform Box(string label, Vector3 pos, Vector3 size, Color col, Material mat)
    {
        var mesh = new Mesh();
        var v = new List<Vector3>(); var c = new List<Color>(); var uv = new List<Vector2>(); var t = new List<int>();
        Vector3 h = size * 0.5f;
        Vector3[] n = { Vector3.right, Vector3.left, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        foreach (var f in n)
        {
            Vector3 a = f.x != 0f ? Vector3.up : Vector3.right;
            Vector3 b = Vector3.Cross(f, a);
            int i = v.Count;
            foreach (var s in new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(-1, 1), new Vector2(1, 1) })
            {
                Vector3 p = f + a * s.x + b * s.y;
                v.Add(new Vector3(p.x * h.x, p.y * h.y, p.z * h.z));
                uv.Add(new Vector2(s.x * 0.5f + 0.5f, s.y * 0.5f + 0.5f));
                c.Add(col * (0.8f + 0.25f * Vector3.Dot(f, new Vector3(-0.4f, 0.8f, -0.4f))));
            }
            t.AddRange(new[] { i, i + 2, i + 1, i + 1, i + 2, i + 3 });
        }
        mesh.SetVertices(v); mesh.SetColors(c); mesh.SetUVs(0, uv); mesh.SetTriangles(t, 0);
        mesh.RecalculateBounds();
        var go = new GameObject(label);
        go.transform.SetParent(transform, false);
        go.transform.position = pos;
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>().sharedMaterial = mat;
        return go.transform;
    }

    static ParticleSystem MakeFire(Transform parent)
    {
        var go = new GameObject("Llamas");
        go.transform.SetParent(parent, false);
        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.35f, 0.7f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.2f, 0.6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.04f, 0.11f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 200;
        main.gravityModifier = -0.15f;
        var em = ps.emission; em.rateOverTime = 0f;
        var sh = ps.shape; sh.shapeType = ParticleSystemShapeType.Sphere; sh.radius = 0.03f;
        var col = ps.colorOverLifetime; col.enabled = true;
        var g = new Gradient();
        g.SetKeys(new[] { new GradientColorKey(new Color(1f, 0.95f, 0.6f), 0f), new GradientColorKey(new Color(1f, 0.5f, 0.1f), 0.4f), new GradientColorKey(new Color(0.5f, 0.08f, 0.02f), 1f) },
                  new[] { new GradientAlphaKey(0.9f, 0f), new GradientAlphaKey(0.7f, 0.5f), new GradientAlphaKey(0f, 1f) });
        col.color = g;
        var size = ps.sizeOverLifetime; size.enabled = true; size.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0.2f));
        var r = go.GetComponent<ParticleSystemRenderer>();
        var mat = new Material(Shader.Find("Sprites/Default"));
        mat.mainTexture = SoftDot();
        r.sharedMaterial = mat;
        r.sortingOrder = 7;
        ps.Play();
        return ps;
    }

    static Texture2D SoftDot()
    {
        int n = 32;
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float dx = (x + 0.5f) / n * 2f - 1f, dy = (y + 0.5f) / n * 2f - 1f;
                float a = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a * a));
            }
        tex.Apply();
        return tex;
    }

    // ---------------------------------------------------------------- cada cuadro

    void Update()
    {
        if (!built || gallery == null) return;
        Vector3 rig = gallery.RigPosition;
        float dt = Time.deltaTime;
        colorTimer -= dt;
        if (colorTimer <= 0f) { colorTimer = 0.2f; UpdateOutsideColors(); }
        UpdateRocks(rig, dt);

        if (!lit)
        {
            float dist = Vector3.Distance(new Vector3(rig.x, standTop.y, rig.z), standTop);
            bool near = dist < reach;
            var hc = hint.color;
            hc.a = Mathf.MoveTowards(hc.a, near ? 1f : 0f, dt * 1.5f);
            hint.color = hc;
            var head = gallery.Head;
            if (head != null) hint.transform.rotation = Quaternion.LookRotation(hint.transform.position - head.position);
            if (gallery.FormGestures != lastGestures)
            {
                lastGestures = gallery.FormGestures;
                if (near) Light();
            }
        }
        else if (dropped)
        {
            // Tirada en el piso: el fuego se va apagando
            torch.position = Vector3.Lerp(torch.position, dropAt, 1f - Mathf.Exp(-8f * dt));
            torch.rotation = Quaternion.Slerp(torch.rotation, Quaternion.Euler(0f, 30f, 88f), 1f - Mathf.Exp(-8f * dt));
            dropFade = Mathf.MoveTowards(dropFade, 0f, dt / 4f);
            var em = fire.emission; em.rateOverTime = 70f * dropFade * dropFade;
            if (fireSound != null) fireSound.volume = 0.35f * dropFade;
            SetLight(fireAt.position, flicker * dropFade);
            FadeDropHint(false, dt);
        }
        else
        {
            // Al salir de la cueva se puede soltar con el mismo gesto (o E en la PC)
            bool atExit = rig.z > z1 - 1.5f;
            bool out_ = flame <= 0f;
            if (dropHint != null) dropHint.text = out_ ? "Una roca apagó la antorcha.\nLevantá los dos brazos\npara volver a prenderla." : "Levantá los dos brazos\ncon las manos abiertas\npara soltar la antorcha.";
            FadeDropHint(atExit || out_, dt);
            if (gallery.FormGestures != lastGestures)
            {
                lastGestures = gallery.FormGestures;
                if (out_) Relight();
                else if (atExit) { Drop(); return; }
            }

            // La antorcha en la mano (o, sin manos, abajo a la derecha de la vista)
            Vector3 palm;
            var head = gallery.Head;
            Vector3 target;
            if (GoyaHands.Palm(out palm)) target = palm + Vector3.down * 0.15f;
            else if (head != null)
            {
                Vector3 f = head.forward; f.y = 0f; f = f.sqrMagnitude > 0.001f ? f.normalized : Vector3.forward;
                Vector3 r = Vector3.Cross(Vector3.up, f);
                target = head.position + f * 0.45f + r * 0.28f + Vector3.down * 0.55f;
            }
            else target = rig + Vector3.down * 0.5f;
            torch.position = Vector3.Lerp(torch.position, target, 1f - Mathf.Exp(-10f * dt));
            torch.rotation = Quaternion.Slerp(torch.rotation, Quaternion.Euler(-10f, 0f, 8f * Mathf.Sin(Time.time * 1.3f)), 1f - Mathf.Exp(-6f * dt));

            flicker = 0.85f + 0.3f * Mathf.PerlinNoise(Time.time * 7f, 0.3f);
            SetLight(fireAt.position, flicker);

            // Las Pinturas Negras aparecen de a poco donde se las alumbra
            foreach (var p in hidden)
                if (p.visibility < 1f && Vector3.Distance(fireAt.position, p.root.position) < appearDistance)
                    p.visibility = Mathf.Min(1f, p.visibility + dt / Mathf.Max(0.1f, appearTime));
            // Encontrada: alumbrada de cerca (y, si es negra, ya aparecida)
            foreach (var p in inside)
                if (!found.Contains(p) && p.visibility >= 1f && Vector3.Distance(fireAt.position, p.root.position) < appearDistance)
                    found.Add(p);
        }
    }

    // ---------------------------------------------------------------- rocas que caen

    // Se usa la fisica de Unity (PhysX): cada roca es un cuerpo rigido con gravedad, rebote, rozamiento y choques
    // con las otras rocas, el piso, las paredes y la persona. Nada de esto esta animado: los escombros se amontonan,
    // ruedan y se empujan segun como cayeron.

    void UpdateRocks(Vector3 rig, float dt)
    {
        if (!colliders) BuildColliders();

        // Grietas: avisan con retumbe y polvo, y despues sueltan las rocas
        for (int i = cracks.Count - 1; i >= 0; i--)
        {
            var c = cracks[i];
            c.time += dt;
            if (c.time < rockWarning) continue;
            for (int k = 0; k < c.count; k++)
            {
                Vector3 at = c.at + new Vector3(Random.Range(-0.45f, 0.45f), Random.Range(-0.1f, 0.1f), Random.Range(-0.45f, 0.45f));
                var r = NewRock(at, Random.Range(rockSize.x, rockSize.y), false);
                r.body.linearVelocity = new Vector3(Random.Range(-0.3f, 0.3f), -Random.Range(0f, 1f), Random.Range(-0.3f, 0.3f));
                r.body.angularVelocity = Random.insideUnitSphere * 4f;
            }
            if (c.dust != null) Destroy(c.dust.gameObject, 2f);
            cracks.RemoveAt(i);
        }

        // Escombros de mas: los mas viejos se hunden en el piso y desaparecen
        int extra = rocks.Count - maxRubble;
        for (int i = rocks.Count - 1; i >= 0; i--)
        {
            var r = rocks[i];
            if (r.body == null) { rocks.RemoveAt(i); continue; }
            r.age += dt;
            bool old = r.age > 40f || (extra > 0 && r.fragment && r.age > 4f);
            if (old || r.body.position.y < floorY - 2f)
            {
                if (old && r.body.position.y > floorY - 0.6f)
                {
                    r.body.isKinematic = true;
                    r.body.transform.position += Vector3.down * dt * 0.25f;
                    continue;
                }
                extra--;
                Destroy(r.body.gameObject);
                rocks.RemoveAt(i);
            }
        }

        if (!fallingRocks || !lit || dropped || !Inside(rig)) return;
        var game = GoyaGame.Instance;
        if (game != null && game.timer && !game.Playing) return;
        rockTimer -= dt;
        if (rockTimer > 0f || cracks.Count > 0) return;
        rockTimer = Random.Range(rockInterval.x, Mathf.Max(rockInterval.x, rockInterval.y));

        // Se desprenden donde vas a estar: adelante en la direccion en que miras (mas lejos si corres)
        var head = gallery.Head;
        Vector3 f = head != null ? head.forward : Vector3.forward; f.y = 0f;
        f = f.sqrMagnitude > 0.001f ? f.normalized : Vector3.forward;
        float ahead = gallery.Running ? 4f : 2.2f;
        Vector3 spot = rig + f * ahead + new Vector3(Random.Range(-0.6f, 0.6f), 0f, 0f);
        float hw = halfWidth * Flare(spot.z) - 0.5f;
        spot.x = Mathf.Clamp(spot.x, cx - hw, cx + hw);
        spot.z = Mathf.Clamp(spot.z, z0 + 1f, z1 - 1f);
        spot.y = floorY + wallHeight + vaultHeight * 0.55f;
        StartCrack(spot, Random.Range(rocksPerFall.x, rocksPerFall.y + 1));
    }

    // Choques que importan, despues de cada paso de la fisica
    void FixedUpdate()
    {
        if (!built || gallery == null) return;
        Vector3 rig = gallery.RigPosition;
        FollowBody(rig);
        for (int i = rocks.Count - 1; i >= 0; i--)
        {
            var r = rocks[i];
            if (r.body == null || r.body.isKinematic) continue;
            Vector3 v = r.body.linearVelocity;

            // Te pega: una roca rapida que toca la capsula de la persona
            if (!r.hit && r.lastVel.magnitude > 2f)
            {
                Vector3 p = r.body.position;
                float y = Mathf.Clamp(p.y, floorY + 0.3f, rig.y);
                Vector3 d = p - new Vector3(rig.x, y, rig.z);
                if (d.magnitude < 0.3f + r.size * 0.5f) { r.hit = true; Hit(p); }
            }

            // Se parte: venia cayendo rapido y freno de golpe contra algo
            if (!r.fragment && r.lastVel.y < -breakSpeed && v.y > r.lastVel.y * 0.4f)
            {
                Shatter(r, r.lastVel);
                rocks.RemoveAt(i);
                continue;
            }
            // Golpe de un escombro contra el piso o contra otra roca: suena mas bajo
            if (r.fragment && (v - r.lastVel).magnitude > 2.5f && r.age > 0.15f)
                PlayAt(hitClips, r.body.position, 0.25f);
            r.lastVel = v;
        }
    }

    void BuildColliders()
    {
        colliders = true;
        stoneMat = new PhysicsMaterial("Piedra")
        {
            bounciness = bounce,
            dynamicFriction = 0.6f,
            staticFriction = 0.8f,
            bounceCombine = PhysicsMaterialCombine.Average,
            frictionCombine = PhysicsMaterialCombine.Average
        };
        var root = new GameObject("Cueva_Fisica").transform;
        root.SetParent(transform, false);
        // Piso
        var floor = new GameObject("Piso").AddComponent<BoxCollider>();
        floor.transform.SetParent(root, false);
        float len = z1 - z0 + 8f;
        floor.transform.position = new Vector3(cx, floorY - 0.5f, (z0 + z1) * 0.5f);
        floor.size = new Vector3(halfWidth * mouthScale * 2f + 6f, 1f, len);
        floor.sharedMaterial = stoneMat;
        // Paredes: una tabla por metro a cada lado, siguiendo el ancho del tunel (que se abre en la boca)
        for (float z = z0 - 2f; z < z1 + 2f; z += 1f)
        {
            float w = halfWidth * Flare(z + 0.5f);
            for (int side = -1; side <= 1; side += 2)
            {
                var wall = new GameObject("Pared").AddComponent<BoxCollider>();
                wall.transform.SetParent(root, false);
                wall.transform.position = new Vector3(cx + side * (w + 0.25f), floorY + wallHeight * 0.5f + 0.5f, z + 0.5f);
                wall.size = new Vector3(0.5f, wallHeight + vaultHeight + 1f, 1.05f);
                wall.sharedMaterial = stoneMat;
            }
        }
        // La persona: capsula que se mueve con el rig y empuja los escombros al caminar
        var me = new GameObject("Cueva_Persona");
        me.transform.SetParent(root, false);
        var cap = me.AddComponent<CapsuleCollider>();
        cap.radius = 0.28f;
        cap.height = gallery.RigPosition.y - floorY;
        cap.direction = 1;
        body = me.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        me.transform.position = FeetCenter(gallery.RigPosition);
    }

    Vector3 FeetCenter(Vector3 rig) { return new Vector3(rig.x, (rig.y + floorY) * 0.5f, rig.z); }

    void FollowBody(Vector3 rig)
    {
        if (body == null) return;
        body.MovePosition(FeetCenter(rig));
    }

    void StartCrack(Vector3 at, int count)
    {
        LoadRockAssets();
        var c = new Crack { at = at, count = Mathf.Max(1, count) };

        // Aviso: polvo y piedritas que caen del techo, y el retumbe
        var go = new GameObject("Roca_Polvo");
        go.transform.SetParent(transform, false);
        go.transform.position = at + Vector3.up * 0.4f;
        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.duration = rockWarning + 0.3f;
        main.loop = false;
        main.startLifetime = 1.2f;
        main.startSpeed = 0.2f;
        main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.07f);
        main.startColor = new Color(0.75f, 0.62f, 0.45f, 0.8f);
        main.gravityModifier = 0.6f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        var em = ps.emission; em.rateOverTime = 30f + 15f * c.count;
        var shp = ps.shape; shp.shapeType = ParticleSystemShapeType.Circle; shp.radius = 0.5f; shp.rotation = new Vector3(90f, 0f, 0f);
        go.GetComponent<ParticleSystemRenderer>().sharedMaterial = dustMat;
        ps.Play();
        c.dust = ps;
        PlayAt(rumbleClips, go.transform.position, 0.9f);
        cracks.Add(c);
    }

    void LoadRockAssets()
    {
        if (rockMat != null) return;
        var sh = Resources.Load<Shader>("Shaders/GoyaCueva");
        rockMat = new Material(sh != null ? sh : Shader.Find("Sprites/Default")) { mainTexture = Resources.Load<Texture2D>("Paisaje/cueva_pared") };
        dustMat = new Material(Shader.Find("Sprites/Default")) { mainTexture = SoftDot() };
        rumbleClips = new[] { Resources.Load<AudioClip>("Sonidos/roca_derrumbe_1"), Resources.Load<AudioClip>("Sonidos/roca_derrumbe_2"), Resources.Load<AudioClip>("Sonidos/roca_derrumbe_3") };
        hitClips = new[] { Resources.Load<AudioClip>("Sonidos/roca_golpe_1"), Resources.Load<AudioClip>("Sonidos/roca_golpe_2"), Resources.Load<AudioClip>("Sonidos/roca_golpe_3") };
    }

    Rock NewRock(Vector3 at, float size, bool fragment)
    {
        LoadRockAssets();
        var go = RockMesh(size);
        go.transform.position = at;
        go.transform.rotation = Random.rotation;
        var col = go.AddComponent<SphereCollider>();
        col.radius = size * 0.42f;
        col.sharedMaterial = stoneMat;
        var rb = go.AddComponent<Rigidbody>();
        rb.mass = 2600f * size * size * size;   // densidad de la piedra
        rb.linearDamping = 0.05f;
        rb.angularDamping = 1.5f;               // no rueda para siempre
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        var r = new Rock { body = rb, size = size, fragment = fragment };
        rocks.Add(r);
        return r;
    }

    // La roca se parte en pedazos que salen despedidos con la energia que traia
    void Shatter(Rock r, Vector3 vel)
    {
        Vector3 at = r.body.position;
        float size = r.size;
        bool wasHit = r.hit;
        Destroy(r.body.gameObject);
        PlayAt(hitClips, at, 1f);
        Puff(at, size);
        int n = Random.Range(fragments.x, fragments.y + 1);
        for (int k = 0; k < n; k++)
        {
            float s = size * Random.Range(0.3f, 0.55f);
            Vector3 dir = Random.onUnitSphere; dir.y = Mathf.Abs(dir.y) * 0.6f + 0.2f;
            var f = NewRock(at + dir * size * 0.3f, s, true);
            f.body.linearVelocity = new Vector3(vel.x, 0f, vel.z) * 0.4f + dir.normalized * (-vel.y) * Random.Range(0.25f, 0.5f);
            f.body.angularVelocity = Random.insideUnitSphere * 10f;
            f.hit = wasHit;   // la misma roca no te pega dos veces
        }
    }

    void Puff(Vector3 at, float size)
    {
        var go = new GameObject("Roca_Golpe");
        go.transform.SetParent(transform, false);
        go.transform.position = at;
        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.duration = 0.2f;
        main.loop = false;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.6f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.4f, 1.6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.18f);
        main.startColor = new Color(0.7f, 0.58f, 0.42f, 0.6f);
        main.gravityModifier = 0.15f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        var em = ps.emission; em.rateOverTime = 0f;
        em.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)(20 + size * 60)) });
        var shp = ps.shape; shp.shapeType = ParticleSystemShapeType.Hemisphere; shp.radius = size * 0.5f; shp.rotation = new Vector3(-90f, 0f, 0f);
        go.GetComponent<ParticleSystemRenderer>().sharedMaterial = dustMat;
        ps.Play();
        Destroy(go, 2.5f);
    }

    // Una roca te pego: la llama de la antorcha se achica (y con el reloj prendido, perdes tiempo)
    void Hit(Vector3 at)
    {
        PlayAt(hitClips, at, 1f);
        var game = GoyaGame.Instance;
        if (game != null && game.timer) game.Penalty(rockPenalty, "Te golpeó una roca");
        if (!lit || dropped || flame <= 0f) return;
        flame = Mathf.Max(0f, flame - hitFlame);
        if (flame <= 0.001f) Extinguish();
        else { var em = fire.emission; em.rateOverTime = 70f * Mathf.Lerp(0.3f, 1f, flame); }
    }

    void Extinguish()
    {
        flame = 0f;
        var em = fire.emission; em.rateOverTime = 0f;
        if (fireSound != null) fireSound.volume = 0f;
    }

    void Relight()
    {
        flame = 0.67f;   // vuelve, pero mas chica que al principio
        var em = fire.emission; em.rateOverTime = 70f * flame;
        if (fireSound != null) fireSound.volume = 0.35f;
        var start = Resources.Load<AudioClip>("Sonidos/fuego_encendido");
        if (start != null) AudioSource.PlayClipAtPoint(start, fireAt.position, 0.8f);
    }

    static void PlayAt(AudioClip[] clips, Vector3 pos, float volume)
    {
        if (clips == null || clips.Length == 0) return;
        var c = clips[Random.Range(0, clips.Length)];
        if (c != null) AudioSource.PlayClipAtPoint(c, pos, volume);
    }

    // Una piedra irregular (esfera deformada), con la textura de las paredes
    GameObject RockMesh(float size)
    {
        int la = 8, lo = 12;
        var v = new List<Vector3>(); var uv = new List<Vector2>(); var c = new List<Color>(); var t = new List<int>();
        float seed = Random.value * 50f;
        for (int i = 0; i <= la; i++)
            for (int j = 0; j <= lo; j++)
            {
                float th = Mathf.PI * i / la, ph = 2f * Mathf.PI * j / lo;
                Vector3 n = new Vector3(Mathf.Sin(th) * Mathf.Cos(ph), Mathf.Cos(th), Mathf.Sin(th) * Mathf.Sin(ph));
                float k = 0.75f + 0.5f * Mathf.PerlinNoise(n.x * 1.7f + seed, n.y * 1.7f + n.z * 1.3f);
                v.Add(n * size * 0.5f * k);
                uv.Add(new Vector2((float)j / lo, (float)i / la));
                c.Add(wallColor * (0.6f + 0.4f * k));
            }
        for (int i = 0; i < la; i++)
            for (int j = 0; j < lo; j++)
            {
                int a = i * (lo + 1) + j, b = a + lo + 1;
                t.AddRange(new[] { a, b, a + 1, a + 1, b, b + 1 });
            }
        var mesh = new Mesh();
        mesh.SetVertices(v); mesh.SetUVs(0, uv); mesh.SetColors(c); mesh.SetTriangles(t, 0);
        mesh.RecalculateBounds();
        var go = new GameObject("Roca");
        go.transform.SetParent(transform, false);
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>().sharedMaterial = rockMat;
        return go;
    }

    void FadeDropHint(bool show, float dt)
    {
        if (dropHint == null) return;
        var c = dropHint.color;
        c.a = Mathf.MoveTowards(c.a, show ? 1f : 0f, dt * 1.5f);
        dropHint.color = c;
        dropHint.gameObject.SetActive(c.a > 0.001f);
        var head = gallery.Head;
        if (head == null || c.a <= 0.001f) return;
        Vector3 f = head.forward; f.y = 0f; f = f.sqrMagnitude > 0.001f ? f.normalized : Vector3.forward;
        dropHint.transform.position = head.position + f * 1.6f + Vector3.down * 0.35f;
        dropHint.transform.rotation = Quaternion.LookRotation(dropHint.transform.position - head.position);
    }

    /// <summary>Suelta la antorcha: cae al piso y se apaga de a poco.</summary>
    public void Drop()
    {
        if (!lit || dropped) return;
        dropped = true;
        Vector3 p = torch.position;
        dropAt = new Vector3(p.x, floorY + 0.05f, p.z + 0.3f);
    }

    void Light()
    {
        lit = true;
        hint.gameObject.SetActive(false);
        var em = fire.emission;
        em.rateOverTime = 70f;
        var start = Resources.Load<AudioClip>("Sonidos/fuego_encendido");
        var loop = Resources.Load<AudioClip>("Sonidos/fuego_loop");
        if (start != null) AudioSource.PlayClipAtPoint(start, fireAt.position, 0.8f);
        if (loop != null)
        {
            fireSound = fireAt.gameObject.AddComponent<AudioSource>();
            fireSound.clip = loop;
            fireSound.loop = true;
            fireSound.spatialBlend = 1f;
            fireSound.minDistance = 0.8f;
            fireSound.volume = 0.35f;
            fireSound.Play();
        }
    }

    void SetLight(Vector3 pos, float intensity)
    {
        intensity *= flame;
        float range = torchRange * Mathf.Lerp(0.4f, 1f, flame);
        Shader.SetGlobalVector("_GoyaTorch", new Vector4(pos.x, pos.y, pos.z, intensity > 0f ? range : 0.0001f));
        Shader.SetGlobalColor("_GoyaTorchColor", torchColor * intensity);
        float appear = gallery != null ? gallery.appear : 1f;
        Shader.SetGlobalColor("_GoyaCaveAmbient", ambient * appear);
    }
}

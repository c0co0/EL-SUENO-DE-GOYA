using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// La cueva: empieza en la mitad de la zona de la guerra y llega hasta Saturno. Adentro esta oscuro.
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

    [Header("Antorcha")]
    public float torchRange = 5.5f;
    public Color torchColor = new Color(1.6f, 1.0f, 0.5f);
    [Tooltip("Luz que hay en la cueva sin antorcha")]
    public Color ambient = new Color(0.05f, 0.045f, 0.04f);
    [Tooltip("A que distancia de la antorcha hay que estar para encenderla (metros)")]
    public float reach = 3.5f;
    [Tooltip("Distancia a la que hay que alumbrar una Pintura Negra para que aparezca, y cuanto tarda")]
    public float appearDistance = 3.2f;
    public float appearTime = 4f;

    GoyaGallery gallery;
    float z0, z1, floorY, cx;
    bool built, lit;
    int lastGestures;
    readonly List<GoyaGallery.Painting> inside = new List<GoyaGallery.Painting>();
    readonly List<GoyaGallery.Painting> hidden = new List<GoyaGallery.Painting>();
    Transform torch, fireAt, stand;
    ParticleSystem fire;
    AudioSource fireSound;
    TextMesh hint;
    Vector3 standTop;
    float flicker;

    public bool Built { get { return built; } }

    /// <summary>Si un punto esta adentro de la cueva.</summary>
    public bool Inside(Vector3 p)
    {
        return built && Mathf.Abs(p.x - cx) < halfWidth && p.z > z0 && p.z < z1;
    }

    /// <summary>Si un punto del piso cae debajo de la cueva o su entrada (ahi no va pasto).</summary>
    public bool Footprint(float x, float z)
    {
        return built && Mathf.Abs(x - cx) < halfWidth + 0.6f && z > z0 - 0.5f && z < z1 + 0.5f;
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
            p.root.position = new Vector3(cx + side * (halfWidth - 0.1f), eyeY + 0.1f, pz);
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
            BuildMouth(rockMat, z0, -1f);
            BuildMouth(rockMat, z1, 1f);
        }
        BuildTorch(c);
        built = true;
        lastGestures = g.FormGestures;
        SetLight(Vector3.zero, 0f);
    }

    // ---------------------------------------------------------------- forma

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
            float arc = 0f;
            for (int i = 0; i < np; i++)
            {
                if (i > 0) arc += Vector2.Distance(prof[i], prof[i - 1]);
                Vector2 p = prof[i];
                float n = Mathf.PerlinNoise(z * 0.7f + 3f, i * 0.45f) * 0.7f + Mathf.PerlinNoise(z * 2.1f, i * 1.3f + 9f) * 0.3f;
                float push = roughness * n * (p.y < 0.2f ? 0.3f : 1f);   // siempre hacia afuera: no tapa las obras
                Vector2 q = p + Outward(p) * push;
                verts.Add(new Vector3(cx + q.x, floorY + q.y, z));
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
                float x = cx - halfWidth - 0.2f + (halfWidth * 2f + 0.4f) * i / 4f;
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

    // La boca: una pared de roca alrededor de la entrada (tapa el tunel visto desde afuera)
    void BuildMouth(Material mat, float z, float facing)
    {
        var prof = Profile();
        int np = prof.Count;
        var verts = new List<Vector3>(); var uvs = new List<Vector2>(); var cols = new List<Color>(); var tris = new List<int>();
        for (int i = 0; i < np; i++)
        {
            Vector2 p = prof[i] + Outward(prof[i]) * roughness * 0.5f;
            float n = Mathf.PerlinNoise(i * 0.5f + z, 4f);
            Vector2 o = new Vector2(p.x * 2.6f, p.y * 1.5f + 1.2f + 1.5f * n);
            if (prof[i].y < 0.01f) o.y = -0.3f;
            verts.Add(new Vector3(cx + p.x, floorY + p.y, z));
            verts.Add(new Vector3(cx + o.x, floorY + o.y, z + facing * 0.6f * n));
            uvs.Add(new Vector2(p.x / 2.5f, p.y / 2.5f));
            uvs.Add(new Vector2(o.x / 2.5f, o.y / 2.5f));
            cols.Add(outsideRock * 0.7f);
            cols.Add(outsideRock * (0.9f + 0.3f * n));
        }
        for (int i = 0; i < np - 1; i++)
        {
            int a = i * 2;
            tris.AddRange(new[] { a, a + 1, a + 2, a + 2, a + 1, a + 3 });
        }
        Make(facing < 0 ? "Cueva_Entrada" : "Cueva_Salida", verts, uvs, cols, tris, mat);
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
        else
        {
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
        }
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
        Shader.SetGlobalVector("_GoyaTorch", new Vector4(pos.x, pos.y, pos.z, intensity > 0f ? torchRange : 0.0001f));
        Shader.SetGlobalColor("_GoyaTorchColor", torchColor * intensity);
        float appear = gallery != null ? gallery.appear : 1f;
        Shader.SetGlobalColor("_GoyaCaveAmbient", ambient * appear);
    }
}

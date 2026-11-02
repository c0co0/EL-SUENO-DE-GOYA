using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// El piso del camino: un terreno de verdad (no una lamina), plano donde se camina, con lomas a los costados
/// que tapan lo que esta detras, y que baja hasta el lago a lo lejos. Encima, matas de pasto: muchas cerca
/// del camino y alrededor del arbol y las rocas (para que salgan del suelo), y menos y mas grandes a lo lejos.
/// Los colores salen de los fondos de Goya y se van oscureciendo con cada epoca; lo lejano se aclara con
/// la bruma, como en La gallina ciega. GoyaEnvironment lo agrega solo (en lugar de la franja de tierra).
/// </summary>
public class GoyaTerrain : MonoBehaviour
{
    [Header("Forma")]
    public float width = 70f;
    [Tooltip("Metros de terreno antes del comienzo y despues del final del camino")]
    public float before = 15f, after = 30f;
    public float cell = 1f;
    [Tooltip("Mitad del ancho que queda plano (el camino y las obras)")]
    public float flatHalf = 4.5f;
    [Tooltip("Alto de las lomas a los costados")]
    public float hillHeight = 1.8f;
    [Tooltip("Desde que distancia al camino el terreno baja hacia el lago")]
    public float lakeStart = 19f;
    public float lakeDepth = 1.6f;

    [Header("Pasto")]
    public int tufts = 1800;
    [Tooltip("Matas alrededor de cada arbol o roca")]
    public int tuftsPerProp = 45;

    [Header("Colores (de los fondos de Goya)")]
    public Color path = new Color(0.56f, 0.48f, 0.40f);      // camino de La gallina ciega
    public Color grassA = new Color(0.45f, 0.41f, 0.21f);    // pasto claro de Duelo a garrotazos
    public Color grassB = new Color(0.30f, 0.30f, 0.17f);
    public Color dry = new Color(0.55f, 0.50f, 0.34f);       // tierra de La cometa
    [Tooltip("Bruma que aclara lo lejano (perspectiva aerea)")]
    public Color haze = new Color(0.82f, 0.80f, 0.72f);
    public float hazeDistance = 45f;

    GoyaGallery gallery;
    GoyaEnvironment environment;
    float floorY;
    Vector3 origin;
    Mesh ground, grass;
    Color[] groundBase, grassBase, groundNow, grassNow;
    Vector3[] groundPos, grassPos;
    float timer;

    public bool Ready { get { return ground != null; } }

    public void Build(GoyaGallery g, GoyaEnvironment env, float floor)
    {
        gallery = g;
        environment = env;
        floorY = floor;
        origin = new Vector3(g.Center.x, floorY, g.Center.z);

        var shader = Resources.Load<Shader>("Shaders/GoyaTerreno");
        if (shader == null) shader = Shader.Find("Goya/Terreno");
        if (shader == null) { Debug.LogWarning("[Terreno] Falta el shader Goya/Terreno"); return; }

        BuildGround(shader);
        BuildGrass(shader);
        UpdateColors();
    }

    /// <summary>Alto del terreno (metros sobre el piso del camino) en un punto.</summary>
    public float Height(float x, float z)
    {
        float ax = Mathf.Abs(x - origin.x);
        float n = Mathf.PerlinNoise(x * 0.09f + 10f, z * 0.09f) - 0.5f;
        float n2 = Mathf.PerlinNoise(x * 0.25f + 40f, z * 0.25f) - 0.5f;
        // Plano en el camino; lomas suaves a los costados; despues baja hacia el lago
        float hills = Mathf.SmoothStep(0f, 1f, (ax - flatHalf) / 5f) * hillHeight * (0.55f + Mathf.PerlinNoise(z * 0.07f, x * 0.05f + 7f));
        float lake = Mathf.SmoothStep(0f, 1f, (ax - lakeStart) / 6f) * (lakeDepth + hillHeight);
        float bumps = (n * 0.6f + n2 * 0.25f) * Mathf.Clamp01((ax - 1.5f) / 4f);
        return hills - lake + bumps;
    }

    void BuildGround(Shader shader)
    {
        float z0 = gallery.Center.z - before, z1 = gallery.Center.z + gallery.WalkLength + after;
        int nx = Mathf.CeilToInt(width / cell) + 1, nz = Mathf.CeilToInt((z1 - z0) / cell) + 1;
        groundPos = new Vector3[nx * nz];
        groundBase = new Color[nx * nz];
        var uv = new Vector2[nx * nz];
        var light = new Vector3(-0.5f, 1f, 0.35f).normalized;
        for (int j = 0; j < nz; j++)
            for (int i = 0; i < nx; i++)
            {
                float x = origin.x - width / 2f + i * cell, z = z0 + j * cell;
                float h = Height(x, z);
                int k = j * nx + i;
                groundPos[k] = new Vector3(x, floorY + h, z);
                uv[k] = new Vector2(x / 6f, z / 3f);   // pinceladas del suelo de Duelo a garrotazos

                // Color: camino ocre al centro, pasto con manchas, mas seco en lo alto, sombra en las laderas
                float ax = Mathf.Abs(x - origin.x);
                float patch = Mathf.PerlinNoise(x * 0.18f + 3f, z * 0.18f + 9f);
                Color c = Color.Lerp(grassA, grassB, patch);
                c = Color.Lerp(c, dry, Mathf.Clamp01(h / hillHeight) * 0.5f * Mathf.PerlinNoise(x * 0.3f, z * 0.3f + 20f));
                float onPath = 1f - Mathf.SmoothStep(0f, 1f, (ax - 1.2f) / (1.2f + 1.5f * Mathf.PerlinNoise(z * 0.2f, 5f)));
                c = Color.Lerp(c, path * (0.9f + 0.2f * patch), onPath);
                Vector3 nrm = new Vector3(Height(x - 0.5f, z) - Height(x + 0.5f, z), 1f, Height(x, z - 0.5f) - Height(x, z + 0.5f)).normalized;
                c *= 0.7f + 0.45f * Mathf.Max(0f, Vector3.Dot(nrm, light));
                c.a = 1f;
                groundBase[k] = c;
            }
        var tris = new List<int>();
        for (int j = 0; j < nz - 1; j++)
            for (int i = 0; i < nx - 1; i++)
            {
                int a = j * nx + i, b = a + nx;
                tris.AddRange(new[] { a, b, a + 1, a + 1, b, b + 1 });
            }
        ground = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        ground.vertices = groundPos;
        ground.uv = uv;
        ground.SetTriangles(tris, 0);
        ground.RecalculateBounds();
        groundNow = new Color[groundPos.Length];

        var mat = new Material(shader);
        mat.mainTexture = Resources.Load<Texture2D>("Paisaje/pinceladas");
        mat.SetFloat("_Cutoff", 0f);
        Make("Terreno", ground, mat);
    }

    void BuildGrass(Shader shader)
    {
        var tex = Resources.Load<Texture2D>("Paisaje/pasto");
        if (tex == null) return;
        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var cols = new List<Color>();
        var tris = new List<int>();
        float z0 = gallery.Center.z - before * 0.6f, z1 = gallery.Center.z + gallery.WalkLength + after * 0.6f;

        // Matas sueltas: mas cerca del camino, chicas; lejos, menos y mas grandes
        for (int n = 0; n < tufts; n++)
        {
            float side = Random.value < 0.5f ? -1f : 1f;
            float ax = 1.4f + Mathf.Pow(Random.value, 2.2f) * (lakeStart + 2f);
            if (Random.value < 0.04f) ax = Random.value * 1.4f;   // alguna en el camino
            float x = origin.x + side * ax, z = Random.Range(z0, z1);
            float size = Mathf.Lerp(0.3f, 1.1f, Mathf.Clamp01(ax / 18f)) * Random.Range(0.7f, 1.3f);
            AddTuft(verts, uvs, cols, tris, x, z, size);
        }
        // Alrededor de cada arbol o roca: que salgan del pasto
        if (gallery.Origins != null)
            foreach (var b in gallery.Origins.PropBases())
                for (int n = 0; n < tuftsPerProp; n++)
                {
                    float a = Random.value * Mathf.PI * 2f;
                    float r = b.w * 0.5f * Mathf.Sqrt(Random.Range(0.15f, 1.1f));
                    AddTuft(verts, uvs, cols, tris, b.x + Mathf.Cos(a) * r, b.z + Mathf.Sin(a) * r * 0.5f, Random.Range(0.35f, 0.8f));
                }

        grass = new Mesh { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        grass.SetVertices(verts);
        grass.SetUVs(0, uvs);
        grass.SetTriangles(tris, 0);
        grass.RecalculateBounds();
        grassPos = verts.ToArray();
        grassBase = cols.ToArray();
        grassNow = new Color[grassPos.Length];

        var mat = new Material(shader);
        mat.mainTexture = tex;
        mat.SetFloat("_Cutoff", 0.45f);
        Make("Pasto", grass, mat);
    }

    void AddTuft(List<Vector3> verts, List<Vector2> uvs, List<Color> cols, List<int> tris, float x, float z, float size)
    {
        float y = floorY + Height(x, z) - 0.03f;
        int variant = Random.Range(0, 4);
        float patch = Mathf.PerlinNoise(x * 0.18f + 3f, z * 0.18f + 9f);
        Color c = Color.Lerp(Color.Lerp(grassA, grassB, patch), dry, Random.value * 0.45f) * 1.15f;
        float rot = Random.value * 180f;
        for (int q = 0; q < 2; q++)   // dos planos cruzados: se ve con volumen desde cualquier lado
        {
            Vector3 r = Quaternion.Euler(0f, rot + q * 90f, 0f) * Vector3.right * (size * 0.5f);
            int i = verts.Count;
            verts.Add(new Vector3(x, y, z) - r);
            verts.Add(new Vector3(x, y, z) + r);
            verts.Add(new Vector3(x, y + size, z) - r);
            verts.Add(new Vector3(x, y + size, z) + r);
            float u0 = variant * 0.25f, u1 = u0 + 0.25f;
            uvs.Add(new Vector2(u0, 0f)); uvs.Add(new Vector2(u1, 0f)); uvs.Add(new Vector2(u0, 1f)); uvs.Add(new Vector2(u1, 1f));
            cols.Add(c * 0.6f); cols.Add(c * 0.6f); cols.Add(c); cols.Add(c);
            tris.AddRange(new[] { i, i + 2, i + 1, i + 1, i + 2, i + 3 });
        }
    }

    void Make(string label, Mesh mesh, Material mat)
    {
        var go = new GameObject(label);
        go.transform.SetParent(transform, false);
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        var r = go.AddComponent<MeshRenderer>();
        r.sharedMaterial = mat;
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        r.receiveShadows = false;
    }

    void LateUpdate()
    {
        if (ground == null) return;
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = 0.15f;   // no hace falta cada cuadro
        UpdateColors();
    }

    // Tinte de la epoca, bruma de lejos y niebla, segun donde esta cada punto y donde esta la persona
    void UpdateColors()
    {
        Vector3 eye = gallery.RigPosition;
        float appear = gallery.appear;
        Apply(groundPos, groundBase, groundNow, eye, appear);
        ground.colors = groundNow;
        if (grass != null)
        {
            Apply(grassPos, grassBase, grassNow, eye, appear);
            grass.colors = grassNow;
        }
    }

    void Apply(Vector3[] pos, Color[] baseCol, Color[] into, Vector3 eye, float appear)
    {
        int zoneA = -1; Color tint = Color.white;
        for (int i = 0; i < pos.Length; i++)
        {
            // El tinte cambia despacio a lo largo del camino: se calcula por metro
            int zm = Mathf.FloorToInt(pos[i].z);
            if (zm != zoneA) { zoneA = zm; tint = environment.ZoneTint(new Vector3(origin.x, 0f, zm)); }
            Color c = baseCol[i];
            c = new Color(c.r * tint.r, c.g * tint.g, c.b * tint.b, 1f);
            float d = Vector3.Distance(pos[i], eye);
            Color hz = new Color(haze.r * tint.r * 1.2f, haze.g * tint.g * 1.2f, haze.b * tint.b * 1.2f);
            c = Color.Lerp(c, hz, (1f - Mathf.Exp(-d / hazeDistance)) * 0.6f);
            c = environment.Fogged(c, pos[i]);
            into[i] = c * appear;
        }
    }
}

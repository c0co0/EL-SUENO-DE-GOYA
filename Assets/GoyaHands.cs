using UnityEngine;

/// <summary>
/// Las manos que capta la camara, dibujadas como polvo dorado (puntitos sobre los huesos de la mano)
/// con unas cintas doradas que giran alrededor cuando la mano esta abierta y levantada.
/// Reemplaza a las esferas de colores de HandOSCReceiver. Se agrega solo.
/// GoyaParticles usa la palma (Palm) como centro del remolino al armar o desarmar una obra.
/// </summary>
public class GoyaHands : MonoBehaviour
{
    public bool show = true;
    [Tooltip("Puntitos por mano")]
    public int dotsPerHand = 420;
    public float dotSize = 0.007f;
    public Color dotColor = new Color(1f, 0.86f, 0.58f, 0.85f);
    [Tooltip("Cintas doradas alrededor de la mano abierta")]
    public int ribbons = 3;
    public Color ribbonColor = new Color(1f, 0.78f, 0.35f, 0.9f);
    public float ribbonRadius = 0.14f;
    public float followSpeed = 20f;

    // Huesos de la mano (MediaPipe: 0 muñeca, 1-4 pulgar, 5-8 indice, 9-12 medio, 13-16 anular, 17-20 meñique)
    static readonly int[] Bones =
    {
        0,1, 1,2, 2,3, 3,4,   0,5, 5,6, 6,7, 7,8,   5,9, 9,10, 10,11, 11,12,
        9,13, 13,14, 14,15, 15,16,   13,17, 0,17, 17,18, 18,19, 19,20,
    };

    class Hand
    {
        public HandOSCReceiver.HandState state;
        public Vector3[] smooth = new Vector3[21];
        public ParticleSystem ps;
        public ParticleSystem.Particle[] parts;
        public int[] bone; public float[] along; public Vector3[] offset; public float[] flick;
        public TrailRenderer[] trails;
        public Vector3[] axisU, axisV;
        public float glow, seen;
        public bool had;
    }

    HandOSCReceiver receiver;
    Hand left, right;
    static GoyaHands instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Auto()
    {
        var r = FindFirstObjectByType<HandOSCReceiver>();
        if (r != null && r.GetComponent<GoyaHands>() == null) r.gameObject.AddComponent<GoyaHands>();
    }

    void Start()
    {
        instance = this;
        receiver = GetComponent<HandOSCReceiver>();
        var mat = new Material(Shader.Find("Sprites/Default"));
        mat.mainTexture = Dot();
        var trailMat = new Material(Shader.Find("Sprites/Default"));
        left = MakeHand("Mano_Izq", mat, trailMat);
        right = MakeHand("Mano_Der", mat, trailMat);
    }

    Hand MakeHand(string label, Material mat, Material trailMat)
    {
        var h = new Hand();
        var go = new GameObject(label);
        go.transform.SetParent(transform, false);
        h.ps = go.AddComponent<ParticleSystem>();
        h.ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = h.ps.main;
        main.loop = false; main.playOnAwake = false; main.maxParticles = dotsPerHand;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = 1000f; main.startSpeed = 0f;
        var em = h.ps.emission; em.enabled = false;
        var sh = h.ps.shape; sh.enabled = false;
        var rend = go.GetComponent<ParticleSystemRenderer>();
        rend.sharedMaterial = mat;
        rend.sortingOrder = 8;

        int n = dotsPerHand;
        h.parts = new ParticleSystem.Particle[n];
        h.bone = new int[n]; h.along = new float[n]; h.offset = new Vector3[n]; h.flick = new float[n];
        int boneCount = Bones.Length / 2;
        for (int i = 0; i < n; i++)
        {
            h.bone[i] = i % boneCount;
            h.along[i] = Random.value;
            h.offset[i] = Random.insideUnitSphere * 0.009f;
            h.flick[i] = Random.value * 10f;
        }

        h.trails = new TrailRenderer[ribbons];
        h.axisU = new Vector3[ribbons]; h.axisV = new Vector3[ribbons];
        for (int k = 0; k < ribbons; k++)
        {
            var t = new GameObject(label + "_Cinta" + k);
            t.transform.SetParent(go.transform, false);
            var tr = t.AddComponent<TrailRenderer>();
            tr.sharedMaterial = trailMat;
            tr.time = 0.45f;
            tr.widthMultiplier = 0.004f;
            tr.minVertexDistance = 0.005f;
            tr.sortingOrder = 8;
            tr.emitting = false;
            h.trails[k] = tr;
            Vector3 n0 = Random.onUnitSphere;
            h.axisU[k] = Vector3.Cross(n0, Random.onUnitSphere).normalized;
            h.axisV[k] = Vector3.Cross(n0, h.axisU[k]).normalized;
        }
        return h;
    }

    void Update()
    {
        if (receiver == null || left == null) return;
        if (receiver.showDebugSpheres) HideSpheres();
        UpdateHand(left, receiver.Left);
        UpdateHand(right, receiver.Right);
    }

    void HideSpheres()
    {
        foreach (var hs in new[] { receiver.Left, receiver.Right })
            if (hs.spheres != null) foreach (var s in hs.spheres) if (s != null) s.gameObject.SetActive(false);
    }

    void UpdateHand(Hand h, HandOSCReceiver.HandState s)
    {
        h.state = s;
        bool vis = show && s.visible;
        if (!vis)
        {
            if (h.had) { h.ps.Clear(); foreach (var tr in h.trails) { tr.emitting = false; } }
            h.had = false;
            return;
        }
        float k = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);
        for (int i = 0; i < 21; i++)
        {
            Vector3 w = transform.TransformPoint(s.points[i]);
            h.smooth[i] = h.had ? Vector3.Lerp(h.smooth[i], w, k) : w;
        }
        if (!h.had) { h.ps.Play(); foreach (var tr in h.trails) tr.Clear(); }
        h.had = true;

        // Puntitos sobre los huesos, temblando apenas
        float time = Time.time;
        for (int i = 0; i < h.parts.Length; i++)
        {
            int b = h.bone[i];
            Vector3 p = Vector3.Lerp(h.smooth[Bones[b * 2]], h.smooth[Bones[b * 2 + 1]], h.along[i]) + h.offset[i];
            p += new Vector3(Mathf.Sin(time * 3f + h.flick[i]), Mathf.Cos(time * 2.6f + h.flick[i]), 0f) * 0.0015f;
            h.parts[i].position = p;
            h.parts[i].startSize = dotSize * (0.6f + 0.6f * Mathf.Abs(Mathf.Sin(time * 4f + h.flick[i])));
            Color c = dotColor;
            c.a *= 0.55f + 0.45f * Mathf.Sin(time * 5f + h.flick[i] * 3f);
            h.parts[i].startColor = c;
            h.parts[i].startLifetime = 1000f;
            h.parts[i].remainingLifetime = 1000f;
        }
        h.ps.SetParticles(h.parts, h.parts.Length);

        // Cintas: aparecen con la mano abierta y levantada (el gesto de armar o de entrar)
        bool raised = HandOSCReceiver.IsRaisedOpen(s, 0.45f);
        h.glow = Mathf.MoveTowards(h.glow, raised ? 1f : (s.open ? 0.35f : 0f), Time.deltaTime * 2f);
        Vector3 palm = PalmOf(h);
        for (int r = 0; r < h.trails.Length; r++)
        {
            var tr = h.trails[r];
            tr.emitting = h.glow > 0.05f;
            float ang = time * (4f + r * 1.3f) + r * 2.1f;
            float rad = ribbonRadius * (0.8f + 0.5f * r / Mathf.Max(1, h.trails.Length - 1));
            tr.transform.position = palm + (h.axisU[r] * Mathf.Cos(ang) + h.axisV[r] * Mathf.Sin(ang)) * rad;
            Color c = ribbonColor; c.a *= h.glow;
            Color end = c; end.a = 0f;
            tr.startColor = c;
            tr.endColor = end;
        }
    }

    static Vector3 PalmOf(Hand h)
    {
        return (h.smooth[0] + h.smooth[5] + h.smooth[9] + h.smooth[13] + h.smooth[17]) / 5f;
    }

    /// <summary>Palma de alguna mano visible (la que este mas levantada), para centrar el remolino.</summary>
    public static bool Palm(out Vector3 palm)
    {
        palm = Vector3.zero;
        if (instance == null || instance.left == null) return false;
        bool l = instance.left.had, r = instance.right.had;
        if (!l && !r) return false;
        if (l && r) palm = (PalmOf(instance.left) + PalmOf(instance.right)) * 0.5f;   // las dos: entre ambas
        else palm = PalmOf(l ? instance.left : instance.right);
        return true;
    }

    static Texture2D Dot()
    {
        int n = 16;
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float dx = (x + 0.5f) / n * 2f - 1f, dy = (y + 0.5f) / n * 2f - 1f;
                float a = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
        tex.Apply();
        return tex;
    }
}

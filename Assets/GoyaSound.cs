using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sonido espacial del recorrido (pensado como un campo envolvente, al estilo Ambeo):
/// - Ambiente de cada zona: 4 fuentes alrededor tuyo (adelante, atras, a los costados) que se mueven con vos
///   pero no giran con la cabeza, asi al girar el sonido queda en su lugar. Al caminar se funde con la zona siguiente.
/// - Sonidos sueltos de cada zona en lugares al azar: pajaros arriba, cañones lejos en el horizonte, susurros cerca.
/// - Pasos al caminar, y el fuego cuando se quema el cabrio (GoyaIntro lo llama).
///
/// Los sonidos estan en Assets/Resources/Sonidos (se generan con tools/sonidos/generar.py; se pueden
/// reemplazar por grabaciones con el mismo nombre). Se agrega solo, no hay que ponerlo en ningun objeto.
/// </summary>
public class GoyaSound : MonoBehaviour
{
    [Header("Volumen")]
    [Range(0f, 1f)] public float master = 1f;
    [Range(0f, 1f)] public float ambience = 0.55f;
    [Range(0f, 1f)] public float events = 0.7f;
    [Range(0f, 1f)] public float steps = 0.45f;
    [Range(0f, 1f)] public float fire = 0.8f;
    [Tooltip("Ambiente durante la introduccion del cabrio (mas bajo, para escuchar las instrucciones)")]
    [Range(0f, 1f)] public float introAmbience = 0.4f;

    [Header("Espacio")]
    [Tooltip("Distancia de las 4 fuentes de ambiente (metros)")]
    public float ringRadius = 4f;
    [Tooltip("Cada cuantos segundos aparece un sonido suelto (minimo y maximo)")]
    public Vector2 eventInterval = new Vector2(2.5f, 7f);

    [Header("Pasos")]
    [Tooltip("Metros entre un paso y otro")]
    public float stepLength = 0.7f;

    // Donde suena cada tipo de sonido suelto: distancia (min, max) y altura (min, max)
    static readonly Dictionary<string, Vector4> Places = new Dictionary<string, Vector4>
    {
        { "ave",      new Vector4(5f, 14f, 3f, 8f) },
        { "clave",    new Vector4(6f, 10f, -0.5f, 0.5f) },
        { "copa",     new Vector4(3f, 7f, -0.6f, 0f) },
        { "susurro",  new Vector4(0.8f, 2f, -0.1f, 0.3f) },
        { "buho",     new Vector4(10f, 20f, 3f, 7f) },
        { "canon",    new Vector4(30f, 45f, 0f, 2f) },
        { "disparos", new Vector4(20f, 35f, 0f, 1f) },
        { "tambor",   new Vector4(15f, 25f, -0.5f, 0.5f) },
        { "gemido",   new Vector4(6f, 12f, -1f, 2f) },
        { "latido",   new Vector4(0.3f, 0.6f, -0.3f, 0f) },
        { "mastica",  new Vector4(1.5f, 3f, 0.5f, 1.5f) },
    };

    class Zone
    {
        public AudioSource[] bed;
        public List<AudioClip> clips = new List<AudioClip>();
        public float weight, timer;
    }

    readonly List<Zone> zones = new List<Zone>();
    readonly List<AudioClip> stepClips = new List<AudioClip>();
    AudioClip fireLoop, fireStart;
    GameObject ring;
    GoyaGallery gallery;
    StereoGyroRig rigScript;
    Transform rig;
    Vector3 lastPos;
    float stepDist, lastZone;
    bool hasLast;

    static GoyaSound instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Auto()
    {
        var g = FindFirstObjectByType<GoyaGallery>();
        if (g != null && g.GetComponent<GoyaSound>() == null) g.gameObject.AddComponent<GoyaSound>();
    }

    void Awake()
    {
        instance = this;
        gallery = GetComponent<GoyaGallery>();
        var all = Resources.LoadAll<AudioClip>("Sonidos");
        var byZone = new SortedDictionary<int, Zone>();
        var beds = new Dictionary<int, List<AudioClip>>();
        foreach (var c in all)
        {
            string n = c.name;
            if (n.StartsWith("pasos")) stepClips.Add(c);
            else if (n == "fuego_loop") fireLoop = c;
            else if (n == "fuego_encendido") fireStart = c;
            else if (n.StartsWith("zona") && n.Length > 5 && char.IsDigit(n[4]))
            {
                int z = n[4] - '1';
                if (!byZone.ContainsKey(z)) { byZone[z] = new Zone(); beds[z] = new List<AudioClip>(); }
                if (n.Contains("_cama_")) beds[z].Add(c);
                else byZone[z].clips.Add(c);
            }
        }
        if (all.Length == 0) Debug.LogWarning("[Sonido] No hay sonidos en Assets/Resources/Sonidos");

        ring = new GameObject("Sonido_Ambiente");
        ring.transform.SetParent(transform, false);
        var dirs = new[] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
        foreach (var kv in byZone)
        {
            var zone = kv.Value;
            var list = beds[kv.Key];
            list.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
            zone.bed = new AudioSource[list.Count == 0 ? 0 : dirs.Length];
            for (int i = 0; i < zone.bed.Length; i++)
            {
                var go = new GameObject("Zona" + (kv.Key + 1) + "_" + i);
                go.transform.SetParent(ring.transform, false);
                go.transform.localPosition = dirs[i] * ringRadius;
                var src = Make(go, list[i % list.Count], true);
                src.minDistance = ringRadius;   // no se apaga por distancia: solo da la direccion
                src.maxDistance = ringRadius * 4f;
                src.volume = 0f;
                src.time = Random.value * list[i % list.Count].length;   // cada una arranca en otro lugar
                src.Play();
                zone.bed[i] = src;
            }
            zone.timer = Random.Range(eventInterval.x, eventInterval.y);
            while (zones.Count <= kv.Key) zones.Add(new Zone { bed = new AudioSource[0] });
            zones[kv.Key] = zone;
        }
    }

    static AudioSource Make(GameObject go, AudioClip clip, bool loop)
    {
        var src = go.AddComponent<AudioSource>();
        src.clip = clip;
        src.loop = loop;
        src.playOnAwake = false;
        src.spatialBlend = 1f;          // 3D: suena desde donde esta
        src.spread = 40f;
        src.dopplerLevel = 0f;
        src.rolloffMode = AudioRolloffMode.Logarithmic;
        src.minDistance = 1.5f;
        src.maxDistance = 80f;
        return src;
    }

    bool FindRig()
    {
        if (rig != null) return true;
        if (rigScript == null) rigScript = FindFirstObjectByType<StereoGyroRig>();
        if (rigScript == null) return false;
        rig = rigScript.transform;
        return true;
    }

    void Update()
    {
        if (!FindRig()) return;
        Vector3 pos = rig.position;
        ring.transform.position = pos;   // las fuentes te siguen, pero no giran con la cabeza

        // Zona actual: en el camino segun donde estas; adentro de una obra (portal) se queda la ultima
        bool path = gallery != null && gallery.pathMode && gallery.PathLength > 0f;
        bool inPortal = gallery != null && Mathf.Abs(pos.x - gallery.Center.x) > 500f;
        if (path && !inPortal) lastZone = gallery.ZoneAt(pos) - 0.5f;   // centro de cada zona = 100% de su ambiente
        float level = ambience * master * (IntroRunning() ? introAmbience : 1f);

        float dt = Time.deltaTime;
        for (int z = 0; z < zones.Count; z++)
        {
            var zone = zones[z];
            float target = Mathf.Clamp01(1f - Mathf.Abs(lastZone - z));
            if (z == 0 && lastZone < 0f) target = 1f;
            if (z == zones.Count - 1 && lastZone > z) target = 1f;
            zone.weight = Mathf.MoveTowards(zone.weight, target, dt * 0.5f);
            foreach (var s in zone.bed) if (s != null) s.volume = zone.weight * level;

            // Sonidos sueltos de la zona, mas seguidos cuanto mas adentro estas
            if (zone.weight > 0.2f && zone.clips.Count > 0)
            {
                zone.timer -= dt * zone.weight;
                if (zone.timer <= 0f)
                {
                    zone.timer = Random.Range(eventInterval.x, eventInterval.y);
                    PlayEvent(zone.clips[Random.Range(0, zone.clips.Count)], pos, zone.weight);
                }
            }
        }

        UpdateSteps(pos);
    }

    GoyaIntro intro;
    bool introSearched;
    bool IntroRunning()
    {
        if (!introSearched) { intro = FindFirstObjectByType<GoyaIntro>(); introSearched = true; }
        return intro != null && intro.isActiveAndEnabled && !intro.Finished;
    }

    void PlayEvent(AudioClip clip, Vector3 around, float weight)
    {
        Vector4 p = new Vector4(4f, 10f, 0f, 2f);
        foreach (var kv in Places) if (clip.name.Contains("_" + kv.Key + "_")) { p = kv.Value; break; }
        float angle = Random.value * Mathf.PI * 2f;
        float dist = Random.Range(p.x, p.y);
        Vector3 at = around + new Vector3(Mathf.Cos(angle) * dist, Random.Range(p.z, p.w), Mathf.Sin(angle) * dist);
        var go = new GameObject("Sonido_" + clip.name);
        go.transform.position = at;
        var src = Make(go, clip, false);
        src.minDistance = Mathf.Max(1f, p.x * 0.6f);   // los lejanos se escuchan igual, pero vienen de lejos
        src.volume = events * master * Mathf.Clamp01(weight * 1.2f);
        src.pitch = Random.Range(0.94f, 1.06f);
        src.Play();
        Destroy(go, clip.length / src.pitch + 0.2f);
    }

    void UpdateSteps(Vector3 pos)
    {
        if (!hasLast) { lastPos = pos; hasLast = true; return; }
        Vector3 d = pos - lastPos;
        d.y = 0f;
        lastPos = pos;
        float m = d.magnitude;
        if (m > 1f) { stepDist = 0f; return; }   // salto (portal, reinicio): no son pasos
        stepDist += m;
        if (stepDist < stepLength || stepClips.Count == 0) return;
        stepDist = 0f;

        var go = new GameObject("Paso");
        go.transform.position = pos + Vector3.down * 1.4f + Random.insideUnitSphere * 0.1f;   // a los pies
        var clip = stepClips[Random.Range(0, stepClips.Count)];
        var src = Make(go, clip, false);
        src.minDistance = 1f;
        src.spread = 90f;
        src.volume = steps * master * Random.Range(0.8f, 1f);
        src.pitch = Random.Range(0.88f, 1.1f);
        src.Play();
        Destroy(go, clip.length / src.pitch + 0.2f);
    }

    // ---------------------------------------------------------------- fuego del cabrio

    /// <summary>El fuego en un lugar: se enciende, crece durante "duration" segundos y despues se apaga solo.</summary>
    public static void Fire(Vector3 at, float duration)
    {
        if (instance == null) instance = FindFirstObjectByType<GoyaSound>();
        if (instance != null) instance.StartCoroutine(instance.FireRoutine(at, duration));
    }

    System.Collections.IEnumerator FireRoutine(Vector3 at, float duration)
    {
        var go = new GameObject("Sonido_Fuego");
        go.transform.position = at;
        if (fireStart != null)
        {
            var s = Make(go, fireStart, false);
            s.minDistance = 2f;
            s.volume = fire * master;
            s.Play();
        }
        if (fireLoop == null) { Destroy(go, 3f); yield break; }
        var loop = Make(go, fireLoop, true);
        loop.minDistance = 2f;
        loop.time = Random.value * fireLoop.length;
        loop.volume = 0f;
        loop.Play();
        float t = 0f, fadeOut = 2.5f;
        while (t < duration + fadeOut)
        {
            t += Time.deltaTime;
            float up = Mathf.Clamp01(t / Mathf.Max(0.1f, duration * 0.6f));
            float down = 1f - Mathf.Clamp01((t - duration) / fadeOut);
            loop.volume = fire * master * up * down;
            yield return null;
        }
        Destroy(go);
    }
}

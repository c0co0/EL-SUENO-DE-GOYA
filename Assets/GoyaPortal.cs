using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Entrar a cada obra: si te quedas mirando de cerca una obra revelada, el cuadro brilla, la pantalla se
/// funde a negro y apareces en una escena aparte (un diorama de esa obra: el cuadro de fondo y las
/// figuras recortadas mas cerca, con niebla entre medio). Se camina hacia adentro como en el resto
/// (dedo levantado / W). Para salir hay que retroceder pasando la entrada (un dedo en cada mano / S),
/// o con la tecla P. Volves al camino, frente a la obra.
///
/// Las figuras recortadas estan en Assets/Resources/Cortes (mismo nombre que la obra, en .png).
/// Las obras sin recorte no tienen portal.
/// Se agrega solo al lado de GoyaGallery, no hay que ponerlo en ningun objeto.
/// En el editor: tecla P = entrar a la obra que miras (sin esperar) / salir.
/// </summary>
public class GoyaPortal : MonoBehaviour
{
    [Header("Entrar")]
    public bool portals = true;
    [Tooltip("Hay que estar a menos de esta distancia (metros) de la obra")]
    public float portalDistance = 2.6f;
    [Tooltip("Segundos mirando la obra para entrar")]
    public float dwell = 3f;
    public float fadeTime = 0.9f;

    [Header("La escena")]
    public float roomDepth = 14f;
    [Tooltip("Distancia de las figuras desde la entrada (el cuadro queda al fondo)")]
    public float figureDistance = 7f;
    public float walkSpeed = 1.3f;
    [Tooltip("Al llegar al fondo de la obra, te saca solo (segundos de espera antes de salir)")]
    public float autoExitDelay = 2f;
    [Tooltip("Tiempo maximo dentro de una obra (segundos); despues te saca solo")]
    public float maxTime = 120f;
    [Tooltip("Cuanto hay que retroceder pasando la entrada para salir (metros)")]
    public float exitBehind = 0.6f;
    [Tooltip("Al acercarte a la figura se vuelve transparente a esta distancia, para poder atravesarla")]
    public float passFade = 2.2f;
    public int mistCount = 18;
    public Color mistColor = new Color(0.78f, 0.72f, 0.66f, 0.15f);

    enum State { Path, ToRoom, Room, ToPath }
    State state = State.Path;

    class Item
    {
        public SpriteRenderer sr;
        public Vector3 baseScale, basePos;
        public float breathe, phase, alpha = 1f;
        public bool fades, mist;
    }

    GoyaGallery gallery;
    StereoGyroRig rigScript;
    Transform rig, head;
    SpriteRenderer fade, glow;
    Sprite softSprite;
    Dictionary<string, Sprite> cutouts;
    readonly List<Item> items = new List<Item>();

    GoyaGallery.Painting candidate, entering;
    float dwellT, cooldown, fadeValue, fingerT, walk, roomTime, endTime;
    Vector3 returnPos, roomOrigin, roomFwd, roomRight;
    GameObject room;
    Sprite pendingCut, pendingBack;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Auto()
    {
        var g = FindFirstObjectByType<GoyaGallery>();
        if (g != null && g.GetComponent<GoyaPortal>() == null) g.gameObject.AddComponent<GoyaPortal>();
    }

    // ---------------------------------------------------------------- preparacion

    bool Init()
    {
        if (gallery == null) gallery = GetComponent<GoyaGallery>();
        if (rigScript == null) rigScript = FindFirstObjectByType<StereoGyroRig>();
        if (gallery == null || rigScript == null) return false;
        rig = rigScript.transform;
        if (head == null) head = rig.Find("Head");
        if (head == null) return false;
        if (softSprite == null) MakeSprites();
        return true;
    }

    void MakeSprites()
    {
        int n = 64;
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float dx = (x + 0.5f) / n * 2f - 1f, dy = (y + 0.5f) / n * 2f - 1f;
                float a = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a * a));
            }
        tex.Apply();
        softSprite = Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), n);

        var white = new Texture2D(4, 4, TextureFormat.RGBA32, false);
        var px = new Color[16];
        for (int i = 0; i < 16; i++) px[i] = Color.white;
        white.SetPixels(px);
        white.Apply();
        var whiteSprite = Sprite.Create(white, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);

        // Pantalla negra pegada a la cabeza (para los fundidos)
        var f = new GameObject("PortalFade");
        f.transform.SetParent(head, false);
        f.transform.localPosition = new Vector3(0f, 0f, 0.3f);
        f.transform.localScale = Vector3.one * 2f;
        fade = f.AddComponent<SpriteRenderer>();
        fade.sprite = whiteSprite;
        fade.sortingOrder = 1000;
        fade.color = new Color(0f, 0f, 0f, 0f);
        fade.enabled = false;

        var g = new GameObject("PortalBrillo");
        g.transform.SetParent(transform, false);
        glow = g.AddComponent<SpriteRenderer>();
        glow.sprite = softSprite;
        glow.sortingOrder = 6;
        glow.enabled = false;
    }

    static string Norm(string n)
    {
        if (n.EndsWith("_0")) n = n.Substring(0, n.Length - 2);
        return n.Trim();
    }

    Sprite CutoutFor(Sprite s)
    {
        if (s == null) return null;
        if (cutouts == null)
        {
            cutouts = new Dictionary<string, Sprite>();
            foreach (var c in Resources.LoadAll<Sprite>("Cortes")) cutouts[Norm(c.name)] = c;
        }
        Sprite r;
        return cutouts.TryGetValue(Norm(s.name), out r) ? r : null;
    }

    // ---------------------------------------------------------------- actualizacion

    void Update()
    {
        if (!Init()) return;
        float dt = Time.deltaTime;
        cooldown -= dt;

        switch (state)
        {
            case State.Path: UpdatePath(dt); break;
            case State.Room: UpdateRoom(dt); break;
            case State.ToRoom:
                fadeValue = Mathf.MoveTowards(fadeValue, 1f, dt / Mathf.Max(0.05f, fadeTime));
                if (fadeValue >= 1f) { EnterRoom(); state = State.Room; }
                break;
            case State.ToPath:
                fadeValue = Mathf.MoveTowards(fadeValue, 1f, dt / Mathf.Max(0.05f, fadeTime));
                if (fadeValue >= 1f) { LeaveRoom(); state = State.Path; }
                break;
        }

        if (state == State.Path || state == State.Room)
            fadeValue = Mathf.MoveTowards(fadeValue, 0f, dt / Mathf.Max(0.05f, fadeTime));
        fade.color = new Color(0f, 0f, 0f, fadeValue);
        fade.enabled = fadeValue > 0.001f;
    }

    bool CanEnter(GoyaGallery.Painting p)
    {
        if (p == null || !p.active || !p.revealed) return false;
        if (p.renderer == null || !p.renderer.enabled || p.renderer.color.a < 0.5f) return false;
        return CutoutFor(p.renderer.sprite) != null;
    }

    void UpdatePath(float dt)
    {
        glow.enabled = false;
        if (!portals || !gallery.enabled || gallery.appear < 0.5f || gallery.paintings.Count == 0) { dwellT = 0f; return; }

        var p = gallery.PaintingInView(portalDistance);
        bool ok = cooldown <= 0f && CanEnter(p);
        bool key = Input.GetKeyDown(KeyCode.P);

        if (ok && p == candidate) dwellT += dt;
        else if (ok) { candidate = p; dwellT = 0f; }
        else dwellT = Mathf.Max(0f, dwellT - dt * 2f);

        if (dwellT > 0.05f && candidate != null)
        {
            float k = Mathf.Clamp01(dwellT / Mathf.Max(0.1f, dwell));
            glow.enabled = true;
            glow.transform.position = candidate.root.position + candidate.root.forward * 0.07f;
            glow.transform.rotation = candidate.root.rotation;
            glow.transform.localScale = new Vector3(candidate.halfW * 2.5f, candidate.halfH * 2.5f, 1f);
            glow.color = new Color(1f, 0.9f, 0.7f, 0.65f * k * k);
        }

        if (ok && (dwellT >= dwell || key)) Begin(p);
        else if (key && CanEnter(p)) Begin(p);
    }

    void Begin(GoyaGallery.Painting p)
    {
        entering = p;
        pendingBack = p.renderer.sprite;
        pendingCut = CutoutFor(p.renderer.sprite);
        returnPos = rig.position;
        gallery.allowMove = false;
        dwellT = 0f;
        glow.enabled = false;
        state = State.ToRoom;
    }

    void EnterRoom()
    {
        Vector3 f = head.forward;
        f.y = 0f;
        if (f.sqrMagnitude < 0.0001f) f = Vector3.forward;
        roomFwd = f.normalized;
        roomRight = Vector3.Cross(Vector3.up, roomFwd);
        roomOrigin = gallery.Center + new Vector3(3000f, 0f, 0f);
        try { BuildRoom(pendingBack, pendingCut); }
        catch (System.Exception e) { Debug.LogError("[Portal] Error armando la escena: " + e); }
        rig.position = new Vector3(roomOrigin.x, returnPos.y, roomOrigin.z);
        walk = 0f;
        fingerT = 0f;
        roomTime = 0f;
        endTime = 0f;
        Debug.Log("[Portal] Entre a la escena de '" + (pendingBack != null ? pendingBack.name : "?") + "' con " + items.Count + " capas. Origen " + roomOrigin + ", mirando " + roomFwd);
    }

    void LeaveRoom()
    {
        if (room != null) Destroy(room);
        items.Clear();
        rig.position = returnPos;
        gallery.allowMove = true;
        cooldown = 4f;
        candidate = null;
    }

    // ---------------------------------------------------------------- la escena

    void BuildRoom(Sprite back, Sprite cut)
    {
        room = new GameObject("EscenaPortal");
        room.transform.position = roomOrigin;
        room.transform.rotation = Quaternion.LookRotation(roomFwd, Vector3.up);
        items.Clear();
        float eyeY = returnPos.y;

        AddLayer(back, roomDepth, eyeY, Color.white, 0f, false);
        AddLayer(cut, figureDistance, eyeY, Color.white, 0.012f, true);

        // Niebla: manchas suaves a distintas distancias, para sentir la profundidad entre las capas
        var rnd = new System.Random(back.name.GetHashCode());
        for (int i = 0; i < mistCount; i++)
        {
            float d = 2.5f + (float)rnd.NextDouble() * (roomDepth - 3f);
            var go = new GameObject("Niebla");
            go.transform.SetParent(room.transform, false);
            float x = ((float)rnd.NextDouble() * 2f - 1f) * (1.5f + d * 0.5f);
            float y = eyeY - roomOrigin.y + ((float)rnd.NextDouble() * 2f - 1f) * (0.8f + d * 0.15f) + 0.4f * d / 10f;
            go.transform.localPosition = new Vector3(x, y, d);
            float size = 3f + (float)rnd.NextDouble() * 4f + d * 0.25f;
            go.transform.localScale = new Vector3(size, size * 0.7f, 1f);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = softSprite;
            sr.sortingOrder = -Mathf.RoundToInt(d * 1.2f);   // siempre por encima del cielo y el paisaje del ambiente (-40..-20)
            sr.color = mistColor;
            items.Add(new Item { sr = sr, baseScale = go.transform.localScale, basePos = go.transform.localPosition, phase = (float)rnd.NextDouble() * 6.28f, mist = true, fades = true });
        }
    }

    void AddLayer(Sprite sprite, float d, float eyeY, Color tint, float breathe, bool fades)
    {
        if (sprite == null) return;
        var go = new GameObject("Capa_" + sprite.name);
        go.transform.SetParent(room.transform, false);
        // Mas lejos = mas grande, en la misma proporcion: desde la entrada todas las capas coinciden
        float k = d / 10f;
        go.transform.localPosition = new Vector3(0f, eyeY - roomOrigin.y + 0.8f * k, d);
        Vector3 size = sprite.bounds.size;
        float s = Mathf.Min(8f / size.y, 12f / size.x) * k;
        go.transform.localScale = Vector3.one * s;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = tint;
        sr.sortingOrder = -Mathf.RoundToInt(d * 1.2f);   // mas lejos = mas atras, pero siempre por encima del cielo (-40..-20)
        items.Add(new Item { sr = sr, baseScale = go.transform.localScale, basePos = go.transform.localPosition, breathe = breathe, phase = Random.value * 6f, fades = fades });
        Debug.Log("[Portal] Capa '" + sprite.name + "' a " + d + " m, escala " + s + ", tamano del sprite " + size + ", orden " + sr.sortingOrder);
    }

    void UpdateRoom(float dt)
    {
        // Moverse: W / dedo levantado = adelante, S / un dedo en cada mano = atras
        float wanted = 0f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) wanted = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) wanted = -1f;
        var hands = gallery.hands;
        if (hands != null)
        {
            bool oneL = HandOSCReceiver.IsOneFinger(hands.Left), oneR = HandOSCReceiver.IsOneFinger(hands.Right);
            fingerT = (oneL || oneR) ? fingerT + dt : 0f;
            if (fingerT >= 0.4f) wanted = (oneL && oneR) ? -1f : 1f;
        }

        Vector3 look = head.forward;
        look.y = 0f;
        if (look.sqrMagnitude < 0.0001f) look = roomFwd;
        look.Normalize();
        walk = Mathf.Lerp(walk, wanted, 1f - Mathf.Exp(-4f * dt));

        Vector3 pos = rig.position + look * (walk * walkSpeed * dt);
        Vector3 rel = pos - roomOrigin;
        float lz = Vector3.Dot(rel, roomFwd), lx = Vector3.Dot(rel, roomRight);
        lx = Mathf.Clamp(lx, -3.5f, 3.5f);
        lz = Mathf.Clamp(lz, -exitBehind - 0.5f, roomDepth - 0.8f);
        rig.position = new Vector3(roomOrigin.x, returnPos.y, roomOrigin.z) + roomFwd * lz + roomRight * lx;

        // Salir: retroceder pasando la entrada (o tecla P)
        if (state == State.Room && fadeValue < 0.2f && (lz <= -exitBehind || Input.GetKeyDown(KeyCode.P)))
        {
            gallery.allowMove = false;
            state = State.ToPath;
        }

        // Salida automatica: al llegar al fondo de la obra (o pasado mucho tiempo) te saca solo
        roomTime += dt;
        if (state == State.Room && fadeValue < 0.2f)
        {
            if (lz >= roomDepth - 1.6f) endTime += dt; else endTime = 0f;
            if (endTime >= autoExitDelay || roomTime >= maxTime)
            {
                gallery.allowMove = false;
                state = State.ToPath;
            }
        }

        // Respiracion, atravesar la figura y niebla a la deriva
        float myZ = Vector3.Dot(rig.position - roomOrigin, roomFwd);
        foreach (var it in items)
        {
            if (it.sr == null) continue;
            if (it.mist)
            {
                it.sr.transform.localPosition = it.basePos + new Vector3(Mathf.Sin(Time.time * 0.12f + it.phase) * 0.8f, Mathf.Sin(Time.time * 0.09f + it.phase * 2f) * 0.3f, 0f);
            }
            else if (it.breathe > 0f)
            {
                it.sr.transform.localScale = it.baseScale * (1f + it.breathe * Mathf.Sin(Time.time * 0.8f + it.phase));
            }
            Color c = it.sr.color;
            float a = it.mist ? mistColor.a : 1f;
            if (it.fades)
            {
                float dz = Mathf.Abs(it.sr.transform.localPosition.z - myZ);
                a *= Mathf.SmoothStep(0f, 1f, dz / Mathf.Max(0.01f, passFade));
            }
            c.a = a;
            it.sr.color = c;
        }
    }
}

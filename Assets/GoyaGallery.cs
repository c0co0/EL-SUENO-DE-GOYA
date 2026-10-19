using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Galeria onirica: pone 3 cuadros en arco frente a la persona, cubiertos por un velo.
/// Las manos funcionan como PUNTEROS: se traza un rayo desde la cabeza a traves de la mano,
/// y al pasar por un cuadro el velo se disuelve y la pintura se revela.
///
/// Usa SpriteRenderer (shader Sprites/Default), que funciona sin problemas en el celular.
///
/// Uso: objeto vacio "Galeria" con este script, y arrastrar las 3 imagenes (importadas
/// como Sprite) en los casilleros.
/// </summary>
public class GoyaGallery : MonoBehaviour
{
    [Header("Cuadros (arrastra las imagenes importadas como Sprite)")]
    public Sprite centerPainting;
    public Sprite leftPainting;
    public Sprite rightPainting;

    [Header("Referencias")]
    [Tooltip("Si queda vacio, lo busca solo")]
    public HandOSCReceiver hands;

    [Tooltip("Visibilidad general de los cuadros (0 a 1). Lo usa la introduccion para hacerlos aparecer de a poco.")]
    [Range(0f, 1f)] public float appear = 1f;

    [Header("Disposicion")]
    [Tooltip("Distancia de los cuadros a la persona (metros)")]
    public float distance = 2.5f;
    [Tooltip("Cuantos grados se separan los cuadros laterales del central")]
    public float sideAngle = 35f;
    [Tooltip("Lado mas largo de un cuadro (metros)")]
    public float paintingMaxSize = 1.6f;
    [Tooltip("El cuadro central se agranda por este factor")]
    public float centerScale = 1.15f;

    [Header("Mano como puntero")]
    [Tooltip("Cuanto se amplifica el movimiento de la mano para llegar a los cuadros")]
    public float reach = 1.6f;
    [Tooltip("Punto de la mano que apunta (9 = centro de la palma)")]
    public int pointerLandmark = 9;
    public float cursorSize = 0.18f;

    [Header("Revelado")]
    [Tooltip("Segundos de mano sobre el cuadro para revelarlo del todo")]
    public float revealTime = 1.2f;
    [Tooltip("Que tan rapido se 'olvida' un revelado a medias (0 = nunca)")]
    public float forgetSpeed = 0.25f;
    [Tooltip("Color y transparencia del cuadro cubierto por el velo")]
    public Color veilColor = new Color(0.35f, 0.35f, 0.5f, 0.12f);

    public class Painting
    {
        public string name;
        public Transform root;
        public SpriteRenderer renderer;
        public float halfW, halfH;
        public float reveal;       // 0 = cubierto, 1 = revelado
        public bool revealed;
        public bool hovered;       // alguna mano esta encima este cuadro
        public bool openHandOver;  // una mano ABIERTA esta encima (se usa en el paso de particulas)
        public float phase;
        public Vector3 hitPoint;   // ultimo punto del cuadro tocado por una mano
    }

    public List<Painting> paintings = new List<Painting>();

    Transform rig;
    SpriteRenderer cursorLeft, cursorRight;

    void Start()
    {
        if (hands == null) hands = FindFirstObjectByType<HandOSCReceiver>();
        if (hands == null)
        {
            Debug.LogError("[Galeria] No encuentro el objeto con HandOSCReceiver (Manos).");
            enabled = false;
            return;
        }
        rig = hands.transform.parent != null ? hands.transform.parent : transform;

        CreatePainting(centerPainting, "Centro", 0f, centerScale);
        CreatePainting(leftPainting, "Izquierda", -sideAngle, 1f);
        CreatePainting(rightPainting, "Derecha", sideAngle, 1f);

        var circle = MakeCircleSprite();
        cursorLeft = CreateCursor("CursorIzquierdo", circle);
        cursorRight = CreateCursor("CursorDerecho", circle);
    }

    void CreatePainting(Sprite sprite, string label, float angle, float scaleMul)
    {
        if (sprite == null)
        {
            Debug.LogWarning("[Galeria] Falta la imagen del cuadro: " + label);
            return;
        }

        Quaternion yaw = Quaternion.Euler(0f, angle, 0f);
        var root = new GameObject("Cuadro_" + label).transform;
        root.SetParent(transform, false);
        root.position = rig.position + yaw * Vector3.forward * distance;
        root.rotation = yaw;

        Vector3 size = sprite.bounds.size;
        float s = paintingMaxSize * scaleMul / Mathf.Max(size.x, size.y);
        root.localScale = Vector3.one * s;

        var sr = root.gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = veilColor;
        sr.sortingOrder = 0;

        paintings.Add(new Painting
        {
            name = label,
            root = root,
            renderer = sr,
            halfW = size.x * s * 0.5f,
            halfH = size.y * s * 0.5f,
            phase = Random.value * 6.28f
        });
    }

    SpriteRenderer CreateCursor(string label, Sprite circle)
    {
        var go = new GameObject(label);
        go.transform.SetParent(transform, false);
        go.transform.localScale = Vector3.one * cursorSize;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = circle;
        sr.sortingOrder = 10;
        sr.enabled = false;
        return sr;
    }

    // Circulo suave generado por codigo (no hace falta ninguna imagen)
    static Sprite MakeCircleSprite()
    {
        const int n = 64;
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < n; x++)
            {
                float dx = (x + 0.5f) / n * 2f - 1f;
                float dy = (y + 0.5f) / n * 2f - 1f;
                float a = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a * a));
            }
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f), n);
    }

    bool GetRayDirection(HandOSCReceiver.HandState hand, out Vector3 dir)
    {
        dir = Vector3.forward;
        if (!hand.visible) return false;

        // Posicion de la mano relativa al Rig (Manos es hijo del Rig, sin rotacion ni escala)
        Vector3 local = hands.transform.localPosition + hand.points[pointerLandmark];
        Vector3 amplified = new Vector3(local.x * reach, local.y * reach, local.z);
        dir = rig.TransformDirection(amplified).normalized;
        return true;
    }

    void ProcessHand(HandOSCReceiver.HandState hand, SpriteRenderer cursor, Color baseColor)
    {
        Vector3 dir;
        if (!GetRayDirection(hand, out dir))
        {
            cursor.enabled = false;
            return;
        }

        Vector3 origin = rig.position;
        Vector3 cursorPos = origin + dir * (distance * 0.96f);

        foreach (var p in paintings)
        {
            float enter;
            var plane = new Plane(p.root.forward, p.root.position);
            if (!plane.Raycast(new Ray(origin, dir), out enter)) continue;

            Vector3 hit = origin + dir * enter;
            Vector3 offset = hit - p.root.position;
            float x = Vector3.Dot(offset, p.root.right);
            float y = Vector3.Dot(offset, p.root.up);

            if (Mathf.Abs(x) <= p.halfW && Mathf.Abs(y) <= p.halfH)
            {
                p.hovered = true;
                p.hitPoint = hit;
                if (hand.open) p.openHandOver = true;
                cursorPos = hit - dir * 0.03f;
            }
        }

        cursor.enabled = true;
        cursor.transform.position = cursorPos;
        cursor.transform.rotation = Quaternion.LookRotation(dir);
        cursor.color = hand.open ? Color.green : baseColor;
    }

    void Update()
    {
        foreach (var p in paintings)
        {
            p.hovered = false;
            p.openHandOver = false;
        }

        ProcessHand(hands.Left, cursorLeft, hands.leftColor);
        ProcessHand(hands.Right, cursorRight, hands.rightColor);

        // Tecla Backspace (en el editor): vuelve a cubrir todos los cuadros para probar de nuevo
        if (Input.GetKeyDown(KeyCode.Backspace)) ResetAll();

        foreach (var p in paintings)
        {
            if (!p.revealed)
            {
                if (p.hovered) p.reveal += Time.deltaTime / revealTime;
                else p.reveal -= Time.deltaTime / revealTime * forgetSpeed;
                p.reveal = Mathf.Clamp01(p.reveal);
                if (p.reveal >= 1f) p.revealed = true;
            }

            float t = Mathf.SmoothStep(0f, 1f, p.reveal);
            Color veil = veilColor;
            veil.a *= 1f + 0.5f * Mathf.Sin(Time.time * 1.5f + p.phase); // el velo "respira"
            Color shown = Color.Lerp(veil, Color.white, t);
            shown.a *= appear;
            p.renderer.color = shown;
        }
    }

    public void ResetAll()
    {
        foreach (var p in paintings)
        {
            p.reveal = 0f;
            p.revealed = false;
        }
    }
}

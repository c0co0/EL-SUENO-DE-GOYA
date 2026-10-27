using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pinturas negras "por dentro": al final del camino, una pintura separada en capas (fondo y figuras)
/// puestas a distintas distancias, como un escenario de teatro. Desde la entrada se ve igual que la pintura,
/// pero al caminar las capas se separan y se puede pasar entre ellas, hasta quedar al lado de las figuras.
///
/// Las capas estan en Assets/Resources/Dioramas (fondo en .jpg, figuras en .png con transparencia),
/// todas del mismo tamaño que la pintura original, asi coinciden.
/// GoyaGallery lo agrega solo en modo camino.
/// </summary>
public class GoyaDiorama : MonoBehaviour
{
    [System.Serializable]
    public class Layer
    {
        [Tooltip("Imagen dentro de Assets/Resources, sin la extension (ej: Dioramas/saturno_figura)")]
        public string image;
        [Tooltip("Distancia desde la entrada del diorama (metros)")]
        public float distance;
        [Tooltip("Cuanto respira (se agranda y achica apenas), para que la figura parezca viva")]
        public float breathe;
    }

    [System.Serializable]
    public class Diorama
    {
        public string name;
        [Tooltip("Nombre de la obra de la lista que reemplaza (para que no aparezca tambien plana en el camino)")]
        public string replaces;
        [Tooltip("Alto que parece tener la pintura vista desde la entrada, si estuviera a 10 m")]
        public float height = 7f;
        [Tooltip("Cuanto mas arriba o abajo de los ojos queda el centro de la pintura (a 10 m)")]
        public float centerOffset = 0.5f;
        public Layer[] layers;
    }

    public Diorama[] dioramas =
    {
        new Diorama
        {
            name = "Saturno",
            replaces = "28 Saturno devorando a su hijo",
            height = 8f,
            centerOffset = 1.2f,
            layers = new[]
            {
                new Layer { image = "Dioramas/saturno_fondo", distance = 16f },
                new Layer { image = "Dioramas/saturno_figura", distance = 8f, breathe = 0.012f },
            }
        },
    };

    [Tooltip("Distancia entre la entrada de un diorama y el siguiente (metros)")]
    public float spacing = 20f;
    [Range(0f, 1f), Tooltip("Cuanto le afecta la niebla del ambiente")]
    public float fog = 0.6f;
    [Tooltip("Lo mas cerca que se puede llegar a la ultima capa (metros)")]
    public float minDistanceToLast = 1.5f;

    class Built
    {
        public SpriteRenderer renderer;
        public Vector3 baseScale;
        public float breathe, phase;
    }
    readonly List<Built> built = new List<Built>();
    GoyaGallery gallery;
    GoyaEnvironment environment;

    /// <summary>La obra tiene diorama (GoyaGallery la saca del camino, para que no aparezca dos veces).</summary>
    public bool Replaces(Sprite sprite)
    {
        if (sprite == null) return false;
        foreach (var d in dioramas) if (!string.IsNullOrEmpty(d.replaces) && sprite.name.StartsWith(d.replaces)) return true;
        return false;
    }

    /// <summary>Metros que ocupan los dioramas despues del final del camino.</summary>
    public float Length
    {
        get
        {
            float last = 0f;
            for (int k = 0; k < dioramas.Length; k++)
                foreach (var l in dioramas[k].layers) last = Mathf.Max(last, k * spacing + l.distance);
            return last;
        }
    }

    public void Build(GoyaGallery g)
    {
        gallery = g;
        environment = GetComponent<GoyaEnvironment>();
        Vector3 c = gallery.Center;
        float eyeY = gallery.RigPosition.y;

        for (int k = 0; k < dioramas.Length; k++)
        {
            var d = dioramas[k];
            float entryZ = c.z + gallery.PathLength + k * spacing;
            int order = -15 - k * 10;
            // Las capas lejanas primero
            var layers = new List<Layer>(d.layers);
            layers.Sort((a, b) => b.distance.CompareTo(a.distance));
            foreach (var l in layers)
            {
                var sprite = Resources.Load<Sprite>(l.image);
                if (sprite == null)
                {
                    Debug.LogWarning("[Diorama] No encuentro la imagen " + l.image + " en Assets/Resources");
                    continue;
                }
                var go = new GameObject("Diorama_" + d.name + "_" + l.image);
                go.transform.SetParent(transform, false);
                // Mas lejos = mas grande, en la misma proporcion: desde la entrada todas las capas coinciden
                float k10 = l.distance / 10f;
                go.transform.position = new Vector3(c.x, eyeY + d.centerOffset * k10, entryZ + l.distance);
                float s = d.height * k10 / sprite.bounds.size.y;
                go.transform.localScale = Vector3.one * s;

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = order++;
                built.Add(new Built { renderer = sr, baseScale = go.transform.localScale, breathe = l.breathe, phase = Random.value * 6f });
            }
        }
    }

    void LateUpdate()
    {
        if (gallery == null) return;
        float appear = gallery.appear;
        foreach (var b in built)
        {
            if (b.breathe > 0f)
                b.renderer.transform.localScale = b.baseScale * (1f + b.breathe * Mathf.Sin(Time.time * 0.8f + b.phase));
            // Niebla mas suave que en las obras: el fondo es enorme y tiene que leerse desde la entrada
            Color col = environment != null ? Color.Lerp(Color.white, environment.Fogged(Color.white, b.renderer.transform.position), fog) : Color.white;
            col.a = appear;
            b.renderer.color = col;
        }
    }
}

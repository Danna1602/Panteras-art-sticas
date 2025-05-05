using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NubeAnimada : MonoBehaviour
{
    [Header("Configuración Movimiento")]
    [SerializeField] private float velocidad = 0.5f;
    [SerializeField] private float amplitud = 0.1f;

    [Header("Configuración Opacidad")]
    [SerializeField] private float velocidadParpadeo = 1f;
    [SerializeField] private float minOpacidad = 0.7f;

    private Material materialNube;
    private Vector3 posicionInicial;
    private float tiempo;

    void Start()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        materialNube = renderer.material; // Usa material instance
        posicionInicial = transform.position;
    }

    void Update()
    {
        tiempo += Time.deltaTime;

        // Movimiento suave (flotar)
        float offsetY = Mathf.Sin(tiempo * velocidad) * amplitud;
        transform.position = posicionInicial + new Vector3(0, offsetY, 0);

        // Efecto de opacidad (parpadeo suave)
        float alpha = Mathf.Lerp(minOpacidad, 1f, Mathf.PingPong(tiempo * velocidadParpadeo, 1));
        Color color = materialNube.color;
        color.a = alpha;
        materialNube.color = color;
    }

    void OnDestroy()
    {
        if (materialNube != null)
            Destroy(materialNube); // Limpieza del material instance
    }
}
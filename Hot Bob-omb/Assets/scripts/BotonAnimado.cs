using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class BotonAnimado : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Configuración de Animación")]
    [SerializeField] private float escalaPresionado = 0.9f;
    [SerializeField] private float velocidadAnim = 10f;

    [Header("Configuración de Sonido")]
    [SerializeField] private AudioClip sonidoClick; // Ahora sí aparecerá en el Inspector

    private Vector3 escalaNormal;
    private Coroutine animacionCoroutine;

    void Start()
    {
        escalaNormal = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (animacionCoroutine != null)
            StopCoroutine(animacionCoroutine);

        animacionCoroutine = StartCoroutine(AnimarEscala(escalaNormal * escalaPresionado));

        // Reproducir sonido si está asignado
        if (sonidoClick != null)
        {
            AudioSource.PlayClipAtPoint(sonidoClick, Camera.main.transform.position, 1.0f);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (animacionCoroutine != null)
            StopCoroutine(animacionCoroutine);

        animacionCoroutine = StartCoroutine(AnimarEscala(escalaNormal));
    }

    IEnumerator AnimarEscala(Vector3 escalaObjetivo)
    {
        while (Vector3.Distance(transform.localScale, escalaObjetivo) > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, escalaObjetivo, velocidadAnim * Time.deltaTime);
            yield return null;
        }
        transform.localScale = escalaObjetivo;
    }
}
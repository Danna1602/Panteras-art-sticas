using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MenuInicio : MonoBehaviour
{
    [Header("CONFIGURACIÓN DE SONIDO")]
    [SerializeField] private AudioClip sonidoClick;
    [Range(0, 1)][SerializeField] private float volumen = 1f;
    [SerializeField] private float delayAccion = 0.2f; // Tiempo mínimo para que suene

    private AudioSource audioSource;
    private bool sonidoReproduciendose = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void EmpezarJuego()
    {
        if (sonidoReproduciendose) return;
        StartCoroutine(EjecutarConSonido("Replica minijuego Hot Bob-Omb"));
    }

    public void SalirJuego()
    {
        if (sonidoReproduciendose) return;
        StartCoroutine(EjecutarConSonido(null));
    }

    private IEnumerator EjecutarConSonido(string escena)
    {
        sonidoReproduciendose = true;

        // Reproducir sonido
        if (sonidoClick != null)
        {
            audioSource.PlayOneShot(sonidoClick, volumen);
            Debug.Log($"Reproduciendo sonido: {sonidoClick.name}");

            // Esperar al menos el tiempo mínimo
            float duracionSonido = sonidoClick.length;
            yield return new WaitForSeconds(Mathf.Min(delayAccion, duracionSonido));
        }
        else
        {
            yield return new WaitForSeconds(delayAccion);
        }

        // Ejecutar acción después del sonido
        if (!string.IsNullOrEmpty(escena))
        {
            if (ControladorAudio.Instance != null)
                ControladorAudio.Instance.DetenerMusica();

            SceneManager.LoadScene(escena);
        }
        else
        {
            if (ControladorAudio.Instance != null)
                ControladorAudio.Instance.DetenerMusica();

            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        sonidoReproduciendose = false;
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorAudio : MonoBehaviour
{
    public static ControladorAudio Instance;
    private AudioSource audioSource;

    [Header("Configuración")]
    public float volumen = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.volume = volumen;
            audioSource.Play();
            Debug.Log("AudioManager creado y música iniciada.");
        }
        else
        {
            Debug.Log("AudioManager duplicado - Destruyendo instancia nueva.");
            Destroy(gameObject);
        }
    }

    public void DetenerMusica()
    {
        if (audioSource != null)
        {
            Debug.Log("Deteniendo música y destruyendo AudioManager.");
            audioSource.Stop();
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("AudioSource no encontrado al detener música.");
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControladorAudio : MonoBehaviour
{
    public static ControladorAudio Instance;
    private AudioSource audioSource;

    [Header("Configuraci�n")]
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
            Debug.Log("AudioManager creado y m�sica iniciada.");
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
            Debug.Log("Deteniendo m�sica y destruyendo AudioManager.");
            audioSource.Stop();
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("AudioSource no encontrado al detener m�sica.");
        }
    }
}
using UnityEngine;

public class GameSoundManager : MonoBehaviour
{
    [Header("Referencias del GameManager")]
    public GameManager gameManager;

    [Header("Clips de Audio")]
    public AudioClip sonidoTomarBomba;
    public AudioClip sonidoLanzarBomba;
    public AudioClip sonidoExplosion;
    public AudioClip sonidoVictoria;

    [Header("Configuración")]
    public AudioSource efectosSource;
    public AudioSource musicaSource;

    private GameObject ultimoPortador;
    private bool bombaEnMovimiento;
    private bool esperandoSonidoTomar;
    private bool victoriaReproducida;
    private const float distanciaMinima = 0.5f;

    void Start()
    {
        if (efectosSource == null) efectosSource = gameObject.AddComponent<AudioSource>();
        if (musicaSource == null) musicaSource = gameObject.AddComponent<AudioSource>();

        if (gameManager != null)
        {
            ultimoPortador = gameManager.ObtenerPortadorBombaActual();
        }
        bombaEnMovimiento = false;
        esperandoSonidoTomar = false;
        victoriaReproducida = false;
    }

    void Update()
    {
        if (gameManager == null || gameManager.bomba == null) return;

        GameObject portadorActual = gameManager.ObtenerPortadorBombaActual();
        BombMovement movimientoBomba = gameManager.bomba.GetComponent<BombMovement>();

        ControlarSonidosTransporte(portadorActual, movimientoBomba);
        ControlarSonidoVictoria();
    }

    void ControlarSonidosTransporte(GameObject portadorActual, BombMovement movimientoBomba)
    {
        if (movimientoBomba == null) return;

        if (movimientoBomba.EstaEnMovimiento() && !bombaEnMovimiento)
        {
            bombaEnMovimiento = true;
            esperandoSonidoTomar = true;
        }

        if (esperandoSonidoTomar && !movimientoBomba.EstaEnMovimiento() && portadorActual != null)
        {
            if (Vector3.Distance(gameManager.bomba.transform.position,
                               portadorActual.transform.position) < distanciaMinima)
            {
                efectosSource.PlayOneShot(sonidoTomarBomba);
                esperandoSonidoTomar = false;
                ultimoPortador = portadorActual;
            }
        }

        bombaEnMovimiento = movimientoBomba.EstaEnMovimiento();
    }

    // Llamar este método desde el GameManager cuando comience la animación de explosión
    public void ReproducirExplosion()
    {
        efectosSource.PlayOneShot(sonidoExplosion);
        ultimoPortador = null;
    }

    void ControlarSonidoVictoria()
    {
        if (Time.timeScale == 0 && !victoriaReproducida && sonidoVictoria != null)
        {
            musicaSource.PlayOneShot(sonidoVictoria);
            victoriaReproducida = true;
        }
    }

    public void ReproducirSonidoLanzar()
    {
        if (sonidoLanzarBomba != null && efectosSource != null)
        {
            efectosSource.PlayOneShot(sonidoLanzarBomba);
        }
    }
}
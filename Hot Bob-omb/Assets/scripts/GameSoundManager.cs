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
    private bool explosionReproducida;
    private bool victoriaReproducida;
    private const float distanciaMinima = 0.5f;
    private float ultimoTiempoSonidoExplosion;
    private bool juegoIniciado = false;

    void Start()
    {
        if (efectosSource == null) efectosSource = gameObject.AddComponent<AudioSource>();
        if (musicaSource == null) musicaSource = gameObject.AddComponent<AudioSource>();

        if (ultimoPortador == null) ;
        bombaEnMovimiento = false;
        esperandoSonidoTomar = false;
        explosionReproducida = false;
        victoriaReproducida = false;
        ultimoTiempoSonidoExplosion = 0f;

        // Retrasar el flag de juego iniciado para evitar sonido al inicio
        Invoke("MarcarJuegoIniciado", 1f);
    }

    void MarcarJuegoIniciado()
    {
        juegoIniciado = true;
    }

    void Update()
    {
        if (gameManager == null || gameManager.bomba == null || !juegoIniciado) return;

        GameObject portadorActual = gameManager.ObtenerPortadorBombaActual();
        BombMovement movimientoBomba = gameManager.bomba.GetComponent<BombMovement>();

        ControlarSonidosTransporte(portadorActual, movimientoBomba);
        ControlarSonidoExplosion();
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

    void ControlarSonidoExplosion()
    {
        // Verificar si la bomba está activa pero no tiene portador (indica que explotó)
        if (gameManager.bomba.activeSelf && gameManager.ObtenerPortadorBombaActual() == null && !explosionReproducida)
        {
            efectosSource.PlayOneShot(sonidoExplosion);
            explosionReproducida = true;
            ultimoTiempoSonidoExplosion = Time.time;
        }
        // Resetear flag después de 1 segundo
        else if (explosionReproducida && Time.time - ultimoTiempoSonidoExplosion > 1f)
        {
            explosionReproducida = false;
        }
    }

    void ControlarSonidoVictoria()
    {
        if (Time.timeScale == 0 && !victoriaReproducida && sonidoVictoria != null)
        {
            musicaSource.Stop();
            efectosSource.PlayOneShot(sonidoVictoria);
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
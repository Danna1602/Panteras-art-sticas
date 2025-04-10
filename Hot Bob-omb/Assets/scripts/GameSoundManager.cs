using UnityEngine;

public class GameSoundManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameManager gameManager;

    [Header("Audio Clips")]
    public AudioClip sonidoTomarBomba;
    public AudioClip sonidoLanzarBomba;
    public AudioClip sonidoExplosion;
    public AudioClip sonidoVictoria;

    public AudioClip sonidoMecha;
    public AudioClip musicaFase1;
    public AudioClip musicaFase2;

    [Header("Configuracion")]
    public AudioSource efectosSource;
    public AudioSource musicaSource;
    public AudioSource mechaSource;

    [Header("Velocidad")]
    public float pitchNormal = 1f;
    public float pitchFase1Acelerado = 1.2f;
    public float pitchFase2Acelerado = 1.5f;
    public float pitchMechaAcelerada = 1.5f;

    [Header("Volumen")]
    [Range(0, 1)] public float volumenGeneral = 1f;
    [Range(0, 1)] public float volumenEfectos = 1f;
    [Range(0, 1)] public float volumenMusica = 1f;
    [Range(0, 1)] public float volumenMecha = 1f;


    private GameObject ultimoPortador;
    private bool bombaEnMovimiento;
    private bool esperandoSonidoTomar;
    private bool explosionReproducida;
    private bool victoriaReproducida;
    private const float distanciaMinima = 0.5f;
    private float ultimoTiempoSonidoExplosion;
    private bool juegoIniciado = false;
    private float tiempoInicioRonda;
    private bool velocidadAumentada = false;
    private bool enFaseFinal = false;

    void Start()
    {
        if (efectosSource == null) efectosSource = gameObject.AddComponent<AudioSource>();
        if (musicaSource == null) musicaSource = gameObject.AddComponent<AudioSource>();
        if (mechaSource == null) mechaSource = gameObject.AddComponent<AudioSource>();

        ResetearEstados();
        ActualizarVolumenes(); // Nueva línea añadida
        Invoke("MarcarJuegoIniciado", 1f);
    }

    void ResetearEstados()
    {
        bombaEnMovimiento = false;
        esperandoSonidoTomar = false;
        explosionReproducida = false;
        victoriaReproducida = false;
        velocidadAumentada = false;
        ultimoTiempoSonidoExplosion = 0f;
        mechaSource.pitch = pitchNormal;
        musicaSource.pitch = pitchNormal;
    }

    // NUEVO MÉTODO: Actualiza todos los volúmenes
    void ActualizarVolumenes()
    {
        efectosSource.volume = volumenGeneral * volumenEfectos;
        musicaSource.volume = volumenGeneral * volumenMusica;
        mechaSource.volume = volumenGeneral * volumenMecha;
    }

    void MarcarJuegoIniciado()
    {
        juegoIniciado = true;
        tiempoInicioRonda = Time.time;
        IniciarAudioRonda();
    }


    void IniciarAudioRonda()
    {
        musicaSource.clip = enFaseFinal ? musicaFase2 : musicaFase1;
        musicaSource.loop = true;
        musicaSource.Play();

        mechaSource.clip = sonidoMecha;
        mechaSource.loop = true;
        mechaSource.Play();
    }

    void Update()
    {
        if (gameManager == null || gameManager.bomba == null || !juegoIniciado) return;

        // Controlar aumento de velocidad con nuevos tiempos por fase
        if (!velocidadAumentada)
        {
            if (!enFaseFinal && Time.time - tiempoInicioRonda > 40f) // 40s para fase 1
            {
                AumentarVelocidadSonidos();
            }
            else if (enFaseFinal && Time.time - tiempoInicioRonda > 10f) // 10s para fase 2
            {
                AumentarVelocidadSonidos();
            }
        }

        // Lógica original sin cambios
        GameObject portadorActual = gameManager.ObtenerPortadorBombaActual();
        BombMovement movimientoBomba = gameManager.bomba.GetComponent<BombMovement>();

        ControlarSonidosTransporte(portadorActual, movimientoBomba);
        ControlarSonidoExplosion();
        ControlarSonidoVictoria();
    }

    void AumentarVelocidadSonidos()
    {
        velocidadAumentada = true;
        mechaSource.pitch = pitchMechaAcelerada;
        musicaSource.pitch = enFaseFinal ? pitchFase2Acelerado : pitchFase1Acelerado;
    }

    // Métodos originales SIN CAMBIOS
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

        if (gameManager.bomba.activeSelf && gameManager.ObtenerPortadorBombaActual() == null && !explosionReproducida)
        {
            efectosSource.PlayOneShot(sonidoExplosion);
            explosionReproducida = true;
            ultimoTiempoSonidoExplosion = Time.time;
            mechaSource.Stop();
        }

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
            mechaSource.Stop();
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


    public void CambiarAFaseFinal()
    {
        enFaseFinal = true;
        tiempoInicioRonda = Time.time;
        velocidadAumentada = false; // Resetear para nueva fase

        musicaSource.clip = musicaFase2;
        musicaSource.pitch = pitchNormal;
        musicaSource.Play();

        mechaSource.pitch = pitchNormal;
        mechaSource.Play();
    }


    // NUEVO: Métodos públicos para ajustar volúmenes desde otros scripts
    public void SetVolumenGeneral(float volumen)
    {
        volumenGeneral = Mathf.Clamp01(volumen);
        ActualizarVolumenes();
    }

    public void SetVolumenEfectos(float volumen)
    {
        volumenEfectos = Mathf.Clamp01(volumen);
        ActualizarVolumenes();
    }

    public void SetVolumenMusica(float volumen)
    {
        volumenMusica = Mathf.Clamp01(volumen);
        ActualizarVolumenes();
    }

    public void SetVolumenMecha(float volumen)
    {
        volumenMecha = Mathf.Clamp01(volumen);
        ActualizarVolumenes();
    }
}
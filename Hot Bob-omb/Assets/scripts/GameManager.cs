using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Configuración del Juego")]
    public GameObject[] personajes;
    public GameObject bomba;
    public float tiempoRondaInicial = 60f;
    public float tiempoRondaFinal = 30f;

    [Header("Estado del Juego")]
    [SerializeField] private GameObject portadorBombaActual;
    [SerializeField] private float temporizador;
    [SerializeField] private bool rondaFinal = false;
    [SerializeField] private bool juegoTerminado = false;

    private float tiempoInicioJuego;
    private bool velocidadAumentada = false;
    private BombaAnimatorController bombaAnimController;

    void Start()
    {
        if (bomba == null)
        {
            Debug.LogError("No se ha asignado el objeto bomba en el Inspector");
            return;
        }

        bombaAnimController = bomba.GetComponent<BombaAnimatorController>();
        if (bombaAnimController == null)
        {
            Debug.LogError("No se encontró el componente BombaAnimatorController en la bomba");
            return;
        }

        IniciarJuego();
    }

    void IniciarJuego()
    {
        tiempoInicioJuego = Time.time;
        velocidadAumentada = false;
        bomba.transform.position = Vector3.zero;
        portadorBombaActual = null;
        rondaFinal = false;
        juegoTerminado = false;
        Time.timeScale = 1f;

        foreach (var personaje in personajes)
        {
            if (personaje != null)
            {
                personaje.SetActive(true);
            }
        }

        ActivarBomba();
        StartCoroutine(EmpezarRonda());
    }

    void ActivarBomba()
    {
        bomba.SetActive(true);
        if (bombaAnimController != null && bomba.activeSelf)
        {
            bombaAnimController.ReiniciarAnimacion();
        }
    }

    IEnumerator EmpezarRonda()
    {
        yield return new WaitForSeconds(1f);
        AsignarBombaAleatoriamente();
        temporizador = rondaFinal ? tiempoRondaFinal : tiempoRondaInicial;
    }

    void Update()
    {
        if (juegoTerminado) return;

        if (!velocidadAumentada && Time.time - tiempoInicioJuego > 20f)
        {
            velocidadAumentada = true;
            Debug.Log("Velocidad de la bomba aumentada!");
        }

        // Solo decrementar temporizador si hay portador activo
        if (portadorBombaActual != null)
        {
            temporizador -= Time.deltaTime;

            if (temporizador <= 0f)
            {
                StartCoroutine(ProcesarExplosion());
            }
        }
    }


    IEnumerator ProcesarExplosion()
    {
        // 1. Sonido de explosión (inmediato al iniciar)
        var soundManager = FindObjectOfType<GameSoundManager>();
        if (soundManager != null) soundManager.ReproducirExplosion();

        // 2. Ejecutar animación de explosión de la bomba
        bombaAnimController.ForzarExplosion();

        // 3. Ejecutar animación de muerte del portador actual
        if (portadorBombaActual != null && portadorBombaActual.activeSelf)
        {
            var animController1 = portadorBombaActual.GetComponent<Personaje1_0AnimatorController>();
            var animController2 = portadorBombaActual.GetComponent<Personaje2_0AnimatorController>();
            var animController3 = portadorBombaActual.GetComponent<Personaje3_0AnimatorController>();

            if (animController1 != null)
                animController1.EjecutarAnimacionMuerte();
            else if (animController2 != null)
                animController2.EjecutarAnimacionMuerte();
            else if (animController3 != null)
                animController3.EjecutarAnimacionMuerte();
        }

        yield return new WaitForSeconds(1f); // Esperar que terminen animaciones

        // 4. Desactivar objetos
        bomba.SetActive(false);

        if (portadorBombaActual != null)
        {
            portadorBombaActual.SetActive(false);
            portadorBombaActual = null;
        }

        // 5. Manejar lógica post-explosión
        int jugadoresVivos = ContarPersonajesActivos();

        if (jugadoresVivos <= 1)
        {
            TerminarJuego(ObtenerPersonajeActivo());
            yield break;
        }

        if (!rondaFinal && jugadoresVivos == 2)
        {
            IniciarRondaFinal();
        }
        else if (!rondaFinal)
        {
            ReiniciarBomba();
        }
        else
        {
            TerminarJuego(ObtenerPersonajeActivo());
        }
    }


    // Métodos auxiliares para mejor organización
    private void IniciarRondaFinal()
    {
        rondaFinal = true;
        tiempoInicioJuego = Time.time;
        velocidadAumentada = false;
        temporizador = tiempoRondaFinal;

        // Asegurarse que la bomba esté activa antes de cambiar de fase
        if (!bomba.activeSelf)
        {
            bomba.SetActive(true);
        }

        // Solo establecer segunda fase si la bomba está activa
        if (bomba.activeSelf && bombaAnimController != null)
        {
            bombaAnimController.EstablecerSegundaFase();
        }

        // Asegurar que tenemos exactamente 2 jugadores activos
        int jugadoresActivos = ContarPersonajesActivos();
        if (jugadoresActivos > 2)
        {
            Debug.LogError("¡Error! No debería haber más de 2 jugadores al iniciar ronda final");
        }

        ActivarBomba();
        AsignarBombaAleatoriamente();
    }

    private void ReiniciarBomba()
    {
        ActivarBomba();
        AsignarBombaAleatoriamente();
    }

    public void IntentarPasarBomba(GameObject personajeObjetivo)
    {
        if (juegoTerminado || personajeObjetivo == null || portadorBombaActual == null) return;
        if (personajeObjetivo == portadorBombaActual) return;
        if (!personajeObjetivo.activeSelf) return;

        BombMovement movimiento = bomba.GetComponent<BombMovement>();
        if (movimiento == null || movimiento.EstaEnMovimiento()) return;

        movimiento.MoverHacia(personajeObjetivo.transform.position);
        portadorBombaActual = personajeObjetivo;

        var soundManager = FindObjectOfType<GameSoundManager>();
        if (soundManager != null) soundManager.ReproducirSonidoLanzar();


        StopAllCoroutines();
        StartCoroutine(ContadorBomba());
    }

    int ContarPersonajesActivos()
    {
        int contador = 0;
        foreach (var personaje in personajes)
        {
            if (personaje != null && personaje.activeSelf) contador++;
        }
        return contador;
    }

    void AsignarBombaAleatoriamente()
    {
        List<GameObject> personajesActivos = new List<GameObject>();
        foreach (var personaje in personajes)
        {
            if (personaje != null && personaje.activeSelf)
                personajesActivos.Add(personaje);
        }

        if (personajesActivos.Count == 0)
        {
            TerminarJuego(null);
            return;
        }

        int indiceAleatorio = Random.Range(0, personajesActivos.Count);
        portadorBombaActual = personajesActivos[indiceAleatorio];

        // Reiniciar el temporizador cuando se asigna nueva bomba
        temporizador = rondaFinal ? tiempoRondaFinal : tiempoRondaInicial;

        BombMovement movimiento = bomba.GetComponent<BombMovement>();
        if (movimiento != null)
        {
            movimiento.MoverHacia(portadorBombaActual.transform.position);
        }
        else
        {
            bomba.transform.position = portadorBombaActual.transform.position;
        }

        if (bomba.activeSelf)
        {
            bombaAnimController.ReiniciarAnimacion();
        }
        else
        {
            ActivarBomba();
        }

        StartCoroutine(ContadorBomba());
    }

    IEnumerator ContadorBomba()
    {
        yield return new WaitForSeconds(3f);

        if (portadorBombaActual != null && bomba.activeSelf &&
            Vector3.Distance(bomba.transform.position, portadorBombaActual.transform.position) < 0.1f)
        {
            yield return StartCoroutine(ProcesarExplosion());
        }
    }

    void ExplotarPortadorActual()
    {
        if (portadorBombaActual != null)
        {
            var soundManager = FindObjectOfType<GameSoundManager>();
            if (soundManager != null) soundManager.ReproducirExplosion();

            var animController1 = portadorBombaActual.GetComponent<Personaje1_0AnimatorController>();
            var animController2 = portadorBombaActual.GetComponent<Personaje2_0AnimatorController>();
            var animController3 = portadorBombaActual.GetComponent<Personaje3_0AnimatorController>();

            if (animController1 != null)
            {
                animController1.EjecutarAnimacionMuerte();
            }
            else if (animController2 != null)
            {
                animController2.EjecutarAnimacionMuerte();
            }
            else if (animController3 != null)
            {
                animController3.EjecutarAnimacionMuerte();
            }
            else
            {
                portadorBombaActual.SetActive(false);
            }

            portadorBombaActual = null;
        }
    }

    void VerificarFinDelJuego()
    {
        int jugadoresRestantes = ContarPersonajesActivos();

        if (jugadoresRestantes <= 1)
        {
            TerminarJuego(ObtenerPersonajeActivo());
        }
        else if (!rondaFinal && jugadoresRestantes == 2)
        {
            // Entrar en ronda final
            rondaFinal = true;
            tiempoInicioJuego = Time.time;
            velocidadAumentada = false;
            ActivarBomba();
            StartCoroutine(EmpezarRonda());
        }
        else if (!rondaFinal)
        {
            // Continuar juego normal
            ActivarBomba();
            AsignarBombaAleatoriamente();
        }
        // En ronda final no hacemos nada más (el juego termina)
    }

    public GameObject ObtenerPortadorBombaActual()
    {
        return portadorBombaActual;
    }

    GameObject ObtenerPersonajeActivo()
    {
        foreach (var personaje in personajes)
        {
            if (personaje != null && personaje.activeSelf)
                return personaje;
        }
        return null;
    }

    void TerminarJuego(GameObject ganador)
    {
        if (juegoTerminado) return;

        juegoTerminado = true;
        StopAllCoroutines();

        if (ganador != null)
        {
            Debug.Log($"¡Juego terminado! Ganador: {ganador.name}");
        }
        else
        {
            Debug.Log("¡El juego terminó sin ganador!");
        }

        Time.timeScale = 0f;
    }
}
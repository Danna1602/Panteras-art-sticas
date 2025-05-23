﻿using UnityEngine;
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
        // Guardar referencia al portador que explotará
        GameObject portadorExplotado = portadorBombaActual;
        portadorBombaActual = null; // Limpiar inmediatamente para evitar múltiples explosiones

        // 1. Ejecutar animación de explosión de la bomba
        bombaAnimController.ForzarExplosion();

        // 2. Ejecutar animación de muerte SOLO del portador actual
        if (portadorExplotado != null && portadorExplotado.activeSelf)
        {
            var animController1 = portadorExplotado.GetComponent<Personaje1_0AnimatorController>();
            var animController2 = portadorExplotado.GetComponent<Personaje2_0AnimatorController>();
            var animController3 = portadorExplotado.GetComponent<Personaje3_0AnimatorController>();

            if (animController1 != null)
                animController1.EjecutarAnimacionMuerte();
            else if (animController2 != null)
                animController2.EjecutarAnimacionMuerte();
            else if (animController3 != null)
                animController3.EjecutarAnimacionMuerte();
        }

        yield return new WaitForSeconds(1f);

        // 3. Desactivar bomba
        bomba.SetActive(false);

        // 4. Manejar transición de fase
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
    } // Métodos auxiliares para mejor organización
    private void IniciarRondaFinal()
    {
        FindObjectOfType<GameSoundManager>().CambiarAFaseFinal();
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
        if (soundManager != null) soundManager.ReproducirSonidoLanzar(); // Punto y coma añadido aquí

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

    IEnumerator TerminarJuegoConVictoria(GameObject ganador)
    {
        juegoTerminado = true;

        if (ganador != null)
        {
            // Ejecutar animación de victoria
            var animController1 = ganador.GetComponent<Personaje1_0AnimatorController>();
            var animController2 = ganador.GetComponent<Personaje2_0AnimatorController>();
            var animController3 = ganador.GetComponent<Personaje3_0AnimatorController>();

            if (animController1 != null)
                animController1.EjecutarAnimacionVictoria();
            else if (animController2 != null)
                animController2.EjecutarAnimacionVictoria();
            else if (animController3 != null)
                animController3.EjecutarAnimacionVictoria();

            // Esperar antes de pausar el juego
            yield return new WaitForSeconds(3f); // Ajusta este tiempo según tu animación
        }

        Time.timeScale = 0f;
        FindObjectOfType<GameGUI>().MostrarGanador();
    }

    void TerminarJuego(GameObject ganador)
    {
        if (juegoTerminado) return;
        StartCoroutine(TerminarJuegoConVictoria(ganador));
    }
}
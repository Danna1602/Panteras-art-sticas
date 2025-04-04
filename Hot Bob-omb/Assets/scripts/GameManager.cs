using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public GameObject[] personajes;
    public GameObject bomba;
    public float tiempoRondaInicial = 60f;
    public float tiempoRondaFinal = 30f;

    private GameObject portadorBombaActual;
    private float temporizador;
    private bool rondaFinal = false;
    private bool juegoTerminado = false;
    private float tiempoInicioJuego;
    private bool velocidadAumentada = false;

    void Start()
    {
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

        StartCoroutine(EmpezarRonda());
    }

    IEnumerator EmpezarRonda()
    {
        yield return new WaitForSeconds(1f);
        AsignarBombaAleatoriamente();
        temporizador = rondaFinal ? tiempoRondaFinal : tiempoRondaInicial;
    }

    void Update()
    {
        if (juegoTerminado || portadorBombaActual == null) return;

        if (!velocidadAumentada && Time.time - tiempoInicioJuego > 20f)
        {
            velocidadAumentada = true;
        }

        temporizador -= Time.deltaTime;

        if (temporizador <= 0f)
        {
            ExplotarPortadorActual();

            if (!rondaFinal && ContarPersonajesActivos() > 1)
            {
                rondaFinal = true;
                tiempoInicioJuego = Time.time;
                velocidadAumentada = false;
                StartCoroutine(EmpezarRonda());
            }
            else
            {
                TerminarJuego(ObtenerPersonajeActivo());
            }
        }
    }

    public void IntentarPasarBomba(GameObject personajeObjetivo)
    {
        if (personajeObjetivo == null || portadorBombaActual == null) return;
        if (personajeObjetivo == portadorBombaActual) return;
        if (!personajeObjetivo.activeSelf) return;

        BombMovement movimiento = bomba.GetComponent<BombMovement>();
        if (movimiento == null || movimiento.EstaEnMovimiento()) return;

        movimiento.MoverHacia(personajeObjetivo.transform.position);
        portadorBombaActual = personajeObjetivo;

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

        if (personajesActivos.Count == 0) return;

        int indiceAleatorio = Random.Range(0, personajesActivos.Count);
        portadorBombaActual = personajesActivos[indiceAleatorio];

        BombMovement movimiento = bomba.GetComponent<BombMovement>();
        if (movimiento != null)
        {
            movimiento.MoverHacia(portadorBombaActual.transform.position);
        }
        else
        {
            bomba.transform.position = portadorBombaActual.transform.position;
        }

        StartCoroutine(ContadorBomba());
    }

    IEnumerator ContadorBomba()
    {
        yield return new WaitForSeconds(3f);

        if (portadorBombaActual != null && bomba.transform.position == portadorBombaActual.transform.position)
        {
            ExplotarPortadorActual();

            if (ContarPersonajesActivos() == 1)
            {
                TerminarJuego(ObtenerPersonajeActivo());
            }
            else
            {
                AsignarBombaAleatoriamente();
            }
        }
    }

    void ExplotarPortadorActual()
    {
        if (portadorBombaActual != null)
        {
            portadorBombaActual.SetActive(false);
            portadorBombaActual = null;
        }
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
        if (ganador == null) return;

        juegoTerminado = true;
        Time.timeScale = 0f;
    }
}
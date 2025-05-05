using UnityEngine;
using System.Linq;

public class CPUController : MonoBehaviour
{
    [Header("Configuración Normal")]
    public float tiempoMinReaccion = 0.5f;
    public float tiempoMaxReaccion = 1f;
    [Range(0f, 1f)] public float probabilidadPasarBomba = 0.98f;

    [Header("Configuración Fase Final")]
    public float tiempoMinFinal = 0.2f;
    public float tiempoMaxFinal = 0.5f;
    [Range(0f, 1f)] public float probabilidadPasarBombaFinal = 0.98f;
    private const float probabilidadExplosionFinal = 0.00001f;

    private GameManager gameManager;
    private float tiempoSiguienteAccion;
    private bool esPortador = false;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager no encontrado");
            return;
        }
        CalcularNuevoTiempoReaccion();
    }

    void Update()
    {
        if (gameManager == null || !gameObject.activeSelf) return;

        // Verificar si es el portador actual usando el método público existente
        bool nuevoEstado = (gameManager.ObtenerPortadorBombaActual() == gameObject);

        if (nuevoEstado != esPortador)
        {
            esPortador = nuevoEstado;
            if (esPortador)
            {
                CalcularNuevoTiempoReaccion();
                VerificarExplosionAleatoria();
            }
        }

        if (esPortador && Time.time >= tiempoSiguienteAccion)
        {
            DecidirAccion();
            CalcularNuevoTiempoReaccion();
        }
    }

    void VerificarExplosionAleatoria()
    {
        // Usamos ContarPersonajesActivos() para determinar si estamos en fase final
        bool esFaseFinal = (gameManager.personajes.Count(p => p != null && p.activeSelf) <= 2);
        float probabilidadActual = esFaseFinal ? probabilidadExplosionFinal : 0.0001f;

        if (Random.value <= probabilidadActual)
        {
            Debug.Log($"{gameObject.name} ¡BOOM! {(esFaseFinal ? "FASE FINAL" : "")}");
            gameManager.IntentarPasarBomba(gameObject);
        }
    }

    void DecidirAccion()
    {
        // Determinamos fase final por cantidad de jugadores vivos
        bool esFaseFinal = (gameManager.personajes.Count(p => p != null && p.activeSelf) <= 2);
        float probabilidadActual = esFaseFinal ? probabilidadPasarBombaFinal : probabilidadPasarBomba;

        if (Random.value <= probabilidadActual)
        {
            PasarBombaAOtroPersonaje();
        }
    }

    void PasarBombaAOtroPersonaje()
    {
        var objetivosValidos = gameManager.personajes
                              .Where(p => p != null && p.activeSelf && p != gameObject)
                              .ToArray();

        if (objetivosValidos.Length == 0) return;

        GameObject objetivo = objetivosValidos[Random.Range(0, objetivosValidos.Length)];
        gameManager.IntentarPasarBomba(objetivo);
    }

    void CalcularNuevoTiempoReaccion()
    {
        // Determinamos fase final por cantidad de jugadores
        bool esFaseFinal = (gameManager.personajes.Count(p => p != null && p.activeSelf) <= 2);

        if (esFaseFinal)
        {
            tiempoSiguienteAccion = Time.time + Random.Range(tiempoMinFinal, tiempoMaxFinal);
        }
        else
        {
            tiempoSiguienteAccion = Time.time + Random.Range(tiempoMinReaccion, tiempoMaxReaccion);
        }
    }
}
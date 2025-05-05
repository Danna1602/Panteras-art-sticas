using UnityEngine;

public class BombMovement : MonoBehaviour
{
    [Header("Configuraci�n de Movimiento")]
    [Tooltip("Duraci�n base del movimiento en segundos")]
    public float duracionBase = 0.5f;
    [Tooltip("Altura m�xima del arco parab�lico")]
    public float altura = 2f;
    [Tooltip("Reducci�n de tiempo despu�s de 20 segundos")]
    public float incrementoVelocidad = 0.2f;

    // Estado privado
    private bool estaEnMovimiento = false;
    private Vector3 posicionInicial;
    private Vector3 posicionDestino;
    private float duracionActual;
    private float tiempoInicio;

    /// <summary>
    /// Verifica si la bomba est� en movimiento
    /// </summary>
    public bool EstaEnMovimiento()
    {
        return estaEnMovimiento;
    }

    /// <summary>
    /// Inicia el movimiento parab�lico hacia el objetivo
    /// </summary>
    public void MoverHacia(Vector3 destino)
    {
        if (estaEnMovimiento) return;

        posicionInicial = transform.position;
        posicionDestino = destino;
        tiempoInicio = Time.time;
        duracionActual = duracionBase - incrementoVelocidad;
        estaEnMovimiento = true;
    }

    void Update()
    {
        if (!estaEnMovimiento) return;

        float progreso = Mathf.Clamp01((Time.time - tiempoInicio) / duracionActual);
        float curvaParabola = 1 - Mathf.Pow(2 * progreso - 1, 2); // Curva parab�lica

        // Aplicar movimiento
        transform.position = Vector3.Lerp(posicionInicial, posicionDestino, progreso) +
                          Vector3.up * curvaParabola * altura;

        // Rotaci�n durante el vuelo
        transform.Rotate(Vector3.forward, 500 * Time.deltaTime);

        // Finalizar movimiento
        if (progreso >= 1f)
        {
            transform.position = posicionDestino;
            transform.rotation = Quaternion.identity;
            estaEnMovimiento = false;
        }
    }
}
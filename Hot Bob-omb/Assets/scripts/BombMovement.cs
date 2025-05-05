using UnityEngine;

public class BombMovement : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float duracionBase = 0.5f;
    public float altura = 2f;
    public float incrementoVelocidad = 0.2f;

    private bool estaEnMovimiento = false;
    private Vector3 posicionInicial;
    private Vector3 posicionDestino;
    private float duracionActual;
    private float tiempoInicio;
    public Vector3 UltimaDireccion { get; private set; }

    public bool EstaEnMovimiento()
    {
        return estaEnMovimiento;
    }

    public void MoverHacia(Vector3 destino)
    {
        if (estaEnMovimiento) return;

        posicionInicial = transform.position;
        posicionDestino = destino;
        UltimaDireccion = (destino - posicionInicial).normalized; // Guarda la dirección
        tiempoInicio = Time.time;
        duracionActual = duracionBase - incrementoVelocidad;
        estaEnMovimiento = true;
    }

    void Update()
    {
        if (!estaEnMovimiento) return;

        float progreso = Mathf.Clamp01((Time.time - tiempoInicio) / duracionActual);
        float curvaParabola = 1 - Mathf.Pow(2 * progreso - 1, 2);

        transform.position = Vector3.Lerp(posicionInicial, posicionDestino, progreso) +
                          Vector3.up * curvaParabola * altura;

        transform.Rotate(Vector3.forward, 500 * Time.deltaTime);

        if (progreso >= 1f)
        {
            transform.position = posicionDestino;
            transform.rotation = Quaternion.identity;
            estaEnMovimiento = false;
        }
    }
}
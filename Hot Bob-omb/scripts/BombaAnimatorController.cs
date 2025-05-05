using UnityEngine;
using System.Collections;

public class BombaAnimatorController : MonoBehaviour
{
    private Animator anim;

    [Header("Duración Estados")]
    public float tiempoIdle = 4f; // Añadido tiempo para Idle
    public float tiempoPreAlerta = 36f;
    public float tiempoAlerta = 5f;
    public float tiempoPeligro = 5f;
    public float tiempoCritico = 10f;
    public float tiempoExplosión = 3f; // Añadido tiempo para Explosion

    void Start()
    {
        anim = GetComponent<Animator>();
        // Inicia en estado Idle automáticamente
        StartCoroutine(SecuenciaAnimacionCompleta());
    }

    IEnumerator SecuenciaAnimacionCompleta()
    {
        // Estado Idle (automático desde Entry)
        yield return new WaitForSeconds(tiempoIdle);

        // Secuencia principal
        anim.SetTrigger("PreAlerta");
        yield return new WaitForSeconds(tiempoPreAlerta);

        anim.SetTrigger("Alerta");
        yield return new WaitForSeconds(tiempoAlerta);

        anim.SetTrigger("Peligro");
        yield return new WaitForSeconds(tiempoPeligro);

        anim.SetTrigger("Critico");
        yield return new WaitForSeconds(tiempoCritico);

        anim.SetTrigger("Explosión");
        yield return new WaitForSeconds(tiempoExplosión);

        Destroy(gameObject); // Destruye la bomba después de la explosión
    }

    public void ReiniciarAnimacion()
    {
        StopAllCoroutines();
        anim.ResetTrigger("PreAlerta"); // Resetea todos los triggers
        anim.ResetTrigger("Alerta");
        anim.ResetTrigger("Peligro");
        anim.ResetTrigger("Critico");
        anim.ResetTrigger("Explosion");
        StartCoroutine(SecuenciaAnimacionCompleta());
    }
}
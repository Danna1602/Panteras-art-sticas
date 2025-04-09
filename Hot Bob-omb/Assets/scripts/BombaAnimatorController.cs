using UnityEngine;
using System.Collections;

public class BombaAnimatorController : MonoBehaviour
{
    private Animator anim;

    [Header("Animator Controllers")]
    public RuntimeAnimatorController animadorFase1;
    public RuntimeAnimatorController animadorFase2;
    public RuntimeAnimatorController animadorExplosionRapida; // Nuevo controlador para explosión rápida

    [Header("Duración Estados Fase 1")]
    public float tiempoIdle = 4f;
    public float tiempoPreAlerta = 36f;
    public float tiempoAlerta = 5f;
    public float tiempoPeligro = 5f;
    public float tiempoCritico = 10f;
    public float tiempoExplosión = 3f;

    [Header("Duración Estados Fase 2")]
    public float tiempoIdle2 = 2f;
    public float tiempoPreAlerta2 = 8f;
    public float tiempoAlerta2 = 5f;
    public float tiempoPeligro2 = 5f;
    public float tiempoCritico2 = 8f;
    public float tiempoExplosion2 = 2f;

    [Header("Duración Explosión Rápida")]
    public float tiempoExplosionRapida = 1f; // Tiempo para la explosión rápida

    private bool esSegundaFase = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(SecuenciaAnimacionCompleta());
    }

    public void EstablecerSegundaFase()
    {
        esSegundaFase = true;
        anim.runtimeAnimatorController = animadorFase2;
        ReiniciarAnimacion();
    }

    IEnumerator SecuenciaAnimacionCompleta()
    {
        if (!esSegundaFase)
        {
            yield return new WaitForSeconds(tiempoIdle);
            anim.SetTrigger("PreAlerta");
            yield return new WaitForSeconds(tiempoPreAlerta);
            anim.SetTrigger("Alerta");
            yield return new WaitForSeconds(tiempoAlerta);
            anim.SetTrigger("Peligro");
            yield return new WaitForSeconds(tiempoPeligro);
            anim.SetTrigger("Critico");
            yield return new WaitForSeconds(tiempoCritico);
            anim.SetTrigger("Explosion");
            yield return new WaitForSeconds(tiempoExplosión);
        }
        else
        {
            yield return new WaitForSeconds(tiempoIdle2);
            anim.SetTrigger("PreAlerta2");
            yield return new WaitForSeconds(tiempoPreAlerta2);
            anim.SetTrigger("Alerta2");
            yield return new WaitForSeconds(tiempoAlerta2);
            anim.SetTrigger("Peligro2");
            yield return new WaitForSeconds(tiempoPeligro2);
            anim.SetTrigger("Critico2");
            yield return new WaitForSeconds(tiempoCritico2);
            anim.SetTrigger("Explosion2");
            yield return new WaitForSeconds(tiempoExplosion2);
        }

        gameObject.SetActive(false);
    }

    public void ForzarExplosion()
    {
        StopAllCoroutines();
        anim.runtimeAnimatorController = animadorExplosionRapida;
        anim.Rebind();
        StartCoroutine(DestruirDespuesDeExplosion());
    }

    IEnumerator DestruirDespuesDeExplosion()
    {
        yield return new WaitForSeconds(tiempoExplosionRapida);
        gameObject.SetActive(false);
    }

    public void ReiniciarAnimacion()
    {
        StopAllCoroutines();
        anim.runtimeAnimatorController = esSegundaFase ? animadorFase2 : animadorFase1;
        anim.Rebind();
        StartCoroutine(SecuenciaAnimacionCompleta());
    }
}
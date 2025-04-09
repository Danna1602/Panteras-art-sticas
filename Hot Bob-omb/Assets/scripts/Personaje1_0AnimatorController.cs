using UnityEngine;
using System.Collections;

public class Personaje1_0AnimatorController : MonoBehaviour
{
    // Referencias (ASIGNAR EN INSPECTOR)
    [SerializeField] private Animator _animator;
    [SerializeField] private GameManager _gameManager;

    // Controladores de Animación (ASIGNAR TODOS)
    public RuntimeAnimatorController Posicion1;
    public RuntimeAnimatorController TomarBomba1;
    public RuntimeAnimatorController LanzarBombaIDLE;
    public RuntimeAnimatorController ExplosionPersonaje1;

    // Duración animaciones (AJUSTAR EN INSPECTOR)
    [Header("Duración Animaciones")]
    public float duracionTomar = 2f;      // Duración deseada para la animación de tomar
    public float duracionLanzar = 0.25f;  // Duración para lanzar
    public float duracionExplosion = 1f;

    // Estados
    private bool _isDead = false;
    private bool _esPortador = false;
    private Coroutine _currentAnimation;

    void Start()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();

        VolverAPosicion1();
    }

    void Update()
    {
        if (_isDead || _gameManager == null) return;

        bool nuevoEstado = (_gameManager.ObtenerPortadorBombaActual() == gameObject);

        // Solo actuar si el estado cambió
        if (nuevoEstado != _esPortador)
        {
            _esPortador = nuevoEstado;

            if (_esPortador)
            {
                IniciarAnimacionTomar();
            }
            else
            {
                IniciarAnimacionLanzar();
            }
        }
    }

    void IniciarAnimacionTomar()
    {
        if (_isDead || !VerificarComponentes(TomarBomba1, "TomarBomba1")) return;

        if (_currentAnimation != null) StopCoroutine(_currentAnimation);

        _animator.runtimeAnimatorController = TomarBomba1;

        // Ajuste de velocidad para que dure exactamente duracionTomar segundos
        if (TomarBomba1.animationClips.Length > 0)
        {
            float duracionOriginal = TomarBomba1.animationClips[0].length;
            _animator.speed = duracionOriginal / duracionTomar;
        }
        else
        {
            _animator.speed = 0.5f; // Valor por defecto si no se encuentra el clip
        }

        _animator.Play("TomarBomba1", 0, 0f); // Nombre simplificado sin "Base Layer."
        _currentAnimation = StartCoroutine(ResetAfterDelay(duracionTomar));
    }

    void IniciarAnimacionLanzar()
    {
        if (_isDead || !VerificarComponentes(LanzarBombaIDLE, "LanzarBombaIDLE")) return;

        if (_currentAnimation != null) StopCoroutine(_currentAnimation);

        _animator.runtimeAnimatorController = LanzarBombaIDLE;
        _animator.speed = 1f; // Velocidad normal
        _animator.Play("LanzarBombaIDLE", 0, 0f);
        _currentAnimation = StartCoroutine(ResetAfterDelay(duracionLanzar));
    }

    public void EjecutarAnimacionMuerte()
    {
        if (_isDead) return;
        _isDead = true;

        if (VerificarComponentes(ExplosionPersonaje1, "ExplosionPersonaje1"))
        {
            if (_currentAnimation != null) StopCoroutine(_currentAnimation);

            _animator.runtimeAnimatorController = ExplosionPersonaje1;
            _animator.speed = 1f;
            _animator.Play("ExplosionPersonaje1", 0, 0f);
            StartCoroutine(DesactivarDespuesDeExplosion());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        VolverAPosicion1();
    }

    IEnumerator DesactivarDespuesDeExplosion()
    {
        yield return new WaitForSeconds(duracionExplosion);
        gameObject.SetActive(false);
    }

    void VolverAPosicion1()
    {
        if (_isDead || _animator == null || Posicion1 == null) return;
        _animator.runtimeAnimatorController = Posicion1;
        _animator.speed = 1f; // Restablecer velocidad normal
        _animator.Play("Posicion1", 0, 0f);
    }

    bool VerificarComponentes(RuntimeAnimatorController controller, string nombre)
    {
        if (_animator == null)
        {
            Debug.LogError("Animator no asignado");
            return false;
        }

        if (controller == null)
        {
            Debug.LogError($"Falta asignar {nombre} en Inspector");
            return false;
        }

        return true;
    }
}
using UnityEngine;
using System.Collections;

public class Personaje2_0AnimatorController : MonoBehaviour
{
    // Referencias (ASIGNAR EN INSPECTOR) - CONSERVADAS IGUAL
    [SerializeField] private Animator _animator;
    [SerializeField] private GameManager _gameManager;

    // Controladores de Animación (ASIGNAR TODOS) - NOMBRES CAMBIADOS PARA PERSONAJE 2
    public RuntimeAnimatorController Posicion2;  // Cambiado de Posicion1
    public RuntimeAnimatorController TomarBomba2; // Cambiado de TomarBomba1
    public RuntimeAnimatorController LanzarBomba2; // Cambiado de LanzarBombaIDLE
    public RuntimeAnimatorController ExplosionPersonaje2; // Cambiado de ExplosionPersonaje1

    // Duración animaciones (AJUSTAR EN INSPECTOR) - CONSERVADAS IGUAL
    [Header("Duración Animaciones")]
    public float duracionTomar = 10f;
    public float duracionLanzar = 0.25f;
    public float duracionExplosion = 1f;

    // Estados - CONSERVADOS IGUAL
    private bool _isDead = false;
    private bool _esPortador = false;
    private Coroutine _currentAnimation;

    void Start()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();

        VolverAPosicion2(); // Cambiado de VolverAPosicion1()
    }

    void Update()
    {
        // Mismo código, no requiere cambios
        if (_isDead || _gameManager == null) return;

        bool nuevoEstado = (_gameManager.ObtenerPortadorBombaActual() == gameObject);

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
        if (_isDead || !VerificarComponentes(TomarBomba2, "TomarBomba2")) return; // Nombre cambiado

        if (_currentAnimation != null) StopCoroutine(_currentAnimation);

        _animator.runtimeAnimatorController = TomarBomba2; // Cambiado

        // Ajuste de velocidad (misma lógica)
        if (TomarBomba2.animationClips.Length > 0) // Cambiado
        {
            float duracionOriginal = TomarBomba2.animationClips[0].length; // Cambiado
            _animator.speed = duracionOriginal / duracionTomar;
        }
        else
        {
            _animator.speed = 0.5f;
        }

        _animator.Play("TomarBomba2", 0, 0f); // Nombre cambiado
        _currentAnimation = StartCoroutine(ResetAfterDelay(duracionTomar));
    }

    void IniciarAnimacionLanzar()
    {
        if (_isDead || !VerificarComponentes(LanzarBomba2, "LanzarBomba2")) return; // Nombre cambiado

        if (_currentAnimation != null) StopCoroutine(_currentAnimation);

        _animator.runtimeAnimatorController = LanzarBomba2; // Cambiado
        _animator.speed = 1f;
        _animator.Play("LanzarBomba2", 0, 0f); // Nombre cambiado
        _currentAnimation = StartCoroutine(ResetAfterDelay(duracionLanzar));
    }

    public void EjecutarAnimacionMuerte()
    {
        if (_isDead) return;
        _isDead = true;

        if (VerificarComponentes(ExplosionPersonaje2, "ExplosionPersonaje2")) // Nombre cambiado
        {
            if (_currentAnimation != null) StopCoroutine(_currentAnimation);

            _animator.runtimeAnimatorController = ExplosionPersonaje2; // Cambiado
            _animator.speed = 1f;
            _animator.Play("ExplosionPersonaje2", 0, 0f); // Nombre cambiado
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
        VolverAPosicion2(); // Nombre cambiado
    }

    IEnumerator DesactivarDespuesDeExplosion()
    {
        yield return new WaitForSeconds(duracionExplosion);
        gameObject.SetActive(false);
    }

    void VolverAPosicion2() // Nombre cambiado
    {
        if (_isDead || _animator == null || Posicion2 == null) return; // Cambiado
        _animator.runtimeAnimatorController = Posicion2; // Cambiado
        _animator.speed = 1f;
        _animator.Play("Posicion2", 0, 0f); // Nombre cambiado
    }

    bool VerificarComponentes(RuntimeAnimatorController controller, string nombre)
    {
        // Mismo código, no requiere cambios
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
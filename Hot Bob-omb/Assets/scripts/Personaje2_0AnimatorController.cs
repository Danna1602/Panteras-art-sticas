using UnityEngine;
using System.Collections;

public class Personaje2_0AnimatorController : MonoBehaviour
{
    // Referencias (ASIGNAR EN INSPECTOR)
    [SerializeField] private Animator _animator;
    [SerializeField] private GameManager _gameManager;

    // Controladores de Animación (ASIGNAR TODOS)
    public RuntimeAnimatorController Posicion2;
    public RuntimeAnimatorController TomarBomba2;
    public RuntimeAnimatorController LanzarBomba2;
    public RuntimeAnimatorController ExplosionPersonaje2;
    public RuntimeAnimatorController VictoriaPersonaje2; // Nuevo

    // Duración animaciones (AJUSTAR EN INSPECTOR)
    [Header("Duración Animaciones")]
    public float duracionTomar = 10f;
    public float duracionLanzar = 0.25f;
    public float duracionExplosion = 1f;

    // Estados
    private bool _isDead = false;
    private bool _esPortador = false;
    private Coroutine _currentAnimation;

    void Start()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();

        VolverAPosicion2();
    }

    void Update()
    {
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
        if (_isDead || !VerificarComponentes(TomarBomba2, "TomarBomba2")) return;

        if (_currentAnimation != null) StopCoroutine(_currentAnimation);

        _animator.runtimeAnimatorController = TomarBomba2;

        if (TomarBomba2.animationClips.Length > 0)
        {
            float duracionOriginal = TomarBomba2.animationClips[0].length;
            _animator.speed = duracionOriginal / duracionTomar;
        }
        else
        {
            _animator.speed = 0.5f;
        }

        _animator.Play("TomarBomba2", 0, 0f);
        _currentAnimation = StartCoroutine(ResetAfterDelay(duracionTomar));
    }

    void IniciarAnimacionLanzar()
    {
        if (_isDead || !VerificarComponentes(LanzarBomba2, "LanzarBomba2")) return;

        if (_currentAnimation != null) StopCoroutine(_currentAnimation);

        _animator.runtimeAnimatorController = LanzarBomba2;
        _animator.speed = 1f;
        _animator.Play("LanzarBomba2", 0, 0f);
        _currentAnimation = StartCoroutine(ResetAfterDelay(duracionLanzar));
    }

    public void EjecutarAnimacionMuerte()
    {
        if (_isDead) return;
        _isDead = true;

        if (VerificarComponentes(ExplosionPersonaje2, "ExplosionPersonaje2"))
        {
            if (_currentAnimation != null) StopCoroutine(_currentAnimation);

            _animator.runtimeAnimatorController = ExplosionPersonaje2;
            _animator.speed = 1f;
            _animator.Play("ExplosionPersonaje2", 0, 0f);
            StartCoroutine(DesactivarDespuesDeExplosion());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Nuevo método para animación de victoria
    public void EjecutarAnimacionVictoria()
    {
        if (_isDead || !VerificarComponentes(VictoriaPersonaje2, "VictoriaPersonaje2")) return;

        if (_currentAnimation != null) StopCoroutine(_currentAnimation);

        _animator.runtimeAnimatorController = VictoriaPersonaje2;
        _animator.speed = 1f;
        _animator.Play("VictoriaPersonaje2", 0, 0f);
    }

    IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        VolverAPosicion2();
    }

    IEnumerator DesactivarDespuesDeExplosion()
    {
        yield return new WaitForSeconds(duracionExplosion);
        gameObject.SetActive(false);
    }

    void VolverAPosicion2()
    {
        if (_isDead || _animator == null || Posicion2 == null) return;
        _animator.runtimeAnimatorController = Posicion2;
        _animator.speed = 1f;
        _animator.Play("Posicion2", 0, 0f);
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
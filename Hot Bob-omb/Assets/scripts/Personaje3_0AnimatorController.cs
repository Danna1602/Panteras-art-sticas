using UnityEngine;
using System.Collections;

public class Personaje3_0AnimatorController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private BombMovement _bombaMovement;

    public RuntimeAnimatorController Posicion3;
    public RuntimeAnimatorController TomarBomba3Izquierda;
    public RuntimeAnimatorController TomarBomba3Derecha;
    public RuntimeAnimatorController LanzarBomba3Izquierda;
    public RuntimeAnimatorController LanzarBomba3Derecha;
    public RuntimeAnimatorController ExplosionPersonaje3;
    public RuntimeAnimatorController VictoriaPersonaje3; // Nuevo

    [Header("Duración Animaciones")]
    public float duracionTomar = 2f;
    public float duracionLanzar = 0.25f;
    public float duracionExplosion = 1f;

    private bool _isDead = false;
    private bool _esPortador = false;
    private Coroutine _currentAnimation;
    private Vector3 _posicionAnteriorBomba;
    private GameObject _ultimoPortadorBomba;

    void Start()
    {
        if (_animator == null) _animator = GetComponent<Animator>();
        if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
        if (_bombaMovement == null) _bombaMovement = FindObjectOfType<BombMovement>();

        VolverAPosicion3();
    }

    void Update()
    {
        if (_isDead || _gameManager == null) return;

        GameObject portadorActual = _gameManager.ObtenerPortadorBombaActual();
        bool nuevoEstado = (portadorActual == gameObject);

        if (portadorActual != null && portadorActual != _ultimoPortadorBomba)
        {
            _posicionAnteriorBomba = _bombaMovement.transform.position;
            _ultimoPortadorBomba = portadorActual;
        }

        if (nuevoEstado != _esPortador)
        {
            _esPortador = nuevoEstado;

            if (_esPortador)
            {
                bool vieneDeIzquierda = _posicionAnteriorBomba.x < transform.position.x;
                IniciarAnimacionTomar(vieneDeIzquierda);
            }
            else
            {
                bool lanzaAIzquierda = _bombaMovement.UltimaDireccion.x < 0;
                IniciarAnimacionLanzar(lanzaAIzquierda);
            }
        }
    }

    void IniciarAnimacionTomar(bool desdeIzquierda)
    {
        RuntimeAnimatorController animController = desdeIzquierda ? TomarBomba3Izquierda : TomarBomba3Derecha;

        if (_isDead || animController == null)
        {
            Debug.LogWarning("Animator Controller no asignado correctamente.");
            return;
        }

        if (_currentAnimation != null) StopCoroutine(_currentAnimation);

        _animator.runtimeAnimatorController = animController;

        if (animController.animationClips.Length > 0)
        {
            float duracionOriginal = animController.animationClips[0].length;
            _animator.speed = duracionOriginal / duracionTomar;
        }
        else
        {
            _animator.speed = 0.5f;
        }

        _currentAnimation = StartCoroutine(ResetAfterDelay(duracionTomar));
    }

    void IniciarAnimacionLanzar(bool haciaIzquierda)
    {
        RuntimeAnimatorController animController = haciaIzquierda ? LanzarBomba3Izquierda : LanzarBomba3Derecha;

        if (_isDead || animController == null)
        {
            Debug.LogWarning("Animator Controller no asignado correctamente.");
            return;
        }

        if (_currentAnimation != null) StopCoroutine(_currentAnimation);

        _animator.runtimeAnimatorController = animController;
        _animator.speed = 1f;

        _currentAnimation = StartCoroutine(ResetAfterDelay(duracionLanzar));
    }

    public void EjecutarAnimacionMuerte()
    {
        if (_isDead) return;
        _isDead = true;

        if (ExplosionPersonaje3 != null)
        {
            if (_currentAnimation != null) StopCoroutine(_currentAnimation);

            _animator.runtimeAnimatorController = ExplosionPersonaje3;
            _animator.speed = 1f;
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
        if (_isDead || !VerificarComponentes(VictoriaPersonaje3, "VictoriaPersonaje3")) return;

        if (_currentAnimation != null) StopCoroutine(_currentAnimation);

        _animator.runtimeAnimatorController = VictoriaPersonaje3;
        _animator.speed = 1f;
        _animator.Play("VictoriaPersonaje3", 0, 0f);
    }

    IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        VolverAPosicion3();
    }

    IEnumerator DesactivarDespuesDeExplosion()
    {
        yield return new WaitForSeconds(duracionExplosion);
        gameObject.SetActive(false);
    }

    void VolverAPosicion3()
    {
        if (_isDead || _animator == null || Posicion3 == null) return;
        _animator.runtimeAnimatorController = Posicion3;
        _animator.speed = 1f;
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
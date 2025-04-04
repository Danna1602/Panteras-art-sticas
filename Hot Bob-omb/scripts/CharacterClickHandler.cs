using UnityEngine;

public class CharacterClickHandler : MonoBehaviour
{
    // Referencia al GameManager (se asignar� desde el Inspector)
    public GameManager gameManager;

    // Este m�todo se llama autom�ticamente cuando se hace clic en el objeto
    private void OnMouseDown()
    {
        // Verificamos tres cosas antes de pasar la bomba:
        // 1. Que el GameManager est� asignado
        // 2. Que el personaje est� activo (no haya sido eliminado)
        // 3. Que el objeto tenga un Collider (para detectar clics)
        if (gameManager != null && gameObject.activeSelf && GetComponent<Collider2D>() != null)
        {
            // Llamamos al m�todo del GameManager para intentar pasar la bomba
            gameManager.TryPassBomb(gameObject);
        }
        else
        {
            // Mensajes de depuraci�n para ayudar a identificar problemas
            if (gameManager == null)
                Debug.LogError("GameManager no est� asignado en " + gameObject.name);
            if (GetComponent<Collider2D>() == null)
                Debug.LogError("No hay Collider2D en " + gameObject.name);
        }
    }
}
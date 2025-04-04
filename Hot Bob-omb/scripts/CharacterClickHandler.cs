using UnityEngine;

public class CharacterClickHandler : MonoBehaviour
{
    // Referencia al GameManager (se asignará desde el Inspector)
    public GameManager gameManager;

    // Este método se llama automáticamente cuando se hace clic en el objeto
    private void OnMouseDown()
    {
        // Verificamos tres cosas antes de pasar la bomba:
        // 1. Que el GameManager esté asignado
        // 2. Que el personaje esté activo (no haya sido eliminado)
        // 3. Que el objeto tenga un Collider (para detectar clics)
        if (gameManager != null && gameObject.activeSelf && GetComponent<Collider2D>() != null)
        {
            // Llamamos al método del GameManager para intentar pasar la bomba
            gameManager.TryPassBomb(gameObject);
        }
        else
        {
            // Mensajes de depuración para ayudar a identificar problemas
            if (gameManager == null)
                Debug.LogError("GameManager no está asignado en " + gameObject.name);
            if (GetComponent<Collider2D>() == null)
                Debug.LogError("No hay Collider2D en " + gameObject.name);
        }
    }
}
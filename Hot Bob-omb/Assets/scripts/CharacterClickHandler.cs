using UnityEngine;

public class CharacterClickHandler : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("No se encontró GameManager en la escena");
        }
    }

    void OnMouseDown()
    {
        if (gameManager != null && gameObject.activeSelf)
        {
            gameManager.IntentarPasarBomba(gameObject);
        }
    }
}
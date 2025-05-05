using UnityEngine;

public class CharacterClickHandler : MonoBehaviour
{
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager no encontrado en la escena");
        }
    }

    void OnMouseDown()
    {
        // Solo procesar si:
        // 1. Hay un GameManager
        // 2. El objeto clickeado está activo
        // 3. El jugador actual (portador de la bomba) es el Personaje 1 (jugador humano)
        if (gameManager != null &&
            gameObject.activeSelf &&
            gameManager.ObtenerPortadorBombaActual() == gameManager.personajes[0]) // personajes[0] = Personaje 1 (jugador)
        {
            // Solo pasar la bomba si el objetivo NO es el mismo jugador
            if (gameObject != gameManager.ObtenerPortadorBombaActual())
            {
                gameManager.IntentarPasarBomba(gameObject);
                Debug.Log($"Intento pasar bomba a: {gameObject.name}");
            }
        }
    }
}
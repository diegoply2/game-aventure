using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RestartButtonScript : MonoBehaviour
{
    // Référence au bouton Start dans l'UI
    public Button restartButton;

    private InputAction startAction;

    void Start()
    {
        // Vérifie si le bouton est assigné dans l'Inspector
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame); // Ajouter un listener pour le clic sur le bouton
        }
        else
        {
            Debug.LogError("Le bouton Restart n'a pas été trouvé !");
        }

        // Initialiser l'action pour la touche Start ou Enter
        startAction = new InputAction(binding: "<Keyboard>/enter");  // Liaison à la touche Enter
        startAction.AddBinding("<Gamepad>/start");  // Liaison au bouton Start du gamepad

        // Activer l'action
        startAction.Enable();
    }

    void Update()
    {
        // Vérifie si la touche Start (ou Enter) est appuyée
        if (startAction != null && startAction.triggered)
        {
            // Si Start ou Enter est appuyé, simuler un clic sur le bouton Restart
            restartButton.onClick.Invoke();
        }
    }

    // Méthode pour redémarrer le jeu
    void RestartGame()
    {
        Debug.Log("Restarting the game...");  // Message de débogage pour vérifier si la méthode est appelée
        Time.timeScale = 1f;  // Réactiver le temps
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // Recharger la scène actuelle
    }

    void OnDisable()
    {
        // Vérifie que startAction n'est pas null avant de la désactiver
        if (startAction != null)
        {
            startAction.Disable();  // Désactive l'action lorsqu'on désactive le script
        }
        else
        {
            Debug.LogWarning("startAction est déjà null, rien à désactiver.");
        }
    }
}

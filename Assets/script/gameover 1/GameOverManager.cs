using UnityEngine;
using UnityEngine.SceneManagement; // Pour recharger la scène
using System.Collections;  // Pour utiliser les coroutines

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverCanvas; // Référence au Canvas Game Over

    void Start()
    {
        // Assure-toi que le canvas est désactivé au départ
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
    }

    // Méthode pour afficher l'écran Game Over
    public void ShowGameOverScreen()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
            StartCoroutine(WaitAndPause());
        }
    }

    // Méthode pour redémarrer le jeu
    public void RestartGame()
    {
        Time.timeScale = 1f;  // Réactiver le temps
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Recharger la scène actuelle
    }

    // Coroutine pour attendre avant de mettre le jeu en pause
    private IEnumerator WaitAndPause()
    {
        yield return new WaitForSeconds(2f); // Attendre 2 secondes
        Time.timeScale = 0f;  // Mettre le jeu en pause
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI;  // UI du Game Over

    private bool isGameOver = false;  

    public static GameManager instance;  // Singleton pour un accès facile

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GameOver()
    {
        if (isGameOver) return; // Évite d'appeler plusieurs fois

        isGameOver = true;
        gameOverUI.SetActive(true);  // Affiche le message Game Over

        Debug.Log("Game Over !");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recharge la scène
    }
}

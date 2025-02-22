using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public bool isParrying { get; set; }
    public bool isAttacking = false;

    private Animator animator;
    private CharacterControllerWithCamera characterController;
    private ParadeScript paradeScript;
    private AttaqueScript attaqueScript;
    private PlayerControls playerControls;

    private HealthSliderUI healthSliderUI;

    public GameObject gameOverCanvasPrefab; // Référence au prefab GameOverCanvas
    private GameOverManager gameOverManager; // Référence au GameOverManager du prefab instancié

    void Start()
    {
        currentHealth = maxHealth;

        // Récupérer le script HealthSliderUI dans la scène
        healthSliderUI = FindObjectOfType<HealthSliderUI>();
        if (healthSliderUI != null)
        {
            healthSliderUI.Initialize(maxHealth);
        }

        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterControllerWithCamera>();
        paradeScript = GetComponent<ParadeScript>();
        attaqueScript = GetComponent<AttaqueScript>();

        playerControls = new PlayerControls();
        playerControls.Enable();

        // Instancier le prefab GameOverCanvas
        if (gameOverCanvasPrefab != null)
        {
            GameObject canvasInstance = Instantiate(gameOverCanvasPrefab);
            canvasInstance.SetActive(false);  // Le canvas est désactivé au début
            gameOverManager = canvasInstance.GetComponent<GameOverManager>();
            if (gameOverManager != null)
            {
                Debug.Log("GameOverManager assigné avec succès.");
            }
            else
            {
                Debug.LogError("GameOverManager non trouvé dans le prefab !");
            }
        }
        else
        {
            Debug.LogError("Le prefab GameOverCanvas n'est pas assigné dans le script !");
        }
    }

    public void TakeDamage(float amount, bool isCritical)
    {
        if (!isParrying)  // Vérification avec la propriété isParrying
        {
            currentHealth -= amount;
        }
        else
        {
            currentHealth -= amount * 0.75f; // Réduction des dégâts si le joueur pare
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            PlayerDeath();
        }

        // Mettre à jour la barre de vie via le script externe
        if (healthSliderUI != null)
        {
            healthSliderUI.UpdateHealth(currentHealth);
        }

        Debug.Log("Santé actuelle : " + currentHealth);
    }

    private void PlayerDeath()
    {
        Debug.Log("Le joueur est mort !");

        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }

        if (characterController != null)
        {
            characterController.DisableMovement();
        }

        if (paradeScript != null)
        {
            paradeScript.enabled = false;
        }

        if (attaqueScript != null)
        {
            attaqueScript.enabled = false;
        }

        if (playerControls != null)
        {
            playerControls.Disable();
        }

        // Si le GameOverManager a été trouvé, afficher l'écran Game Over
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOverScreen();  // Afficher l'écran Game Over
        }
        else
        {
            Debug.LogError("GameOverManager est introuvable lors de la mort du joueur !");
        }
    }

    public void RestoreHealth(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        if (healthSliderUI != null)
        {
            healthSliderUI.UpdateHealth(currentHealth);
        }

        Debug.Log("Santé restaurée. Nouvelle santé : " + currentHealth);
    }
}

using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    // Utilisation des propriétés pour isParrying et isAttacking
    public bool isParrying { get; set; }
    public bool isAttacking = false;  // Correctement déclaré en tant que public ou avec une propriété
  // Utilisation de la propriété 'isAttacking' au lieu de la variable publique

    private Animator animator;
    private CharacterControllerWithCamera characterController;
    private ParadeScript paradeScript;
    private AttaqueScript attaqueScript;
    private PlayerControls playerControls;

    private HealthSliderUI healthSliderUI;

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
} // ✅ Assure-toi que cette accolade ferme bien la classe

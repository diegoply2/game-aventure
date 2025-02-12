using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ParadeScript : MonoBehaviour
{
    private PlayerControls playerControls; 
    private Animator animator; 
    public bool isParrying = false; 

    private InputAction parryAction; 
    private CharacterControllerWithCamera characterController; 
    private PlayerHealth playerHealth; 

    void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable(); // Activer les actions

        animator = GetComponent<Animator>();
        parryAction = playerControls.Player.Parade;

        characterController = GetComponent<CharacterControllerWithCamera>();
        playerHealth = GetComponent<PlayerHealth>();

        // Action pour démarrer la parade
        parryAction.started += ctx => StartParry();
        parryAction.canceled += ctx => EndParry();
    }

    public void StartParry()
    {
        if (isParrying || playerHealth.currentHealth <= 0) return; // Vérifier si le joueur est déjà en train de parer ou est mort

        isParrying = true;

        // Mettre à jour isParrying dans PlayerHealth
        if (playerHealth != null)
        {
            playerHealth.isParrying = true;  // Utilisation de isParrying dans PlayerHealth
        }

        // Désactiver le mouvement pendant la parade
        if (characterController != null)
        {
            characterController.DisableMovement();
        }

        // Lancer l'animation de parade
        if (animator != null)
        {
            animator.SetBool("Parade", true);
        }
    }

    public void EndParry()
    {
        if (!isParrying) return;

        isParrying = false;

        // Réinitialiser isParrying dans PlayerHealth
        if (playerHealth != null)
        {
            playerHealth.isParrying = false;  // Réinitialisation de isParrying dans PlayerHealth
        }

        // Réactiver le mouvement après la parade
        if (characterController != null)
        {
            characterController.EnableMovement();
        }

        // Arrêter l'animation de parade
        if (animator != null)
        {
            animator.SetBool("Parade", false);
        }
    }

    void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Disable();
        }
    }
}

using UnityEngine;
using UnityEngine.UI;  // Ajoutez ceci pour pouvoir utiliser les composants UI comme Slider

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f; // Santé maximale du joueur
    public float currentHealth;    // Santé actuelle du joueur

    public bool IsParrying { get; set; } // Indicateur pour la parade

    public Slider healthSlider;  // Référence au Slider

    private Animator animator; // Référence à l'Animator
    private CharacterControllerWithCamera characterController; // Référence au script de mouvement
    private ParadeScript paradeScript; // Référence au script de parade
    private AttaqueScript attaqueScript; // Référence au script d'attaque
    private PlayerControls playerControls; // Référence aux contrôles

    public bool isAttacking;

    void Start()
    {
        currentHealth = maxHealth; // Initialisation de la santé actuelle à la santé maximale
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;  // Définir la valeur maximale du slider
            healthSlider.value = currentHealth; // Définir la valeur initiale du slider
        }

        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterControllerWithCamera>();
        paradeScript = GetComponent<ParadeScript>();
        attaqueScript = GetComponent<AttaqueScript>();

        playerControls = new PlayerControls();
        playerControls.Enable(); // Activer les actions
    }

    void Update()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth; // Mettre à jour le slider à chaque frame
        }
    }

    public void TakeDamage(float amount, bool isCritical)
    {
        if (!IsParrying)
        {
            currentHealth -= amount; // Soustraire les dégâts
        }
        else
        {
            currentHealth -= amount * 0.75f; // Réduction des dégâts de 25% en cas de parade
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            PlayerDeath(); // Appeler la fonction PlayerDeath si la santé est à 0 ou moins
        }

        // Mettre à jour le Slider
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        Debug.Log("Santé actuelle : " + currentHealth);
    }

    // Fonction appelée lorsque le joueur meurt
    private void PlayerDeath()
{
    // Implémenter la logique de la mort du joueur ici
    Debug.Log("Le joueur est mort !");

    // Désactiver les actions du joueur sans désactiver le GameObject entier
    if (animator != null)
    {
        animator.SetBool("IsDead", true); // Assurez-vous d'avoir un paramètre "IsDead" dans l'Animator pour jouer une animation de mort
    }

    if (characterController != null)
    {
        characterController.DisableMovement(); // Désactiver les mouvements du joueur
    }

    if (paradeScript != null)
    {
        paradeScript.enabled = false; // Désactiver le script de parade
    }

    if (attaqueScript != null)
    {
        attaqueScript.enabled = false; // Désactiver le script d'attaque
    }

    if (playerControls != null)
    {
        playerControls.Disable(); // Désactiver les contrôles
    }

    // Appliquer une force vers le bas pour que le joueur tombe au sol
    if (characterController != null)
    {
        // Forcer la position du joueur au sol
        Vector3 currentPosition = transform.position;
        currentPosition.y = 0; // Ajustez la hauteur Y à 0 pour qu'il touche le sol (ou la hauteur souhaitée)
        transform.position = currentPosition;
    }

    // Si le personnage est un Rigidbody, vous pouvez aussi ajouter une force vers le bas
    if (GetComponent<Rigidbody>() != null)
    {
        GetComponent<Rigidbody>().useGravity = true;  // Assurez-vous que la gravité est activée
        GetComponent<Rigidbody>().linearVelocity = new Vector3(GetComponent<Rigidbody>().linearVelocity.x, -5f, GetComponent<Rigidbody>().linearVelocity.z); // Appliquer une force vers le bas pour qu'il tombe
    }

    // Vous pouvez également arrêter les autres actions comme l'attaque ou les animations de mouvement si nécessaire
}

public void RestoreHealth(float amount)
{
    currentHealth += amount; // Ajouter la quantité de santé à la santé actuelle
    if (currentHealth > maxHealth) // Assurez-vous que la santé ne dépasse pas la santé maximale
    {
        currentHealth = maxHealth;
    }

    if (healthSlider != null)
    {
        healthSlider.value = currentHealth; // Mettre à jour le slider
    }

    Debug.Log("Santé restaurée. Nouvelle santé : " + currentHealth);
}



}
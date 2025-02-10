using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;
    private bool isDead = false;
    private CharacterController characterController;  // Déclare la variable pour le CharacterController

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        
        // Récupère le CharacterController à partir de l'objet
        characterController = GetComponent<CharacterController>();

        // Vérifie si le CharacterController existe sur l'objet
        if (characterController == null)
        {
            Debug.LogError("Aucun CharacterController trouvé sur " + gameObject.name);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        // Afficher combien de points de santé l'ennemi perd
        Debug.Log("L'ennemi a perdu " + amount + " points de santé. Santé restante : " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

private void Die()
{
    isDead = true;

    // Désactiver l'animation de mouvement et jouer l'animation de mort
    if (animator != null)
    {
        animator.SetBool("EnemyDie", true);  // Active l'animation de mort
    }

    // Désactiver le CharacterController s'il existe
    if (characterController != null)
    {
        characterController.enabled = false;  // Désactive le CharacterController pour éviter les conflits
    }

    // Désactiver la physique pour éviter que l'ennemi tombe
    Rigidbody rb = GetComponent<Rigidbody>();
    if (rb != null)
    {
        rb.isKinematic = true;  // Désactiver la gravité pour éviter la chute
        rb.linearVelocity = Vector3.zero;  // Stopper tout mouvement
    }

    // Ne PAS désactiver le Collider, sinon l'ennemi traverse le sol !
    Collider col = GetComponent<Collider>();
    if (col != null)
    {
        col.enabled = true; // Laisse le collider actif pour qu'il reste sur le sol
    }

    // **Nouvelle ligne** : Empêcher la poursuite dans EnemyController
    EnemyController enemyController = GetComponent<EnemyController>();
    if (enemyController != null)
    {
        enemyController.Die();  // Appelle la méthode Die() dans EnemyController
    }

    // L'ennemi ne disparaît PAS et ne tombe plus à travers le sol !
}





}

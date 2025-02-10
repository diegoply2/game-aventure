using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public float healthRestoreAmount = 20f; // Montant de la santé restaurée par la potion
    public float disappearDelay = 1.5f; // Délai avant que la potion disparaisse après avoir été ramassée

    private PlayerHealth playerHealth; // Référence au script PlayerHealth
    private GameObject magicCircle2; // Référence au "Magic Circle 2" (enfant de Potion Vie)

    void Start()
    {
        Debug.Log("Start() de HealthPotion appelé.");
        
        // Récupérer la référence au script PlayerHealth sur le joueur
        playerHealth = FindObjectOfType<PlayerHealth>();

        // Récupérer la référence à "Magic Circle 2" qui est un enfant de la potion
        magicCircle2 = transform.Find("Magic Circle 2")?.gameObject;

        if (magicCircle2 != null)
        {
            magicCircle2.SetActive(true); // S'assurer que le cercle magique est actif dès le début
            Debug.Log("Magic Circle 2 activé.");
        }
        else
        {
            Debug.LogWarning("Magic Circle 2 n'est pas un enfant de Potion Vie !");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Vérifier si l'objet qui entre en collision est le joueur
        if (other.CompareTag("Player"))
        {
            // Restaurer la santé du joueur
            if (playerHealth != null)
            {
                playerHealth.RestoreHealth(healthRestoreAmount); // Restaurer la vie
                Debug.Log("Potion utilisée, vie restaurée !");
            }

            // Désactiver "Magic Circle 2" avant que la potion disparaisse
            if (magicCircle2 != null)
            {
                magicCircle2.SetActive(false); // Désactiver le cercle magique
                Debug.Log("Magic Circle 2 désactivé.");
            }

            // Détruire la potion après un délai avant disparition
            Destroy(gameObject, disappearDelay); // La potion disparaît après un délai
        }
    }
}

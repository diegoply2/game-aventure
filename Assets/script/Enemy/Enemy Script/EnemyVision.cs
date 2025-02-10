using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public float viewDistance = 20f;
    public float viewAngle = 120f;
    public Transform player;
    public LayerMask whatIsPlayer;

    public bool playerInSight { get; private set; }

    void Start()
    {
        playerInSight = false;
    }

    void Update()
    {
        DetectPlayer();
    }

    void DetectPlayer()
{
    Vector3 directionToPlayer = player.position - transform.position;
    float angleToPlayer = Vector3.Angle(directionToPlayer, transform.forward);

    // Debugging: Affichage du rayon
    Debug.DrawRay(transform.position + Vector3.up * 1.5f, directionToPlayer.normalized * viewDistance, Color.red);

    // Vérification si le joueur est dans le champ de vision de l'ennemi
    if (angleToPlayer <= viewAngle / 2f && directionToPlayer.magnitude <= viewDistance)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1.5f, directionToPlayer.normalized, out hit, viewDistance))
        {
            if (hit.transform.CompareTag("Player"))  // Assurez-vous que le tag "Player" est bien appliqué sur le joueur
            {
                if (!playerInSight)
                {
                    Debug.Log("Joueur détecté !");
                }
                playerInSight = true; // Le joueur est dans le champ de vision
                return;
            }
        }
    }

    // Si le joueur n'est plus dans le champ de vision
    if (playerInSight)
    {
        playerInSight = false;
        Debug.Log("Joueur hors de vue.");
    }
}

}

using UnityEngine;
using System.Collections;

public class EnemyParry : MonoBehaviour
{
    public GameObject player;  // Référence à l'objet du joueur
    public float parryChance = 0.5f;  // 50% de chance de parer à chaque contact
    public float minParryDelay = 0.5f;  // Temps minimum avant de parer
    public float maxParryDelay = 1.5f;  // Temps maximum avant de parer

    private Animator animator;
    private bool isParrying = false;

    // Référence au script EnemyAttackSound pour jouer les sons
    private EnemyAttackSound enemyAttackSoundScript;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Récupérer la référence au script EnemyAttackSound
        enemyAttackSoundScript = GetComponent<EnemyAttackSound>();
        if (enemyAttackSoundScript == null)
        {
            Debug.LogError("Le script EnemyAttackSound n'est pas attaché à l'ennemi.");
        }

        // Recherche du joueur par son tag "Player"
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Aucun objet avec le tag 'Player' trouvé dans la scène.");
        }
    }

    // Cette méthode est appelée lorsque l'ennemi entre en contact avec le joueur
    void OnTriggerEnter(Collider other)
    {
        // Vérifie si l'ennemi entre en contact avec le joueur
        if (other.CompareTag("Player") && !isParrying)
        {
            StartCoroutine(AttemptParry());
        }
    }

    // Coroutine qui tente de parer après un délai aléatoire
    private IEnumerator AttemptParry()
    {
        // Choisir un délai aléatoire pour la parade
        float delay = Random.Range(minParryDelay, maxParryDelay);
        yield return new WaitForSeconds(delay);

        // Tirage aléatoire pour déterminer si l'ennemi va parer
        if (Random.value < parryChance)  // Si l'ennemi décide de parer
        {
            // L'ennemi décide de parer
            isParrying = true;
            animator.SetBool("EnemyParry", true);  // Déclenche l'animation de parade

            // Joue immédiatement le son de parade
            if (enemyAttackSoundScript != null)
            {
                enemyAttackSoundScript.EnemyPlayParrySound();
            }

            // Réinitialiser la parade après un délai
            yield return new WaitForSeconds(1f);  // Durée de la parade
            isParrying = false;
            animator.SetBool("EnemyParry", false);  // Arrêter l'animation de parade
        }
        else
        {
            Debug.Log("L'ennemi n'a pas paré et a reçu l'attaque !");
        }
    }
}

using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float minAttackDelay = 0.5f;
    [SerializeField] private float maxAttackDelay = 1.5f;
    [SerializeField] private float parryChance = 0.5f;  // Probabilité de parade

    private bool isAttacking = false;
    private bool isParrying = false; // Indicateur pour savoir si l'ennemi est en train de parer
    private Transform player;
    private Animator animator;
    private ParadeScript playerParadeScript;

    private EnemyAttackSound EnemyAttackSound;
    private EnemyAttackSound EnemyParrySound;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        playerParadeScript = player.GetComponent<ParadeScript>();

        EnemyAttackSound = GetComponent<EnemyAttackSound>();
        EnemyParrySound = GetComponent<EnemyAttackSound>();
    }

    public void Update()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange && !isAttacking)
        {
            StartCoroutine(AttackWithDelay());
        }

        // Si l'ennemi est en train de parer, jouer l'animation de parade
        if (isParrying)
        {
            animator.SetBool("EnemyParry", true);
        }
        else
        {
            animator.SetBool("EnemyParry", false);
        }
    }

    private IEnumerator AttackWithDelay()
    {
        isAttacking = true;
        float attackDelay = Random.Range(minAttackDelay, maxAttackDelay);
        yield return new WaitForSeconds(attackDelay);

        if (animator != null)
        {
            animator.SetBool("EnemyAttack", true);
        }

        EnemyAttackSound?.EnemyPlayAttackSound();

        yield return new WaitForSeconds(1f);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                if (playerParadeScript != null && playerParadeScript.isParrying && !playerHealth.isAttacking)
                {
                    playerHealth.TakeDamage(Random.Range(2f, 5f) * 0.75f, false);
                    EnemyParrySound?.EnemyPlayParrySound();
                }
                else
                {
                    playerHealth.TakeDamage(Random.Range(2f, 5f), false);
                }
            }
        }

        if (animator != null)
        {
            animator.SetBool("EnemyAttack", false);
        }

        isAttacking = false;
    }

    // Ajouter la logique de parade dans OnTriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerSword"))
        {
            AttaqueScript playerAttackScript = player.GetComponent<AttaqueScript>();

            if (playerAttackScript != null && !playerParadeScript.isParrying && !playerAttackScript.isAttacking)
            {
                if (Random.value < parryChance)
                {
                    // L'ennemi décide de parer
                    isParrying = true;
                    animator.SetBool("EnemyParry", true);

                    // Ajouter un délai pour arrêter la parade après un certain temps
                    StartCoroutine(StopParryAfterDelay(1f));
                }
            }
        }
    }

    // Méthode pour arrêter la parade après un délai
    private IEnumerator StopParryAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isParrying = false;
        animator.SetBool("EnemyParry", false);
    }
}

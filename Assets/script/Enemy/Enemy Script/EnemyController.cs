using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;              // Vitesse de déplacement de l'ennemi
    [SerializeField] private float pursuitSpeed = 10f;          // Vitesse de poursuite de l'ennemi
    private Vector3 startPosition;                              // Position de départ de l'ennemi

    private CharacterController characterController;
    private Animator animator;

    private Vector2 randomMovementDirection;
    [SerializeField] private float randomChangeInterval = 2f;   // Temps entre chaque changement de direction
    private float randomTimer = 0f;

    private EnemyVision enemyVision;

    private bool isPursuing = false;
    private float pursueRemainingTime = 0f;                      // Timer pour poursuivre après la perte de vue du joueur
    [SerializeField] private float pursueDuration = 8f;         // Durée de poursuite après perte de vue

    private bool isPaused = false;                             // Indicateur si l'ennemi est en pause
    private float pauseTimer = 0f;                             // Timer pour la pause
    [SerializeField] private float minPauseDuration = 1f;      // Durée minimale de la pause
    [SerializeField] private float maxPauseDuration = 3f;      // Durée maximale de la pause

    [SerializeField] private float rotationSpeed = 5f;         // Vitesse de rotation
    private Vector3 currentMovementDirection;

    private float pursuitTimerAfterLost = 8f;                   // Temps de poursuite après sortie du champ de vision

    private EnemyAttack enemyAttack;                           // Référence au script EnemyAttack

    // Ajoute cette variable
    private bool isDead = false;

    private bool isAttacking = false;


    void Awake()
    {
        startPosition = transform.position;

        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("Le CharacterController n'a pas été trouvé sur l'objet.");
        }

        enemyVision = GetComponent<EnemyVision>();
        if (enemyVision == null)
        {
            Debug.LogError("Le script EnemyVision n'a pas été trouvé sur l'objet.");
        }

        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("L'Animator n'a pas été trouvé sur l'objet.");
        }

        // Applique la direction aléatoire initiale
        ChooseNewRandomDirection();

        // Initialisation du script EnemyAttack
        enemyAttack = GetComponent<EnemyAttack>();
        if (enemyAttack == null)
        {
            Debug.LogError("Le script EnemyAttack n'a pas été trouvé sur l'objet.");
        }
    }

    void Update()
{
    if (isDead)
    {
        return; // Ne fait rien si l'ennemi est mort
    }

    // Si l'ennemi voit le joueur
    if (enemyVision != null && enemyVision.playerInSight)
    {
        if (!isPursuing) // Si l'ennemi n'est pas déjà en poursuite
        {
            StartPursuing(); // Démarrer la poursuite
        }

        // Réinitialiser les timers de poursuite et de recherche après la perte de vue
        pursueRemainingTime = pursueDuration;   // Réinitialise la durée de poursuite
        pursuitTimerAfterLost = 0f;             // Réinitialise le temps de recherche après perte de vue
    }
    else
    {
        // Si l'ennemi perd de vue le joueur
        if (isPursuing)
        {
            pursueRemainingTime -= Time.deltaTime; // Diminuer le temps de poursuite

            if (pursueRemainingTime <= 0f) // Si le temps de poursuite est écoulé, arrête la poursuite
            {
                StopPursuing();
            }
        }

        // Si l'ennemi ne voit plus le joueur et que le temps de recherche est écoulé
        if (!enemyVision.playerInSight && pursueRemainingTime <= 0f)
        {
            // Continue de chercher le joueur pendant un certain temps
            if (pursuitTimerAfterLost > 0f)
            {
                pursuitTimerAfterLost -= Time.deltaTime;
                MoveTowardsPlayer();  // Déplacement pour continuer à chercher
            }
            else
            {
                // Si tout est écoulé, passe au mouvement aléatoire
                HandleRandomMovement();
            }
        }
    }

    // Si l'ennemi est en poursuite, ou continue de chercher après la perte de vue, se déplacer vers le joueur
    if (isPursuing || pursuitTimerAfterLost > 0f)
    {
        MoveTowardsPlayer(); // Se déplacer vers le joueur
    }
    else
    {
        randomTimer += Time.deltaTime;
        if (randomTimer >= randomChangeInterval && !isPaused)
        {
            ChooseNewRandomDirection();
            randomTimer = 0f;
        }
        HandleRandomMovement();  // Mouvement aléatoire
    }
}



    void HandleRandomMovement()
    {
        if (isPaused)
        {
            // Si l'ennemi est en pause, décrémente le timer de la pause
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f)
            {
                isPaused = false;  // Sortir de la pause lorsque le temps est écoulé
                print("L'ennemi reprend son mouvement.");
            }
            return;  // Ne pas déplacer l'ennemi pendant la pause
        }

        // Vérifie si le CharacterController est activé avant de déplacer
        if (characterController != null && characterController.enabled)
        {
            // Calculer la direction de mouvement en fonction de la direction du regard de l'ennemi
            Vector3 forwardDirection = transform.forward;  // Utilise la direction avant de l'ennemi
            Vector3 moveDirection = new Vector3(randomMovementDirection.x, 0f, randomMovementDirection.y).normalized;

            // Assurez-vous que la direction arrière est bloquée, en ne permettant pas un mouvement en arrière
            if (Vector3.Dot(forwardDirection, moveDirection) < 0f) // Si la direction de mouvement est en arrière
            {
                // Le mouvement doit être ajusté pour être soit sur le côté, soit vers l'avant
                moveDirection = new Vector3(randomMovementDirection.x, 0f, Mathf.Max(0f, randomMovementDirection.y)).normalized;
            }

            // Lissage de la direction de mouvement
            currentMovementDirection = Vector3.Lerp(currentMovementDirection, moveDirection, Time.deltaTime * 5f);  // Ajustez la valeur pour le lissage

            // Déplacer l'ennemi en fonction de la direction
            characterController.Move(currentMovementDirection * moveSpeed * Time.deltaTime);
            UpdateAnimator(currentMovementDirection);

            // Décider si l'ennemi doit faire une pause
            if (Random.Range(0f, 1f) < 0.05f)  // 5% de chance de faire une pause à chaque mise à jour
            {
                StartPause();
            }

            // Rotation fluide : Faire tourner l'ennemi progressivement vers la direction du mouvement
            if (currentMovementDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(currentMovementDirection);  // Trouver la direction du mouvement
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);  // Rotation fluide
            }
        }
        else
        {
            Debug.LogWarning("CharacterController est désactivé. Impossible de déplacer l'ennemi.");
        }
    }

    void ChooseNewRandomDirection()
    {
        randomMovementDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        transform.rotation = Quaternion.LookRotation(new Vector3(randomMovementDirection.x, 0, randomMovementDirection.y));
    }

    void StartPursuing()
    {
        isPursuing = true; // Commencer la poursuite
        pursuitTimerAfterLost = pursueDuration; // Définir la durée pendant laquelle l'ennemi continue de chercher
        pursueRemainingTime = pursueDuration; // Réinitialiser la durée de poursuite
        print("L'ennemi commence à vous poursuivre !");
    }

    void StopPursuing()
    {
        isPursuing = false; // Arrêter la poursuite
        pursuitTimerAfterLost = 0f; // Réinitialiser le timer de recherche après la perte de vue
        print("L'ennemi arrête de vous poursuivre.");
    }

    void MoveTowardsPlayer()
{
    if (isDead) return;  // **Ajouté** : Vérifie si l'ennemi est mort avant de continuer

    if (characterController == null || !characterController.enabled) return;

    if (enemyVision == null || enemyVision.player == null) return;

    Vector3 directionToPlayer = (enemyVision.player.position - transform.position).normalized;

    float distanceToPlayer = Vector3.Distance(transform.position, enemyVision.player.position);

    if (distanceToPlayer <= 2f)
    {
        enemyAttack.Update();
        return;
    }

    float currentSpeed = isPursuing ? pursuitSpeed : moveSpeed;

    characterController.Move(directionToPlayer * currentSpeed * Time.deltaTime);

    if (directionToPlayer != Vector3.zero)
    {
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    UpdateAnimator(directionToPlayer);
}


    void StartPause()
    {
        isPaused = true;
        pauseTimer = Random.Range(minPauseDuration, maxPauseDuration);  // Pause aléatoire entre min et max
        print("L'ennemi fait une pause pendant " + pauseTimer + " secondes.");
    }

    void UpdateAnimator(Vector3 direction)
    {
        if (animator != null)
        {
            animator.SetFloat("AxeX", direction.x);
            animator.SetFloat("AxeY", Mathf.Max(0.1f, direction.z));
        }
    }

    // Ajoute cette méthode pour gérer la mort de l'ennemi
    public void Die()
    {
        isDead = true;
        // Désactiver le CharacterController si nécessaire
        if (characterController != null)
        {
            characterController.enabled = false;  // Empêche tout mouvement
        }

        // Autres actions lors de la mort (animation, etc.)
        animator.SetBool("EnemyDie", true);  // Joue l'animation de mort
        GetComponent<Collider>().enabled = false;  // Désactive les collisions
    }
}

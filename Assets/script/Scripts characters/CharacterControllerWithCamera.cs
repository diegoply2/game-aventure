using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControllerWithCamera : MonoBehaviour
{
    private PlayerControls playerControls; // Référence aux contrôles personnalisés

    // Variables de mouvement
    public float moveSpeed = 5f; // Vitesse de déplacement normale
    public float runSpeed = 10f; // Vitesse de course
    public float rotationSpeed = 700f; // Vitesse de rotation du personnage
    public float deadzone = 0.3f; // Valeur de la deadzone (vous pouvez ajuster cette valeur)

    public float rightStickDeadzone = 0.3f; // Deadzone pour le joystick droit

    // Variables de caméra
    public Transform cameraTransform; // Référence à la caméra pour calculer le mouvement relatif
    private Vector2 moveInput; // Entrée du stick gauche pour les mouvements
    private Vector2 lookInput; // Entrée du stick droit pour la rotation de la caméra

    private CharacterController characterController;
    private Vector3 velocity;
    private Animator animator; // Référence à l'Animator

    // Variables pour le saut
    public float jumpHeight = 3.5f; // Hauteur du saut
    public float gravityValue = -9.81f; // Valeur de la gravité
    private bool isJumping = false; // Indicateur pour savoir si le personnage est en train de sauter
    private bool isGrounded; // Indicateur si le personnage est au sol

    private InputAction jumpAction; // Action pour le saut
    private InputAction runAction; // Action pour courir (LeftShoulder dans PlayerControls)
    private InputAction attackAction; // Action pour attaquer

    public Vector2 smoothMoveInput; // Pour lisser les entrées de mouvement
    public bool isRunning = false; // Indicateur pour savoir si le personnage court
    public bool IsRun = false; // Booléen pour l'animation de la course

    private AttaqueScript attackScript; // Référence au script d'attaque
    private ParadeScript parryScript;

    public Transform Enemy;

    private bool jumpRequested = false; // Booléen pour indiquer que le saut a été demandé


    void Awake()
    {
        // Vérifier la présence de la caméra principale
        cameraTransform = Camera.main?.transform;

        // Initialisation de playerControls
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.Enable();
            Debug.Log("playerControls initialisé dans Awake.");
        }

        // Vérification de l'initialisation du CharacterController, Animator et autres composants
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        attackScript = GetComponent<AttaqueScript>();
        parryScript = GetComponent<ParadeScript>();

        // Obtenir les actions du player
        jumpAction = playerControls.Player.Jump;
        jumpAction.started += ctx => HandleJump();

        runAction = playerControls.Player.Run;
        runAction.started += ctx => StartRunning();
        runAction.canceled += ctx => StopRunning();

        attackAction = playerControls.Player.Attack;

        // S'assurer que l'ennemi commence sur le sol
        SnapToGround();
    }

void Update()
{
    if (attackScript != null && attackScript.isAttacking || (parryScript != null && parryScript.isParrying))
    {
        smoothMoveInput = Vector2.zero; // Désactiver le mouvement pendant l'attaque ou la parade
    }
    else
    {
        moveInput = playerControls.Player.Move.ReadValue<Vector2>();
        lookInput = playerControls.Player.Camera.ReadValue<Vector2>();

        // Applique la deadzone au joystick gauche
        ApplyDeadzone(ref moveInput);

        // Applique la deadzone au joystick droit (pour la rotation de la caméra)
        ApplyDeadzone(ref lookInput, rightStickDeadzone);

        smoothMoveInput = Vector2.Lerp(smoothMoveInput, moveInput, 0.2f);
        if (moveInput == Vector2.zero)
            smoothMoveInput = Vector2.zero;
    }

    isGrounded = characterController.isGrounded;

    if (!isRunning && !isJumping && moveInput == Vector2.zero && attackScript != null && !attackScript.isAttacking)
    {
        if (attackAction.triggered)
        {
            Attack();
        }
    }

    MoveCharacter();
    RotateCharacterWithCamera();

    // Gestion du saut uniquement quand la touche est pressée
    if (isGrounded)
    {
        velocity.y = -2f; // Empêche la dérive dans les airs si au sol

        // Si le saut a été demandé (touche appuyée)
        if (jumpRequested)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue); // Effectuer le saut
            jumpRequested = false; // Réinitialiser l'état de demande de saut
            animator.SetBool("IsJumping", true); // Activer l'animation de saut uniquement quand la touche est pressée
        }
        else
        {
            animator.SetBool("IsJumping", false); // Désactiver l'animation de saut quand au sol
        }
    }
    else
    {
        velocity.y += gravityValue * Time.deltaTime; // Appliquer la gravité en l'air
    }

    characterController.Move(velocity * Time.deltaTime);
    UpdateAnimations();
}


    void MoveCharacter()
    {
        // Calculer la distance entre le joueur et l'ennemi
        float distanceToEnemy = Vector3.Distance(transform.position, Enemy.position);  // Supposons que `enemy` soit une référence à l'ennemi

        // Commenter ou supprimer cette partie pour que le joueur puisse se déplacer même lorsqu'il est proche de l'ennemi
        // if (distanceToEnemy <= 2f)  // Ajuste cette distance selon tes besoins
        // {
        //     smoothMoveInput = Vector2.zero;  // Désactiver le mouvement
        // }

        // Si smoothMoveInput n'est pas nul, continuer à appliquer le mouvement
        if (smoothMoveInput != Vector2.zero)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0f; right.y = 0f; forward.Normalize(); right.Normalize();
            Vector3 moveDirection = (forward * smoothMoveInput.y + right * smoothMoveInput.x).normalized;

            float currentMoveSpeed = isRunning ? runSpeed : moveSpeed;
            characterController.Move(moveDirection * currentMoveSpeed * Time.deltaTime);
        }
    }

    void RotateCharacterWithCamera()
    {
        if (lookInput.sqrMagnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(lookInput.x, lookInput.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }


// Mise à jour des animations (si nécessaire)
void UpdateAnimations()
{
    if (animator != null)
    {
        // On récupère l'input de déplacement (vous pouvez utiliser moveInput ou smoothMoveInput selon vos besoins)
        Vector2 move = moveInput; 
        float moveX = move.x;
        float moveY = move.y;

        // Si le joueur bouge (pour éviter les interférences quand le stick est au repos)
        if (move.magnitude > 0.1f)
        {
            // On normalise la direction et on calcule le dot produit avec la direction "avant" (0,1)
            Vector2 normalizedMove = move.normalized;
            float dot = Vector2.Dot(normalizedMove, Vector2.up);

            // Si le joystick est dirigé dans un cône de 45° autour de l'avant 
            // (dot >= cos(45°) soit environ 0.707) et que la composante avant est trop faible,
            // on la force à une valeur minimale (ici 0.45) pour déclencher l'animation "walk"
            if (dot >= 0.707f && moveY < 0.45f)
            {
                moveY = 0.45f;
            }
        }

        // On transmet les valeurs à l'Animator (le blend tree se charge de gérer l'animation "walk")
        animator.SetFloat("MoveX", moveX);
        animator.SetFloat("MoveY", moveY);

        // L'animation "Run" se déclenche uniquement si le bouton "Run" est pressé (vérifié via isRunning)
        animator.SetBool("IsRun", isRunning);
    }
}

    private void StartRunning()
    {
        isRunning = true;
        IsRun = true;
    }

    private void StopRunning()
    {
        isRunning = false;
        IsRun = false;
    }

// Modifiez la méthode ApplyDeadzone pour accepter un paramètre deadzone
private void ApplyDeadzone(ref Vector2 input, float deadzone = 0.3f)
{
    if (input.magnitude < deadzone)
    {
        input = Vector2.zero;
    }
    else
    {
        input = input.normalized * ((input.magnitude - deadzone) / (1 - deadzone));
    }
}


// Empêcher le saut si le bouton n'est pas enfoncé
// Empêcher le saut si le bouton n'est pas enfoncé
private void HandleJump()
{
    if (isGrounded && !isJumping)
    {
        jumpRequested = true; // Le saut est demandé uniquement lorsque "Jump" est appuyé
    }
}

    void Attack()
    {
        if (attackScript != null)
        {
            attackScript.StartAttack();
        }
    }

    void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Disable();
            Debug.Log("Contrôles du joueur désactivés.");
        }
        else
        {
            Debug.LogWarning("playerControls est null. Impossible de désactiver.");
        }

        // Désactivation des autres scripts liés si nécessaire
        if (attackScript != null)
        {
            attackScript.StopAttack(); // Si vous avez une fonction d'arrêt d'attaque
        }
    }

    public void Die()
    {
        // Logique de mort du joueur
        // Désactive les contrôles à la mort du joueur
        playerControls.Player.Disable();
        Debug.Log("Le joueur est mort. Les contrôles sont désactivés.");
    }

    public void DisableMovement()
    {
        moveInput = Vector2.zero; // Désactive le mouvement
    }

    public void EnableMovement()
    {
        moveInput = playerControls.Player.Move.ReadValue<Vector2>();
    }

    void SnapToGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            // Positionner l'ennemi juste au-dessus du point de collision du terrain
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.1f, transform.position.z);
            Debug.Log("Ennemi placé correctement sur le sol.");
        }
    }
}
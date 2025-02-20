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

    // Variables de caméra
    public Transform cameraTransform; // Référence à la caméra pour calculer le mouvement relatif
    private Vector2 moveInput; // Entrée du stick gauche pour les mouvements
    private Vector2 lookInput; // Entrée du stick droit pour la rotation de la caméra

    private CharacterController characterController;
    private Vector3 velocity;
    private Animator animator; // Référence à l'Animator

    // Variables pour le saut
    public float jumpHeight = 2f; // Hauteur du saut
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
    CheckGrounded(); // Utilise le Raycast pour une meilleure détection du sol

    if (attackScript != null && attackScript.isAttacking || (parryScript != null && parryScript.isParrying))
    {
        smoothMoveInput = Vector2.zero;
    }
    else
    {
        moveInput = playerControls.Player.Move.ReadValue<Vector2>();
        lookInput = playerControls.Player.Camera.ReadValue<Vector2>();
        ApplyDeadzone(ref moveInput);

        smoothMoveInput = Vector2.Lerp(smoothMoveInput, moveInput, 0.2f);
        if (moveInput == Vector2.zero)
            smoothMoveInput = Vector2.zero;
    }

    if (isGrounded)
    {
        velocity.y = -2f; // Garde le joueur collé au sol
        animator.SetBool("IsJumping", false);
    }
    else
    {
        velocity.y += gravityValue * Time.deltaTime;
        animator.SetBool("IsJumping", true);
    }

    characterController.Move(velocity * Time.deltaTime);
    MoveCharacter();
    RotateCharacterWithCamera();
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

    void UpdateAnimations()
    {
        if (animator != null)
        {
            float moveX = smoothMoveInput.x;
            float moveY = smoothMoveInput.y;
            animator.SetFloat("MoveX", moveX);
            animator.SetFloat("MoveY", moveY);

            bool isMovingForward = moveY > 0.1f;
            bool isMovingBackward = moveY < -0.1f;

            if (isGrounded)
            {
                if (isMovingForward || isMovingBackward)
                {
                    animator.SetBool("IsRun", IsRun);
                }
                else
                {
                    animator.SetBool("IsRun", false);
                }
            }
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

    private void ApplyDeadzone(ref Vector2 input)
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

    private void HandleJump()
{
    if (isGrounded && velocity.y <= 0f) // Vérifier que le joueur ne tombe pas déjà
    {
        isJumping = true;
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue); // Appliquer la vélocité du saut
        animator.SetBool("IsJumping", true);
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

    void CheckGrounded()
{
    float extraHeight = 0.2f; // Ajuste cette valeur si besoin
    RaycastHit hit;

    if (Physics.Raycast(transform.position, Vector3.down, out hit, characterController.height / 2 + extraHeight))
    {
        isGrounded = true;
    }
    else
    {
        isGrounded = false;
    }

    animator.SetBool("IsJumping", !isGrounded);
}
}
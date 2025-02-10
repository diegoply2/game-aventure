using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    private Animator animator;
    private Rigidbody2D rb;
    private Vector2 movement;
    private PlayerControls controls; // Instance des contrôles définis dans Input Actions
    private InputAction moveAction; // Action pour le mouvement du joueur
    private InputAction jumpAction; // Action pour le saut (bouton "A")

    private void Awake()
    {
        // Initialisation des contrôles via le fichier d'Input Actions généré
        controls = new PlayerControls();
        
        // Obtenir l'action de mouvement (par exemple, déplace le joueur)
        moveAction = controls.Player.Move;
        // Obtenir l'action de saut (par exemple, bouton "A")
        jumpAction = controls.Player.Jump;
        
        // Lier les événements de l'action de saut à une méthode
        jumpAction.performed += ctx => HandleJump();
    }

    private void OnEnable()
    {
        // Activer les actions d'entrée
        controls.Enable();
    }

    private void OnDisable()
    {
        // Désactiver les actions d'entrée
        controls.Disable();
    }

    private void Start()
    {
        // Initialisation des composants
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Lire les valeurs d'entrée pour le mouvement
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        movement = moveInput * speed;

        // Animer les déplacements du joueur en fonction des axes X et Y
        animator.SetFloat("MoveX", moveInput.x);
        animator.SetFloat("MoveY", moveInput.y);

        // Détection de l'appui sur le bouton "A" (saut)
        if (jumpAction.triggered)
        {
            Debug.Log("Bouton A pressé !");
        }
    }

    private void FixedUpdate()
    {
        // Appliquer la vélocité calculée au Rigidbody2D pour déplacer le joueur
        rb.linearVelocity = movement;  // Utiliser 'velocity' pour déplacer le Rigidbody2D
    }

    private void HandleJump()
    {
        // Logic for jump (e.g., apply force, trigger jump animation, etc.)
        Debug.Log("Saut effectué !");
    }
}

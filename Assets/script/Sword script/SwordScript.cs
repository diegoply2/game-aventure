using UnityEngine;
using UnityEngine.InputSystem;

public class SwordScript : MonoBehaviour
{
    public GameObject sword;  // Référence à l'objet de l'épée
    public Transform playerHand;  // La main du joueur où l'épée doit être attachée
    public Vector3 offsetPosition;  // Décalage de la position de l'épée par rapport à la main
    public Vector3 rotationSpeed;  // Vitesse de rotation de l'épée sur les axes X, Y, Z
    public float smoothness = 5f;  // Fluidité du mouvement (le plus grand, plus fluide)
    public Vector3 rotationOffset;  // Décalage de la rotation de l'épée par rapport à la main

    private Quaternion targetRotation;
    private AttaqueScript attaqueScript;  // Référence au script AttaqueScript

    private Animator playerAnimator;  // Référence à l'Animator du joueur
    private bool isFirstPersonView = false;  // Pour vérifier si le joueur est en vue à la première personne

    private PlayerControls controls;  // Référence au contrôleur d'entrée
    private Vector2 cameraInput;  // Pour gérer l'entrée de la caméra (joystick droit)

    void Awake()
    {
        // Créez un nouvel objet de contrôle
        controls = new PlayerControls();
    }

    void Start()
    {
        // Si l'épée n'est pas assignée dans l'Inspector, on la trouve par tag
        if (sword == null)
        {
            sword = GameObject.FindGameObjectWithTag("PlayerSword");
        }

        if (sword != null)
        {
            Debug.Log("L'épée est assignée correctement.");
        }
        else
        {
            Debug.LogError("L'épée n'est pas assignée !");
        }

        // Récupère la référence à AttaqueScript et Animator
        attaqueScript = GetComponentInParent<AttaqueScript>();
        playerAnimator = GetComponentInParent<Animator>();

        // Vérifiez si la main du joueur est assignée
        if (playerHand == null)
        {
            Debug.LogError("La main du joueur n'est pas assignée dans l'Inspector !");
        }
    }

    void OnEnable()
    {
        // Active les actions du Input System
        controls.Enable();
    }

    void OnDisable()
    {
        // Désactive les actions du Input System
        controls.Disable();
    }

    void Update()
{
    // Vérifier l'état de la vue
    isFirstPersonView = CameraFollow.Instance.IsFirstPersonView();

    // Lire l'entrée du joystick droit pour la rotation de la caméra (et de l'épée)
    cameraInput = controls.Player.Camera.ReadValue<Vector2>();  // "Camera" est mappé au joystick droit

    if (sword != null && playerHand != null)
    {
        // Si en vue à la troisième personne, l'épée suit la position de la main
        if (!isFirstPersonView)
        {
            sword.transform.position = playerHand.position + offsetPosition;

            // Appliquer un décalage de rotation de l'épée par rapport à la main du joueur
            sword.transform.rotation = playerHand.rotation * Quaternion.Euler(rotationOffset);
        }
        else
        {
            // Supprimez cette partie pour éviter le changement de position de l'épée en vue à la première personne
            // sword.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1.0f;  // Position à 1 unité devant la caméra
        }

        // Calculer la rotation de l'épée pendant l'attaque ou avec la souris
        if (attaqueScript.isAttacking)
        {
            // Calculer la rotation de l'épée en fonction de l'input gamepad (joystick droit)
            float rotationX = cameraInput.x * rotationSpeed.x;
            float rotationY = cameraInput.y * rotationSpeed.y;

            // Créer une nouvelle rotation basée sur l'entrée du joystick droit
            targetRotation = Quaternion.Euler(rotationX, rotationY, 0f);

            // Appliquer la rotation avec une interpolation douce pour la fluidité
            sword.transform.rotation = Quaternion.Slerp(sword.transform.rotation, targetRotation, smoothness * Time.deltaTime);
        }
    }
}


    // Assurez-vous que le collider de l'épée est un Trigger
    void OnTriggerEnter(Collider other)
    {
        // Vérifie si l'épée touche un objet avec le tag "Enemy" et si l'attaque est en cours
        if (other.CompareTag("Enemy") && attaqueScript.isAttacking)
        {
            // Récupère la référence à EnemyHealth
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Inflige des dégâts à l'ennemi (valeur aléatoire entre 2 et 8)
                float randomDamage = Random.Range(2f, 8f);  
                enemyHealth.TakeDamage(randomDamage); 
                Debug.Log("L'ennemi a perdu " + randomDamage + " points de santé !");
            }
        }
    }
}

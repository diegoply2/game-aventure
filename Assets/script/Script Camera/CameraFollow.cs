using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Personnage ou objet à suivre
    public float distance = 5f; // Distance de la caméra par rapport au personnage
    public float height = 2f; // Hauteur de la caméra par rapport au personnage
    public float rotationSpeed = 5f; // Vitesse de rotation de la caméra autour du personnage
    public float smoothSpeed = 0.125f; // Vitesse de lissage du mouvement de la caméra

    private float currentRotation = 0f; // Rotation actuelle autour de l'axe Y
    private Vector3 currentVelocity = Vector3.zero; // Vitesse actuelle de la caméra pour le lissage

    private PlayerControls controls;  // Référence aux contrôles du joueur
    private Transform cameraTransform; // Référence à la caméra

    private bool isFirstPersonView = false;  // Indicateur pour la vue à la première personne
    private bool cameraTogglePressed = false; // Indicateur pour vérifier si le bouton a été pressé

    // Singleton : une seule instance de CameraFollow accessible globalement
    public static CameraFollow Instance { get; private set; }

    void Awake()
{
    // Vérifie si une instance existe déjà
    if (Instance == null)
    {
        Instance = this;  // Si ce n'est pas le cas, définit cette instance comme l'unique
    }
    else if (Instance != this)
    {
        // Si une autre instance existe déjà, on détruit ce GameObject
        Destroy(gameObject);
    }
}

    void OnEnable()
    {
        // Initialisation des contrôles
        if (controls == null)
        {
            controls = new PlayerControls();
            controls.Enable(); // Assurez-vous que les contrôles sont initialisés
        }
    }

    void OnDisable()
    {
        // Désactiver les contrôles si définis
        if (controls != null)
        {
            controls.Disable();
        }
    }

    void Update()
    {
        if (target == null)
            return; // Si le target est null, ne rien faire.

        // Lire la valeur de l'action Camera1 (clic droit du joystick)
        bool cameraToggle = controls.Player.Camera1.ReadValue<float>() > 0f;  // Vérifie si le bouton est pressé

        // Si le bouton est appuyé et qu'il n'a pas encore été pris en compte
        if (cameraToggle && !cameraTogglePressed)
        {
            cameraTogglePressed = true;  // Empêche les multiples changements pendant un seul clic
            ToggleCameraView();  // Bascule la vue de la caméra
        }
        else if (!cameraToggle)
        {
            cameraTogglePressed = false;  // Réinitialiser lorsque le bouton est relâché
        }

        if (!isFirstPersonView)
        {
            // Récupérer les entrées du stick droit (pour la rotation de la caméra)
            Vector2 cameraInput = controls.Player.Camera.ReadValue<Vector2>();
            float horizontalInput = cameraInput.x;
            float verticalInput = cameraInput.y;

            // Calcul de la nouvelle rotation en fonction de l'entrée (rotation autour de l'axe Y)
            currentRotation += horizontalInput * rotationSpeed * Time.deltaTime;

            // Calcul de la position désirée de la caméra derrière le joueur
            Vector3 desiredPosition = target.position - target.forward * distance + Vector3.up * height;

            // Application du lissage de la position pour une caméra fluide
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed);

            // Faire tourner la caméra autour du personnage en fonction de l'entrée
            transform.RotateAround(target.position, Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);

            // Toujours faire en sorte que la caméra regarde le personnage (en tenant compte de la hauteur)
            transform.LookAt(target.position + Vector3.up * height);
        }
        else
    {
        // Décalage léger vers l'arrière pour éviter que la caméra soit trop proche
        Vector3 offset = -target.forward * 0f; // Ajustez la valeur pour reculer plus ou moins
        transform.position = target.position + Vector3.up * height + offset;
        transform.rotation = target.rotation; // La caméra suit la rotation du joueur.
    }
    }

    void ToggleCameraView()
    {
        // Alterner entre la vue à la première personne et la vue à la troisième personne
        isFirstPersonView = !isFirstPersonView;

        if (isFirstPersonView)
        {
            SwitchToFirstPersonView();
        }
        else
        {
            SwitchToThirdPersonView();
        }
    }

    void SwitchToFirstPersonView()
    {
        // Code pour passer à la vue à la première personne
        Debug.Log("Passage à la vue à la première personne");
    }

    void SwitchToThirdPersonView()
    {
        // Code pour passer à la vue à la troisième personne
        Debug.Log("Passage à la vue à la troisième personne");
    }

    public bool IsFirstPersonView()
    {
        return isFirstPersonView;  // Assurez-vous que 'isFirstPersonView' est public ou accessible
    }

}


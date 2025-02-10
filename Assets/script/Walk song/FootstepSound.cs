using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip footstepSoundWalk; // Son de marche
    public AudioClip footstepSoundRun;  // Son de course
    public AudioClip jumpSound;         // Son de saut
    public AudioClip landSound;         // Son d'atterrissage
    public float walkInterval = 0.5f;   // Intervalle entre les pas en marchant
    public float runInterval = 0.3f;    // Intervalle entre les pas en courant

    public float soundVolume = 1f;      // Volume global pour les sons (0 à 1)

    private CharacterController characterController;
    private CharacterControllerWithCamera playerController;
    private AttaqueScript attaqueScript; // Référence au script d'attaque
    private float footstepTimer = 0f;
    private bool wasGrounded = true; // Pour détecter les sauts et atterrissages

    void Awake()
    {
        // Récupérer l'AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Ajouter un AudioSource si absent
        }

        // Appliquer le volume
        audioSource.volume = soundVolume;

        // Récupérer le CharacterController et le script de mouvement
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<CharacterControllerWithCamera>();
        attaqueScript = GetComponent<AttaqueScript>(); // Référence au script d'attaque

        if (characterController == null)
        {
            Debug.LogError("CharacterController est manquant sur l'objet !");
        }
        if (playerController == null)
        {
            Debug.LogError("CharacterControllerWithCamera est manquant !");
        }
        if (attaqueScript == null)
        {
            Debug.LogError("AttaqueScript est manquant !");
        }
    }

    void Update()
    {
        if (playerController == null || characterController == null || attaqueScript == null) return;

        // Détection du saut
        if (wasGrounded && !characterController.isGrounded)
        {
            PlayJumpSound();
        }

        // Détection de l'atterrissage
        if (!wasGrounded && characterController.isGrounded)
        {
            PlayLandSound();
        }

        wasGrounded = characterController.isGrounded; // Mettre à jour l'état du sol

        // Vérifier que le personnage est bien au sol et a un mouvement actif
        if (characterController.isGrounded)
        {
            float moveMagnitude = playerController.smoothMoveInput.magnitude; // Utiliser smoothMoveInput pour plus de fluidité

            if (moveMagnitude > 0.1f)
            {
                footstepTimer += Time.deltaTime;

                // Déterminer l'intervalle et le son à jouer en fonction de la course ou de la marche
                float interval = playerController.isRunning ? runInterval : walkInterval;
                AudioClip footstepClip = playerController.isRunning ? footstepSoundRun : footstepSoundWalk;

                if (footstepTimer >= interval)
                {
                    PlayFootstepSound(footstepClip);
                    footstepTimer = 0f;
                }
            }
            else
            {
                footstepTimer = 0f;
            }
        }
    }

    private void PlayFootstepSound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void PlayJumpSound()
    {
        if (jumpSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    private void PlayLandSound()
    {
        if (landSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(landSound);
        }
    }
}

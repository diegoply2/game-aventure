using UnityEngine;

public class AttackSound : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip attackSound;  // Le son d'attaque
    public AudioClip parrySound;   // Son de parade
    public float volume = 0.6f;    // Définir le volume à 0.6 (peut être ajusté)

    void Awake()
    {
        // Récupérer l'AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Ajouter un AudioSource si absent
        }

        // Définir le volume global de l'AudioSource
        audioSource.volume = volume;
    }

    public void PlayAttackSound()
    {
        if (attackSound != null && audioSource != null)
        {
            // Jouer le son d'attaque
            audioSource.PlayOneShot(attackSound);
        }
        else
        {
            Debug.LogWarning("Attack sound or AudioSource not found!");
        }
    }

    // Méthode pour jouer le son de parade
    public void PlayParrySound()
    {
        if (parrySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(parrySound);
        }
    }
}

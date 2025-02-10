using UnityEngine;

public class EnemyAttackSound : MonoBehaviour
{
    private AudioSource EnemyaudioSource;  // Déclarer une variable privée pour AudioSource
    public AudioClip EnemyattackSound;     // Le son d'attaque
    public AudioClip EnemyparrySound;      // Son de parade
    public float volume = 0.6f;            // Définir le volume à 0.6 (peut être ajusté)

    void Awake()
    {
        // Récupérer l'AudioSource
        EnemyaudioSource = GetComponent<AudioSource>();
        if (EnemyaudioSource == null)
        {
            Debug.LogWarning("AudioSource component is missing. Adding one.");
            EnemyaudioSource = gameObject.AddComponent<AudioSource>(); // Ajouter un AudioSource si absent
        }
        else
        {
            Debug.Log("AudioSource found.");
        }

        // Vérification des AudioClips
        if (EnemyattackSound == null)
        {
            Debug.LogWarning("Attack sound not assigned!");
        }
        if (EnemyparrySound == null)
        {
            Debug.LogWarning("Parry sound not assigned!");
        }

        // Définir le volume global de l'AudioSource
        EnemyaudioSource.volume = volume;
    }

    public void EnemyPlayAttackSound()
    {
        if (EnemyattackSound != null && EnemyaudioSource != null)
        {
            // Jouer le son d'attaque
            EnemyaudioSource.PlayOneShot(EnemyattackSound);
        }
        else
        {
            Debug.LogWarning("Attack sound or AudioSource not found!");
        }
    }

    // Méthode pour jouer le son de parade
    public void EnemyPlayParrySound()
    {
        if (EnemyparrySound != null && EnemyaudioSource != null)
        {
            EnemyaudioSource.PlayOneShot(EnemyparrySound);
        }
    }
}

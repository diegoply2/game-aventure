using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] musicTracks;
    private AudioSource audioSource;
    private float minTrackTime = 60f; // 1 minute
    private float maxTrackTime = 180f; // 3 minutes
    private float nextTrackTime;
    [SerializeField] private float initialFadeInDuration = 20f; // Durée du fade-in au démarrage
    [SerializeField] private float maxVolume = 0.60f; // Volume maximum de la musique

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = maxVolume; // Assurez-vous que le volume ne dépasse pas 0.45
        PlayRandomTrackWithFade();
    }

void PlayRandomTrackWithFade()
{
    if (musicTracks.Length == 0) return;

    int randomIndex = Random.Range(0, musicTracks.Length);
    audioSource.clip = musicTracks[randomIndex];

    float upperBound = 0f;
    // Si le clip est assez long, on choisit un point de départ aléatoire
    // de manière à ce qu'il reste au moins 90 secondes avant la fin.
    if (audioSource.clip.length > 90f)
    {
        // On calcule deux bornes : 
        // - 75% de la longueur totale (pour respecter l'ancienne contrainte)
        // - la longueur totale moins 90 secondes (pour être sûr qu'il reste au moins 90 sec)
        upperBound = Mathf.Min(audioSource.clip.length * 0.75f, audioSource.clip.length - 90f);
    }
    // Si le clip est trop court, on démarre dès le début (0f).
    audioSource.time = Random.Range(0f, upperBound);

    audioSource.Play();

    // Fade-in dès le début
    StartCoroutine(FadeIn(audioSource, initialFadeInDuration));

    // Choix du moment pour la prochaine transition (reste inchangé)
    nextTrackTime = Random.Range(minTrackTime, maxTrackTime);
    Invoke(nameof(FadeOutAndNextTrack), nextTrackTime);
}


    void FadeOutAndNextTrack()
    {
        StartCoroutine(FadeOutAndChangeTrack(1.5f)); // 1.5 secondes de transition douce
    }

    System.Collections.IEnumerator FadeOutAndChangeTrack(float fadeDuration)
    {
        float startVolume = audioSource.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0;
        PlayRandomTrackWithFade();

        StartCoroutine(FadeIn(audioSource, fadeDuration));
    }

    System.Collections.IEnumerator FadeIn(AudioSource source, float fadeDuration)
    {
        float targetVolume = maxVolume; // Utiliser maxVolume au lieu de 1.0f

        // Si c'est un fade-in initial, commence avec une faible volume
        if (source.volume == 0)
        {
            source.volume = 0f;
        }

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(0, targetVolume, t / fadeDuration);
            yield return null;
        }

        source.volume = targetVolume;
    }
}

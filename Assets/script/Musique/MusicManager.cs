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
        audioSource.time = Random.Range(0f, audioSource.clip.length * 0.75f); // Commencer à un moment aléatoire
        audioSource.Play();

        // Fade-in de la musique dès le début du jeu
        StartCoroutine(FadeIn(audioSource, initialFadeInDuration));

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

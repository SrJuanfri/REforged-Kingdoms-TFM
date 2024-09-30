using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class RandomMusicPlayerWithDucking : MonoBehaviour
{
    public AudioClip[] musicTracks; // Canciones
    public float fadeDuration = 2f; // Duración del fade entre pistas
    public float timeBetweenTracks = 2f; // Tiempo entre pistas
    public float duckVolume = 0.2f; // Volumen cuando hay otros sonidos
    public float normalVolume = 1f; // Volumen normal cuando no hay otros sonidos
    public float duckFadeDuration = 0.5f; // Duración del fade cuando se aplica el ducking

    public AudioMixerGroup musicGroup; // Referencia al grupo de música en el Audio Mixer

    private AudioSource audioSource;
    private int currentTrackIndex = -1;
    private bool isDucked = false; // Indica si el volumen está reducido

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // No destruir el objeto al cambiar de escena
    }

    void Start()
    {
        // Verifica que hay canciones asignadas
        if (musicTracks.Length == 0)
        {
            Debug.LogError("No hay canciones asignadas en musicTracks.");
            return;
        }

        // Asegúrate de que el AudioSource esté presente
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Asignar el grupo de música al AudioSource
        if (musicGroup != null)
        {
            audioSource.outputAudioMixerGroup = musicGroup;
        }

        // Establece el volumen normal para todas las canciones
        audioSource.volume = normalVolume;

        // Comienza a reproducir las canciones
        StartCoroutine(PlayRandomTrack());
    }

    void Update()
    {
        // Verifica si hay otros sonidos reproduciéndose
        CheckForOtherSounds();
    }

    IEnumerator PlayRandomTrack()
    {
        while (true)
        {
            // Espera si hay un tiempo entre canciones
            if (currentTrackIndex != -1)
            {
                yield return new WaitForSeconds(timeBetweenTracks);
            }

            // Elige una canción aleatoria
            int newTrackIndex;
            do
            {
                newTrackIndex = Random.Range(0, musicTracks.Length);
            } while (newTrackIndex == currentTrackIndex); // Evita repetir la misma pista

            currentTrackIndex = newTrackIndex;
            AudioClip nextTrack = musicTracks[currentTrackIndex];

            // Realiza un fade-out de la pista actual si está sonando
            if (audioSource.isPlaying)
            {
                yield return StartCoroutine(FadeOut(audioSource, fadeDuration));
            }

            // Cambia la pista y ajusta el volumen a `normalVolume`
            audioSource.clip = nextTrack;
            audioSource.volume = normalVolume; // Volumen igual para todas las pistas
            audioSource.Play();
            yield return StartCoroutine(FadeIn(audioSource, fadeDuration));

            // Espera hasta que la pista termine
            yield return new WaitForSeconds(nextTrack.length);
        }
    }

    // Función para hacer el fade-in
    IEnumerator FadeIn(AudioSource audioSource, float duration)
    {
        float startVolume = 0f;

        while (audioSource.volume < normalVolume)
        {
            audioSource.volume += Time.deltaTime / duration;
            yield return null;
        }

        audioSource.volume = normalVolume;
    }

    // Función para hacer el fade-out
    IEnumerator FadeOut(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Restablece el volumen original para la siguiente pista
    }

    // Comprueba si otros sonidos están reproduciéndose y aplica ducking
    void CheckForOtherSounds()
    {
        // Encuentra todos los AudioSources que no sean este
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in allAudioSources)
        {
            if (source != audioSource && source.isPlaying)
            {
                // Si otro sonido está reproduciéndose, reduce el volumen
                if (!isDucked)
                {
                    StartCoroutine(DuckMusic());
                }
                return;
            }
        }

        // Si no hay otros sonidos, restaura el volumen
        if (isDucked)
        {
            StartCoroutine(RestoreMusicVolume());
        }
    }

    // Función para reducir el volumen de la música
    IEnumerator DuckMusic()
    {
        isDucked = true;
        float targetVolume = duckVolume;

        while (audioSource.volume > targetVolume)
        {
            audioSource.volume -= Time.deltaTime / duckFadeDuration;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    // Función para restaurar el volumen de la música
    IEnumerator RestoreMusicVolume()
    {
        isDucked = false;
        float targetVolume = normalVolume;

        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += Time.deltaTime / duckFadeDuration;
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}

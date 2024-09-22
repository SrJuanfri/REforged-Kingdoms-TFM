using UnityEngine;
using System.Collections.Generic;

public class CharacterVoice : MonoBehaviour
{
    [Header("Gender Selection")]
    [SerializeField] private bool isFemale = true;

    [Header("Voice Clips")]
    [SerializeField] private List<AudioClip> femaleVoiceClips; // Lista de clips femeninos
    [SerializeField] private List<AudioClip> maleVoiceClips;   // Lista de clips masculinos
    [SerializeField] private AudioSource audioSource;

    [Header("Audio Settings")]
    [SerializeField] private float femalePitch = 1.0f;
    [SerializeField] private float malePitch = 0.9f;  // Un poco m�s bajo para la voz masculina
    [SerializeField] private float pitchVariation = 0.02f;  // Menor variaci�n de tono
    [SerializeField] private float volumeVariation = 0.02f; // Menor variaci�n de volumen
    [SerializeField] private float baseVolume = 0.5f;  // Volumen base m�s bajo

    [Header("Low Pass Filter")]
    [SerializeField] private AudioLowPassFilter lowPassFilter;
    [SerializeField] private float femaleCutoff = 5000f;
    [SerializeField] private float maleCutoff = 3500f;  // Frecuencia m�s alta para evitar sonidos opacos

    [Header("Reverb Filter")]
    [SerializeField] private AudioReverbFilter reverbFilter;
    [SerializeField] private float femaleReverbLevel = 100f;  // Reverb m�s bajo
    [SerializeField] private float maleReverbLevel = 150f;    // Reverb m�s bajo

    private float maxAudioDuration = 3f; // M�ximo de 3 segundos

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.spatialBlend = 0.5f;  // Sonido m�s centrado
        audioSource.volume = baseVolume;  // Configurar el volumen base

        if (lowPassFilter == null)
        {
            lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
        }

        if (reverbFilter == null)
        {
            reverbFilter = gameObject.AddComponent<AudioReverbFilter>();
        }
    }

    public void PlayVoice()
    {
        // Variar ligeramente el volumen para hacer que las voces suenen m�s naturales
        audioSource.volume = baseVolume + Random.Range(-volumeVariation, volumeVariation);

        AudioClip selectedClip = null;

        if (isFemale && femaleVoiceClips.Count > 0)
        {
            // Seleccionar aleatoriamente un clip de la lista femenina
            selectedClip = femaleVoiceClips[Random.Range(0, femaleVoiceClips.Count)];
            audioSource.pitch = femalePitch + Random.Range(-pitchVariation, pitchVariation);
            lowPassFilter.cutoffFrequency = femaleCutoff;
            reverbFilter.reverbLevel = femaleReverbLevel;
        }
        else if (!isFemale && maleVoiceClips.Count > 0)
        {
            // Seleccionar aleatoriamente un clip de la lista masculina
            selectedClip = maleVoiceClips[Random.Range(0, maleVoiceClips.Count)];
            audioSource.pitch = malePitch + Random.Range(-pitchVariation, pitchVariation);
            lowPassFilter.cutoffFrequency = maleCutoff;
            reverbFilter.reverbLevel = maleReverbLevel;
        }

        if (selectedClip != null)
        {
            audioSource.clip = selectedClip;
            audioSource.Play();

            // Detener el audio despu�s de 3 segundos o menos si el clip es m�s corto
            Invoke(nameof(StopVoice), Mathf.Min(maxAudioDuration, selectedClip.length));
        }
        else
        {
            Debug.LogWarning("No voice clip found for the selected gender.");
        }
    }

    private void StopVoice()
    {
        audioSource.Stop();
    }
}

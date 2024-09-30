using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(AudioSource))]
public class ButtonSound : MonoBehaviour
{
    private Button button;
    private AudioSource audioSource;

    void Start()
    {
        // Obt�n el componente Button y AudioSource
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();

        // Asigna la funci�n que se ejecutar� al hacer clic en el bot�n
        button.onClick.AddListener(PlaySound);
    }

    // Funci�n para reproducir el sonido
    void PlaySound()
    {
        // Verifica que el GlobalSoundManager est� presente y tenga asignado un sonido
        if (GlobalSoundManager.Instance != null && GlobalSoundManager.Instance.globalButtonClickSound != null)
        {
            // Reproduce el sonido global
            audioSource.PlayOneShot(GlobalSoundManager.Instance.globalButtonClickSound);
        }
        else
        {
            Debug.LogWarning("No se ha asignado ning�n sonido global en el GlobalSoundManager.");
        }
    }
}

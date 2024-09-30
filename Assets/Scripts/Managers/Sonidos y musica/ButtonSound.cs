using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(AudioSource))]
public class ButtonSound : MonoBehaviour
{
    private Button button;
    private AudioSource audioSource;

    void Start()
    {
        // Obtén el componente Button y AudioSource
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();

        // Asigna la función que se ejecutará al hacer clic en el botón
        button.onClick.AddListener(PlaySound);
    }

    // Función para reproducir el sonido
    void PlaySound()
    {
        // Verifica que el GlobalSoundManager esté presente y tenga asignado un sonido
        if (GlobalSoundManager.Instance != null && GlobalSoundManager.Instance.globalButtonClickSound != null)
        {
            // Reproduce el sonido global
            audioSource.PlayOneShot(GlobalSoundManager.Instance.globalButtonClickSound);
        }
        else
        {
            Debug.LogWarning("No se ha asignado ningún sonido global en el GlobalSoundManager.");
        }
    }
}

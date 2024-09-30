using UnityEngine;

public class GlobalSoundManager : MonoBehaviour
{
    public static GlobalSoundManager Instance;

    // Sonido compartido que se usar� en todos los botones
    public AudioClip globalButtonClickSound;

    private void Awake()
    {
        // Asegura que esta instancia sea �nica (Singleton)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No destruir al cargar nuevas escenas
        }
        else
        {
            Destroy(gameObject); // Elimina instancias duplicadas
        }
    }
}

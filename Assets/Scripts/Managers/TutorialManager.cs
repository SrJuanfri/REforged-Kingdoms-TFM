using System.Collections;
using System.Linq;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public Transform playerCamera; // C�mara en primera persona del jugador
    public MonoBehaviour[] playerControlScripts; // Scripts que controlan al jugador
    public Transform objectToAnimate; // Objeto que har� una animaci�n hacia abajo al final
    public AudioClip soundEffect; // Sonido opcional que se reproducir�
    public AudioSource audioSource; // Fuente de audio para reproducir el sonido

    public float headTiltDownAngle = 30f; // �ngulo m�ximo hacia abajo
    public float subtleHeadTiltUpAngle = 5f;   // �ngulo de subida m�s sutil
    public float animationDuration = 1.5f; // Duraci�n de cada parte de la animaci�n
    public float waitTimeAtBottom = 2f; // Tiempo que la c�mara permanecer� abajo
    public float waitTimeAfterSubtleUp = 3f; // Tiempo extra que la c�mara se mantendr� abajo despu�s de la subida
    public float objectFallDistance = 5f; // Distancia l�mite para destruir el objeto

    private Quaternion initialRotation;

    void Start()
    {
        // Almacena la rotaci�n inicial de la c�mara
        initialRotation = playerCamera.localRotation;

        // A�adir autom�ticamente todos los scripts que no est�n ya en playerControlScripts
        AddMissingPlayerScripts();

        // Verifica o a�ade un AudioSource si no existe
        EnsureAudioSource();

        DisablePlayerControls();
        StartCoroutine(PlayCameraTiltAnimation());
    }

    void AddMissingPlayerScripts()
    {
        // Obtiene todos los MonoBehaviour del objeto Player
        var allPlayerScripts = playerCamera.GetComponentInParent<Transform>().GetComponentsInChildren<MonoBehaviour>();

        // A�ade los que no est�n ya en playerControlScripts
        playerControlScripts = playerControlScripts.Concat(allPlayerScripts.Except(playerControlScripts)).ToArray();
    }

    void DisablePlayerControls()
    {
        // Desactiva los scripts de control del jugador
        foreach (var script in playerControlScripts)
        {
            script.enabled = false;
        }
    }

    void EnablePlayerControls()
    {
        // Reactiva los scripts de control del jugador
        foreach (var script in playerControlScripts)
        {
            script.enabled = true;
        }
    }

    void EnsureAudioSource()
    {
        // Si no se ha asignado un AudioSource, intenta obtener uno del objeto
        if (audioSource == null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();

            // Si no tiene uno, lo a�ade
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    IEnumerator PlayCameraTiltAnimation()
    {
        // Inclinaci�n hacia abajo
        yield return TiltCamera(headTiltDownAngle, animationDuration);

        // Mantiene la c�mara inclinada hacia abajo por un tiempo
        yield return new WaitForSeconds(waitTimeAtBottom);

        // Sube un poco (sutilmente)
        yield return TiltCamera(subtleHeadTiltUpAngle, animationDuration);

        // Baja otra vez para leer bien
        yield return TiltCamera(headTiltDownAngle, animationDuration);

        // Mantiene la c�mara inclinada hacia abajo por m�s tiempo despu�s de la peque�a subida
        yield return new WaitForSeconds(waitTimeAfterSubtleUp);

        // Vuelve a la posici�n recta
        yield return TiltCamera(0f, animationDuration);

        // Reproduce sonido si se ha asignado
        if (soundEffect && audioSource)
        {
            audioSource.PlayOneShot(soundEffect);
        }

        // Inicia la animaci�n del objeto hacia abajo
        if (objectToAnimate != null)
        {
            StartCoroutine(AnimateObjectDown(objectToAnimate));
        }

        // Reactiva los controles del jugador
        EnablePlayerControls();
    }

    IEnumerator TiltCamera(float targetAngle, float duration)
    {
        float timeElapsed = 0;
        Quaternion startRotation = playerCamera.localRotation;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(targetAngle, 0f, 0f);

        while (timeElapsed < duration)
        {
            playerCamera.localRotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.localRotation = targetRotation;
    }

    IEnumerator AnimateObjectDown(Transform obj)
    {
        Vector3 startPosition = obj.localPosition;
        Vector3 targetPosition = startPosition + Vector3.down * objectFallDistance; // Se mueve hacia abajo

        float duration = 1f; // Duraci�n de la animaci�n
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            obj.localPosition = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            // Comprueba si el objeto ha alcanzado o superado la distancia l�mite
            if (Vector3.Distance(startPosition, obj.localPosition) >= objectFallDistance)
            {
                Destroy(obj.gameObject); // Destruye el objeto si est� lo suficientemente lejos
                yield break;
            }

            yield return null;
        }

        obj.localPosition = targetPosition;
    }
}

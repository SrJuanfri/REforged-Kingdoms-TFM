using System.Collections;
using System.Linq;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public Transform playerCamera; // Cámara en primera persona del jugador
    public MonoBehaviour[] playerControlScripts; // Scripts que controlan al jugador
    public Transform objectToAnimate; // Objeto que hará una animación hacia abajo al final
    public AudioClip soundEffect; // Sonido opcional que se reproducirá
    public AudioSource audioSource; // Fuente de audio para reproducir el sonido

    public float headTiltDownAngle = 30f; // Ángulo máximo hacia abajo
    public float subtleHeadTiltUpAngle = 5f;   // Ángulo de subida más sutil
    public float animationDuration = 1.5f; // Duración de cada parte de la animación
    public float waitTimeAtBottom = 2f; // Tiempo que la cámara permanecerá abajo
    public float waitTimeAfterSubtleUp = 3f; // Tiempo extra que la cámara se mantendrá abajo después de la subida
    public float objectFallDistance = 5f; // Distancia límite para destruir el objeto

    private Quaternion initialRotation;

    void Start()
    {
        // Almacena la rotación inicial de la cámara
        initialRotation = playerCamera.localRotation;

        // Añadir automáticamente todos los scripts que no estén ya en playerControlScripts
        AddMissingPlayerScripts();

        // Verifica o añade un AudioSource si no existe
        EnsureAudioSource();

        DisablePlayerControls();
        StartCoroutine(PlayCameraTiltAnimation());
    }

    void AddMissingPlayerScripts()
    {
        // Obtiene todos los MonoBehaviour del objeto Player
        var allPlayerScripts = playerCamera.GetComponentInParent<Transform>().GetComponentsInChildren<MonoBehaviour>();

        // Añade los que no estén ya en playerControlScripts
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

            // Si no tiene uno, lo añade
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    IEnumerator PlayCameraTiltAnimation()
    {
        // Inclinación hacia abajo
        yield return TiltCamera(headTiltDownAngle, animationDuration);

        // Mantiene la cámara inclinada hacia abajo por un tiempo
        yield return new WaitForSeconds(waitTimeAtBottom);

        // Sube un poco (sutilmente)
        yield return TiltCamera(subtleHeadTiltUpAngle, animationDuration);

        // Baja otra vez para leer bien
        yield return TiltCamera(headTiltDownAngle, animationDuration);

        // Mantiene la cámara inclinada hacia abajo por más tiempo después de la pequeña subida
        yield return new WaitForSeconds(waitTimeAfterSubtleUp);

        // Vuelve a la posición recta
        yield return TiltCamera(0f, animationDuration);

        // Reproduce sonido si se ha asignado
        if (soundEffect && audioSource)
        {
            audioSource.PlayOneShot(soundEffect);
        }

        // Inicia la animación del objeto hacia abajo
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

        float duration = 1f; // Duración de la animación
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            obj.localPosition = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            // Comprueba si el objeto ha alcanzado o superado la distancia límite
            if (Vector3.Distance(startPosition, obj.localPosition) >= objectFallDistance)
            {
                Destroy(obj.gameObject); // Destruye el objeto si está lo suficientemente lejos
                yield break;
            }

            yield return null;
        }

        obj.localPosition = targetPosition;
    }
}

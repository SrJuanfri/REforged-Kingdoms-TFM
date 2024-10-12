using System.Collections;
using UnityEngine;
using TMPro;

public class FinDelJuegoAnimation : MonoBehaviour
{
    public TMP_Text finDelJuegoText; // Referencia al componente TextMeshPro
    public float typingSpeed = 0.05f; // Velocidad de escritura
    public float scalingDuration = 1f; // Duración de la animación de escalado
    public float finalScale = 1.5f; // Escala final
    public Color startColor = new Color(1, 1, 1, 0); // Color inicial transparente
    public Color endColor = new Color(1, 1, 1, 1); // Color final (visible)

    private string mensaje = "Fin del Juego";
    private bool animationStarted = false; // Bool para saber si la animación ha empezado
    private bool animationFinished = false; // Bool para saber si la animación ha terminado

    void Start()
    {
        // Ocultar inicialmente el texto
        finDelJuegoText.text = "";
        finDelJuegoText.color = startColor;

        // Iniciar la animación de escritura
        //StartCoroutine(EscribirTexto());
    }

    // Método para comprobar si la animación ha comenzado
    public bool IsAnimationStarted()
    {
        return animationStarted;
    }

    // Método para comprobar si la animación ha terminado
    public bool IsAnimationFinished()
    {
        return animationFinished;
    }

    public IEnumerator EscribirTexto()
    {
        // Marcar la animación como empezada
        animationStarted = true;

        // Escribir letra por letra con efecto de máquina de escribir
        foreach (char letra in mensaje.ToCharArray())
        {
            finDelJuegoText.text += letra;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Una vez que el texto ha sido escrito, iniciar la animación de escalado
        StartCoroutine(AnimarEscalado());
    }

    IEnumerator AnimarEscalado()
    {
        // Obtener el tiempo inicial
        float elapsedTime = 0f;

        // Escalar gradualmente el texto
        while (elapsedTime < scalingDuration)
        {
            // Calcular el nuevo tamaño
            float scale = Mathf.Lerp(1f, finalScale, elapsedTime / scalingDuration);
            finDelJuegoText.transform.localScale = new Vector3(scale, scale, scale);

            // Cambiar el color del texto para hacerlo visible
            finDelJuegoText.color = Color.Lerp(startColor, endColor, elapsedTime / scalingDuration);

            // Incrementar el tiempo
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurarse de que el texto llega a la escala final y es completamente visible
        finDelJuegoText.transform.localScale = new Vector3(finalScale, finalScale, finalScale);
        finDelJuegoText.color = endColor;

        // Marcar la animación como terminada
        animationFinished = true;
    }
}

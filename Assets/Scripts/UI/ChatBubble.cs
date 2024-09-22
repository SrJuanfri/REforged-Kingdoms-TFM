using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
    public static ChatBubble Create(Transform parent, Vector3 localPosition, string text, float destroyTime = 6f)
    {
        Transform chatBubbleTransform = Instantiate(GameAssets.i.pfChatBubble, parent);
        chatBubbleTransform.localPosition = localPosition;

        // Configurar la burbuja de chat
        ChatBubble chatBubble = chatBubbleTransform.GetComponent<ChatBubble>();
        chatBubble.Setup(text);

        // Destruir la burbuja despu�s del tiempo especificado (o 6 segundos si no se pasa el valor)
        Destroy(chatBubbleTransform.gameObject, destroyTime);

        // Retornar la referencia al objeto ChatBubble
        return chatBubble;
    }

    // M�todo para crear una burbuja de chat con icono y un par�metro opcional para el tiempo de destrucci�n
    public static ChatBubble Create(Transform parent, Vector3 localPosition, IconType iconType, string text, float destroyTime = 6f)
    {
        Debug.Log("chat bubble");
        Transform chatBubbleTransform = Instantiate(GameAssets.i.pfChatBubble, parent);
        chatBubbleTransform.localPosition = localPosition;

        // Configurar la burbuja de chat con icono
        chatBubbleTransform.GetComponent<ChatBubble>().Setup(iconType, text);

        // Destruir la burbuja despu�s del tiempo especificado (o 6 segundos si no se pasa el valor)
        Destroy(chatBubbleTransform.gameObject, destroyTime);

        return chatBubbleTransform.GetComponent<ChatBubble>();
    }

    public enum IconType
    {
        MuyInsatisfecho,
        Insatisfecho,
        Neutral,
        Contento,
        MuyFeliz
    }

    [SerializeField] private Sprite muyInsatisfechoIconSprite;
    [SerializeField] private Sprite insatisfechoIconSprite;
    [SerializeField] private Sprite neutralIconSprite;
    [SerializeField] private Sprite contentoIconSprite;
    [SerializeField] private Sprite muyFelizIconSprite;

    private SpriteRenderer backgroundSpriteRenderer;
    private SpriteRenderer iconSpriteRenderer;
    private TextMeshPro textMeshPro;

    private void Awake()
    {
        backgroundSpriteRenderer = transform.Find("Background").GetComponent<SpriteRenderer>();
        iconSpriteRenderer = transform.Find("Icon").GetComponent<SpriteRenderer>();
        textMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
    }

    // Configuraci�n para burbuja con icono
    private void Setup(IconType iconType, string text)
    {
        text = InsertLineBreaks(text, 30); // Inserta saltos de l�nea si la frase es muy larga
        StartCoroutine(TypeText(text));

        Vector2 textSize = textMeshPro.GetRenderedValues(false);

        Vector2 padding = new Vector2(7f, 3f);
        backgroundSpriteRenderer.size = textSize + padding;

        Vector3 offset = new Vector2(-3f, 0f);
        backgroundSpriteRenderer.transform.localPosition = new Vector3(backgroundSpriteRenderer.size.x / 2f, 0f) + offset;

        iconSpriteRenderer.sprite = GetIconSprite(iconType);
        iconSpriteRenderer.gameObject.SetActive(true);
    }

    // Configuraci�n para burbuja sin icono
    private void Setup(string text)
    {
        text = InsertLineBreaks(text, 30); // Inserta saltos de l�nea si la frase es muy larga
        StartCoroutine(TypeText(text));  // Escribe el texto letra a letra

        Vector2 textSize = textMeshPro.GetRenderedValues(false);

        Vector2 padding = new Vector2(7f, 3f);
        backgroundSpriteRenderer.size = textSize + padding;

        Vector3 offset = new Vector2(-3f, 0f);
        backgroundSpriteRenderer.transform.localPosition = new Vector3(backgroundSpriteRenderer.size.x / 2f, 0f) + offset;

        // Desactiva el icono si no es necesario
        iconSpriteRenderer.gameObject.SetActive(false);
    }

    private IEnumerator TypeText(string text)
    {
        textMeshPro.text = "";

        // Forzar la actualizaci�n del mesh para asegurar que el tama�o se ajuste correctamente desde el inicio
        textMeshPro.ForceMeshUpdate();

        // Definir m�rgenes a la izquierda y derecha
        float leftMargin = 5f;  // Ajusta el margen izquierdo
        float rightMargin = 3f; // Ajusta el margen derecho

        Vector2 padding = new Vector2(leftMargin + rightMargin, 3f); // Ajustar el padding para ambos lados
        Vector2 initialSize = textMeshPro.GetRenderedValues(false);
        backgroundSpriteRenderer.size = initialSize + padding;

        // Ajustar la posici�n inicial del fondo para que comience m�s a la izquierda
        Vector3 initialPosition = backgroundSpriteRenderer.transform.localPosition;
        backgroundSpriteRenderer.transform.localPosition = new Vector3(initialPosition.x - leftMargin, initialPosition.y, initialPosition.z);

        foreach (char c in text)
        {
            textMeshPro.text += c;

            // Actualizar el tama�o del texto en cada iteraci�n
            textMeshPro.ForceMeshUpdate();
            Vector2 textSize = textMeshPro.GetRenderedValues(false);

            // Actualizar el tama�o del fondo seg�n el texto
            backgroundSpriteRenderer.size = textSize + padding;

            // Mantener el fondo alineado a la izquierda, con margen en ambos lados
            backgroundSpriteRenderer.transform.localPosition = new Vector3((backgroundSpriteRenderer.size.x / 2f) - leftMargin, initialPosition.y, initialPosition.z);

            yield return new WaitForSeconds(0.05f);  // Controla la velocidad del tipeo
        }
    }


    private Sprite GetIconSprite(IconType iconType)
    {
        switch (iconType)
        {
            default:
            case IconType.MuyInsatisfecho:
                return muyInsatisfechoIconSprite;
            case IconType.Insatisfecho:
                return insatisfechoIconSprite;
            case IconType.Neutral:
                return neutralIconSprite;
            case IconType.Contento:
                return contentoIconSprite;
            case IconType.MuyFeliz:
                return muyFelizIconSprite;
        }
    }

    // M�todo para insertar saltos de l�nea autom�ticos si la frase es muy larga
    private string InsertLineBreaks(string text, int maxCharactersPerLine)
    {
        string[] words = text.Split(' ');
        string result = "";
        int currentLineLength = 0;

        foreach (string word in words)
        {
            // Si a�adir la palabra excede el l�mite de caracteres por l�nea, a�adir un salto de l�nea
            if (currentLineLength + word.Length > maxCharactersPerLine)
            {
                result += "\n";
                currentLineLength = 0;
            }

            result += word + " ";
            currentLineLength += word.Length + 1; // +1 para el espacio
        }

        return result.TrimEnd(); // Eliminar el espacio final
    }
}

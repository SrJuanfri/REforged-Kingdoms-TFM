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

        // Destruir la burbuja después del tiempo especificado (o 6 segundos si no se pasa el valor)
        Destroy(chatBubbleTransform.gameObject, destroyTime);

        // Retornar la referencia al objeto ChatBubble
        return chatBubble;
    }

    // Método para crear una burbuja de chat con icono y un parámetro opcional para el tiempo de destrucción
    public static ChatBubble Create(Transform parent, Vector3 localPosition, IconType iconType, string text, float destroyTime = 6f)
    {
        Debug.Log("chat bubble");
        Transform chatBubbleTransform = Instantiate(GameAssets.i.pfChatBubble, parent);
        chatBubbleTransform.localPosition = localPosition;

        // Configurar la burbuja de chat con icono
        chatBubbleTransform.GetComponent<ChatBubble>().Setup(iconType, text);

        // Destruir la burbuja después del tiempo especificado (o 6 segundos si no se pasa el valor)
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

    // Configuración para burbuja con icono
    private void Setup(IconType iconType, string text)
    {
        text = InsertLineBreaks(text, 30); // Inserta saltos de línea si la frase es muy larga
        textMeshPro.SetText(text);
        textMeshPro.ForceMeshUpdate();
        Vector2 textSize = textMeshPro.GetRenderedValues(false);

        Vector2 padding = new Vector2(7f, 3f);
        backgroundSpriteRenderer.size = textSize + padding;

        Vector3 offset = new Vector2(-3f, 0f);
        backgroundSpriteRenderer.transform.localPosition = new Vector3(backgroundSpriteRenderer.size.x / 2f, 0f) + offset;

        iconSpriteRenderer.sprite = GetIconSprite(iconType);
        iconSpriteRenderer.gameObject.SetActive(true);
    }

    // Configuración para burbuja sin icono
    private void Setup(string text)
    {
        text = InsertLineBreaks(text, 30); // Inserta saltos de línea si la frase es muy larga
        textMeshPro.SetText(text);
        textMeshPro.ForceMeshUpdate();
        Vector2 textSize = textMeshPro.GetRenderedValues(false);

        Vector2 padding = new Vector2(7f, 3f);
        backgroundSpriteRenderer.size = textSize + padding;

        Vector3 offset = new Vector2(-3f, 0f);
        backgroundSpriteRenderer.transform.localPosition = new Vector3(backgroundSpriteRenderer.size.x / 2f, 0f) + offset;

        // Desactiva el icono si no es necesario
        iconSpriteRenderer.gameObject.SetActive(false);
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

    // Método para insertar saltos de línea automáticos si la frase es muy larga
    private string InsertLineBreaks(string text, int maxCharactersPerLine)
    {
        string[] words = text.Split(' ');
        string result = "";
        int currentLineLength = 0;

        foreach (string word in words)
        {
            // Si añadir la palabra excede el límite de caracteres por línea, añadir un salto de línea
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

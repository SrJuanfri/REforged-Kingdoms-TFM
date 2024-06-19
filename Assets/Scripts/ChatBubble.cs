using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
    public static void Create(Transform parent, Vector3 localPosition, IconType iconType, string text)
    {
        Transform chatBubbleTransform = Instantiate(GameAssets.i.pfChatBubble, parent);
        chatBubbleTransform.localPosition = localPosition;
        
        chatBubbleTransform.GetComponent<ChatBubble>().Setup(iconType, text);
        
        Destroy(chatBubbleTransform.gameObject, 6f);
    }
    
    public enum IconType
    {
        Happy,
        Neutral,
        Sad,
    }

    [SerializeField] private Sprite sadIconSprite;
    [SerializeField] private Sprite happyIconSprite;
    [SerializeField] private Sprite neutralIconSprite;
    private SpriteRenderer backgroundSpriteRenderer;
    private SpriteRenderer iconSpriteRenderer;
    private TextMeshPro TextMeshPro;
    private void Awake()
    {
        backgroundSpriteRenderer = transform.Find("Background").GetComponent<SpriteRenderer>();
        iconSpriteRenderer = transform.Find("Icon").GetComponent<SpriteRenderer>();
        TextMeshPro = transform.Find("Text").GetComponent<TextMeshPro>();
    }
    
    private void Setup(IconType iconType, string text)
    {
        TextMeshPro.SetText(text);
        TextMeshPro.ForceMeshUpdate();
        Vector2 textSize = TextMeshPro.GetRenderedValues(false);
        
        Vector2 padding = new Vector2(7f, 3f);
        backgroundSpriteRenderer.size = textSize + padding;

        Vector3 offset = new Vector2(-3f, 0f);
        backgroundSpriteRenderer.transform.localPosition = new Vector3(backgroundSpriteRenderer.size.x / 2f, 0f) + offset;

        iconSpriteRenderer.sprite = GetIconSprite(iconType);
    }

    private Sprite GetIconSprite(IconType iconType)
    {
        switch (iconType)
        {
            default:
            case IconType.Happy:
                return happyIconSprite;
            case IconType.Neutral:
                return neutralIconSprite;
            case IconType.Sad:
                return sadIconSprite;
        }
    }
    
}

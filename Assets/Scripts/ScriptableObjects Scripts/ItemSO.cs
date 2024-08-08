using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ItemSO : ScriptableObject
{
    public enum ItemType
    {
        Metal,
        Wood,
        Other
    }

    public string itemName;
    public GameObject inputItem;
    public Transform prefab;
    public int value;
    public ItemType itemType;  // Categoría del ítem
}

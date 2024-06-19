using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public GameObject inputItem;
    public Transform prefab;
    public int value;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CraftingRecipeSO : ScriptableObject 
{
    public Sprite sprite;
    public List<ItemSO> inputItemSOList;
    public ItemSO outputItemSO;
}

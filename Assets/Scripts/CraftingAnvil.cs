using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingAnvil : MonoBehaviour
{
    /*
    [SerializeField] private BoxCollider placeItemsAreaCollider;

    private CraftingRecipeSO craftingRecipeSO;

    public void Craft()
    {
        Debug.Log("Craft");

        Collider[] colliderArray = Physics.OverlapBox(transform.position + placeItemsAreaCollider.center,
            placeItemsAreaCollider.size, placeItemsAreaCollider.transform.rotation);

        List<ItemSO> inputItemList = new List<ItemSO>(craftingRecipeSO.inputItemSOList);
        
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out ItemSOHolder itemSOHolder))
            {
                inputItemList.Remove(itemSOHolder.itemSO);
            }
        }

        if (inputItemList.Count == 0)
        {
            //Tenemos todos los items requeridos 
            Debug.Log("Yes");
        }
        else
        {
            Debug.Log("No");
        }
    }
    */
}

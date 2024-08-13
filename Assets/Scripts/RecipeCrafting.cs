using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RecipeCrafting : Interactable
{
    public Image craftImage;
    [SerializeField] private BoxCollider placeItemsAreaCollider;
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private Transform vfxSpawnItem;
    [HideInInspector] public CraftingRecipeSO craftingRecipeSO;

    public void Craft()
    {
        Debug.Log("Craft");

        Collider[] colliderArray = Physics.OverlapBox(transform.position + placeItemsAreaCollider.center,
            placeItemsAreaCollider.size, placeItemsAreaCollider.transform.rotation);

        List<ItemSO> inputItemList = new List<ItemSO>(craftingRecipeSO.inputItemSOList);

        List<GameObject> consumeItemGameObjectList = new List<GameObject>();

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out ItemSOHolder itemSOHolder))
            {
                if (inputItemList.Contains(itemSOHolder.itemSO))
                {
                    inputItemList.Remove(itemSOHolder.itemSO);
                    consumeItemGameObjectList.Add(collider.gameObject);
                }
            }
        }

        if (inputItemList.Count == 0)
        {
            //Tenemos todos los items requeridos 

            Debug.Log("Yes");

            Transform spawnedItemTransform = Instantiate(craftingRecipeSO.outputItemSO.prefab, itemSpawnPoint.position,
                itemSpawnPoint.rotation);

            Instantiate(vfxSpawnItem, itemSpawnPoint.position, itemSpawnPoint.rotation);

            foreach(GameObject consumeItemGameObject in consumeItemGameObjectList)
            {
                Destroy(consumeItemGameObject);
            }
        }
        
    }


}

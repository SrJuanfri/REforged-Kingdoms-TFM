using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] private BoxCollider placeItemsAreaCollider;
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private CraftingRecipeSO craftingRecipeSO;
    
    private Animator animator;
    public string sellText;
    public string endText;
    public ChatBubble.IconType emotion;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    public void Interact()
    {
        ChatBubble.Create(transform.transform, new Vector3(-0.6f, 1.7f, 0f), emotion,
            sellText);
        
        animator.SetTrigger("Talk");
    }

    public void Sell()
    {
        Debug.Log("Sell");

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

            Transform spawnedItemTransform = Instantiate(craftingRecipeSO.outputItemSO.prefab, itemSpawnPoint.position, itemSpawnPoint.rotation);

            foreach(GameObject consumeItemGameObject in consumeItemGameObjectList)
            {
                Destroy(consumeItemGameObject);
            }
            
            ChatBubble.Create(transform.transform, new Vector3(-0.6f, 1.7f, 0f), emotion,
                endText);
        }
    }
}

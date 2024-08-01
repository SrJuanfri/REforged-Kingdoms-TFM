using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : Interactable
{
    [SerializeField] private Registradora registradora;
    [SerializeField] private ItemSO itemSO;
    [SerializeField] private Transform itemSpawnPoint;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateItem()
    {
        if (registradora.moneyNumber >= itemSO.value)
        {
            Transform spawnedItemTransform = Instantiate(itemSO.prefab, itemSpawnPoint.position, itemSpawnPoint.rotation);
            registradora.SubstractNumberText(itemSO.value);
        }
    }
}

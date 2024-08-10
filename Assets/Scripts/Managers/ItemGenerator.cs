using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemGenerator : Interactable
{
    [SerializeField] private Registradora registradora;
    [SerializeField] private ItemSO itemSO;
    [SerializeField] private Transform itemSpawnPoint;


    public void CreateItem()
    {
        if (registradora.moneyNumber >= itemSO.value)
        {
            Transform itemCreation = Instantiate(itemSO.prefab, itemSpawnPoint.position, itemSpawnPoint.rotation);
            registradora.countText.text = (registradora.moneyNumber - itemSO.value).ToString();
            registradora.moneyNumber -= itemSO.value;
        }
    }
}

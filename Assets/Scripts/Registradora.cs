using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Registradora : MonoBehaviour
{
    [SerializeField] private BoxCollider storageItemsAreaCollider;
    [SerializeField] private TextMeshPro countText;
    [SerializeField] private List<ItemSO> listItemStoragedSO;
    public int moneyNumber;
    void Start()
    {
        UpdateNumberText();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ItemSOHolder itemSOHolder))
        {
            //Debug.Log("Dentro");
            
            for (int i = 0; i <= listItemStoragedSO.Count; i++)
            {
                if (itemSOHolder.itemSO == listItemStoragedSO[i])
                {
                    //Debug.Log(listItemStoragedSO[i].itemName);
                    
                    moneyNumber += listItemStoragedSO[i].value;
                    UpdateNumberText();
                    Destroy(other.GameObject());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    void UpdateNumberText()
    {
        countText.text = moneyNumber.ToString();
    }

    public void SubstractNumberText(int valueItem)
    {
        countText.text = (moneyNumber - valueItem).ToString();
    }
}

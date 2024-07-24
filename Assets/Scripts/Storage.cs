using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] private BoxCollider storageItemsAreaCollider;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private ItemSO itemStoragedSO;
    private int objectsNumber;

    private ItemSOHolder itemSOHolder;

    void Start()
    {
        UpdateNumberText();

        nameText.text = itemStoragedSO.itemName;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ItemSOHolder itemSOHolder))
        {
            if (itemSOHolder.itemSO == itemStoragedSO)
            {
                objectsNumber += 1;
                UpdateNumberText();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out ItemSOHolder itemSOHolder))
        {
            if (itemSOHolder.itemSO == itemStoragedSO)
            {
                objectsNumber -= 1;
                UpdateNumberText();
            }
        }
    }

    void UpdateNumberText()
    {
        countText.text = "Número: " + objectsNumber;
    }
}

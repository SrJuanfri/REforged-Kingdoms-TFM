using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Registradora : MonoBehaviour
{
    [SerializeField] private BoxCollider storageItemsAreaCollider;
    public TextMeshPro countText;
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
            // Debug.Log("Dentro");

            // Cambiado a i < listItemStoragedSO.Count
            for (int i = 0; i < listItemStoragedSO.Count; i++)
            {
                if (itemSOHolder.itemSO == listItemStoragedSO[i])
                {
                    // Debug.Log(listItemStoragedSO[i].itemName);

                    moneyNumber += listItemStoragedSO[i].value;
                    UpdateNumberText();
                    Destroy(other.gameObject);  // Cambiado a gameObject en lugar de GameObject()
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // No implementado aún
    }

    void UpdateNumberText()
    {
        countText.text = moneyNumber.ToString();
    }
}

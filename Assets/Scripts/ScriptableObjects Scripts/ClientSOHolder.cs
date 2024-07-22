using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class ClientSOHolder : MonoBehaviour
{
    public CustomerManager clientSO;

    private void Start()
    {
        clientSO.Start();

        Interaction interaction = gameObject.GetComponent<Interaction>();
        interaction.sellText = clientSO.currentOrder.orderText;
    }
}


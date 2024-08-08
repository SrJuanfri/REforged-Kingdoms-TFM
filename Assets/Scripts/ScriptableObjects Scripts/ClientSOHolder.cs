using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSOHolder : MonoBehaviour
{
    [SerializeField] private CustomerManager clientSO;

    public CustomerManager ClientSO => clientSO;

    private void Start()
    {
        clientSO.Start();

        Interaction interaction = GetComponent<Interaction>();
        if (clientSO.currentOrder != null)
        {
            interaction.craftingRecipeSO = clientSO.currentOrder.CraftingRecipe;
            interaction.SetCustomerState(clientSO.currentOrder.CustomerState);
        }
    }
}

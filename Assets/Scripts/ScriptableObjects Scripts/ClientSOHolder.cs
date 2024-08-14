using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSOHolder : MonoBehaviour
{
    [SerializeField] private CustomerManager clientSO;

    // Propiedad pública de solo lectura para obtener el CustomerManager
    public CustomerManager ClientSO => clientSO;

    private void Start()
    {
        // Verificar si clientSO está asignado
        if (clientSO == null)
        {
            Debug.LogError("ClientSO no está asignado en " + gameObject.name);
            return;
        }

        // Llamar a Start() en clientSO solo si clientSO no es null
        clientSO.Start();

        // Intentar obtener el componente CustomerController
        CustomerController customerController = GetComponent<CustomerController>();
        if (customerController == null)
        {
            Debug.LogError("No se encontró el componente CustomerController en " + gameObject.name);
            return;
        }

        // Verificar si hay una orden actual antes de usarla
        if (clientSO.currentOrder != null)
        {
            // Configurar el CustomerController con la información de la orden actual
            customerController.craftingRecipeSO = clientSO.currentOrder.CraftingRecipe;

            // Obtener y establecer el estado del cliente usando CustomerStateHandler
            CustomerStateHandler stateHandler = GetComponent<CustomerStateHandler>();
            if (stateHandler != null)
            {
                stateHandler.SetCustomerState(clientSO.currentOrder.CustomerState);
            }
            else
            {
                Debug.LogError("CustomerStateHandler no se encontró en " + gameObject.name);
            }
        }
        else
        {
            Debug.LogWarning("No hay una orden actual en ClientSO.");
        }
    }
}

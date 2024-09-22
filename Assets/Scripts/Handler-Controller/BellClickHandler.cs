using UnityEngine;

public class BellClickHandler : Interactable
{
    private MerchantController merchantController;
    private CustomerController[] customerControllers;

    void Start()
    {
        // Buscar el controlador del mercader
        merchantController = FindObjectOfType<MerchantController>();

        // Buscar todos los controladores de clientes
        customerControllers = FindObjectsOfType<CustomerController>();
    }

    public void OnBellClicked()
    {
        Debug.Log("Bell clicked. Merchant or customer leaving.");

        if (merchantController != null && merchantController.currentState == MerchantController.State.ShowProducts)
        {
            Debug.Log("Bell clicked. Merchant leaving.");
            merchantController.Leave();
        }

        foreach (var customerController in customerControllers)
        {
            if (customerController != null && customerController.currentState == CustomerController.State.Shopping)
            {
                customerController.Leave();
                break; // Solo un cliente a la vez
            }
        }
    }
}

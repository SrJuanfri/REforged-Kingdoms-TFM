using UnityEngine;
using System.Collections;

public class DayCycleManager : MonoBehaviour
{
    public CustomerSelector customerSelector;
    public float customerShoppingDuration = 180f; // Duración de la fase de clientes en segundos, ajustable desde el Inspector
    private float dayTimer;

    private enum DayPhase
    {
        MerchantArrival,
        CustomerShopping,
        DayTransition
    }

    private DayPhase currentPhase;
    private MerchantController merchantController;

    private void Start()
    {
        merchantController = FindObjectOfType<MerchantController>();
        merchantController.OnMerchantLeft += StartCustomerShoppingPhase;

        currentPhase = DayPhase.MerchantArrival;
        StartCoroutine(DayCycleRoutine());
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator DayCycleRoutine()
    {
        while (true)
        {
            switch (currentPhase)
            {
                case DayPhase.MerchantArrival:
                    yield return StartCoroutine(MerchantArrivalPhase());
                    break;

                case DayPhase.CustomerShopping:
                    yield return StartCoroutine(CustomerShoppingPhase());
                    break;

                case DayPhase.DayTransition:
                    yield return StartCoroutine(DayTransitionPhase());
                    break;
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator MerchantArrivalPhase()
    {
        Debug.Log("Merchant Arrival Phase started.");
        // El mercader llega y muestra productos
        // No se necesita esperar aquí, ya que el mercader permanecerá hasta que se pulse la campana
        while (currentPhase == DayPhase.MerchantArrival)
        {
            yield return null;
        }
        Debug.Log("Merchant Arrival Phase ended.");
    }

    private void StartCustomerShoppingPhase()
    {
        currentPhase = DayPhase.CustomerShopping;
        StartCoroutine(CustomerShoppingPhase());
    }

    private IEnumerator CustomerShoppingPhase()
    {
        Debug.Log("Customer Shopping Phase started.");
        dayTimer = customerShoppingDuration; // Usar la duración ajustable desde el Inspector

        while (dayTimer > 0)
        {
            if (!customerSelector.selectedCustomer)
            {
                SelectAndSendCustomer();
            }
           
            yield return new WaitUntil(() => customerSelector.selectedCustomer.GetComponent<CustomerController>().currentState == CustomerController.State.Shopping);
            yield return new WaitUntil(() => customerSelector.selectedCustomer.GetComponent<CustomerController>().currentState == CustomerController.State.ReturnToStart);
            yield return new WaitUntil(() => customerSelector.selectedCustomer.GetComponent<CustomerController>().currentState == CustomerController.State.Idle);
            dayTimer -= Time.deltaTime;
        }

        currentPhase = DayPhase.DayTransition;
        Debug.Log("Customer Shopping Phase ended.");
    }

    private IEnumerator DayTransitionPhase()
    {
        Debug.Log("Day Transition Phase started.");

        // Aquí puedes añadir la lógica para manejar la transición al siguiente día
        yield return new WaitForSeconds(1.0f); // Simular tiempo de transición

        currentPhase = DayPhase.MerchantArrival;
        Debug.Log("Day Transition Phase ended.");
    }

    private void SelectAndSendCustomer()
    {
        customerSelector.SelectRandomCustomer();
        GameObject selectedCustomer = customerSelector.selectedCustomer;
        if (selectedCustomer != null)
        {
            CustomerController customerController = selectedCustomer.GetComponent<CustomerController>();
            if (customerController != null)
            {
                customerController.shouldGoToShop = true;
                customerController.shouldReturnToStart = false; // Asegurarnos de que no se vayan automáticamente
            }
        }
    }
}

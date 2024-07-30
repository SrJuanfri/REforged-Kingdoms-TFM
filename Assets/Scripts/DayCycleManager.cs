using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using TMPro;

public class DayCycleManager : MonoBehaviour
{
    public CustomerSelector customerSelector;
    public float customerShoppingDuration = 180f; // Duración de la fase de clientes en segundos, ajustable desde el Inspector
    public float transitionTime = 2f;
    public GameObject transitionCanvas;
    public GameObject transitionTextDay;
    private float dayTimer;
    private DayNightController dayNightController;
    private int dayNumber = 0;

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

        dayTimer = 0;

        transitionTextDay.SetActive(false);

        merchantController = FindObjectOfType<MerchantController>();
        merchantController.OnMerchantLeft += StartCustomerShoppingPhase;

        dayNightController = GetComponent<DayNightController>();
        dayNightController.SecondsInFullDay = (int)customerShoppingDuration;

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
    private IEnumerator MerchantArrivalPhase()
    {
        Debug.Log("Merchant Arrival Phase started.");
        dayNightController.stop = false;
        merchantController.setStateGoToShop();
        customerSelector.selectedCustomer = null;
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
        if (!dayNightController.dayStarted)
        {
            dayNightController.StartDayCycle();
        }

        StartCoroutine(CustomerShoppingPhase());
    }

    private IEnumerator CustomerShoppingPhase()
    {
        Debug.Log("Customer Shopping Phase started.");
        dayTimer = customerShoppingDuration; // Usar la duración ajustable desde el Inspector

        while (dayTimer > 0)
        {
            if (!customerSelector.selectedCustomer /*|| customerSelector.selectedCustomer.GetComponent<CustomerController>().currentState == CustomerController.State.Idle*/)
            {
                SelectAndSendCustomer();
            }
            dayTimer -= Time.deltaTime;

            // Llamar a la función para comprobar si el cliente se está yendo
            CheckIfCustomerLeaving();

            yield return null; // Continuar en el siguiente frame
        }

        // Esperar a que todos los clientes hayan terminado antes de transicionar
        yield return StartCoroutine(WaitForCustomersToFinish());

        currentPhase = DayPhase.DayTransition;
        Debug.Log("Customer Shopping Phase ended.");
    }

    private void CheckIfCustomerLeaving()
    {
        if (customerSelector.selectedCustomer != null)
        {
            CustomerController customerController = customerSelector.selectedCustomer.GetComponent<CustomerController>();
            if (customerController.currentState == CustomerController.State.ReturnToStart)
            {
                customerSelector.selectedCustomer = null;
            }
        }
    }
    private IEnumerator WaitForCustomersToFinish()
    {
        while (true)
        {
            bool allCustomersIdle = true;
            foreach (var customer in FindObjectsOfType<CustomerController>())
            {
                if (customer.currentState != CustomerController.State.Idle)
                {
                    allCustomersIdle = false;
                    break;
                }
            }

            if (allCustomersIdle)
            {
                yield break; // Todos los clientes han terminado, salir de la corrutina
            }

            yield return null; // Esperar al siguiente frame
        }
    }

    private IEnumerator DayTransitionPhase()
    {
        Debug.Log("Day Transition Phase started.");

        // Aquí puedes añadir la lógica para manejar la transición al siguiente día
        dayNumber++;
        transitionTextDay.SetActive(true);
        transitionTextDay.GetComponent<TextMeshProUGUI>().text = "Día " + dayNumber.ToString();
        GameObject Fade = Initiate.Fade(Color.black, 1.0f);
        Fade.transform.parent = transitionCanvas.transform;
        Fade.transform.SetSiblingIndex(0);
        dayNightController.stop = true;
        yield return new WaitForSeconds(transitionTime); // Simular tiempo de transición

        Initiate.DoneFading();
        Debug.Log("Day Transition Phase ended.");
        Fade.GetComponent<Fader>().OnLevelFinishedLoading();
        transitionTextDay.SetActive(false);
        dayTimer = 0;
        currentPhase = DayPhase.MerchantArrival;
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

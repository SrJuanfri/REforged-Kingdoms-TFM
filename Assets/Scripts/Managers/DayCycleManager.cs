using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    private List<GameObject> selectedCustomersToday = new List<GameObject>();

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
        dayTimer = customerShoppingDuration;

        while (dayTimer > 0)
        {
            if (!customerSelector.selectedCustomer)
            {
                SelectAndSendCustomer();
            }
            dayTimer -= Time.deltaTime;

            CheckIfCustomerLeaving();

            yield return null;
        }

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
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator DayTransitionPhase()
    {
        Debug.Log("Day Transition Phase started.");

        dayNumber++;
        transitionTextDay.SetActive(true);
        transitionTextDay.GetComponent<TextMeshProUGUI>().text = "Día " + dayNumber.ToString();
        GameObject fade = Initiate.Fade(Color.black, 1.0f);
        fade.transform.parent = transitionCanvas.transform;
        fade.transform.SetSiblingIndex(0);

        // Detener el ciclo de día y noche al final del día
        dayNightController.stop = true;

        yield return new WaitForSeconds(transitionTime);

        // Reiniciar el ciclo de día y noche
        dayNightController.ResetDayCycle();

        Initiate.DoneFading();
        Debug.Log("Day Transition Phase ended.");
        fade.GetComponent<Fader>().OnLevelFinishedLoading();
        transitionTextDay.SetActive(false);

        dayTimer = 0;
        currentPhase = DayPhase.MerchantArrival;

        // Reiniciar la lista de clientes seleccionados para el nuevo día
        selectedCustomersToday.Clear();
    }

    private void SelectAndSendCustomer()
    {
        customerSelector.SelectRandomCustomer();

        while (selectedCustomersToday.Contains(customerSelector.selectedCustomer) && customerSelector.customers.Count > selectedCustomersToday.Count)
        {
            customerSelector.SelectRandomCustomer();
        }

        GameObject selectedCustomer = customerSelector.selectedCustomer;
        if (selectedCustomer != null)
        {
            CustomerController customerController = selectedCustomer.GetComponent<CustomerController>();
            if (customerController != null)
            {
                customerController.shouldGoToShop = true;
                customerController.shouldReturnToStart = false;

                selectedCustomersToday.Add(selectedCustomer);
            }
        }
    }
}

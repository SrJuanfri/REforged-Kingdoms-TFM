using UnityEngine;
using System.Collections;

public class DayCycleManager : MonoBehaviour
{
    public CustomerSelector customerSelector;
    public float dayDuration = 180f; // Duración del día en segundos (3 minutos)
    private float dayTimer;

    private void Start()
    {
        dayTimer = dayDuration;
        StartCoroutine(DayCycleRoutine());
    }

    private IEnumerator DayCycleRoutine()
    {
        while (dayTimer > 0)
        {
            SelectAndSendCustomer();
            // Esperar hasta que el cliente vuelva a su posición inicial
            yield return new WaitUntil(() => customerSelector.selectedCustomer.GetComponent<CustomerController>().currentState == CustomerController.State.Idle);
            yield return null; // Asegura que se detecte correctamente el cambio de estado
        }

        Debug.Log("Day ended.");
    }

    private void Update()
    {
        if (dayTimer > 0)
        {
            dayTimer -= Time.deltaTime;
        }
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
                customerController.shouldReturnToStart = true;
            }
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CustomerSelector : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> customers;
    [HideInInspector]
    public GameObject selectedCustomer;

    [SerializeField] private float decreasePercentage = 10f; // Porcentaje de reducción para el cliente seleccionado
    [SerializeField] private float increasePercentage = 10f; // Porcentaje de aumento para los demás clientes

    // Selecciona un cliente aleatorio basado en las probabilidades de aparición
    public void SelectRandomCustomer()
    {
        // Obtener todos los objetos con la etiqueta "Customer"
        customers = GameObject.FindGameObjectsWithTag("Customer")
            .Where(obj => obj.layer == LayerMask.NameToLayer("Customer"))
            .ToList();

        if (customers.Count == 0)
        {
            Debug.LogWarning("No customers found on the Customer layer.");
            return;
        }

        // Crear una lista de probabilidades de aparición como porcentajes
        List<float> appearanceProbabilities = new List<float>();

        foreach (GameObject customer in customers)
        {
            ClientSOHolder holder = customer.GetComponent<ClientSOHolder>();
            if (holder != null && holder.ClientSO != null)
            {
                appearanceProbabilities.Add(holder.ClientSO.AppearanceProbability);
            }
            else
            {
                appearanceProbabilities.Add(0); // Si no hay CustomerManager, la probabilidad es 0
            }
        }

        // Elegir un cliente aleatorio basado en las probabilidades
        float totalProbability = appearanceProbabilities.Sum();
        if (totalProbability == 0)
        {
            Debug.LogWarning("Total appearance probability is zero. No customers can be selected.");
            return;
        }

        float randomPoint = Random.value * totalProbability;

        for (int i = 0; i < customers.Count; i++)
        {
            if (randomPoint < appearanceProbabilities[i])
            {
                selectedCustomer = customers[i];
                Debug.Log("Selected customer: " + selectedCustomer.name);

                // Reducir la probabilidad del cliente seleccionado
                AdjustProbabilities(i);

                return;
            }
            else
            {
                randomPoint -= appearanceProbabilities[i];
            }
        }
    }
    private void AdjustProbabilities(int selectedIndex)
    {
        for (int i = 0; i < customers.Count; i++)
        {
            ClientSOHolder holder = customers[i].GetComponent<ClientSOHolder>();

            if (holder != null && holder.ClientSO != null)
            {
                if (i == selectedIndex)
                {
                    // Reducir la probabilidad del cliente seleccionado
                    float newProbability = holder.ClientSO.AppearanceProbability * (1 - decreasePercentage / 100);
                    holder.ClientSO.SetAppearanceProbability(newProbability);
                }
                else
                {
                    // Aumentar la probabilidad de los otros clientes
                    float newProbability = holder.ClientSO.AppearanceProbability * (1 + increasePercentage / 100);
                    holder.ClientSO.SetAppearanceProbability(newProbability);
                }
            }
        }
    }

}

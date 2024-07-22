using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CustomerSelector : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> customers;
    [HideInInspector]
    public GameObject selectedCustomer;

    void Start()
    {
        SelectRandomCustomer();
    }

    public void SelectRandomCustomer()
    {
        // Recoger todos los objetos de la capa Customer
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
            if (holder != null && holder.clientSO != null)
            {
                appearanceProbabilities.Add(holder.clientSO.appearanceProbability);
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
                return;
            }
            else
            {
                randomPoint -= appearanceProbabilities[i];
            }
        }
    }
}

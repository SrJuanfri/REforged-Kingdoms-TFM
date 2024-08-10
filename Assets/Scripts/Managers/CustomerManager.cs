using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

[CreateAssetMenu(fileName = "CustomerData", menuName = "ScriptableObjects/CustomerData", order = 1)]
public class CustomerManager : ScriptableObject
{
    [SerializeField] private TextMeshProUGUI price;
    [SerializeField] private List<OrderData> ordersData = new List<OrderData>();
    [HideInInspector] public OrderData currentOrder;
    [SerializeField] private float appearanceProbability = 100;

    // Propiedad para acceder a appearanceProbability
    public float AppearanceProbability => appearanceProbability;

    public void Start()
    {
        // Obtener la �ltima orden no completada
        currentOrder = GetLastUncompletedOrder();

        if (currentOrder != null && price != null)
        {
            // Actualizar los campos de texto con los datos de la orden actual
            price.text = currentOrder.Price.ToString();
        }
    }

    // Metodo para verificar si todos los pedidos del cliente est�n completados
    public bool AllOrdersCompleted()
    {
        // Usamos LINQ para verificar si todos los pedidos est�n completados
        return ordersData.All(order => order.IsCompleted);
    }

    // Método para obtener la �ltima orden no completada
    private OrderData GetLastUncompletedOrder()
    {
        for (int i = ordersData.Count - 1; i >= 0; i--)
        {
            if (!ordersData[i].IsCompleted)
            {
                return ordersData[i];
            }
        }
        return null; // Retorna null si no se encuentra ninguna orden no completada
    }
}

[System.Serializable]
public class OrderData
{
    public string orderText;
    public CraftingRecipeSO craftingRecipe;
    public CustomerState customerState;
    public bool isCompleted;
    public int price;

    // Propiedad para acceder a isCompleted
    public bool IsCompleted
    {
        get => isCompleted;
        set => isCompleted = value;
    }

    // Propiedad para acceder a OrderText
    public string OrderText
    {
        get => orderText;
        set => orderText = value;
    }

    // Propiedad para acceder a CraftingRecipe
    public CraftingRecipeSO CraftingRecipe
    {
        get => craftingRecipe;
        set => craftingRecipe = value;
    }

    // Propiedad para acceder a CustomerState
    public CustomerState CustomerState
    {
        get => customerState;
        set => customerState = value;
    }

    // Propiedad para acceder al precio
    public int Price
    {
        get => price;
        set => price = value;
    }
}

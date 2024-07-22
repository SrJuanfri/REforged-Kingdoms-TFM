using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.Animations;
using UnityEngine.UI;
using System.Linq;

[CreateAssetMenu(fileName = "CustomerData", menuName = "ScriptableObjects/CustomerData", order = 1)]
public class CustomerManager : ScriptableObject
{
    public TextMeshProUGUI price;

    public List<OrderData> ordersData = new List<OrderData>();
    private List<Order> orders = new List<Order>();
    [HideInInspector] public Order currentOrder;

    public float appearanceProbability = 100;

    public void Start()
    {
        // Convertir los datos de las órdenes a objetos de tipo Order
        orders = ConvertToOrders();

        // Obtener la última orden no completada
        currentOrder = GetLastUncompletedOrder();

        if (currentOrder != null && price != null)
        {
            // Actualizar los campos de texto con los datos de la orden actual
            price.text = currentOrder.GetPrice().ToString();
        }
    }

    // Método para verificar si todos los pedidos del cliente están completados
    public bool AllOrdersCompleted()
    {
        // Usamos LINQ para verificar si todos los pedidos están completados
        return orders.All(order => order.GetIsCompleted());
    }

    // Método para obtener la última orden no completada
    private Order GetLastUncompletedOrder()
    {
        for (int i = orders.Count - 1; i >= 0; i--)
        {
            if (!orders[i].GetIsCompleted())
            {
                return orders[i];
            }
        }
        return null; // Retorna null si no se encuentra ninguna orden no completada
    }

    public List<Order> ConvertToOrders()
    {
        List<Order> orders = new List<Order>();
        foreach (OrderData orderData in ordersData)
        {
            orders.Add(Order.FromOrderData(orderData));
        }
        return orders;
    }

    // Método para convertir de Order a OrderData
    public List<OrderData> ConvertToOrderData(List<Order> orders)
    {
        List<OrderData> ordersData = new List<OrderData>();
        foreach (Order order in orders)
        {
            ordersData.Add(order.ToOrderData());
        }
        return ordersData;
    }
}

[System.Serializable]
public class OrderData
{
    public string orderText;
    public Order.Design design;
    public Order.BladeMaterial bladeMaterial;
    public Order.HandleMaterial handleMaterial;
    public bool isCompleted;
    public int price;
    public bool isWeapon;
}

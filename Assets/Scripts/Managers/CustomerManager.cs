using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

[CreateAssetMenu(fileName = "CustomerData", menuName = "ScriptableObjects/CustomerData", order = 1)]
public class CustomerManager : ScriptableObject
{
    [SerializeField] private TextMeshProUGUI price;

    // Lista de órdenes
    public List<OrderData> ordersData = new List<OrderData>();

    [HideInInspector] public OrderData currentOrder;
    [SerializeField] private float appearanceProbability = 100;

    // Booleano que indica si el cliente tiene un evento especial activo
    [SerializeField] private bool hasEvent = false;

    // Lista de órdenes que activan eventos, cada una con un índice y una frase especial
    [SerializeField] public List<EventOrder> eventOrders = new List<EventOrder>();  // Lista de pares (índice, frase de evento)

    // Evento activo para este cliente
    [SerializeField]
    private string activeEvent;

    public float AppearanceProbability => appearanceProbability;

    public void Start()
    {
        currentOrder = GetLastUncompletedOrder();

        if (currentOrder != null && price != null)
        {
            price.text = currentOrder.Price.ToString();
        }

        // Solo verificar eventos si hasEvent es true
        if (hasEvent)
        {
            CheckForEventActivation();
        }
    }

    // Método para cambiar la probabilidad
    public void SetAppearanceProbability(float newProbability)
    {
        appearanceProbability = Mathf.Clamp(newProbability, 0, 100);  // Limita la probabilidad entre 0 y 100
    }

    public bool AllOrdersCompleted()
    {
        return ordersData.All(order => order.IsCompleted);
    }

    private OrderData GetLastUncompletedOrder()
    {
        // Recorre la lista de pedidos desde el principio hacia el final
        for (int i = 0; i < ordersData.Count; i++)
        {
            // Si el pedido no está completado, lo devuelve
            if (!ordersData[i].IsCompleted)
            {
                return ordersData[i];
            }
        }
        // Si no encuentra ningún pedido sin completar, devuelve null
        return null;
    }


    // Método para comprobar si la orden actual activa un evento
    private void CheckForEventActivation()
    {
        if (!hasEvent || eventOrders == null) return;  // No hay eventos si hasEvent es false o eventOrders es null

        int currentOrderIndex = ordersData.IndexOf(currentOrder);

        // Verificar si hay un evento para el índice actual
        var eventOrder = eventOrders.Find(e => e.orderIndex == currentOrderIndex);
        if (eventOrder != null)
        {
            ActivateCustomerEvent("EventoEspecial");
        }
        else
        {
            DeactivateCustomerEvent();
        }
    }

    public string GetEventPhraseForOrder(int orderIndex, string item, string metal, string wood)
    {
        if (!hasEvent || eventOrders == null) return null;  // No hay frase de evento si hasEvent es false o eventOrders es null

        // Busca la frase de evento para la orden actual
        var eventOrder = eventOrders.Find(e => e != null && e.orderIndex == orderIndex);

        if (eventOrder != null)
        {
            // Reemplazar los marcadores {item}, {metal}, {wood}
            return eventOrder.eventPhrase
                .Replace("{item}", item)
                .Replace("{metal}", metal)
                .Replace("{wood}", wood);
        }

        return null;
    }

    public void ActivateCustomerEvent(string eventName)
    {
        activeEvent = eventName;
        hasEvent = true;
        //Debug.Log($"Evento '{eventName}' activado para el cliente.");
    }

    public void DeactivateCustomerEvent()
    {
        activeEvent = null;
        hasEvent = false;
        //Debug.Log("Evento desactivado para el cliente.");
    }

    // Mostrar la lista de eventos solo si hasEvent es true
    private void OnValidate()
    {
        // Mostrar la lista solo si hasEvent es true y eventOrders no es null
        if (hasEvent && eventOrders != null)
        {
            // Validar las frases de evento si hasEvent es true
            foreach (var eventOrder in eventOrders)
            {
                if (eventOrder == null || eventOrder.eventPhrase == null)
                {
                    Debug.LogWarning("EventOrder o eventPhrase es null.");
                    continue;
                }

                if (!eventOrder.IsValid())
                {
                    Debug.LogWarning($"La frase del evento en el índice {eventOrder.orderIndex} no contiene todos los marcadores necesarios {{item}}, {{metal}}, {{wood}}.");
                }
            }
        }
    }
}

[System.Serializable] // Asegurarse de que sea serializable para que se pueda mostrar en el Inspector
public class EventOrder
{
    public int orderIndex;      // Índice de la orden
    public string eventPhrase;  // Frase que se activará cuando esta orden esté activa

    [SerializeField]
    private string eventInfo;   // Texto informativo del evento (modificable desde el Inspector)

    // Validar que la frase tenga los marcadores requeridos
    public bool IsValid()
    {
        // Verificar si eventPhrase no es null
        if (string.IsNullOrEmpty(eventPhrase))
        {
            Debug.LogWarning("eventPhrase es null o está vacío en EventOrder.");
            return false;
        }

        return eventPhrase.Contains("{item}") && eventPhrase.Contains("{metal}") && eventPhrase.Contains("{wood}");
    }

    // Método para acceder a la información del evento
    public string GetEventInfo()
    {
        return eventInfo;
    }

    // Método para actualizar la información del evento
    public void SetEventInfo(string info)
    {
        eventInfo = info;
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

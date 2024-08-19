using UnityEngine;

[RequireComponent(typeof(CustomerController))]
public class CustomerStateHandler : MonoBehaviour
{
    private CustomerStateManager customerStateManager;
    private CustomerController customerController;
    private IndicatorManager indicatorManager;
    private CustomerState currentState;

    private void Awake()
    {
        customerStateManager = FindObjectOfType<CustomerStateManager>();
        customerController = GetComponent<CustomerController>();
        indicatorManager = FindObjectOfType<IndicatorManager>();

        // Verificar que las referencias se hayan inicializado correctamente
        if (customerStateManager == null)
        {
            Debug.LogError("CustomerStateManager no encontrado. Asegúrate de que está en la escena.");
        }

        if (customerController == null)
        {
            Debug.LogError("CustomerController no encontrado en el objeto.");
        }

        if (indicatorManager == null)
        {
            Debug.LogError("IndicatorManager no encontrado. Asegúrate de que está en la escena.");
        }
    }

    private void Start()
    {
        if (customerStateManager != null)
        {
            UpdateCustomerState(50); // Inicializa el estado del cliente con un valor de satisfacción del 50%
        }
        else
        {
            Debug.LogError("No se puede actualizar el estado del cliente porque CustomerStateManager no está inicializado.");
        }
    }

    public void SetCustomerState(CustomerState state)
    {
        currentState = state;
        UpdateCustomerEmotion();
    }

    public void UpdateCustomerState(int percentage)
    {
        if (customerStateManager != null)
        {
            currentState = customerStateManager.GetCustomerStateFromPercentage(percentage);
            UpdateCustomerEmotion();
        }
        else
        {
            Debug.LogError("No se puede actualizar el estado del cliente porque CustomerStateManager no está inicializado.");
        }
    }

    private void UpdateCustomerEmotion()
    {
        if (customerController != null)
        {
            customerController.SetEmotion(ConvertCustomerStateToEmotion(currentState));
        }
        else
        {
            Debug.LogError("No se puede actualizar la emoción del cliente porque CustomerController no está inicializado.");
        }
    }

    private ChatBubble.IconType ConvertCustomerStateToEmotion(CustomerState state)
    {
        switch (state)
        {
            case CustomerState.MuyInsatisfecho:
                return ChatBubble.IconType.MuyInsatisfecho;
            case CustomerState.Insatisfecho:
                return ChatBubble.IconType.Insatisfecho;
            case CustomerState.Neutral:
                return ChatBubble.IconType.Neutral;
            case CustomerState.Contento:
                return ChatBubble.IconType.Contento;
            case CustomerState.MuyFeliz:
                return ChatBubble.IconType.MuyFeliz;
            default:
                Debug.LogWarning("Unknown customer state: " + state);
                return ChatBubble.IconType.Neutral;
        }
    }

    public void UpdateIndicatorsAndState(string itemType, string quality)
    {
        if (indicatorManager != null)
        {
            indicatorManager.ProcessItem(itemType, quality);
            UpdateCustomerState(indicatorManager.Satisfaction);
        }
        else
        {
            Debug.LogWarning("IndicatorManager no encontrado.");
        }
    }

    public CustomerState GetCurrentCustomerState()
    {
        return currentState;
    }
}

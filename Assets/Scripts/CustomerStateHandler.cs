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
    }

    private void Start()
    {
        UpdateCustomerState(50); // Inicializa el estado del cliente con un valor de satisfacción del 50%
    }

    public void SetCustomerState(CustomerState state)
    {
        currentState = state;
        UpdateCustomerEmotion();
    }

    public void UpdateCustomerState(int percentage)
    {
        currentState = customerStateManager.GetCustomerStateFromPercentage(percentage);
        UpdateCustomerEmotion();
    }

    private void UpdateCustomerEmotion()
    {
        customerController.SetEmotion(ConvertCustomerStateToEmotion(currentState));
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
            // Procesa el ítem para actualizar los indicadores
            indicatorManager.ProcessItem(itemType, quality);

            // Actualiza el estado del cliente basado en el nivel de satisfacción actual
            UpdateCustomerState(indicatorManager.Satisfaction);
        }
        else
        {
            Debug.LogWarning("IndicatorManager not found.");
        }
    }

    public CustomerState GetCurrentCustomerState()
    {
        return currentState;
    }
}

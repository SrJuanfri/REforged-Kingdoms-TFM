using UnityEngine;

public enum CustomerState
{
    MuyInsatisfecho,  // 0-10%
    Insatisfecho,     // 11-30%
    Neutral,          // 31-60%
    Contento,         // 61-89%
    MuyFeliz          // 90-100%
}

public class CustomerStateManager : MonoBehaviour
{
    /// <summary>
    /// Convierte un porcentaje en un estado de cliente.
    /// </summary>
    /// <param name="percentage">El porcentaje (0-100) que se va a convertir.</param>
    /// <returns>El estado del cliente correspondiente al porcentaje dado.</returns>
    public CustomerState GetCustomerStateFromPercentage(int percentage)
    {
        if (percentage < 0 || percentage > 100)
        {
            Debug.LogWarning("Percentage out of bounds: " + percentage);
            return CustomerState.Neutral; // Valor por defecto en caso de porcentaje inválido
        }

        if (percentage >= 0 && percentage <= 10)
        {
            return CustomerState.MuyInsatisfecho;
        }
        else if (percentage >= 11 && percentage <= 30)
        {
            return CustomerState.Insatisfecho;
        }
        else if (percentage >= 31 && percentage <= 60)
        {
            return CustomerState.Neutral;
        }
        else if (percentage >= 61 && percentage <= 89)
        {
            return CustomerState.Contento;
        }
        else if (percentage >= 90 && percentage <= 100)
        {
            return CustomerState.MuyFeliz;
        }
        else
        {
            Debug.LogWarning("Unhandled percentage: " + percentage);
            return CustomerState.Neutral; // Valor por defecto en caso de porcentaje no manejado
        }
    }
}

using UnityEngine;

public class MoneyCheck : MonoBehaviour
{
    private Registradora registradora;  // Referencia al script Registradora
    private int initialMoney;  // El valor inicial de moneyNumber al comenzar la escena
    private doorManager[] doorManagers;  // Referencia a todos los scripts doorManager

    private void Start()
    {
        // Encontrar el script Registradora en la escena
        registradora = FindObjectOfType<Registradora>();

        if (registradora == null)
        {
            Debug.LogError("No se encontró el script Registradora en la escena.");
            return;
        }

        // Guardar el valor inicial de moneyNumber
        initialMoney = registradora.moneyNumber;

        // Buscar todos los scripts de tipo doorManager y desactivarlos
        doorManagers = FindObjectsOfType<doorManager>();
        foreach (doorManager door in doorManagers)
        {
            door.enabled = false;  // Asegurarse de que empiezan desactivados
            Debug.Log("doorManager desactivado.");
        }
    }

    private void Update()
    {
        // Si moneyNumber es mayor que el valor inicial, activar los scripts doorManager
        if (registradora.moneyNumber > initialMoney)
        {
            ActivateDoors();
        }
    }

    // Método para activar todos los scripts doorManager
    private void ActivateDoors()
    {
        foreach (doorManager door in doorManagers)
        {
            if (!door.enabled)  // Verificar si el script está desactivado
            {
                door.enabled = true;  // Activar el script
                Debug.Log("doorManager activado.");
            }
        }
    }
}

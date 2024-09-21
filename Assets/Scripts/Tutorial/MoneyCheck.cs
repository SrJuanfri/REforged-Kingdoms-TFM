using UnityEngine;

public class MoneyCheck : MonoBehaviour
{
    private Registradora registradora;  // Referencia al script Registradora
    private int initialMoney;  // El valor inicial de moneyNumber al comenzar la escena
    private doorManager[] doorManagers;  // Referencia a todos los scripts doorManager

    [SerializeField] private GameObject cartel;
    [HideInInspector] public bool checkedMoney;
    private void Start()
    {
        checkedMoney = false;
        // Encontrar el script Registradora en la escena
        registradora = FindObjectOfType<Registradora>();
        cartel.SetActive(true);

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
            door.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void Update()
    {
        // Si moneyNumber es mayor que el valor inicial, activar los scripts doorManager
        if (registradora.moneyNumber > initialMoney)
        {
            checkedMoney = true;
            //ActivateDoors();
        }
    }

    // Método para activar todos los scripts doorManager
    public void ActivateDoors()
    {
        foreach (doorManager door in doorManagers)
        {
            if (!door.enabled)  // Verificar si el script está desactivado
            {
                door.enabled = true;  // Activar el script
                door.gameObject.GetComponent<BoxCollider>().enabled = true;
                cartel.SetActive(false);
            }
        }
    }

    public void DesactivateDoors()
    {
        if (doorManagers != null && doorManagers.Length > 0)
        {
            foreach (doorManager door in doorManagers)
            {
                if (!door.enabled)  // Verificar si el script está desactivado
                {
                    door.enabled = false;  // Activar el script
                    door.gameObject.GetComponent<BoxCollider>().enabled = false;
                    cartel.SetActive(true);
                }
            }
        }

    }
}

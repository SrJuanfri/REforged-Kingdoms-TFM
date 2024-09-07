using UnityEngine;

public class Newspaper : Interactable
{
    private ObjectGrabbable objectGrabbable;  // Para detectar si el peri�dico est� siendo agarrado
    private PlayerUI playerUI;  // Referencia a PlayerUI para actualizar los prompts

    [SerializeField] private Transform firstChild;  // El primer hijo (vista normal del peri�dico)
    [SerializeField] private Transform secondChild; // El segundo hijo (modo lectura del peri�dico)
    [SerializeField] private Transform readPoint;   // Punto donde se mover� el conjunto cuando el segundo hijo est� activo (Asignado desde el Inspector)

    private Transform originalParent;  // Guardar el padre original del peri�dico
    private bool isReading = false;  // Estado para saber si estamos leyendo el peri�dico

    private void Awake()
    {
        // Obtener el componente ObjectGrabbable desde los hijos del objeto padre
        objectGrabbable = GetComponentInChildren<ObjectGrabbable>();
        playerUI = FindObjectOfType<PlayerUI>();  // Obtener la referencia a PlayerUI

        if (objectGrabbable == null)
        {
            Debug.LogError("No ObjectGrabbable component found in children of " + gameObject.name);
        }

        // Asegurarse de que el primer hijo est� activo y el segundo hijo est� desactivado al iniciar
        if (firstChild != null)
        {
            firstChild.gameObject.SetActive(true);
            Debug.Log("First child (vista normal) activado.");
        }
        if (secondChild != null)
        {
            secondChild.gameObject.SetActive(false);
            Debug.Log("Second child (modo lectura) desactivado.");
        }

        // Guardar el padre original del conjunto
        originalParent = transform.parent;
        Debug.Log("Original parent guardado.");
    }

    private void Update()
    {
        // Solo mostrar el prompt secundario si el peri�dico est� siendo agarrado
        if (objectGrabbable != null && objectGrabbable.IsBeingGrabbed())
        {
            Debug.Log("Peri�dico est� siendo agarrado.");
            playerUI.UpdateSecondaryActionText(actionPrompt, actionKey);  // Mantener esta l�nea

            // Solo detectar el primer frame en que se pulsa la tecla Q
            if (Input.GetKeyDown(KeyCode.Q))  // Si se presiona la tecla de acci�n (por ejemplo, 'Q')
            {
                Debug.Log("Tecla Q presionada, alternando estado de lectura.");
                ToggleReadingMode();
            }
        }
        else
        {
            Debug.Log("Peri�dico no est� siendo agarrado.");
            playerUI.ClearSecondaryActionText();
        }
    }

    private void ToggleReadingMode()
    {
        if (!isReading)
        {
            Debug.Log("Activando modo lectura.");

            // Si no estamos leyendo, activar el segundo hijo (modo lectura) y desactivar el primero
            firstChild.gameObject.SetActive(false);
            secondChild.gameObject.SetActive(true);
            Debug.Log("Modo lectura activado.");

            // Mover el padre bajo ReadPoint y resetear la posici�n y rotaci�n
            transform.SetParent(readPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            Debug.Log("Transform movido a ReadPoint.");

            actionPrompt = "Cerrar";  // Cambiar el prompt a "Cerrar"
        }
        else
        {
            Debug.Log("Saliendo del modo lectura.");

            // Si estamos leyendo, activar el primer hijo y desactivar el segundo
            firstChild.gameObject.SetActive(true);
            secondChild.gameObject.SetActive(false);
            Debug.Log("Modo normal activado.");

            // Volver al padre original y resetear la posici�n
            transform.SetParent(originalParent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            Debug.Log("Transform movido al padre original.");

            actionPrompt = "Leer";  // Cambiar el prompt de nuevo a "Leer"
        }

        isReading = !isReading;  // Cambiar el estado de lectura
        Debug.Log("isReading cambiado a: " + isReading);
    }

    protected override void Interact()
    {
        if (objectGrabbable != null)
        {
            if (objectGrabbable.IsBeingGrabbed())
            {
                Debug.Log("Soltando peri�dico.");
                objectGrabbable.Drop();  // Soltar el peri�dico
            }
            else
            {
                Debug.Log("Agarrando peri�dico.");
                objectGrabbable.Grab(transform);  // Agarrar el peri�dico
            }
        }
    }
}

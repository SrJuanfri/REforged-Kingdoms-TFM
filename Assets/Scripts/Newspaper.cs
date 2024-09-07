using UnityEngine;

public class Newspaper : Interactable
{
    private ObjectGrabbable objectGrabbable;  // Para detectar si el periódico está siendo agarrado
    private PlayerUI playerUI;  // Referencia a PlayerUI para actualizar los prompts

    [SerializeField] private Transform firstChild;  // El primer hijo (vista normal del periódico)
    [SerializeField] private Transform secondChild; // El segundo hijo (modo lectura del periódico)
    [SerializeField] private Transform readPoint;   // Punto donde se moverá el conjunto cuando el segundo hijo esté activo (Asignado desde el Inspector)

    private Transform originalParent;  // Guardar el padre original del periódico
    private bool isReading = false;  // Estado para saber si estamos leyendo el periódico

    private void Awake()
    {
        // Obtener el componente ObjectGrabbable desde los hijos del objeto padre
        objectGrabbable = GetComponentInChildren<ObjectGrabbable>();
        playerUI = FindObjectOfType<PlayerUI>();  // Obtener la referencia a PlayerUI

        if (objectGrabbable == null)
        {
            Debug.LogError("No ObjectGrabbable component found in children of " + gameObject.name);
        }

        // Asegurarse de que el primer hijo está activo y el segundo hijo está desactivado al iniciar
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
        // Solo mostrar el prompt secundario si el periódico está siendo agarrado
        if (objectGrabbable != null && objectGrabbable.IsBeingGrabbed())
        {
            Debug.Log("Periódico está siendo agarrado.");
            playerUI.UpdateSecondaryActionText(actionPrompt, actionKey);  // Mantener esta línea

            // Solo detectar el primer frame en que se pulsa la tecla Q
            if (Input.GetKeyDown(KeyCode.Q))  // Si se presiona la tecla de acción (por ejemplo, 'Q')
            {
                Debug.Log("Tecla Q presionada, alternando estado de lectura.");
                ToggleReadingMode();
            }
        }
        else
        {
            Debug.Log("Periódico no está siendo agarrado.");
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

            // Mover el padre bajo ReadPoint y resetear la posición y rotación
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

            // Volver al padre original y resetear la posición
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
                Debug.Log("Soltando periódico.");
                objectGrabbable.Drop();  // Soltar el periódico
            }
            else
            {
                Debug.Log("Agarrando periódico.");
                objectGrabbable.Grab(transform);  // Agarrar el periódico
            }
        }
    }
}

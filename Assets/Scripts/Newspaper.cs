using UnityEngine;
using TMPro;

public class Newspaper : Interactable
{
    private ObjectGrabbable objectGrabbable;  // Para detectar si el peri�dico est� siendo agarrado
    private PlayerUI playerUI;  // Referencia a PlayerUI para actualizar los prompts

    [SerializeField] private Transform firstChild;  // El primer hijo (vista normal del peri�dico)
    [SerializeField] private Transform secondChild; // El segundo hijo (modo lectura del peri�dico)
    [SerializeField] private Transform readPoint;   // Punto donde se mover� el conjunto cuando el segundo hijo est� activo (Asignado desde el Inspector)
    [SerializeField] private TextMeshProUGUI eventInfoText;  // El componente TextMeshProUGUI para mostrar la informaci�n del evento

    private Transform originalParent;  // Guardar el padre original del peri�dico
    private Vector3 originalPosition;  // Guardar la posici�n original
    private Quaternion originalRotation;  // Guardar la rotaci�n original
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
        }
        if (secondChild != null)
        {
            secondChild.gameObject.SetActive(false);
        }

        // Guardar el padre, posici�n y rotaci�n originales
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    private void Update()
    {
        // Solo mostrar el prompt secundario si el peri�dico est� siendo agarrado
        if (objectGrabbable != null && objectGrabbable.IsBeingGrabbed())
        {
            playerUI.UpdateSecondaryActionText(actionPrompt, actionKey);  // Mantener esta l�nea

            // Solo detectar el primer frame en que se pulsa la tecla Q
            if (Input.GetKeyDown(KeyCode.Q))  // Si se presiona la tecla de acci�n (por ejemplo, 'Q')
            {
                ToggleReadingMode();
            }
        }
        else
        {
            playerUI.ClearSecondaryActionText();
        }
    }

    private void ToggleReadingMode()
    {
        if (!isReading)
        {
            // Si no estamos leyendo, activar el segundo hijo (modo lectura) y desactivar el primero
            firstChild.gameObject.SetActive(false);
            secondChild.gameObject.SetActive(true);

            // Mover el padre bajo ReadPoint y resetear la posici�n y rotaci�n
            transform.SetParent(readPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            actionPrompt = "Cerrar";  // Cambiar el prompt a "Cerrar"
        }
        else
        {
            // Si estamos leyendo, activar el primer hijo y desactivar el segundo
            firstChild.gameObject.SetActive(true);
            secondChild.gameObject.SetActive(false);

            // Restablecer el transform original
            ResetToOriginalTransform();

            actionPrompt = "Leer";  // Cambiar el prompt de nuevo a "Leer"
        }

        isReading = !isReading;  // Cambiar el estado de lectura
    }

    // M�todo para restablecer el transform original
    public void ResetToOriginalTransform()
    {
        transform.SetParent(originalParent);       // Restablecer el padre original
        transform.localPosition = originalPosition;  // Restablecer la posici�n original
        transform.localRotation = originalRotation;  // Restablecer la rotaci�n original
    }

    // M�todo para actualizar el texto del evento en el peri�dico
    public void UpdateEventInfo(string eventInfo)
    {
        if (eventInfoText != null)
        {
            eventInfoText.text = eventInfo;  // Actualizar el texto con la informaci�n del evento
        }
    }

    protected override void Interact()
    {
        if (objectGrabbable != null)
        {
            if (objectGrabbable.IsBeingGrabbed())
            {
                objectGrabbable.Drop();  // Soltar el peri�dico
            }
            else
            {
                objectGrabbable.Grab(transform);  // Agarrar el peri�dico
            }
        }
    }
}

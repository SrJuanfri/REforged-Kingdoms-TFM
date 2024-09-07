using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineSelection : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float efectiveDistance;
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;

    // Variable p�blica para que otros scripts accedan al objeto resaltado
    public Transform HighlightedObject => highlight;

    // Nueva variable p�blica para devolver el objeto al que el jugador est� apuntando
    public Transform PointedObject => raycastHit.transform;

    void Update()
    {
        // Desactivar el outline del objeto resaltado previamente si existe
        if (highlight)
        {
            Outline previousOutline = highlight.GetComponent<Outline>();
            if (previousOutline)
            {
                previousOutline.enabled = false;
            }
            highlight = null;
        }

        // Realizar el raycast para detectar objetos seleccionables
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
                out raycastHit, efectiveDistance))
        {
            // Guardar el objeto que se est� apuntando, aunque no sea seleccionable
            Transform pointedObject = raycastHit.transform;

            // Comprobar si el objeto tiene el tag "Selectable"
            if (pointedObject.CompareTag("Selectable"))
            {
                highlight = pointedObject;
                Outline outline = highlight.GetComponent<Outline>();
                if (outline == null)
                {
                    outline = highlight.gameObject.AddComponent<Outline>();
                    outline.OutlineColor = Color.green;
                    outline.OutlineWidth = 6.0f;
                }
                outline.enabled = true;
            }
            else
            {
                highlight = null;
            }
        }

        // Gestionar la selecci�n al presionar la tecla E
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (highlight)
            {
                // Desactivar el outline de la selecci�n anterior si exist�a
                if (selection)
                {
                    Outline previousSelectionOutline = selection.GetComponent<Outline>();
                    if (previousSelectionOutline)
                    {
                        previousSelectionOutline.enabled = false;
                    }
                }

                // Guardar la nueva selecci�n y activar su outline
                selection = highlight;
                Outline selectionOutline = selection.GetComponent<Outline>();
                if (selectionOutline)
                {
                    selectionOutline.enabled = true;
                }
                else
                {
                    selectionOutline = selection.gameObject.AddComponent<Outline>();
                    selectionOutline.OutlineColor = Color.green;
                    selectionOutline.OutlineWidth = 6.0f;
                    selectionOutline.enabled = true;
                }

                highlight = null;
            }
            else
            {
                // Si no hay objeto resaltado, desactivar la selecci�n actual
                if (selection)
                {
                    Outline selectionOutline = selection.GetComponent<Outline>();
                    if (selectionOutline)
                    {
                        selectionOutline.enabled = false;
                    }
                    selection = null;
                }
            }
        }
    }
}

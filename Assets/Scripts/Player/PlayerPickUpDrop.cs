using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;  // Punto donde se colocará cualquier objeto agarrado
    [SerializeField] private LayerMask pickUpLayerMask;

    private ObjectGrabbable objectGrabbable;
    private float pickUpDistance = 2.5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!objectGrabbable)  // Si no se sostiene ningún objeto
            {
                // Realizar raycast para detectar objetos
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask))
                {
                    // Intentar recoger un objeto
                    if (raycastHit.transform.TryGetComponent(out objectGrabbable))
                    {
                        // Agarrar el objeto (independientemente de si es un periódico o no)
                        objectGrabbable.Grab(objectGrabPointTransform);
                    }
                }
            }
            else
            {
                // Soltar el objeto
                objectGrabbable.Drop();
                objectGrabbable = null;
            }
        }
    }

    // Método para obtener el objeto que el jugador sostiene
    public ObjectGrabbable GetHeldObject()
    {
        return objectGrabbable;
    }
}

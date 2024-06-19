using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;

    private ObjectGrabbable objectGrabbable;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(objectGrabbable == null)
            {
                //No llevamos objeto e intentamos cogerlo
                float pickUpDistance = 2f;
                if (Physics.Raycast(playerCameraTransform.transform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent(out objectGrabbable))
                    {
                        objectGrabbable.Grab(objectGrabPointTransform);
                    }
                }
            }
            else
            {
                //Llevamos un objeto y lo intentamos tirar
                objectGrabbable.Drop();
                objectGrabbable = null;
            }
            
        }
    }
}

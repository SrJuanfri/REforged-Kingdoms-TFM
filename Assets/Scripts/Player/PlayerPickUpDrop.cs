using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;
    public Transform readPoint;
    [SerializeField] private LayerMask pickUpLayerMask;

    private ObjectGrabbable objectGrabbable;
    private Newspaper newspaper;
    private float pickUpDistance = 2.5f;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(!objectGrabbable)
            {
                //No llevamos objeto e intentamos cogerlo
                
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!newspaper)
            {
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
                        out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask))
                {
                    if (raycastHit.transform.TryGetComponent(out newspaper))
                    {
                        Debug.Log("Coger Periodico");
                        newspaper.Read(readPoint);
                    }
                }
            }
            else
            {
                Debug.Log("Soltar Periodico");
                newspaper.Drop();
                newspaper = null;
            }
        }
    }
}

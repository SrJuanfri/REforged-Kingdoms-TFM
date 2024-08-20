using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask customerLayerMask;
    [SerializeField] private LayerMask buttonLayerMask;

    private float interactionDistance = 2.5f; // Distancia de interacción

    private void FixedUpdate()
    {
        // Detectar interacción con clientes
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Raycast para detectar clientes
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
                    out RaycastHit hit, interactionDistance, customerLayerMask))
            {
                // Comprobar si el objeto tiene un CustomerController
                if (hit.transform.TryGetComponent(out CustomerController customerController))
                {
                    Debug.Log("Hit Customer");
                    customerController.InteractNPC();
                    customerController.SellNPC();
                }
                // Comprobar si el objeto tiene un MerchantController
                else if (hit.transform.TryGetComponent(out MerchantController merchantController))
                {
                    Debug.Log("Hit Merchant");
                    merchantController.InteractNPC();
                }
            }
        }

        // Detectar interacción con botones (u otros objetos de acción)
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Raycast para detectar botones
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
                    out RaycastHit hit, interactionDistance, buttonLayerMask))
            {
                // Comprobar si el objeto tiene un ItemGenerator
                if (hit.transform.TryGetComponent(out ItemGenerator itemGenerator))
                {
                    Debug.Log("Hit Item Generator");
                    itemGenerator.CreateItem();
                }

                if (hit.transform.TryGetComponent(out PlaneGenerator planeGenerator))
                {
                    Debug.Log("Hit Plane Generator");
                    planeGenerator.AddPlane();
                }
            }
        }
    }
}

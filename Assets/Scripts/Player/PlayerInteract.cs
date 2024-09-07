using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask interactableLayerMask; // Combinar las capas en una sola
    private float interactionDistance = 2.5f; // Distancia de interacción

    private void FixedUpdate()
    {
        // Detectar interacción con objetos interactuables
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Un único raycast que detecta tanto clientes como botones
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
                                out RaycastHit hit, interactionDistance, interactableLayerMask))
            {
                // Primero, comprobamos si es un cliente o comerciante
                if (hit.transform.TryGetComponent(out CustomerController customerController))
                {
                    Debug.Log("Hit Customer");
                    customerController.InteractNPC();
                    customerController.SellNPC();
                }
                else if (hit.transform.TryGetComponent(out MerchantController merchantController))
                {
                    Debug.Log("Hit Merchant");
                    merchantController.InteractNPC();
                }
                // Si no es cliente, comprobamos si es un generador de items
                else if (hit.transform.TryGetComponent(out ItemGenerator itemGenerator))
                {
                    Debug.Log("Hit Item Generator");
                    itemGenerator.CreateItem();
                }
                else if (hit.transform.TryGetComponent(out PlaneGenerator planeGenerator))
                {
                    Debug.Log("Hit Plane Generator");
                    planeGenerator.AddPlane();
                }
            }
        }
    }
}

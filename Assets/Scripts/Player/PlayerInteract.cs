using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask interactableLayerMask; // Máscaras de capas combinadas para objetos interactuables
    private float interactionDistance = 2.5f; // Distancia de interacción

    private void Update()
    {
        // Detectar si se presiona la tecla E para interactuar
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log("E key pressed, attempting interaction...");
            PerformInteraction();
        }
    }

    // Método que maneja la interacción utilizando raycast
    private void PerformInteraction()
    {
        // Realizar un raycast desde la cámara del jugador hacia adelante
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
                            out RaycastHit hit, interactionDistance, interactableLayerMask))
        {
            //Debug.Log($"Raycast hit: {hit.transform.name} at distance {hit.distance}");

            // Detectar y manejar la interacción según el tipo de objeto
            if (hit.transform.TryGetComponent(out CustomerController customerController))
            {
                //Debug.Log("Hit Customer, starting interaction.");
                customerController.InteractNPC();
                customerController.SellNPC();
            }
            else if (hit.transform.TryGetComponent(out MerchantController merchantController))
            {
                //Debug.Log("Hit Merchant, starting interaction.");
                merchantController.InteractNPC();
            }
            else if (hit.transform.TryGetComponent(out ItemGenerator itemGenerator))
            {
                //Debug.Log("Hit Item Generator, creating item.");
                itemGenerator.CreateItem();
            }
            else if (hit.transform.TryGetComponent(out PlaneGenerator planeGenerator))
            {
                //Debug.Log("Hit Plane Generator, adding plane.");
                planeGenerator.AddPlane();
            }
            else if (hit.transform.TryGetComponent(out ChangeButtons changeButtons))
            {
                //Debug.Log("Hit Item Generator, creating item.");
                changeButtons.ChangeButtonInterface();
            }
            else if (hit.transform.TryGetComponent(out BellClickHandler bellClickHandler))
            {
                //Debug.Log("Hit Plane Generator, adding plane.");
                bellClickHandler.OnBellClicked();
            }
            else
            {
                //Debug.LogWarning("Hit object, but no interactable component found.");
            }
        }
        else
        {
            //Debug.LogWarning("Raycast did not hit any object within range.");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask customerLayerMask;
    [SerializeField] private LayerMask buttonLayerMask;
    
    private float customerDistance = 3f;

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
                    out RaycastHit raycastHitCustomer, customerDistance, customerLayerMask))
            {
                if (raycastHitCustomer.transform.TryGetComponent(out Interaction interaction))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        Debug.Log("Hit Customer");
                        interaction.InteractNPC();
                        interaction.SellNPC();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
                    out RaycastHit raycastHitButton, customerDistance, buttonLayerMask))
            {
                if (raycastHitButton.transform.TryGetComponent(out ItemGenerator itemGenerator))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        itemGenerator.CreateItem();
                    }
                }
            }
        }
    }
}

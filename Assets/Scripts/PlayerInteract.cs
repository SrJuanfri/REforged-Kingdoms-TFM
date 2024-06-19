using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask customerLayerMask;
    
    private float customerDistance = 5f;

    private void Update()
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
                        interaction.Interact();
                        interaction.Sell();
                    }
                }

            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrafting : MonoBehaviour
{

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask craftLayerMask;
    [SerializeField] private LayerMask objectLayerMask;
    private float interactDistance = 3f;

    private void FixedUpdate()
    {
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit,
                interactDistance, objectLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out ObjectGrabbable objectGrabbable))
            {
                objectGrabbable.ShowName();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
                    out RaycastHit raycastHit, interactDistance, craftLayerMask))
            {
                if (raycastHit.transform.TryGetComponent(out RecipeBlueprint recipeBlueprint))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        recipeBlueprint.Craft();
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        Debug.Log("NextRecipe");
                        recipeBlueprint.NextRecipe();
                    }

                }
            }
        }
    }
}

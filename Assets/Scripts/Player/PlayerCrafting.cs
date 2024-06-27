using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrafting : MonoBehaviour
{

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask craftLayerMask;
    private float interactDistance = 2f;

    private PlayerUI playerUI;

    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
    }

    private void FixedUpdate()
    {
        playerUI.actionUI.SetActive(false);
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit,
                interactDistance))
        {
            if (raycastHit.collider.GetComponent<Interactable>())
            {
                playerUI.UpdateActionText(raycastHit.collider.GetComponent<Interactable>().actionPrompt, raycastHit.collider.GetComponent<Interactable>().actionKey);
                playerUI.actionUI.SetActive(true);
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

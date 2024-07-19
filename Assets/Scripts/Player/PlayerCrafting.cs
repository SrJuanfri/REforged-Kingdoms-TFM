using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrafting : MonoBehaviour
{

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private LayerMask craftLayerMask;
    private float interactDistance = 2.5f;

    private PlayerUI playerUI;

    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
    }

    private void FixedUpdate()
    {
        //Checkeamos si tiene capacidad de interactuar y si tiene nombre y valor
        
        playerUI.actionUI.SetActive(false);
        playerUI.infoUI.SetActive(false);
        
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit,
                interactDistance))
        {
            if (raycastHit.collider.GetComponent<Interactable>())
            {
                //Update interactuabilidad
                
                playerUI.UpdateActionText(raycastHit.collider.GetComponent<Interactable>().actionPrompt,
                    raycastHit.collider.GetComponent<Interactable>().actionKey);
                playerUI.actionUI.SetActive(true);
            }

            if (raycastHit.collider.GetComponent<ItemSOHolder>())
            {
                //Update nombre y valor
                
                playerUI.UpdateInfoText(raycastHit.collider.GetComponent<ItemSOHolder>().itemSO.itemName, 
                    raycastHit.collider.GetComponent<ItemSOHolder>().itemSO.value);
                playerUI.infoUI.SetActive(true);
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

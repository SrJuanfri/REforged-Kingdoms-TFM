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
            
            if (raycastHit.collider.GetComponent<WeaponOrToolSOHolder>())
            {
                //Update nombre y valor
                
                playerUI.UpdateInfoText(raycastHit.collider.GetComponent<WeaponOrToolSOHolder>().weaponOrToolSO.itemName, 
                    raycastHit.collider.GetComponent<WeaponOrToolSOHolder>().weaponOrToolSO.value);
                playerUI.infoUI.SetActive(true);
            }
            
            if (raycastHit.collider.GetComponent<ItemGenerator>())
            {
                //Update nombre y valor
                
                playerUI.UpdateInfoText(raycastHit.collider.GetComponent<ItemGenerator>().itemSO.itemName, 
                    raycastHit.collider.GetComponent<ItemGenerator>().itemSO.value);
                playerUI.infoUI.SetActive(true);
            }

            if (raycastHit.collider.GetComponent<PlaneGenerator>())
            {
                // Obtener el craftingRecipeSO del PlaneGenerator
                var planeGenerator = raycastHit.collider.GetComponent<PlaneGenerator>();

                // Obtener siempre la primera combinación de materiales
                var selectedCombination = planeGenerator.craftingRecipeSO.materialCombinations[0];

                // Actualizar la UI con el nombre y precio base del item de salida correspondiente
                playerUI.UpdateInfoText(selectedCombination.outputItemSO.itemName, planeGenerator.craftingRecipeSO.designBasePrice);
                playerUI.infoUI.SetActive(true);
            }

        }
    }

    private void Update()
    {
        {
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
                    out RaycastHit raycastHit, interactDistance, craftLayerMask))
            {
                if (raycastHit.transform.TryGetComponent(out RecipeCrafting recipeCrafting))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        recipeCrafting.Craft();
                    }
                }

                if (raycastHit.transform.TryGetComponent(out RecipeSelection recipeSelection))
                {
                    if (Input.GetKeyDown(KeyCode.E) && recipeSelection)
                    {
                        recipeSelection.NextRecipeWeapons();
                    }
                }

                if (raycastHit.transform.TryGetComponent(out RecipeSelectionTool recipeSelectionTool))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        recipeSelectionTool.NextRecipeTool();
                    }
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGenerator : Interactable
{
    [SerializeField] private Registradora registradora;
    public CraftingRecipeSO craftingRecipeSO;
    [SerializeField] private RecipeSelection recipeSelection;

    public void AddPlane()
    {
        int value = craftingRecipeSO.designBasePrice; // Obtener el valor de la receta

        // Check if the player has enough money to purchase the recipe
        if (registradora.moneyNumber >= value)
        {
            // Use the new AddRecipe method to add the recipe to the list and update the UI
            recipeSelection.AddRecipe(craftingRecipeSO);
            Debug.Log("Plano Añadido");

            // Deduct the value of the recipe from the player's money and update the display
            registradora.moneyNumber -= value;
            registradora.countText.text = registradora.moneyNumber.ToString();
        }
        else
        {
            Debug.Log("Not enough money to add this recipe.");
        }
    }
}

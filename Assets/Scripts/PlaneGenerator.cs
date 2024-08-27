using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGenerator : Interactable
{
    [SerializeField] private Registradora registradora;
    [SerializeField] private CraftingRecipeSO craftingRecipeSO;
    [SerializeField] private RecipeSelection recipeSelection;

    public void AddPlane()
    {
        int value = craftingRecipeSO.TotalPrice; // Obtener el valor de la receta

        if (registradora.moneyNumber >= value)
        {
            recipeSelection.craftingRecipeSOListWeapons.Add(craftingRecipeSO);

            registradora.countText.text = (registradora.moneyNumber - value).ToString();
            registradora.moneyNumber -= value;
        }
    }
}

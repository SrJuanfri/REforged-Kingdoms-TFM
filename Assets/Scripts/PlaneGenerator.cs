using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlaneGenerator : Interactable
{
    [SerializeField] private Registradora registradora;
    public CraftingRecipeSO craftingRecipeSO;
    [SerializeField] private RecipeSelection recipeSelection;

    public void AddPlane()
    {
        int value = craftingRecipeSO.designBasePrice; // Obtener el valor de la receta
        
        if ((registradora.moneyNumber >= value))
        {
            recipeSelection.craftingRecipeSOListWeapons.Add(craftingRecipeSO);
            Debug.Log("Plano Añadido");

            registradora.countText.text = (registradora.moneyNumber - value).ToString();
            registradora.moneyNumber -= value;
        }
        
    }
}

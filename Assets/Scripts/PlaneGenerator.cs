using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneGenerator : Interactable
{
    [SerializeField] private Registradora registradora;
    [SerializeField] private CraftingRecipeSO craftingRecipeSO;
    [SerializeField] private RecipeSelection recipeSelection;
    [SerializeField] private int value;


    public void AddPlane()
    {
        if (registradora.moneyNumber >= value)
        {
            recipeSelection.craftingRecipeSOListWeapons.Add(craftingRecipeSO);
            
            registradora.countText.text = (registradora.moneyNumber - value).ToString();
            registradora.moneyNumber -= value;
        }
    }
}

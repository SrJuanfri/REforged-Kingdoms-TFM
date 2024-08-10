using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeSelection : Interactable
{
    public Image recipeImage;
    [SerializeField] private List<CraftingRecipeSO> craftingRecipeSOListWeapons;
    [SerializeField] private List<CraftingRecipeSO> craftingRecipeSOListTools;
    
    [HideInInspector] public CraftingRecipeSO craftingRecipeSO;
    
    private void Awake()
    {
        craftingRecipeSO = craftingRecipeSOListWeapons[0];
    }

    public void NextRecipeWeapons()
    {
        if(!craftingRecipeSO)
        {
            //Pone la primera receta de la lista si no hay ninguna mostrandose

            craftingRecipeSO = craftingRecipeSOListWeapons[0];
        }
        else
        {
            //Aumenta en una posición la lista para mostrar la siguiente receta

            int index = craftingRecipeSOListWeapons.IndexOf(craftingRecipeSO);
            index = (index + 1) % craftingRecipeSOListWeapons.Count;
            craftingRecipeSO = craftingRecipeSOListWeapons[index];
        }

        recipeImage.sprite = craftingRecipeSO.sprite;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeSelection : Interactable
{
    public Image recipeImage;
    public RecipeCrafting recipeCrafting;
    public List<CraftingRecipeSO> craftingRecipeSOListWeapons;
    [SerializeField] private List<CraftingRecipeSO> craftingRecipeSOListTools;
    [SerializeField] private TextMeshProUGUI page;
    
    [HideInInspector] public CraftingRecipeSO craftingRecipeSO;
    
    private void Awake()
    {
        craftingRecipeSO = craftingRecipeSOListWeapons[0];
        recipeCrafting.craftingRecipeSO = craftingRecipeSO;
        recipeCrafting.craftImage.sprite = recipeImage.sprite;
        page.text = "1 / " + craftingRecipeSOListWeapons.Count;
    }

    public void NextRecipeWeapons()
    {
        if(!craftingRecipeSO)
        {
            //Pone la primera receta de la lista si no hay ninguna mostrandose

            craftingRecipeSO = craftingRecipeSOListWeapons[0];
            recipeCrafting.craftingRecipeSO = craftingRecipeSO;
        }
        else
        {
            //Aumenta en una posición la lista para mostrar la siguiente receta

            int index = craftingRecipeSOListWeapons.IndexOf(craftingRecipeSO);
            index = (index + 1) % craftingRecipeSOListWeapons.Count;
            craftingRecipeSO = craftingRecipeSOListWeapons[index];
            recipeCrafting.craftingRecipeSO = craftingRecipeSO;
            page.text = (index+1) + " / " + craftingRecipeSOListWeapons.Count;
        }

        recipeImage.sprite = craftingRecipeSO.sprite;
        recipeCrafting.craftImage.sprite = recipeImage.sprite;
    }
}

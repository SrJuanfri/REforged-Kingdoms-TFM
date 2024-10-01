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
        // Initialize with the first recipe in the list
        if (craftingRecipeSOListWeapons.Count > 0)
        {
            craftingRecipeSO = craftingRecipeSOListWeapons[0];
            recipeCrafting.craftingRecipeSO = craftingRecipeSO;
            recipeCrafting.craftImage.sprite = recipeImage.sprite;
            UpdatePageText();
        }
    }

    public void NextRecipeWeapons()
    {
        if (craftingRecipeSOListWeapons.Count == 0) return; // Avoid errors if the list is empty

        if (!craftingRecipeSO)
        {
            // Sets the first recipe if none is being displayed
            craftingRecipeSO = craftingRecipeSOListWeapons[0];
        }
        else
        {
            // Move to the next recipe in the list
            int index = craftingRecipeSOListWeapons.IndexOf(craftingRecipeSO);
            index = (index + 1) % craftingRecipeSOListWeapons.Count; // Loop back to the start
            craftingRecipeSO = craftingRecipeSOListWeapons[index];
        }

        recipeCrafting.craftingRecipeSO = craftingRecipeSO;
        recipeImage.sprite = craftingRecipeSO.sprite;
        recipeCrafting.craftImage.sprite = recipeImage.sprite;
        UpdatePageText();
    }

    // Method to add a new recipe and update UI
    public void AddRecipe(CraftingRecipeSO newRecipe)
    {
        if (newRecipe == null) return;

        // Add the new recipe to the list
        craftingRecipeSOListWeapons.Add(newRecipe);

        // If there was no recipe before, set the current recipe to the newly added one
        if (craftingRecipeSO == null)
        {
            craftingRecipeSO = newRecipe;
            recipeCrafting.craftingRecipeSO = craftingRecipeSO;
            recipeImage.sprite = craftingRecipeSO.sprite;
            recipeCrafting.craftImage.sprite = recipeImage.sprite;
        }

        // Update the page text
        UpdatePageText();
    }

    // Helper method to update the page number text
    private void UpdatePageText()
    {
        if (craftingRecipeSOListWeapons.Count == 0)
        {
            page.text = "0 / 0";
        }
        else
        {
            int currentIndex = craftingRecipeSOListWeapons.IndexOf(craftingRecipeSO) + 1;
            page.text = currentIndex + " / " + craftingRecipeSOListWeapons.Count;
        }
    }
}

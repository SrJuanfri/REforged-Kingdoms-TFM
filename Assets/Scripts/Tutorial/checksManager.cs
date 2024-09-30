using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class checksManager : MonoBehaviour
{
    [SerializeField] GameObject check1;
    [SerializeField] GameObject check2;
    [SerializeField] GameObject check3;
    [SerializeField] GameObject check4;
    [SerializeField] GameObject check5;
    [SerializeField] GameObject check6;

    [SerializeField] Newspaper newspaper;
    [SerializeField] RecipeSelection recipeSelection;
    [SerializeField] RecipeCrafting recipeCrafting;
    [SerializeField] DestroyAndReplace destroyAndReplace;
    [SerializeField] MoneyCheck moneyCheck;

    // Start is called before the first frame update
    void Start()
    {
        check1.SetActive(false);
        check2.SetActive(false);
        check3.SetActive(false);
        check4.SetActive(false);
        check5.SetActive(false);
        check6.SetActive(false);

        moneyCheck.DesactivateDoors();
    }

    // Update is called once per frame
    void Update()
    {
        if (newspaper.getIsReading())
        {
            check1.SetActive(true);
        }

        if (recipeSelection.craftingRecipeSO.name.Contains("MiniDagger"))
        {
            check2.SetActive(true);
        }
        else
        {
            check2.SetActive(false);
        }

        if (recipeCrafting.GetItemsInCollider().Count > 1 && CheckForMetalAndWoodItems(recipeCrafting.GetItemsInCollider()))
        {
            check3.SetActive(true);
        }

        if (recipeCrafting.GetLastCreatedItem() != null && recipeCrafting.GetLastCreatedItem().inputItem.name.Contains("Mini"))
        {
            check4.SetActive(true);
        }
        if (destroyAndReplace.replaced)
        {
            check5.SetActive(true);
        }
        if (moneyCheck.checkedMoney)
        {
            check6.SetActive(true);
        }
        if (check1.activeInHierarchy && check2.activeInHierarchy && check3.activeInHierarchy 
            && check4.activeInHierarchy && check5.activeInHierarchy && check6.activeInHierarchy)
        {
            moneyCheck.ActivateDoors();
        }
    }

    public bool CheckForMetalAndWoodItems(List<ItemSO> itemSOs)
    {
        // Flags para verificar si tenemos los tipos Metal y Wood
        bool hasMetal = false;
        bool hasWood = false;

        // Recorremos todos los items para comprobar si hay de tipo Metal y Wood
        foreach (ItemSO item in itemSOs)
        {
            if (item.itemType == ItemSO.ItemType.Metal && item.name.Contains("C"))
            {
                hasMetal = true;
            }
            else if (item.itemType == ItemSO.ItemType.Wood)
            {
                hasWood = true;
            }

            // Si ya tenemos ambos, no necesitamos seguir buscando
            if (hasMetal && hasWood)
            {
                break;
            }
        }

        // Verificamos si ambos tipos de items están presentes
        if (hasMetal && hasWood)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}

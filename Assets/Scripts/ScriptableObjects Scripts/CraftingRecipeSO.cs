using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "ScriptableObjects/CraftingRecipe", order = 1)]
public class CraftingRecipeSO : ScriptableObject
{
    public Sprite sprite;
    public List<ItemSO> inputItemSOList;
    public WeaponOrToolSO outputItemSO;
    public bool isWeapon;

    // Campo para el precio base del diseño
    public int designBasePrice;

    // Propiedad para calcular el precio total
    public int TotalPrice
    {
        get
        {
            int total = designBasePrice; // Incluir el precio base del diseño
            foreach (var item in inputItemSOList)
            {
                total += item.value;
            }
            outputItemSO.value = total;
            return total;
        }
    }

    // Propiedad para obtener los nombres de los materiales
    public Dictionary<string, string> MaterialNames
    {
        get
        {
            string metalName = "metal desconocido";
            string woodName = "madera desconocida";

            foreach (var item in inputItemSOList)
            {
                if (item.itemType == ItemSO.ItemType.Metal)
                {
                    metalName = item.itemName;
                }
                else if (item.itemType == ItemSO.ItemType.Wood)
                {
                    woodName = item.itemName;
                }
            }

            return new Dictionary<string, string> {
                { "metal", metalName },
                { "wood", woodName }
            };
        }
    }
}

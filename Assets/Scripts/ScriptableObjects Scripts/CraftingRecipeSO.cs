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
    public Dictionary<string, HashSet<string>> MaterialNames
    {
        get
        {
            HashSet<string> metalNames = new HashSet<string>();
            HashSet<string> woodNames = new HashSet<string>();

            foreach (var item in inputItemSOList)
            {
                if (item.itemType == ItemSO.ItemType.Metal)
                {
                    metalNames.Add(item.itemName); // Añade si no está en el HashSet
                }
                else if (item.itemType == ItemSO.ItemType.Wood)
                {
                    woodNames.Add(item.itemName); // Añade si no está en el HashSet
                }
            }

            return new Dictionary<string, HashSet<string>> {
            { "metals", metalNames },
            { "woods", woodNames }
        };
        }
    }

}

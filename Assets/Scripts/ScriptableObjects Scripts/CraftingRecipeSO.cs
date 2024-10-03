using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "ScriptableObjects/CraftingRecipe", order = 1)]
public class CraftingRecipeSO : ScriptableObject
{
    public Sprite sprite;
    public bool isWeapon;

    // Campo para el precio base del diseño
    public int designBasePrice;

    // Clase para contener las combinaciones de materiales y el item de salida específico
    [System.Serializable]
    public class MaterialCombination
    {
        public List<ItemSO> materials; // Lista de materiales
        public WeaponOrToolSO outputItemSO; // Item de salida específico para esta combinación
    }

    // Lista de posibles combinaciones de materiales
    [Tooltip("Cada entrada es una combinación de materiales posible con un item de salida específico.")]
    public List<MaterialCombination> materialCombinations;  // Lista de combinaciones de materiales

    // Propiedad para calcular el precio total basado en una combinación activa
    public int TotalPrice
    {
        get
        {
            int total = designBasePrice; // Incluir el precio base del diseño
            if (materialCombinations.Count > 0)
            {
                // Se asume que estás calculando el precio de la primera combinación, pero puedes ajustarlo
                foreach (var item in materialCombinations[0].materials)
                {
                    total += item.value;
                }
            }
            return total;
        }
    }

    // Propiedad para obtener los nombres de los materiales de la combinación activa
    public Dictionary<string, HashSet<string>> MaterialNames
    {
        get
        {
            HashSet<string> metalNames = new HashSet<string>();
            HashSet<string> woodNames = new HashSet<string>();

            if (materialCombinations.Count > 0)
            {
                // Utilizar la primera combinación como ejemplo (puedes ajustarlo según el uso en tu proyecto)
                foreach (var item in materialCombinations[0].materials)
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
            }

            return new Dictionary<string, HashSet<string>> {
                { "metals", metalNames },
                { "woods", woodNames }
            };
        }
    }
}

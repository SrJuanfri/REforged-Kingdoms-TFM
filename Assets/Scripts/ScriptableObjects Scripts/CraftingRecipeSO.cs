using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "ScriptableObjects/CraftingRecipe", order = 1)]
public class CraftingRecipeSO : ScriptableObject
{
    public Sprite sprite;
    public bool isWeapon;

    // Campo para el precio base del dise�o
    public int designBasePrice;

    // Clase para contener las combinaciones de materiales y el item de salida espec�fico
    [System.Serializable]
    public class MaterialCombination
    {
        public List<ItemSO> materials; // Lista de materiales
        public WeaponOrToolSO outputItemSO; // Item de salida espec�fico para esta combinaci�n
    }

    // Lista de posibles combinaciones de materiales
    [Tooltip("Cada entrada es una combinaci�n de materiales posible con un item de salida espec�fico.")]
    public List<MaterialCombination> materialCombinations;  // Lista de combinaciones de materiales

    // Propiedad para calcular el precio total basado en una combinaci�n activa
    public int TotalPrice
    {
        get
        {
            int total = designBasePrice; // Incluir el precio base del dise�o
            if (materialCombinations.Count > 0)
            {
                // Se asume que est�s calculando el precio de la primera combinaci�n, pero puedes ajustarlo
                foreach (var item in materialCombinations[0].materials)
                {
                    total += item.value;
                }
            }
            return total;
        }
    }

    // Propiedad para obtener los nombres de los materiales de la combinaci�n activa
    public Dictionary<string, HashSet<string>> MaterialNames
    {
        get
        {
            HashSet<string> metalNames = new HashSet<string>();
            HashSet<string> woodNames = new HashSet<string>();

            if (materialCombinations.Count > 0)
            {
                // Utilizar la primera combinaci�n como ejemplo (puedes ajustarlo seg�n el uso en tu proyecto)
                foreach (var item in materialCombinations[0].materials)
                {
                    if (item.itemType == ItemSO.ItemType.Metal)
                    {
                        metalNames.Add(item.itemName); // A�ade si no est� en el HashSet
                    }
                    else if (item.itemType == ItemSO.ItemType.Wood)
                    {
                        woodNames.Add(item.itemName); // A�ade si no est� en el HashSet
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

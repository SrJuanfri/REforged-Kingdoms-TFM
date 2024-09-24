using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{
    [Header("Monedas Disponibles")]
    [SerializeField] private ItemSO coin1;
    [SerializeField] private ItemSO coin5;
    [SerializeField] private ItemSO coin10;
    [SerializeField] private ItemSO coin15;
    [SerializeField] private ItemSO coin30;
    [SerializeField] private ItemSO coin50;
    [SerializeField] private ItemSO coin100;

    private Dictionary<int, ItemSO> coins = new Dictionary<int, ItemSO>();

    private void Awake()
    {
        // Inicializa el diccionario con los valores y los ítems correspondientes
        coins.Add(1, coin1);
        coins.Add(5, coin5);
        coins.Add(10, coin10);
        coins.Add(15, coin15);
        coins.Add(30, coin30);
        coins.Add(50, coin50);
        coins.Add(100, coin100);
    }

    public ItemSO GetCoinByValue(int value)
    {
        if (coins.TryGetValue(value, out ItemSO coin))
        {
            return coin;
        }
        else
        {
            Debug.LogWarning($"No coin found for value: {value}");
            return null;
        }
    }

    public List<ItemSO> GetCoinsForValue(int amount)
    {
        List<ItemSO> result = new List<ItemSO>();
        int[] denominations = { 100, 50, 30, 15, 10, 5, 1 };

        foreach (int denom in denominations)
        {
            while (amount >= denom)
            {
                ItemSO coin = GetCoinByValue(denom);
                if (coin != null)
                {
                    result.Add(coin);
                    amount -= denom;
                }
            }
            if (amount <= 0)
                break;
        }

        if (amount > 0)
        {
            Debug.LogWarning("Not enough denominations to cover the amount.");
        }

        return result;
    }
}

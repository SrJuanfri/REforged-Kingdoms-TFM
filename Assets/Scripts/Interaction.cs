using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interaction : Interactable
{
    [SerializeField] private BoxCollider placeItemsAreaCollider;
    [SerializeField] private Transform itemSpawnPoint;
    public CraftingRecipeSO craftingRecipeSO;

    private Animator animator;
    private string sellText;
    public string endText;
    public ChatBubble.IconType emotion;

    private CustomerPhraseManager phraseManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        phraseManager = FindObjectOfType<CustomerPhraseManager>(); // Aseg�rate de que el CustomerPhraseManager est� en la escena
    }

    public void SetCustomerState(CustomerState state)
    {
        // Obtener el CraftingRecipeSO de la orden actual
        CraftingRecipeSO craftingRecipeSO = GetComponent<ClientSOHolder>().ClientSO.currentOrder.craftingRecipe;

        // Obtener el índice de la combinación de materiales especificado en la orden actual
        int combinationIndex = GetComponent<ClientSOHolder>().ClientSO.currentOrder.materialCombinationIndex;

        // Verificar que el índice esté dentro de los límites de las combinaciones disponibles
        if (combinationIndex < 0 || combinationIndex >= craftingRecipeSO.materialCombinations.Count)
        {
            Debug.LogError("Índice de combinación fuera de los límites en SetCustomerState.");
            return;
        }

        // Obtener el item de salida y los materiales de la combinación seleccionada
        var selectedCombination = craftingRecipeSO.materialCombinations[combinationIndex];
        string item = selectedCombination.outputItemSO.itemName;

        // Filtrar los metales y maderas de la combinación seleccionada
        List<string> metals = selectedCombination.materials
            .Where(material => material.itemType == ItemSO.ItemType.Metal)
            .Select(material => material.itemName)
            .ToList();

        List<string> woods = selectedCombination.materials
            .Where(material => material.itemType == ItemSO.ItemType.Wood)
            .Select(material => material.itemName)
            .ToList();

        // Si no hay metales o maderas, usar "desconocido" como fallback
        if (metals.Count == 0) metals.Add("desconocido");
        if (woods.Count == 0) woods.Add("desconocido");

        // Obtener y asignar la frase de pedido usando las listas de metales y maderas
        sellText = phraseManager.GetOrderPhrase(state, item, metals, woods);

        // Obtener y asignar la frase de despedida
        endText = phraseManager.GetFarewellPhrase(state);

        // Actualizar orderText en CustomerManager
        CustomerManager customerManager = GetComponent<ClientSOHolder>().ClientSO;

        if (customerManager.currentOrder != null)
        {
            customerManager.currentOrder.OrderText = sellText;
        }
    }


    public void InteractNPC()
    {
        ChatBubble.Create(transform, new Vector3(-0.6f, 1.7f, 0f), emotion, sellText);
        animator.SetTrigger("Talk");
    }

    /*public void SellNPC()
    {
        Debug.Log("Sell");

        // Detectar �tems en el �rea del collider
        Collider[] colliderArray = Physics.OverlapBox(transform.position + placeItemsAreaCollider.center, placeItemsAreaCollider.size / 2);

        // Lista de �tems requeridos para la receta
        List<ItemSO> inputItemList = new List<ItemSO>(craftingRecipeSO.inputItemSOList);

        // Lista de �tems consumidos
        List<GameObject> consumeItemGameObjectList = new List<GameObject>();

        // Verificar qu� �tems se han entregado
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent(out ItemSOHolder itemSOHolder))
            {
                if (inputItemList.Contains(itemSOHolder.itemSO))
                {
                    // Remover �tem de la lista de �tems requeridos
                    inputItemList.Remove(itemSOHolder.itemSO);
                    // Agregar �tem a la lista de �tems a consumir
                    consumeItemGameObjectList.Add(collider.gameObject);
                }
            }
        }

        // Verificar si todos los �tems requeridos han sido entregados
        if (inputItemList.Count == 0)
        {
            Debug.Log("Todos los �tems requeridos han sido entregados.");
       
            // Instanciar el objeto de salida
            Transform spawnedItemTransform = Instantiate(craftingRecipeSO.outputItemSO.prefab, itemSpawnPoint.position, itemSpawnPoint.rotation);

            // Destruir los �tems consumidos
            foreach (GameObject consumeItemGameObject in consumeItemGameObjectList)
            {
                Destroy(consumeItemGameObject);
            }

            // Crear burbuja de chat para despedida
            ChatBubble.Create(transform, new Vector3(-0.6f, 1.7f, 0f), emotion, endText);

            // Hacer que el cliente se marche
            GetComponent<CustomerController>().Leave();
        }
        else
        {
            Debug.Log("Faltan ítems para completar la receta.");
        }
    }*/

}

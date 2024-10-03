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
        // Obtener valores de CraftingRecipeSO
        string item = craftingRecipeSO.outputItemSO.itemName;

        // Obtener los nombres de los materiales como listas
        Dictionary<string, HashSet<string>> materials = craftingRecipeSO.MaterialNames;
        List<string> metals = materials.ContainsKey("metals") ? materials["metals"].ToList() : new List<string> { "desconocido" };
        List<string> woods = materials.ContainsKey("woods") ? materials["woods"].ToList() : new List<string> { "desconocido" };

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

    public void SellNPC()
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
    }

}

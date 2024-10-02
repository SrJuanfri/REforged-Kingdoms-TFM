using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CustomerStateHandler))]
public class CustomerController : Interactable
{
    [Header("Interaction Settings")]
    [SerializeField] private BoxCollider placeItemsAreaCollider;
    [SerializeField] private Transform itemSpawnPoint;

    [Header("Chat Settings")]
    private string sellText;
    private string endText;
    public ChatBubble.IconType emotion;

    [Header("Navigation Settings")]
    public Transform shopDestination;
    public float walkingSpeed = 1.5f;

    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private Transform player;
    private CustomerPhraseManager phraseManager;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private PlayerPickUpDrop playerPickUpDrop;
    private Bank bank;
    private CustomerStateHandler customerStateHandler;
    private CustomerManager customerManager;

    public State currentState { get; private set; }
    public bool shouldGoToShop { get; set; } = false;
    public bool shouldReturnToStart { get; set; } = false;

    public event Action<CustomerController> OnCustomerReachedShop;

    [Header("Character Voice")]
    private CharacterVoice characterVoice;  // Referencia al script CharacterVoice

    // Referencia a la burbuja de chat activa
    private ChatBubble activeChatBubble;

    // Referencia al CapsuleCollider
    private CapsuleCollider capsuleCollider;

    public enum State
    {
        Idle,
        GoToShop,
        Shopping,
        ReturnToStart
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        phraseManager = FindObjectOfType<CustomerPhraseManager>();
        startPosition = transform.position;
        startRotation = transform.rotation; // Capture the initial rotation
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent.speed = walkingSpeed;
        customerStateHandler = GetComponent<CustomerStateHandler>();
        customerManager = GetComponent<ClientSOHolder>().ClientSO;

        // Establecer la emoción inicial en Neutral
        emotion = ChatBubble.IconType.Neutral;

        // Obtener el CharacterVoice
        characterVoice = GetComponent<CharacterVoice>();

        // Obtener el CapsuleCollider del cliente
        capsuleCollider = GetComponent<CapsuleCollider>();

        SetState(State.Idle);
    }


    private void Start()
    {
        playerPickUpDrop = FindObjectOfType<PlayerPickUpDrop>();
        bank = FindObjectOfType<Bank>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.GoToShop:
                UpdateGoToShop();
                break;
            case State.Shopping:
                UpdateShopping();
                break;
            case State.ReturnToStart:
                UpdateReturnToStart();
                break;
        }

        if (navMeshAgent.velocity.sqrMagnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);
        }

        if (navMeshAgent.velocity.sqrMagnitude > 0.1f)
        {
            // Si el cliente se está moviendo, activa la animación de caminar
            animator.SetBool("Walk", true);
            transform.rotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);
        }
        else
        {
            // Si el cliente está quieto, desactiva la animación de caminar
            animator.SetBool("Walk", false);
        }

    }

    private void SetState(State newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case State.Idle:
                EnterIdle();
                break;
            case State.GoToShop:
                EnterGoToShop();
                break;
            case State.Shopping:
                EnterShopping();
                break;
            case State.ReturnToStart:
                EnterReturnToStart();
                break;
        }
    }

    private void EnterIdle()
    {
        animator.SetBool("Walk", false);
    }

    private void UpdateIdle()
    {
        if (shouldGoToShop)
        {
            SetState(State.GoToShop);
        }
    }

    private void EnterGoToShop()
    {
        navMeshAgent.destination = shopDestination.position;
        animator.SetBool("Walk", true);
    }

    private void UpdateGoToShop()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            SetState(State.Shopping);

            // Notificar que el cliente ha llegado a la tienda
            OnCustomerReachedShop?.Invoke(this);
        }
    }

    private void EnterShopping()
    {
        animator.SetBool("Walk", false);
        transform.rotation = shopDestination.rotation;
    }

    private void UpdateShopping()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private void EnterReturnToStart()
    {
        navMeshAgent.destination = startPosition;
        animator.SetBool("Walk", true);
    }

    private void UpdateReturnToStart()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            // Cuando llega al punto de inicio, desactiva la animación de caminar
            animator.SetBool("Walk", false);
            transform.rotation = startRotation;
            SetState(State.Idle);
            shouldGoToShop = false;
        }
        else
        {
            // Si aún está caminando, asegúrate de que la animación de caminar esté activa
            animator.SetBool("Walk", true);
        }
    }

    public void InteractNPC()
    {
        // Asegúrate de actualizar el sellText antes de interactuar
        UpdateSellText();

        if (!IsPlayerHoldingItem())
        {
            StartCoroutine(DisplayTextWithVoice(sellText));  // Muestra el texto y reproduce la voz
            animator.SetTrigger("Talk");
        }
    }


    private IEnumerator DisplayTextWithVoice(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogError("DisplayTextWithVoice: text is null or empty!");
            yield break; // Salir del coroutine si el texto es nulo o vacío
        }

        // Desactivar el CapsuleCollider
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = false;
        }

        // Reproducir la voz del cliente
        if (characterVoice != null)
        {
            characterVoice.PlayVoice();
        }

        // Crear la burbuja de chat
        activeChatBubble = ChatBubble.Create(transform, new Vector3(-0.6f, 1.7f, 0f), emotion, text);

        // Esperar hasta que termine la frase
        yield return new WaitForSeconds(text.Length * 0.1f);  // Ajustar la duración según la longitud del texto

        // Reactivar el CapsuleCollider
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = true;
        }
    }

    // Método para destruir la burbuja de chat cuando el cliente se va
    public void DestroyChatBubble()
    {
        if (activeChatBubble != null)
        {
            Destroy(activeChatBubble);
            activeChatBubble = null; // Resetear la referencia
        }
    }


    public bool IsPlayerHoldingItem()
    {
        return playerPickUpDrop != null && playerPickUpDrop.GetHeldObject() != null;
    }


    public bool CheckEventOrderCompletion(EventOrder eventOrder)
    {
        if (playerPickUpDrop == null || customerManager == null)
        {
            Debug.LogError("PlayerPickUpDrop or CustomerManager reference is missing.");
            return false;
        }

        ObjectGrabbable heldObject = playerPickUpDrop.GetHeldObject();
        if (heldObject == null)
        {
            Debug.LogWarning("The player is not holding any valid object.");
            return false;
        }

        WeaponOrToolSOHolder holder = heldObject.GetComponentInChildren<WeaponOrToolSOHolder>();
        if (holder == null || holder.weaponOrToolSO == null)
        {
            Debug.LogWarning("The held object does not have a valid WeaponOrToolSOHolder or WeaponOrToolSO.");
            return false;
        }

        WeaponOrToolSO heldWeaponOrTool = holder.weaponOrToolSO;

        // Verificar si el EventOrder coincide con la orden actual
        int currentOrderIndex = customerManager.ordersData.IndexOf(customerManager.currentOrder);
        if (eventOrder == null || eventOrder.orderIndex != currentOrderIndex)
        {
            Debug.Log("No event associated with this order.");
            return false; // No hay evento para este pedido
        }

        // Verificar si el ítem entregado es un arma
        bool isWeapon = heldWeaponOrTool.itemType == WeaponOrToolSO.ItemType.Weapon;

        // Verificar si el tipo de ítem entregado coincide con el esperado en la orden
        if (heldWeaponOrTool.itemType == customerManager.currentOrder.craftingRecipe.outputItemSO.itemType)
        {
            // Pedido completado correctamente con el ítem correcto
            customerManager.currentOrder.IsCompleted = true;

            if (isWeapon)
            {
                Debug.Log($"Evento completado con un arma: {heldWeaponOrTool.itemName}. Frase del evento: {eventOrder.eventPhrase}");
                return true; // Pedido completado con un arma
            }
            else
            {
                Debug.Log("El pedido fue completado con una herramienta.");
                return false; // Se completó con una herramienta, pero no es un arma
            }
        }

        Debug.Log("El ítem entregado no coincide con el tipo esperado en el pedido.");
        return false;
    }


    public void SellNPC()
    {
        if (playerPickUpDrop == null || bank == null)
        {
            Debug.LogError("PlayerPickUpDrop or Bank reference is missing.");
            return;
        }

        ObjectGrabbable heldObject = playerPickUpDrop.GetHeldObject();
        if (heldObject == null)
        {
            Debug.Log("Player is not holding a valid object.");
            return;
        }

        WeaponOrToolSOHolder holder = heldObject.GetComponentInChildren<WeaponOrToolSOHolder>();
        if (holder == null)
        {
            Debug.Log("The held object does not have a WeaponOrToolSOHolder component.");
            return;
        }

        WeaponOrToolSO heldWeaponOrTool = holder.weaponOrToolSO;
        if (heldWeaponOrTool == null)
        {
            Debug.Log("The held object does not have a WeaponOrToolSO.");
            return;
        }

        ItemMatchResult result = CheckHeldItemMatch();
        if (result.AllMatch)
        {
            int itemValue = heldWeaponOrTool.value;
            List<ItemSO> coinsToInstantiate = GetCoinsForValue(itemValue);
            Debug.Log(coinsToInstantiate.Count);
            if (coinsToInstantiate != null)
            {
                foreach (ItemSO coin in coinsToInstantiate)
                {
                    Instantiate(coin.prefab, itemSpawnPoint.position, itemSpawnPoint.rotation);
                }
            }
            else
            {
                Debug.LogWarning("No coins found for value: " + itemValue);
            }

            customerStateHandler.UpdateIndicatorsAndState(heldWeaponOrTool.itemType.ToString(), "Bien Hecha");
        }
        else
        {
            ProcessNonMatchingItem(result);
        }

        // Asumimos que tienes una lista de EventOrders en el CustomerManager
        foreach (EventOrder eventOrder in customerManager.eventOrders)
        {
            if (CheckEventOrderCompletion(eventOrder))
            {
                //Debug.Log("Pedido de evento completado correctamente con un arma.");
                Newspaper newspaper = FindFirstObjectByType<Newspaper>();
                newspaper.UpdateEventInfo(eventOrder.eventPhrase); // Cambiar el texto del periódico
            }
        }


        Destroy(heldObject.gameObject);
        UpdateEndText();
        ChatBubble.Create(transform, new Vector3(-0.6f, 1.7f, 0f), emotion, endText);
        Leave();
    }

    private void ProcessNonMatchingItem(ItemMatchResult result)
    {
        CraftingRecipeSO craftingRecipeSO = GetComponent<ClientSOHolder>().ClientSO.currentOrder.craftingRecipe;
        if (result.TypeMatch && !result.MetalMatch && !result.WoodMatch)
        {
            customerStateHandler.UpdateIndicatorsAndState(craftingRecipeSO.outputItemSO.itemType.ToString(), "Mal Hecha (Diseño y Material)");
        }
        else if (result.TypeMatch && result.MetalMatch && !result.WoodMatch)
        {
            customerStateHandler.UpdateIndicatorsAndState(craftingRecipeSO.outputItemSO.itemType.ToString(), "Mal Hecha (Material)");
        }
        else if (result.TypeMatch && !result.MetalMatch && result.WoodMatch)
        {
            customerStateHandler.UpdateIndicatorsAndState(craftingRecipeSO.outputItemSO.itemType.ToString(), "Mal Hecha (Material)");
        }
        else if (!result.TypeMatch && result.MetalMatch && !result.WoodMatch)
        {
            customerStateHandler.UpdateIndicatorsAndState(craftingRecipeSO.outputItemSO.itemType.ToString(), "Rechazada");
        }
        else if (!result.TypeMatch && !result.MetalMatch && result.WoodMatch)
        {
            customerStateHandler.UpdateIndicatorsAndState(craftingRecipeSO.outputItemSO.itemType.ToString(), "Rechazada");
        }
        else if (!result.TypeMatch && result.MetalMatch && result.WoodMatch)
        {
            customerStateHandler.UpdateIndicatorsAndState(craftingRecipeSO.outputItemSO.itemType.ToString(), "Rechazada");
        }
        else if (!result.TypeMatch && !result.MetalMatch && !result.WoodMatch)
        {
            customerStateHandler.UpdateIndicatorsAndState(craftingRecipeSO.outputItemSO.itemType.ToString(), "Rechazada");
        }
        else
        {
            Debug.Log("Unhandled combination of match results.");
        }
    }

    public void UpdateSellText()
    {
        CraftingRecipeSO craftingRecipeSO = GetComponent<ClientSOHolder>().ClientSO.currentOrder.craftingRecipe;
        string item = craftingRecipeSO.outputItemSO.itemName;
        Dictionary<string, string> materials = craftingRecipeSO.MaterialNames;
        string metal = materials.ContainsKey("metal") ? materials["metal"] : "desconocido";
        string wood = materials.ContainsKey("wood") ? materials["wood"] : "desconocido";

        int currentOrderIndex = customerManager.ordersData.IndexOf(customerManager.currentOrder);

        sellText = customerManager.GetEventPhraseForOrder(currentOrderIndex, item, metal, wood) ??
                   phraseManager.GetOrderPhrase(customerStateHandler.GetCurrentCustomerState(), item, metal, wood);

        if (string.IsNullOrEmpty(sellText))
        {
            Debug.LogError("sellText is null or empty in UpdateSellText!");
        }

        if (customerManager.currentOrder != null)
        {
            customerManager.currentOrder.OrderText = sellText;
        }
    }


    private void UpdateEndText()
    {
        endText = phraseManager.GetFarewellPhrase(customerStateHandler.GetCurrentCustomerState());

        if (string.IsNullOrEmpty(endText))
        {
            Debug.LogError("endText is null or empty in UpdateEndText!");
        }
    }


    public void SetEmotion(ChatBubble.IconType newEmotion)
    {
        emotion = newEmotion;
    }

    private List<ItemSO> GetCoinsForValue(int value)
    {
        List<ItemSO> result = new List<ItemSO>();
        int[] coinValues = { 100, 50, 30, 15, 10, 5, 1 };

        foreach (int coinValue in coinValues)
        {
            while (value >= coinValue)
            {
                ItemSO coin = bank.GetCoinByValue(coinValue);
                if (coin != null)
                {
                    result.Add(coin);
                    value -= coinValue;
                }
                else
                {
                    Debug.LogWarning("No coin found for value: " + coinValue);
                    return null;
                }
            }
        }

        return result.Count > 0 ? result : null;
    }

    public ItemMatchResult CheckHeldItemMatch()
    {
        ItemMatchResult result = new ItemMatchResult();
        CraftingRecipeSO craftingRecipeSO = GetComponent<ClientSOHolder>().ClientSO.currentOrder.craftingRecipe;

        if (playerPickUpDrop == null)
        {
            Debug.LogError("PlayerPickUpDrop reference is missing.");
            return result;
        }

        ObjectGrabbable heldObject = playerPickUpDrop.GetHeldObject();
        if (heldObject == null)
        {
            Debug.Log("Player is not holding a valid object.");
            return result;
        }

        WeaponOrToolSOHolder holder = heldObject.GetComponentInChildren<WeaponOrToolSOHolder>();
        if (holder == null)
        {
            Debug.Log("The held object does not have a WeaponOrToolSOHolder component.");
            return result;
        }

        WeaponOrToolSO heldWeaponOrTool = holder.weaponOrToolSO;
        if (heldWeaponOrTool == null)
        {
            Debug.Log("The held object does not have a WeaponOrToolSO.");
            return result;
        }

        if (craftingRecipeSO == null)
        {
            Debug.LogError("CraftingRecipeSO reference is missing.");
            return result;
        }

        WeaponOrToolSO requestedItem = craftingRecipeSO.outputItemSO;

        result.TypeMatch = heldWeaponOrTool.itemType == requestedItem.itemType;
        result.MetalMatch = heldWeaponOrTool.metal == requestedItem.metal;
        result.WoodMatch = heldWeaponOrTool.wood == requestedItem.wood;

        return result;
    }

    [Serializable]
    public class ItemMatchResult
    {
        public bool TypeMatch { get; set; }
        public bool MetalMatch { get; set; }
        public bool WoodMatch { get; set; }
        public bool AllMatch => TypeMatch && MetalMatch && WoodMatch;
    }

    public void Leave()
    {
        if (currentState == State.Shopping)
        {
            DestroyChatBubble(); // Destruir la burbuja de chat cuando el cliente se va
            SetState(State.ReturnToStart);
        }
    }
}

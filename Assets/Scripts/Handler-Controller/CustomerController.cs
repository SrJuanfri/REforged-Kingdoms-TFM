using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : Interactable
{
    [Header("Interaction Settings")]
    [SerializeField] private BoxCollider placeItemsAreaCollider; // Collider para detectar ítems en el área
    [SerializeField] private Transform itemSpawnPoint; // Punto donde se instanciará el ítem resultante
    public CraftingRecipeSO craftingRecipeSO; // Receta de fabricación asociada

    [Header("Chat Settings")]
    private string sellText; // Texto para mostrar al interactuar con el cliente
    private string endText; // Texto de despedida
    public ChatBubble.IconType emotion; // Emoción para la burbuja de chat

    [Header("Navigation Settings")]
    public Transform shopDestination; // Destino del cliente en la tienda
    public float walkingSpeed = 1.5f; // Velocidad de navegación

    private Animator animator; // Componente Animator para animaciones
    private NavMeshAgent navMeshAgent; // Componente NavMeshAgent para navegación
    private Transform player; // Referencia al jugador
    private CustomerPhraseManager phraseManager; // Gestor de frases del cliente
    private Vector3 startPosition; // Posición de inicio del cliente

    private PlayerPickUpDrop playerPickUpDrop;
    private Bank bank;
    public State currentState { get; private set; } // Estado actual del cliente
    public bool shouldGoToShop { get; set; } = false; // Indica si el cliente debe ir a la tienda
    public bool shouldReturnToStart { get; set; } = false; // Indica si el cliente debe regresar al inicio

    // Enum para los estados del cliente
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
        phraseManager = FindObjectOfType<CustomerPhraseManager>(); // Busca el CustomerPhraseManager en la escena
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform; // Encuentra al jugador por el tag
        navMeshAgent.speed = walkingSpeed; // Establece la velocidad del agente de navegación
        SetState(State.Idle);
    }

    private void Start()
    {
        playerPickUpDrop = FindObjectOfType<PlayerPickUpDrop>();
        bank = FindObjectOfType<Bank>();
    }

    [Serializable]
    public class ItemMatchResult
    {
        public bool TypeMatch { get; set; }
        public bool MetalMatch { get; set; }
        public bool WoodMatch { get; set; }
        public bool AllMatch => TypeMatch && MetalMatch && WoodMatch;
    }

    public ItemMatchResult CheckHeldItemMatch()
    {
        ItemMatchResult result = new ItemMatchResult();

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

        // Verificar tipo de ítem
        result.TypeMatch = heldWeaponOrTool.itemType == requestedItem.itemType;

        // Verificar metal y madera
        result.MetalMatch = heldWeaponOrTool.metal == requestedItem.metal;
        result.WoodMatch = heldWeaponOrTool.wood == requestedItem.wood;

        return result;
    }

    private void Update()
    {
        // Actualiza el comportamiento basado en el estado actual
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

        // Rotar el cliente para que mire en la dirección de movimiento
        if (navMeshAgent.velocity.sqrMagnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);
        }
    }

    private void SetState(State newState)
    {
        currentState = newState;

        // Ejecuta acciones específicas al cambiar de estado
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

    // Estado Idle: Cliente inactivo
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

    // Estado GoToShop: Cliente se dirige a la tienda
    private void EnterGoToShop()
    {
        navMeshAgent.destination = shopDestination.position;
        animator.SetBool("Walk", true);
    }

    private void UpdateGoToShop()
    {
        // Verifica si ha llegado a la tienda
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            SetState(State.Shopping);
        }
    }

    // Estado Shopping: Cliente está en la tienda
    private void EnterShopping()
    {
        animator.SetBool("Walk", false);
        transform.rotation = shopDestination.rotation; // Mira hacia la tienda
    }

    private void UpdateShopping()
    {
        // Hace que el cliente mire al jugador
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    // Estado ReturnToStart: Cliente regresa al punto inicial
    private void EnterReturnToStart()
    {
        navMeshAgent.destination = startPosition;
        animator.SetBool("Walk", true);
    }

    private void UpdateReturnToStart()
    {
        // Verifica si ha llegado al punto de inicio
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            SetState(State.Idle);
            shouldGoToShop = false;
        }
    }

    public bool IsPlayerHoldingItem()
    {
        if (playerPickUpDrop == null)
        {
            Debug.LogError("PlayerPickUpDrop reference is missing.");
            return false;
        }

        // Verificar si hay un objeto en las manos del jugador
        return playerPickUpDrop.GetHeldObject() != null;
    }

    // Interactúa con el cliente: Muestra el texto de venta y activa la animación
    public void InteractNPC()
    {
        if (!IsPlayerHoldingItem())
        {
            ChatBubble.Create(transform, new Vector3(-0.6f, 1.7f, 0f), emotion, sellText);
            animator.SetTrigger("Talk");
        }       
    }

    // Configura el estado del cliente y actualiza los textos de interacción
    public void SetCustomerState(CustomerState state)
    {
        string item = craftingRecipeSO.outputItemSO.itemName;
        Dictionary<string, string> materials = craftingRecipeSO.MaterialNames;
        string metal = materials.ContainsKey("metal") ? materials["metal"] : "desconocido";
        string wood = materials.ContainsKey("wood") ? materials["wood"] : "desconocido";

        sellText = phraseManager.GetOrderPhrase(state, item, metal, wood);
        endText = phraseManager.GetFarewellPhrase(state);

        CustomerManager customerManager = GetComponent<ClientSOHolder>().ClientSO;

        if (customerManager.currentOrder != null)
        {
            customerManager.currentOrder.OrderText = sellText;
        }
    }

    // Procesa la venta de ítems y crea el ítem resultante
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
            // Suponiendo que el valor de la moneda está en el WeaponOrToolSO
            int itemValue = heldWeaponOrTool.value; // Asigna el valor de la moneda aquí

            // Obtener la moneda de acuerdo al valor
            List<ItemSO> coinsToInstantiate = GetCoinsForValue(itemValue);
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
        }
        else
        {
            // Diagnóstico detallado
            if (result.TypeMatch && !result.MetalMatch && !result.WoodMatch)
            {
                Debug.Log("Held item type matches, but metal and wood do not match the order.");
            }
            else if (result.TypeMatch && result.MetalMatch && !result.WoodMatch)
            {
                Debug.Log("Held item type and metal match, but wood does not match the order.");
            }
            else if (result.TypeMatch && !result.MetalMatch && result.WoodMatch)
            {
                Debug.Log("Held item type and wood match, but metal does not match the order.");
            }
            else if (!result.TypeMatch && result.MetalMatch && !result.WoodMatch)
            {
                Debug.Log("Held item metal matches, but type and wood do not match the order.");
            }
            else if (!result.TypeMatch && !result.MetalMatch && result.WoodMatch)
            {
                Debug.Log("Held item wood matches, but type and metal do not match the order.");
            }
            else if (!result.TypeMatch && result.MetalMatch && result.WoodMatch)
            {
                Debug.Log("Held item metal and wood match, but type does not match the order.");
            }
            else if (!result.TypeMatch && !result.MetalMatch && !result.WoodMatch)
            {
                Debug.Log("Held item does not match the order at all.");
            }
            else
            {
                Debug.Log("Unhandled combination of match results.");
            }
        }

        // Destruye el objeto que el jugador sostiene (en caso de no pasar la validación)
        Destroy(heldObject.gameObject);

        // Actualiza el texto de despedida según el estado actual del cliente
        UpdateEndText();

        // Muestra el texto de despedida y hace que el cliente se marche
        ChatBubble.Create(transform, new Vector3(-0.6f, 1.7f, 0f), emotion, endText);
        Leave(); // Cambia al estado de irse
    }

    public CustomerState ConvertEmotionToCustomerState(ChatBubble.IconType emotion)
    {
        switch (emotion)
        {
            case ChatBubble.IconType.Happy:
                return CustomerState.Happy;
            case ChatBubble.IconType.Neutral:
                return CustomerState.Neutral;
            case ChatBubble.IconType.Sad:
                return CustomerState.Angry;
            default:
                Debug.LogWarning("Unknown emotion type: " + emotion);
                return CustomerState.Neutral; // Estado por defecto en caso de emoción desconocida
        }
    }

    private void UpdateEndText()
    {
        // Convierte la emoción actual en un CustomerState
        CustomerState state = ConvertEmotionToCustomerState(emotion);

        // Utiliza customerState para obtener el texto de despedida adecuado
        endText = phraseManager.GetFarewellPhrase(state);
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

    // Hace que el cliente se marche
    public void Leave()
    {
        if (currentState == State.Shopping)
        {
            SetState(State.ReturnToStart);
        }
    }
}

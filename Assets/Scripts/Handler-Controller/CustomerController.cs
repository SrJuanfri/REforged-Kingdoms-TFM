using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CustomerStateHandler))]
public class CustomerController : Interactable
{
    [SerializeField] private int apperanceProbability;
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
    private string currentChatText; // Para almacenar el texto actual de la burbuja de chat
    private bool isFarewellTextActive = false; // Indica si la burbuja contiene el texto de despedida

    // Referencia al CapsuleCollider
    private CapsuleCollider capsuleCollider;

    private DayProgressManager dayProgressManager; // Referencia al DayProgressManager

    // Referencia al Rigidbody
    private Rigidbody rb;

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
        dayProgressManager = FindObjectOfType<DayProgressManager>(); // Encuentra el DayProgressManager en la escena

        customerManager.SetAppearanceProbability(apperanceProbability);

        // Establecer la emoción inicial en Neutral
        emotion = ChatBubble.IconType.Neutral;

        // Obtener el CharacterVoice
        characterVoice = GetComponent<CharacterVoice>();

        // Obtener el CapsuleCollider del cliente
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Obtener el Rigidbody del objeto
        rb = GetComponent<Rigidbody>();

        // Asegurarse de que todas las rotaciones estén desbloqueadas al inicio
        rb.constraints = RigidbodyConstraints.None;

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

        // Aquí puedes registrar la asistencia al cliente
        if (currentState == State.Shopping)
        {
            dayProgressManager.RecordClientAttendance(); // Registra la asistencia al cliente
        }
    }

    private void UpdateShopping()
    {
        if (player != null)
        {
            // Bloquear todas las rotaciones mientras se está en UpdateShopping
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            // Solo aplica la rotación si la dirección ha cambiado significativamente
            if (Quaternion.Angle(transform.rotation, lookRotation) > 10f) // Umbral para evitar pequeñas vibraciones
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
        }
        else
        {
            // Desbloquear la rotación en el eje Y y mantener bloqueadas las rotaciones en X y Z
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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

        // Crear la burbuja de chat y guardar el texto actual
        activeChatBubble = ChatBubble.Create(transform, new Vector3(-0.6f, 1.7f, 0f), emotion, text);
        currentChatText = text;

        // Esperar hasta que termine la frase
        yield return new WaitForSeconds(text.Length * 0.1f);  // Ajustar la duración según la longitud del texto

        // Reactivar el CapsuleCollider
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = true;
        }
    }

    // Método para destruir la burbuja de chat si no es de despedida
    public void DestroyChatBubble()
    {
        if (activeChatBubble != null && !isFarewellTextActive)
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
        Debug.Log($"Player is holding a valid item: {heldWeaponOrTool.itemName}, ItemType: {heldWeaponOrTool.itemType}");

        // Verificar si el EventOrder coincide con la orden actual
        int currentOrderIndex = customerManager.ordersData.IndexOf(customerManager.currentOrder);
        Debug.Log($"Current order index: {currentOrderIndex}, EventOrder index: {eventOrder?.orderIndex ?? -1}");

        if (eventOrder == null || eventOrder.orderIndex != currentOrderIndex)
        {
            Debug.Log("No event associated with this order.");
            return false; // No hay evento para este pedido
        }

        // Obtener la combinación de materiales actual de la orden
        CraftingRecipeSO craftingRecipeSO = customerManager.currentOrder.craftingRecipe;
        int combinationIndex = customerManager.currentOrder.materialCombinationIndex;
        Debug.Log($"Material combination index: {combinationIndex}");

        // Verificar que el índice esté dentro de los límites de las combinaciones disponibles
        if (combinationIndex < 0 || combinationIndex >= craftingRecipeSO.materialCombinations.Count)
        {
            Debug.LogError("Índice de combinación fuera de los límites en CheckEventOrderCompletion.");
            return false;
        }

        // Obtener el item de salida correspondiente a la combinación seleccionada
        WeaponOrToolSO expectedItem = craftingRecipeSO.materialCombinations[combinationIndex].outputItemSO;
        Debug.Log($"Expected item: {expectedItem.itemName}, Expected ItemType: {expectedItem.itemType}");

        // Verificar si el ítem entregado es un arma
        bool isWeapon = heldWeaponOrTool.itemType == WeaponOrToolSO.ItemType.Weapon;
        Debug.Log(isWeapon ? "Held item is a weapon." : "Held item is not a weapon.");

        // Verificar si el tipo de ítem entregado coincide con el esperado en la combinación seleccionada
        Debug.Log($"Comparing held item type ({heldWeaponOrTool.itemType}) with expected item type ({expectedItem.itemType})");
        if (heldWeaponOrTool.itemType == expectedItem.itemType)
        {
            customerManager.currentOrder.IsCompleted = true;
            Debug.Log("The item matches the expected type.");

            if (isWeapon)
            {
                Debug.Log($"Evento completado con un arma: {heldWeaponOrTool.itemName}. Frase del evento: {eventOrder.eventPhrase}");
                return true; // Pedido completado con un arma
            }
            else
            {
                Debug.Log("The order was completed with a tool, not a weapon.");
                return false; // Se completó con una herramienta, pero no es un arma
            }
        }

        Debug.LogWarning("The delivered item does not match the expected item type in the order.");
        return false;
    }



    public void SellNPC()
    {
        if (playerPickUpDrop == null || bank == null)
        {
            //.LogError("PlayerPickUpDrop or Bank reference is missing.");
            return;
        }

        ObjectGrabbable heldObject = playerPickUpDrop.GetHeldObject();
        if (heldObject == null)
        {
            //Debug.Log("Player is not holding a valid object.");
            return;
        }

        WeaponOrToolSOHolder holder = heldObject.GetComponentInChildren<WeaponOrToolSOHolder>();
        if (holder == null)
        {
            //Debug.Log("The held object does not have a WeaponOrToolSOHolder component.");
            return;
        }

        WeaponOrToolSO heldWeaponOrTool = holder.weaponOrToolSO;
        if (heldWeaponOrTool == null)
        {
            //Debug.Log("The held object does not have a WeaponOrToolSO.");
            return;
        }

        ItemMatchResult result = CheckHeldItemMatch();
        if (result.AllMatch)
        {
            int itemValue = heldWeaponOrTool.value;
            List<ItemSO> coinsToInstantiate = GetCoinsForValue(itemValue);
            //Debug.Log(coinsToInstantiate.Count);
            if (coinsToInstantiate != null)
            {
                foreach (ItemSO coin in coinsToInstantiate)
                {
                    Instantiate(coin.prefab, itemSpawnPoint.position, itemSpawnPoint.rotation);
                }
            }
            else
            {
                //Debug.LogWarning("No coins found for value: " + itemValue);
            }

            Debug.Log($"Updating indicators and state with ItemType: {heldWeaponOrTool.itemType.ToString()} and Status: 'Bien Hecha'.");
            customerStateHandler.UpdateIndicatorsAndState(heldWeaponOrTool.itemType.ToString(), "Bien Hecha");
            GetComponent<ClientSOHolder>().ClientSO.currentOrder.isCompleted = true;
            dayProgressManager.RecordCorrectOrder();
        }
        else
        {
            ProcessNonMatchingItem(result, heldWeaponOrTool);
            dayProgressManager.RecordIncorrectOrder();
        }

        // Asumimos que tienes una lista de EventOrders en el CustomerManager
        foreach (EventOrder eventOrder in customerManager.eventOrders)
        {
            if (CheckEventOrderCompletion(eventOrder))
            {
                //Debug.Log("Pedido de evento completado correctamente con un arma.");
                Newspaper newspaper = FindFirstObjectByType<Newspaper>();
                newspaper.UpdateEventInfo(eventOrder.GetEventInfo()); // Cambiar el texto del periódico
            }
        }


        Destroy(heldObject.gameObject);
        UpdateEndText();
        ChatBubble.Create(transform, new Vector3(-0.6f, 1.7f, 0f), emotion, endText);
        Leave(false);
    }

    private void ProcessNonMatchingItem(ItemMatchResult result, WeaponOrToolSO heldWeaponOrTool)
    {
        int itemValue = heldWeaponOrTool.value;
        // Obtener el CraftingRecipeSO de la orden actual
        CraftingRecipeSO craftingRecipeSO = GetComponent<ClientSOHolder>().ClientSO.currentOrder.craftingRecipe;

        // Obtener el índice de la combinación de materiales especificado en OrderData
        int combinationIndex = GetComponent<ClientSOHolder>().ClientSO.currentOrder.materialCombinationIndex;

        // Verificar que el índice esté dentro de los límites
        if (combinationIndex < 0 || combinationIndex >= craftingRecipeSO.materialCombinations.Count)
        {
            Debug.LogError("Índice de combinación fuera de los límites en ProcessNonMatchingItem.");
            return;
        }

        // Obtener el item de salida correspondiente a la combinación
        WeaponOrToolSO requestedItem = craftingRecipeSO.materialCombinations[combinationIndex].outputItemSO;

        // Determinar el mensaje en función de las coincidencias
        if (result.TypeMatch && !result.PrefabMatch)
        {
            // Si coincide el diseño (tipo) pero no el material
            customerStateHandler.UpdateIndicatorsAndState(requestedItem.itemType.ToString(), "Mal Hecha (Material)");
            itemValue = (int)Math.Round(heldWeaponOrTool.value / 1.5f, MidpointRounding.ToEven);

        }
        else if (!result.TypeMatch && result.PrefabMatch)
        {
            // Si coincide el material pero el tipo (diseño) es incorrecto
            customerStateHandler.UpdateIndicatorsAndState(requestedItem.itemType.ToString(), "Mal Hecha (Diseño)");
            itemValue = (int)Math.Round(heldWeaponOrTool.value / 2f, MidpointRounding.ToEven);
        }
        else if (!result.TypeMatch && !result.PrefabMatch)
        {
            // Si no coinciden ni el diseño ni el material
            customerStateHandler.UpdateIndicatorsAndState(requestedItem.itemType.ToString(), "Mal Hecha (Diseño y Material)");
            itemValue = (int)Math.Round(heldWeaponOrTool.value / 3f, MidpointRounding.ToEven);
        }
        else
        {
            // En cualquier otro caso, se rechaza
            customerStateHandler.UpdateIndicatorsAndState(requestedItem.itemType.ToString(), "Rechazada");
        }

        List<ItemSO> coinsToInstantiate = GetCoinsForValue(itemValue);
        //Debug.Log(coinsToInstantiate.Count);
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

    public void rechazar()
    {
        // Obtener el CraftingRecipeSO de la orden actual
        CraftingRecipeSO craftingRecipeSO = GetComponent<ClientSOHolder>().ClientSO.currentOrder.craftingRecipe;
        // Obtener el índice de la combinación de materiales especificado en OrderData
        int combinationIndex = GetComponent<ClientSOHolder>().ClientSO.currentOrder.materialCombinationIndex;
        // Obtener el item de salida correspondiente a la combinación
        WeaponOrToolSO requestedItem = craftingRecipeSO.materialCombinations[combinationIndex].outputItemSO;
        customerStateHandler.UpdateIndicatorsAndState(requestedItem.itemType.ToString(), "Rechazada");

        Leave(true);
    }

    public void UpdateSellText()
    {
        // Obtener el CraftingRecipeSO de la orden actual
        CraftingRecipeSO craftingRecipeSO = GetComponent<ClientSOHolder>().ClientSO.currentOrder.craftingRecipe;

        // Obtener el índice de la combinación de materiales especificado en OrderData
        int combinationIndex = GetComponent<ClientSOHolder>().ClientSO.currentOrder.materialCombinationIndex;

        // Verificar que el índice esté dentro de los límites de las combinaciones disponibles
        if (combinationIndex < 0 || combinationIndex >= craftingRecipeSO.materialCombinations.Count)
        {
            Debug.LogError("Índice de combinación fuera de los límites en UpdateSellText!");
            return;
        }

        // Obtener la combinación de materiales especificada (ahora usando la clase MaterialCombination)
        List<ItemSO> selectedCombination = craftingRecipeSO.materialCombinations[combinationIndex].materials;

        // Obtener el item de salida de la combinación seleccionada
        string item = craftingRecipeSO.materialCombinations[combinationIndex].outputItemSO.itemName;

        // Filtrar los metales y maderas de la combinación seleccionada
        List<string> metals = selectedCombination
            .Where(item => item.itemType == ItemSO.ItemType.Metal)
            .Select(item => item.itemName)
            .ToList();

        List<string> woods = selectedCombination
            .Where(item => item.itemType == ItemSO.ItemType.Wood)
            .Select(item => item.itemName)
            .ToList();

        // Si no hay metales o maderas, poner "desconocido" como fallback
        if (metals.Count == 0) metals.Add("desconocido");
        if (woods.Count == 0) woods.Add("desconocida");

        int currentOrderIndex = customerManager.ordersData.IndexOf(customerManager.currentOrder);

        // Usar las listas de metales y maderas en GetOrderPhrase
        sellText = customerManager.GetEventPhraseForOrder(currentOrderIndex, item, metals, woods) ??
                   phraseManager.GetOrderPhrase(customerStateHandler.GetCurrentCustomerState(), item, metals, woods);

        if (string.IsNullOrEmpty(sellText))
        {
            Debug.LogError("sellText is null or empty in UpdateSellText!");
        }

        // Actualizar el texto de la orden actual
        if (customerManager.currentOrder != null)
        {
            customerManager.currentOrder.OrderText = sellText;
        }
    }


    // Coroutine para esperar a que se escriba el texto completo si es de despedida
    private IEnumerator WaitForFarewellTextToComplete()
    {
        // Una vez que el texto esté completamente escrito, podemos esperar un tiempo antes de destruir la burbuja
        yield return new WaitForSeconds(6f); // Esperar 3 segundos adicionales o el tiempo que creas conveniente

        // Destruir la burbuja y hacer que el cliente se vaya
        DestroyChatBubble();
        SetState(State.ReturnToStart);
    }

    private void UpdateEndText()
    {
        // Obtener la frase de despedida del CustomerPhraseManager
        endText = phraseManager.GetFarewellPhrase(customerStateHandler.GetCurrentCustomerState());

        if (string.IsNullOrEmpty(endText))
        {
            Debug.LogError("endText is null or empty in UpdateEndText!");
        }
    }
    private void UpdateInsatisfechoText()
    {
        // Obtener las frases de despedida del estado "Insatisfecho"
        string insatisfechoPhrase = phraseManager.GetFarewellPhrase(CustomerState.Insatisfecho);

        if (string.IsNullOrEmpty(insatisfechoPhrase))
        {
            Debug.LogError("insatisfechoPhrase is null or empty in UpdateInsatisfechoText!");
        }
        else
        {
            endText = insatisfechoPhrase; // Asignar la frase obtenida a la variable endText
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

        // Obtener el índice de la combinación de materiales especificado en la orden actual
        int combinationIndex = GetComponent<ClientSOHolder>().ClientSO.currentOrder.materialCombinationIndex;

        // Verificar que el índice esté dentro de los límites
        if (combinationIndex < 0 || combinationIndex >= craftingRecipeSO.materialCombinations.Count)
        {
            Debug.LogError("Índice de combinación fuera de los límites.");
            return result;
        }

        // Obtener el item solicitado correspondiente a la combinación actual
        WeaponOrToolSO requestedItem = craftingRecipeSO.materialCombinations[combinationIndex].outputItemSO;

        // Comparar el tipo de item usando itemName en minúsculas y sin espacios
        string heldItemName = heldWeaponOrTool.itemName.Replace(" ", "").ToLower();
        string requestedItemName = requestedItem.itemName.Replace(" ", "").ToLower();

        result.TypeMatch = heldItemName == requestedItemName;

        // Comparar si el prefab que sostiene el jugador es una instancia exacta del prefab solicitado
        result.PrefabMatch = GameObject.ReferenceEquals(heldWeaponOrTool.prefab, requestedItem.prefab);

        return result;
    }


    [Serializable]
    public class ItemMatchResult
    {
        public bool TypeMatch { get; set; }
        public bool PrefabMatch { get; set; }

        // El resultado completo será verdadero solo si coinciden el tipo y el prefab
        public bool AllMatch => TypeMatch && PrefabMatch;
    }


    public void Leave(bool rejected)
    {
        // Si el cliente está en el estado de Shopping, proceder con la salida
        if (currentState == State.Shopping)
        {
            // Desbloquear la rotación en el eje Y y mantener bloqueadas las rotaciones en X y Z
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            // Mostrar el texto de despedida si es el momento de irse
            UpdateEndText();
            DisplayFarewellText(rejected); // Mostrar el texto de despedida

            // Verificar si la burbuja tiene el texto de despedida
            if (currentChatText == endText)
            {
                isFarewellTextActive = true;
                StartCoroutine(WaitForFarewellTextToComplete()); // Esperar a que el texto de despedida se complete antes de moverse
            }
            else
            {
                DestroyChatBubble(); // Destruir la burbuja si no es de despedida
                SetState(State.ReturnToStart);
            }
        }
    }

    // Método para mostrar el texto de despedida
    private void DisplayFarewellText(bool rejected)
    {
        if (rejected)
        {
            UpdateInsatisfechoText();
        }
        if (!string.IsNullOrEmpty(endText))
        {
            StartCoroutine(DisplayTextWithVoice(endText));  // Mostrar la burbuja con el texto de despedida
            currentChatText = endText; // Establecer el texto actual como el texto de despedida
        }
    }

}

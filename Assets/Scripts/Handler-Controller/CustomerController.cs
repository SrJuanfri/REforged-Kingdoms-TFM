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
    public CraftingRecipeSO craftingRecipeSO;

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
    private PlayerPickUpDrop playerPickUpDrop;
    private Bank bank;
    private CustomerStateHandler customerStateHandler;

    public State currentState { get; private set; }
    public bool shouldGoToShop { get; set; } = false;
    public bool shouldReturnToStart { get; set; } = false;

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
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent.speed = walkingSpeed;
        customerStateHandler = GetComponent<CustomerStateHandler>();
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
            SetState(State.Idle);
            shouldGoToShop = false;
        }
    }

    public void InteractNPC()
    {
        if (!IsPlayerHoldingItem())
        {
            UpdateSellText();
            ChatBubble.Create(transform, new Vector3(-0.6f, 1.7f, 0f), emotion, sellText);
            animator.SetTrigger("Talk");
        }
    }

    public bool IsPlayerHoldingItem()
    {
        return playerPickUpDrop != null && playerPickUpDrop.GetHeldObject() != null;
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

        Destroy(heldObject.gameObject);
        UpdateEndText();
        ChatBubble.Create(transform, new Vector3(-0.6f, 1.7f, 0f), emotion, endText);
        Leave();
    }

    private void ProcessNonMatchingItem(ItemMatchResult result)
    {
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
        string item = craftingRecipeSO.outputItemSO.itemName;
        Dictionary<string, string> materials = craftingRecipeSO.MaterialNames;
        string metal = materials.ContainsKey("metal") ? materials["metal"] : "desconocido";
        string wood = materials.ContainsKey("wood") ? materials["wood"] : "desconocido";

        sellText = phraseManager.GetOrderPhrase(customerStateHandler.GetCurrentCustomerState(), item, metal, wood);

        CustomerManager customerManager = GetComponent<ClientSOHolder>().ClientSO;

        if (customerManager.currentOrder != null)
        {
            customerManager.currentOrder.OrderText = sellText;
        }
    }

    private void UpdateEndText()
    {
        endText = phraseManager.GetFarewellPhrase(customerStateHandler.GetCurrentCustomerState());
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
            SetState(State.ReturnToStart);
        }
    }
}

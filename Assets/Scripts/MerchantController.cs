using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MerchantController : MonoBehaviour
{
    public Transform shopDestination; // Destino del mercader
    private Transform player; // Referencia al jugador
    public Transform exitPoint; // Punto de salida del mercader

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    [HideInInspector]
    public State currentState;
    private GameObject merchantInventory;

    public delegate void MerchantEventHandler();
    public event MerchantEventHandler OnMerchantLeft;

    public enum State
    {
        GoToShop,
        ShowProducts,
        Leave
    }

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Buscar el jugador por el tag

        merchantInventory = GameObject.FindGameObjectWithTag("MerchantInventory");
        merchantInventory.SetActive(false);
        //SetState(State.GoToShop);
    }

    void Update()
    {
        switch (currentState)
        {
            case State.GoToShop:
                UpdateGoToShop();
                break;
            case State.ShowProducts:
                UpdateShowProducts();
                break;
            case State.Leave:
                UpdateLeave();
                break;
        }

        // Manejar el Animator basado en la velocidad del NavMeshAgent
        //animator.SetFloat("Speed", navMeshAgent.velocity.magnitude);
    }

    public void setStateGoToShop()
    {
        SetState(State.GoToShop);
    }

    private void SetState(State newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case State.GoToShop:
                EnterGoToShop();
                break;
            case State.ShowProducts:
                EnterShowProducts();
                break;
            case State.Leave:
                EnterLeave();
                break;
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
            SetState(State.ShowProducts);
        }
    }

    private void EnterShowProducts()
    {
        animator.SetBool("Walk", false);
        Debug.Log("Merchant is showing products.");
        merchantInventory.SetActive(true);
        StartCoroutine(ShowProductsRoutine());
    }

    private void UpdateShowProducts()
    {
        // Hacer que el mercader mire al jugador
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private IEnumerator ShowProductsRoutine()
    {
        // Mantener esta corrutina activa hasta que se pulse la campana
        while (currentState == State.ShowProducts)
        {
            yield return null;
        }
    }

    public void Leave()
    {
        if (currentState == State.ShowProducts)
        {
            SetState(State.Leave);
        }
    }

    private void EnterLeave()
    {
        //Debug.Log("leaving.");
        navMeshAgent.destination = exitPoint.position;
        animator.SetBool("Walk", true);
        merchantInventory.SetActive(false);
    }

    private void UpdateLeave()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            if (animator.GetBool("Walk"))
            {
                animator.SetBool("Walk", false);
                OnMerchantLeft?.Invoke();
            }
        }
    }
}

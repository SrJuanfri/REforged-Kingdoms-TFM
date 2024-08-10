using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CustomerController : Interactable
{
    public Transform shopDestination;
    private NavMeshAgent navMeshAgent;
    private Vector3 startPosition;
    private Transform player; // Referencia al jugador
    [HideInInspector]
    public State currentState;

    public bool shouldGoToShop = false;
    public bool shouldReturnToStart = false;
    public float walkingSpeed = 1.5f; // Ajusta la velocidad aquí

    private Animator animator;

    public delegate void CustomerEventHandler(CustomerController customer);
    public event CustomerEventHandler OnCustomerReturned;

    [HideInInspector]
    public enum State
    {
        Idle,
        GoToShop,
        Shopping,
        ReturnToStart
    }

    void Start()
    {
        shouldGoToShop = false;
        shouldReturnToStart = false;
        startPosition = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform; // Buscar el jugador por el tag
        navMeshAgent.speed = walkingSpeed; // Establece la velocidad del NavMeshAgent aquí
        animator = GetComponent<Animator>();
        SetState(State.Idle);
    }

    void Update()
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

    public void SetState(State newState)
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
        // Set the Walk boolean in the Animator to false
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
        // Set the Walk boolean in the Animator to true
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
        // Set the Walk boolean in the Animator to false
        animator.SetBool("Walk", false);

        // Rotar el cliente para que mire en la misma dirección que el shopDestination
        transform.rotation = shopDestination.rotation;

        // Optional: Add any logic to handle the shopping behavior
        //StartCoroutine(ShoppingRoutine());
    }

    private void UpdateShopping()
    {
        // Hacer que el cliente mire al jugador
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    /*private IEnumerator ShoppingRoutine()
    {
        // Simulate shopping for a few seconds
        yield return new WaitForSeconds(2.0f);

        if (shouldReturnToStart)
        {
            SetState(State.ReturnToStart);
        }
        else
        {
            SetState(State.Idle);
        }
    }*/

    private void EnterReturnToStart()
    {
        navMeshAgent.destination = startPosition;
        // Set the Walk boolean in the Animator to true
        animator.SetBool("Walk", true);
    }

    public void Leave()
    {
        if (currentState == State.Shopping)
        {
            SetState(State.ReturnToStart);
        }
    }

    private void UpdateReturnToStart()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            SetState(State.Idle);
            shouldGoToShop = false;
            // Notify that the customer has returned
            //OnCustomerReturned?.Invoke(this);
        }
    }
}

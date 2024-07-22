using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CustomerController : MonoBehaviour
{
    public Transform shopDestination;
    private NavMeshAgent navMeshAgent;
    private Vector3 startPosition;
    [HideInInspector]
    public State currentState;

    public bool shouldGoToShop;
    public bool shouldReturnToStart;
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
        startPosition = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
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
        // Optional: Add any update logic for the shopping state
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

    private void UpdateReturnToStart()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            SetState(State.Idle);
            // Notify that the customer has returned
            OnCustomerReturned?.Invoke(this);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CatInteractionTutorial : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private OutlineSelection outlineSelection;
    private NavMeshAgent navMeshAgent;
    private bool isInteracting = false;
    private bool isSitting = true;

    public List<Transform> waypoints;
    private int currentWaypointIndex = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Permitir que el NavMeshAgent controle la rotación mientras se mueve
        navMeshAgent.updateRotation = true;

        outlineSelection = FindObjectOfType<OutlineSelection>();

        if (animator == null) Debug.LogError("No Animator component found on " + gameObject.name);
        if (audioSource == null) Debug.LogError("No AudioSource component found on " + gameObject.name);
        if (outlineSelection == null) Debug.LogError("No OutlineSelection found in the scene.");
        if (navMeshAgent == null) Debug.LogError("No NavMeshAgent component found on " + gameObject.name);

        // Inicialmente, el gato está sentado
        animator.Play("SitDown");
    }

    private void Update()
    {
        // Verificar si el jugador presiona la tecla E y está apuntando al gato
        if (Input.GetKeyDown(KeyCode.E) && IsPlayerPointingAtCat() && !isInteracting)
        {
            InteractWithCat();
        }

        // Verificar si el NavMeshAgent ha llegado al destino
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (isInteracting && !isSitting)
            {
                SitDown();
            }
        }
    }

    private bool IsPlayerPointingAtCat()
    {
        if (outlineSelection.PointedObject != null && outlineSelection.PointedObject.name == "Cat")
        {
            return true;
        }
        return false;
    }

    public void InteractWithCat()
    {
        if (!isInteracting && isSitting)
        {
            StandUpAndMove();
        }
    }

    private void StandUpAndMove()
    {
        isInteracting = true;
        isSitting = false;

        // Permitir que el NavMeshAgent controle la rotación mientras se mueve
        navMeshAgent.updateRotation = true;

        animator.Play("StandUp");

        StartCoroutine(WaitForStandUpAnimation());
    }

    private IEnumerator WaitForStandUpAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        MoveToNextWaypoint();
    }

    private void MoveToNextWaypoint()
    {
        if (waypoints.Count == 0) return;

        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        Transform nextWaypoint = waypoints[currentWaypointIndex];

        // Aquí permitimos que el NavMeshAgent controle la rotación y el movimiento
        navMeshAgent.SetDestination(nextWaypoint.position);
        animator.Play("WalkCycle");

        // Corregir la dirección si el gato está caminando hacia atrás
        Vector3 direction = nextWaypoint.position - transform.position;
        direction.y = 0; // Ignorar la rotación en el eje Y

        // Invertir la rotación 180 grados si el gato camina hacia atrás
        if (Vector3.Dot(transform.forward, direction.normalized) < 0)
        {
            transform.rotation = Quaternion.LookRotation(-direction);
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }


    private void SitDown()
    {
        animator.Play("SitDown");
        isSitting = true;
        isInteracting = false;

        // Desactivar la rotación automática del NavMeshAgent cuando esté sentado
        navMeshAgent.updateRotation = false;
    }
}

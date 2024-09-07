using System.Collections;
using UnityEngine;

public class CatInteraction : Interactable
{
    private Animator animator;
    private AudioSource audioSource;
    private OutlineSelection outlineSelection; // Referencia al script de OutlineSelection
    private bool isInteracting = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        // Buscar el componente OutlineSelection en la escena
        outlineSelection = FindObjectOfType<OutlineSelection>();

        if (animator == null)
        {
            Debug.LogError("No Animator component found on " + gameObject.name);
        }

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on " + gameObject.name);
        }

        if (outlineSelection == null)
        {
            Debug.LogError("No OutlineSelection found in the scene.");
        }
    }

    private void Update()
    {
        // Verificar si el jugador presiona la tecla E y está apuntando al gato (accediendo a OutlineSelection)
        if (Input.GetKeyDown(KeyCode.E) && IsPlayerPointingAtCat())
        {
            BaseInteract();
        }
    }

    // Método que verifica si el jugador está apuntando al gato a través de OutlineSelection
    private bool IsPlayerPointingAtCat()
    {
        // Si el objeto resaltado por OutlineSelection es este gato
        if (outlineSelection.PointedObject != null && outlineSelection.PointedObject.name == "Cat")
        {
            //Debug.Log("Player is pointing at the cat.");
            return true;
        }

        //Debug.Log("Player is not pointing at the cat.");
        return false;
    }

    protected override void Interact()
    {
        if (!isInteracting && animator != null)
        {
            //Debug.Log("Interacted with " + gameObject.name);
            PlaySound();
            StartCoroutine(PlayAnimations());
        }
    }

    private void PlaySound()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    private IEnumerator PlayAnimations()
    {
        isInteracting = true;

        // Cambiar a SitDown
        animator.Play("SitDown");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(0.7f);

        // Cambiar a StandUp
        animator.Play("StandUp");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Volver a IdleNorm
        animator.Play("IdleNorm");

        isInteracting = false;
    }
}

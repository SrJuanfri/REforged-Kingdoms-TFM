using System.Collections;
using UnityEngine;

public class CatInteraction : Interactable
{
    private Animator animator;
    private AudioSource audioSource;
    private bool isInteracting = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (animator == null)
        {
            Debug.LogError("No Animator component found on " + gameObject.name);
        }

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on " + gameObject.name);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            BaseInteract();
        }
    }

    protected override void Interact()
    {
        if (!isInteracting && animator != null)
        {
            Debug.Log("Interacted with " + gameObject.name);
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

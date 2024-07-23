using UnityEngine;

public class PlayHalfAnimation : MonoBehaviour
{
    public Animator animator;
    public string animationName;
    private float halfDuration;
    private bool isPlayingFirstHalf;
    private bool isPlayingSecondHalf;
    private bool isFirstHalfPlayed = false;

    void Start()
    {
        // Obtiene el tiempo de duraci�n de la animaci�n
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName)
            {
                halfDuration = clip.length / 2;
                break;
            }
        }
    }

    void Update()
    {
        if (isPlayingFirstHalf)
        {
            // Verifica el tiempo transcurrido de la animaci�n
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 0.5f)
            {
                animator.speed = 0; // Pausa la animaci�n
                isPlayingFirstHalf = false;
                isFirstHalfPlayed = true;
            }
        }

        if (isPlayingSecondHalf)
        {
            // Verifica el tiempo transcurrido de la animaci�n
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1.0f)
            {
                animator.speed = 0; // Pausa la animaci�n
                isPlayingSecondHalf = false;
            }
        }
    }

    void OnDisable()
    {
        if (isFirstHalfPlayed && !isPlayingSecondHalf)
        {
            PlaySecondHalf();
        }
    }

    private void OnEnable()
    {
        PlayFirstHalf();
    }

    public void PlayFirstHalf()
    {
        animator.Play(animationName, 0, 0);
        animator.speed = 1; // Aseg�rate de que la animaci�n se reproduzca
        isPlayingFirstHalf = true;
    }

    public void PlaySecondHalf()
    {
        animator.Play(animationName, 0, 0.5f); // Comienza desde la mitad
        animator.speed = 1; // Aseg�rate de que la animaci�n se reproduzca
        isPlayingSecondHalf = true;
    }
}

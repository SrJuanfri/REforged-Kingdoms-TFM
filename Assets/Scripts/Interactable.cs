using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string actionPrompt;
    public string actionKey;

    public void BaseInteract()
    {
        Interact();
    }

    protected virtual void Interact()
    {
        
    }

}

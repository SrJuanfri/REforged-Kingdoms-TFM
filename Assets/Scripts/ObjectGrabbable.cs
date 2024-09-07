using System.Collections;
using UnityEngine;

public class ObjectGrabbable : Interactable
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;
    private ItemSOHolder itemSOHolder;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        itemSOHolder = GetComponent<ItemSOHolder>();
        UpdateActionPrompt(); // Actualiza el prompt al iniciar
    }

    // Método para agarrar el objeto
    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.useGravity = false;
        objectRigidbody.isKinematic = true;
        UpdateActionPrompt(); // Cambiar el prompt a "Soltar"
    }

    // Método para soltar el objeto
    public void Drop()
    {
        this.objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;
        objectRigidbody.isKinematic = false;
        UpdateActionPrompt(); // Cambiar el prompt a "Coger"
    }

    // Método que devuelve si el objeto está siendo agarrado
    public bool IsBeingGrabbed()
    {
        return objectGrabPointTransform != null;
    }

    // Actualiza el actionPrompt dependiendo de si el objeto está agarrado
    private void UpdateActionPrompt()
    {
        if (IsBeingGrabbed())
        {
            actionPrompt = "Soltar";  // Cambiar el texto a "Soltar" si está siendo agarrado
        }
        else
        {
            actionPrompt = "Coger";  // Cambiar el texto a "Coger" si no está siendo agarrado
        }
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform)
        {
            float lerpSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectRigidbody.MovePosition(newPosition);
        }
    }

    protected override void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        // Ejecuta la lógica de interacción
        if (IsBeingGrabbed())
        {
            Drop();
        }
        else
        {
            Grab(objectGrabPointTransform);  // Se debe pasar el Transform correcto al interactuar
        }
    }
}

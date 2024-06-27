using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
        
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;
        objectRigidbody.useGravity = false;
        objectRigidbody.isKinematic = true;
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;
        objectRigidbody.isKinematic = false;
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
    }
}

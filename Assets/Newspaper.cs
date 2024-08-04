using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Newspaper : Interactable
{
    private Transform readPointTransform;
    private Rigidbody objectRigidbody;
    
    void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }
    public void Read(Transform readPointTransform)
    {
        this.readPointTransform = readPointTransform;
        objectRigidbody.useGravity = false;
    }

    public void Drop()
    {
        this.readPointTransform = null;
        objectRigidbody.useGravity = true;
    }
    
    private void FixedUpdate()
    {
        if (readPointTransform)
        {
            float lerpSpeed = 100f;
            Vector3 newPosition = Vector3.Lerp(transform.position, readPointTransform.position, Time.deltaTime * lerpSpeed);
            objectRigidbody.MovePosition(newPosition);
        }
    }
}

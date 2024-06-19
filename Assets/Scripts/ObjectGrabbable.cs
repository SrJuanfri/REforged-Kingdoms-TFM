using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;
    [HideInInspector] public string name;
    [HideInInspector] public int value;

    private ItemSOHolder itemSOHolder;
    private ObjectInfoShower objectInfoShower;


    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        itemSOHolder = GetComponent<ItemSOHolder>();

        name = itemSOHolder.itemSO.name;
        value = itemSOHolder.itemSO.value;
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

    public void ShowName()
    {
        Debug.Log(name +"Tiene un valor de " + value.ToString() + " monedas");
        objectInfoShower.ShowInfo(name, value);
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
}

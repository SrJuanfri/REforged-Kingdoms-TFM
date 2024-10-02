using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform objectGrabPointTransform;  // Where any grabbed object will be placed
    [SerializeField] private LayerMask pickUpLayerMask;

    private ObjectGrabbable objectGrabbable;
    private Newspaper newspaper;  // Reference to the Newspaper component
    private float pickUpDistance = 2.5f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Case 1: If no object is currently held, try to pick up an object
            if (objectGrabbable == null)
            {
                // Perform a raycast to detect objects in front of the player
                if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask))
                {
                    Debug.Log("Raycast hit object: " + raycastHit.transform.name);  // Log the object that was hit

                    // Try to get the ObjectGrabbable component
                    if (raycastHit.transform.TryGetComponent(out objectGrabbable))
                    {
                        Debug.Log("Detected grabbable object: " + raycastHit.transform.name);

                        // Instead of relying on the raycast for the Newspaper reference, use FindObjectOfType to ensure it's correctly assigned
                        newspaper = FindObjectOfType<Newspaper>();

                        if (newspaper != null && objectGrabbable == newspaper.GetComponent<ObjectGrabbable>())
                        {
                            Debug.Log("Newspaper detected using FindObjectOfType.");

                            // Only allow grabbing if not currently in reading mode
                            if (!newspaper.getIsReading())
                            {
                                objectGrabbable.Grab(objectGrabPointTransform);
                                newspaper.SetGrabbed(true);
                                Debug.Log("Picked up the newspaper.");
                            }
                            else
                            {
                                Debug.Log("Cannot grab the newspaper while reading.");
                                objectGrabbable = null;  // Clear the grab reference
                                newspaper = null;  // Clear the newspaper reference
                            }
                        }
                        else
                        {
                            Debug.Log("Not a newspaper, treating as a normal object: " + raycastHit.transform.name);
                            // If it's not a newspaper, grab the object normally
                            objectGrabbable.Grab(objectGrabPointTransform);
                            Debug.Log("Picked up a non-newspaper object.");
                        }
                    }
                    else
                    {
                        Debug.Log("No grabbable object detected.");
                    }
                }
            }
            // Case 2: If an object is already held, try to drop it
            else
            {
                //Debug.Log("Attempting to drop the object.");

                // Check if the object being held is the newspaper
                if (newspaper != null)
                {
                    //Debug.Log("Currently holding the newspaper.");

                    // Prevent dropping the newspaper if it's in reading mode
                    if (newspaper.getIsReading())
                    {
                        Debug.Log("Cannot drop the newspaper while reading.");
                        return;  // Early return, prevents the drop action from continuing
                    }

                    // Drop the newspaper if it's not being read
                    newspaper.SetGrabbed(false);
                    //Debug.Log("Dropped the newspaper.");
                    objectGrabbable.Drop();  // Drop the held newspaper
                    objectGrabbable = null;  // Clear references
                    newspaper = null;  // Clear the newspaper reference
                    return;  // Ensure no further actions happen after dropping the newspaper
                }

                // If the held object isn't the newspaper, drop the object
                if (objectGrabbable != null)
                {
                    Debug.Log("Dropping a non-newspaper object.");
                    objectGrabbable.Drop();
                    objectGrabbable = null;  // Clear references
                }
            }
        }
    }

    // Method to get the currently held object
    public ObjectGrabbable GetHeldObject()
    {
        return objectGrabbable;
    }
}

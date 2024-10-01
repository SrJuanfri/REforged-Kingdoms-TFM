using UnityEngine;
using TMPro;

public class Newspaper : Interactable
{
    private ObjectGrabbable objectGrabbable;  // To detect if the newspaper is being grabbed
    private PlayerUI playerUI;  // Reference to PlayerUI to update prompts

    [SerializeField] private Transform firstChild;  // The first child (normal view of the newspaper)
    [SerializeField] private Transform secondChild; // The second child (reading mode of the newspaper)
    [SerializeField] private Transform readPoint;   // Point where the set will move when the second child is active (Assigned from the Inspector)
    [SerializeField] private TextMeshProUGUI eventInfoText;  // TextMeshProUGUI component to display event information

    private Transform originalParent;  // Save the original parent of the newspaper
    private Vector3 originalPosition;  // Save the original position
    private Quaternion originalRotation;  // Save the original rotation
    private bool isReading = false;  // State to know if we are reading the newspaper
    private bool isGrabbed = false;  // Track if the newspaper is currently grabbed

    private void Awake()
    {
        // Get the ObjectGrabbable component from the children of the parent object
        objectGrabbable = GetComponentInChildren<ObjectGrabbable>();
        playerUI = FindObjectOfType<PlayerUI>();  // Get reference to PlayerUI

        if (objectGrabbable == null)
        {
            Debug.LogError("No ObjectGrabbable component found in children of " + gameObject.name);
        }

        // Ensure the first child is active and the second child is deactivated on start
        if (firstChild != null)
        {
            firstChild.gameObject.SetActive(true);
        }
        if (secondChild != null)
        {
            secondChild.gameObject.SetActive(false);
        }

        // Save the original parent, position, and rotation
        originalParent = transform.parent;
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    private void Update()
    {
        // Only show secondary prompt if the newspaper is being grabbed
        if (objectGrabbable != null && objectGrabbable.IsBeingGrabbed())
        {
            if (!isReading)  // Show prompt only if not in reading mode
            {
                playerUI.UpdateSecondaryActionText(actionPrompt, actionKey);
            }

            // Detect first frame where Q is pressed to toggle reading mode
            if (Input.GetKeyDown(KeyCode.Q))
            {
                ToggleReadingMode();
            }
        }
        else
        {
            playerUI.ClearSecondaryActionText();
        }
    }

    public bool getIsReading()
    {
        return isReading;
    }

    public void SetGrabbed(bool grabbed)
    {
        isGrabbed = grabbed;
        Debug.Log("Newspaper grabbed state set to: " + isGrabbed);
    }

    public bool IsGrabbed()
    {
        return isGrabbed;
    }

    private void ToggleReadingMode()
    {
        if (!isReading)
        {
            Debug.Log("Switching to reading mode.");
            firstChild.gameObject.SetActive(false);
            secondChild.gameObject.SetActive(true);

            transform.SetParent(readPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            actionPrompt = "Close";  // Change prompt to "Close"
        }
        else
        {
            Debug.Log("Switching out of reading mode.");
            firstChild.gameObject.SetActive(true);
            secondChild.gameObject.SetActive(false);

            ResetToOriginalTransform();
            actionPrompt = "Read";  // Change prompt back to "Read"
        }

        isReading = !isReading;
        Debug.Log("Newspaper isReading state is now: " + isReading);
    }

    public void ResetToOriginalTransform()
    {
        transform.SetParent(originalParent);
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
        Debug.Log("Newspaper reset to original position.");
    }

    public void UpdateEventInfo(string eventInfo)
    {
        if (eventInfoText != null)
        {
            eventInfoText.text = eventInfo;
        }
    }

    protected override void Interact()
    {
        if (objectGrabbable != null)
        {
            if (objectGrabbable.IsBeingGrabbed())
            {
                if (!isReading)
                {
                    Debug.Log("Dropping the newspaper.");
                    objectGrabbable.Drop();
                    SetGrabbed(false);
                }
                else
                {
                    Debug.Log("Cannot drop the newspaper while reading.");
                }
            }
            else
            {
                Debug.Log("Picking up the newspaper.");
                objectGrabbable.Grab(transform);
                SetGrabbed(true);
            }
        }
    }
}

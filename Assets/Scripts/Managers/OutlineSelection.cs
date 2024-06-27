using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OutlineSelection : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float efectiveDistance;
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;
    
    void Update()
    {
        if (highlight)
        {
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            highlight = null;
        }
        
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward,
                out raycastHit, efectiveDistance))
        {
            highlight = raycastHit.transform;
            
            if (highlight.CompareTag("Selectable") && highlight != selection)
            {
                if (highlight.gameObject.GetComponent<Outline>())
                {
                    highlight.gameObject.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    Outline outline = highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.green;
                    highlight.gameObject.GetComponent<Outline>().OutlineWidth = 6.0f;
                }
            }
            else
            {
                highlight = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (highlight)
            {
                if (selection)
                {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                }

                selection = raycastHit.transform;
                selection.GameObject().GetComponent<Outline>().enabled = true;
                highlight = null;
            }
            else
            {
                if (selection)
                {
                    selection.gameObject.GetComponent<Outline>().enabled = false;
                    selection = null;
                }
            }
        }
    }
        
}

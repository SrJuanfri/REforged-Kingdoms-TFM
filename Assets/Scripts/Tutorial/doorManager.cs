using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class doorManager : Interactable
{
    private OutlineSelection outlineSelection; // Referencia al script de OutlineSelection

    // Start is called before the first frame update
    void Start()
    {
        // Buscar el componente OutlineSelection en la escena
        outlineSelection = FindObjectOfType<OutlineSelection>();
    }

    // Update is called once per frame
    void Update()
    {
        // Verificar si el jugador presiona la tecla E y está apuntando al gato (accediendo a OutlineSelection)
        if (Input.GetKeyDown(KeyCode.E) && IsPlayerPointingAtDoor())
        {
            BaseInteract();
        }
    }

    protected override void Interact()
    {
        SceneManager.LoadScene(2);
    }

    // Método que verifica si el jugador está apuntando al gato a través de OutlineSelection
    private bool IsPlayerPointingAtDoor()
    {
        // Si el objeto resaltado por OutlineSelection es este gato
        if (outlineSelection.PointedObject != null && outlineSelection.PointedObject.name.Contains("Door"))
        {
            //Debug.Log("Player is pointing at the cat.");
            return true;
        }

        //Debug.Log("Player is not pointing at the cat.");
        return false;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class RightClickDetector : MonoBehaviour
{
    public TW_MultiStrings_RandomPointer randomPointer;

    void Update()
    {
        // Detectar si se ha hecho clic derecho (bot�n 1 del rat�n)
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Clic derecho detectado");
            OnRightClick();
        }
    }

    // M�todo que se llama cuando se detecta un clic derecho
    void OnRightClick()
    {
        // Aqu� puedes poner la l�gica que quieras ejecutar
        Debug.Log("Acci�n del clic derecho ejecutada");

        if (randomPointer.HasFinishedWriting())
        {
            SceneManager.LoadScene("Tutorial");
        }
        else{
            randomPointer.NextString();
        }
    }
}

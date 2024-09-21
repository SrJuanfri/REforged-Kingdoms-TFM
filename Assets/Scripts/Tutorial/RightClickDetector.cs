using UnityEngine;
using UnityEngine.SceneManagement;

public class RightClickDetector : MonoBehaviour
{
    public TW_MultiStrings_RandomPointer randomPointer;

    void Update()
    {
        // Detectar si se ha hecho clic derecho (botón 1 del ratón)
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Clic derecho detectado");
            OnRightClick();
        }
    }

    // Método que se llama cuando se detecta un clic derecho
    void OnRightClick()
    {
        // Aquí puedes poner la lógica que quieras ejecutar
        Debug.Log("Acción del clic derecho ejecutada");

        if (randomPointer.HasFinishedWriting())
        {
            SceneManager.LoadScene("Tutorial");
        }
        else{
            randomPointer.NextString();
        }
    }
}

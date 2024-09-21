using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject canvasPause;
    [SerializeField] private FirstPersonLook firstPersonLookScript; // Referencia al script de control de la cámara

    void Start()
    {
        // Asegúrate de que el script FirstPersonLook esté asignado
        if (firstPersonLookScript == null)
        {
            firstPersonLookScript = Camera.main.GetComponent<FirstPersonLook>(); // Si no se asigna, intenta obtenerlo de la cámara principal
        }

        // Asegurarse de que el canvas de pausa esté desactivado al inicio
        canvasPause.SetActive(false);
    }

    void Update()
    {

    }

    public void QuitApplication()
    {
        // Cierra la aplicación
        Application.Quit();
        Debug.Log("La aplicación se ha cerrado.");
    }

    // Método para alternar el estado del canvas de pausa
    public void ToggleCanvas()
    {
        if (canvasPause.activeSelf)
        {
            // Si el canvas ya está activo, lo desactivamos
            canvasPause.SetActive(false); // Desactivar el menú de pausa
            LockCursor(); // Bloquear el cursor
            firstPersonLookScript.enabled = true; // Reactivar el control de la cámara
            Time.timeScale = 1f; // Restablecer la velocidad del juego
        }
        else
        {
            // Si el canvas no está activo, lo activamos
            canvasPause.SetActive(true); // Activar el menú de pausa
            UnlockCursor(); // Liberar el cursor
            firstPersonLookScript.enabled = false; // Desactivar el control de la cámara
            Time.timeScale = 0f; // Pausar el juego
        }
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; // Asegurarse de que el tiempo esté restaurado antes de cargar el menú
        SceneManager.LoadScene("MainMenu");
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Liberar el cursor
        Cursor.visible = true; // Mostrar el cursor
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Bloquear el cursor en el centro de la pantalla
        Cursor.visible = false; // Ocultar el cursor
    }
}

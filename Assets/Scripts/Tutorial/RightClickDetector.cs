using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RightClickDetector : MonoBehaviour
{
    public TW_MultiStrings_RandomPointer randomPointer;
    public FinDelJuegoAnimation finDelJuegoAnimation;

    // Índice de la escena seleccionada en Build Settings
    [SerializeField] private int selectedSceneIndex = 0;

    private string[] scenes;

    private void Awake()
    {
        // Obtener todas las escenas desde Build Settings
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        scenes = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }
    }

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
        Debug.Log("Acción del clic derecho ejecutada");

        // Verifica si el randomPointer ha terminado de escribir
        if (randomPointer.HasFinishedWriting())
        {
            // Si la animación no ha empezado, la iniciamos
            if (!finDelJuegoAnimation.IsAnimationStarted())
            {
                randomPointer.gameObject.SetActive(false);
                StartCoroutine(finDelJuegoAnimation.EscribirTexto());  // Llamar como corutina
            }
            // Si la animación ha terminado, cargamos la escena seleccionada
            else if (finDelJuegoAnimation.IsAnimationFinished())
            {
                // Validación del índice de la escena seleccionada
                if (selectedSceneIndex >= 0 && selectedSceneIndex < scenes.Length)
                {
                    SceneManager.LoadScene(scenes[selectedSceneIndex]);
                }
                else
                {
                    Debug.LogError("Índice de escena seleccionado fuera de rango.");
                }
            }
        }
        else
        {
            // Si randomPointer no ha terminado, pasamos al siguiente string
            randomPointer.NextString();
        }
    }


#if UNITY_EDITOR
    // Custom editor para mostrar un desplegable con las escenas de Build Settings
    [CustomEditor(typeof(RightClickDetector))]
    public class RightClickDetectorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            RightClickDetector script = (RightClickDetector)target;

            // Obtener todas las escenas desde Build Settings
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            string[] scenes = new string[sceneCount];
            for (int i = 0; i < sceneCount; i++)
            {
                scenes[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            }

            // Mostrar un desplegable con las escenas disponibles en Build Settings
            script.selectedSceneIndex = EditorGUILayout.Popup("Escena a Cargar", script.selectedSceneIndex, scenes);

            // Guardar cambios si se ha modificado el inspector
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }

            DrawDefaultInspector();
        }
    }
#endif
}

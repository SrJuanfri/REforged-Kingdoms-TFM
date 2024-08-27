using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class ChangeScene : MonoBehaviour
{
    // Variable para almacenar la escena seleccionada
    public string selectedScene;

    // M�todo para cambiar de escena
    public void ChangeToScene()
    {
        if (!string.IsNullOrEmpty(selectedScene))
        {
            SceneManager.LoadScene(selectedScene);
        }
        else
        {
            Debug.LogError("No se ha seleccionado ninguna escena.");
        }
    }

    // M�todo para salir de la aplicaci�n
    public void QuitApplication()
    {
        Debug.Log("Saliendo de la aplicaci�n...");
        Application.Quit();

        // Si estamos en el editor de Unity, detener la reproducci�n
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ChangeScene))]
    public class ChangeSceneEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Obtiene la referencia al script objetivo
            ChangeScene changeScene = (ChangeScene)target;

            // Obt�n todas las escenas en el proyecto
            string[] scenes = GetSceneNames();

            // Encuentra el �ndice de la escena actualmente seleccionada
            int selectedIndex = System.Array.IndexOf(scenes, changeScene.selectedScene);

            // Si la escena seleccionada no se encuentra, resetea el �ndice a 0
            if (selectedIndex == -1) selectedIndex = 0;

            // Desplegable en el Inspector con todas las escenas
            selectedIndex = EditorGUILayout.Popup("Escena a cargar", selectedIndex, scenes);

            // Asigna la escena seleccionada al script
            changeScene.selectedScene = scenes[selectedIndex];

            // Guardar cambios
            if (GUI.changed)
            {
                EditorUtility.SetDirty(changeScene);
                EditorSceneManager.MarkSceneDirty(changeScene.gameObject.scene);
            }

            // Base inspector GUI para que no se omitan otros elementos del script
            DrawDefaultInspector();
        }

        // M�todo para obtener todos los nombres de las escenas del proyecto
        private string[] GetSceneNames()
        {
            int sceneCount = EditorSceneManager.sceneCountInBuildSettings;
            string[] scenes = new string[sceneCount];
            for (int i = 0; i < sceneCount; i++)
            {
                scenes[i] = System.IO.Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
            }
            return scenes;
        }
    }
#endif
}

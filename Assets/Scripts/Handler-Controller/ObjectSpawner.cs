using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; // El prefab del objeto a spawnear
    public float movementMargin = 0.5f; // Margen de movimiento permitido antes de spawnear un nuevo objeto

    private GameObject currentObject; // El objeto actualmente spawneado
    private Vector3 originalPosition; // La posición original del objeto spawneado

    void Start()
    {
        SpawnObject();
    }

    void Update()
    {
        if (currentObject == null || Vector3.Distance(currentObject.transform.GetChild(0).transform.position, originalPosition) > movementMargin)
        {
            SpawnObject();
        }
    }

    void SpawnObject()
    {
        currentObject = Instantiate(objectToSpawn, transform.position, transform.rotation, transform);
        originalPosition = currentObject.transform.position;
    }
}

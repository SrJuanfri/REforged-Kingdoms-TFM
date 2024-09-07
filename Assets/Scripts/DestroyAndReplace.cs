using UnityEngine;
using System.Collections;

public class DestroyAndReplace : MonoBehaviour
{
    [SerializeField] private GameObject replacementPrefab;  // Prefab que será instanciado en lugar del objeto destruido
    private PlayerPickUpDrop playerPickUpDrop;  // Referencia al script PlayerPickUpDrop del jugador
    private float requiredCollisionTime = 1f;  // Tiempo mínimo de colisión (1 segundo)

    private void Start()
    {
        // Encontrar el script PlayerPickUpDrop en la escena (ajústalo según tu escena)
        playerPickUpDrop = FindObjectOfType<PlayerPickUpDrop>();

        if (playerPickUpDrop == null)
        {
            Debug.LogError("PlayerPickUpDrop script not found in the scene.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si el objeto tiene "Dagger" o "Daga" en su nombre
        if (collision.gameObject.name.Contains("Dagger") || collision.gameObject.name.Contains("Daga"))
        {
            // Iniciar la corutina para contar el tiempo de colisión
            StartCoroutine(CollisionCheck(collision.gameObject));
        }
    }

    private IEnumerator CollisionCheck(GameObject obj)
    {
        float collisionTime = 0f;

        // Mientras el objeto siga colisionando y no esté siendo sujetado por el jugador
        while (collisionTime < requiredCollisionTime)
        {
            // Verificar si el objeto está siendo sujetado por el jugador
            if (IsObjectHeldByPlayer(obj))
            {
                Debug.Log("El jugador está sujetando el objeto, cancelando la destrucción.");
                yield break;  // Si el objeto está siendo sujetado, se detiene la corutina
            }

            // Incrementar el tiempo de colisión
            collisionTime += Time.deltaTime;

            // Esperar al siguiente frame
            yield return null;
        }

        // Una vez que se ha mantenido la colisión por el tiempo requerido y no está siendo sujetado
        Debug.Log("El objeto ha colisionado por suficiente tiempo, destruyendo e instanciando el prefab.");
        Destroy(obj);

        // Instanciar el prefab en la posición del objeto destruido
        if (replacementPrefab != null)
        {
            Instantiate(replacementPrefab, obj.transform.position, obj.transform.rotation);
        }
    }

    // Método para verificar si el objeto está siendo sujetado por el jugador
    private bool IsObjectHeldByPlayer(GameObject obj)
    {
        // Si el jugador tiene un objeto, verificamos si el objeto en la mano es el mismo que colisionó
        ObjectGrabbable heldObject = playerPickUpDrop.GetHeldObject();

        if (heldObject != null && heldObject.gameObject == obj)
        {
            Debug.Log("El jugador está sujetando el objeto, no se destruirá.");
            return true;  // El jugador está sujetando el objeto
        }

        return false;  // El jugador no está sujetando el objeto
    }
}

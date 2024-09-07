using UnityEngine;
using System.Collections;

public class DestroyAndReplace : MonoBehaviour
{
    [SerializeField] private GameObject replacementPrefab;  // Prefab que ser� instanciado en lugar del objeto destruido
    private PlayerPickUpDrop playerPickUpDrop;  // Referencia al script PlayerPickUpDrop del jugador
    private float requiredCollisionTime = 1f;  // Tiempo m�nimo de colisi�n (1 segundo)

    private void Start()
    {
        // Encontrar el script PlayerPickUpDrop en la escena (aj�stalo seg�n tu escena)
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
            // Iniciar la corutina para contar el tiempo de colisi�n
            StartCoroutine(CollisionCheck(collision.gameObject));
        }
    }

    private IEnumerator CollisionCheck(GameObject obj)
    {
        float collisionTime = 0f;

        // Mientras el objeto siga colisionando y no est� siendo sujetado por el jugador
        while (collisionTime < requiredCollisionTime)
        {
            // Verificar si el objeto est� siendo sujetado por el jugador
            if (IsObjectHeldByPlayer(obj))
            {
                Debug.Log("El jugador est� sujetando el objeto, cancelando la destrucci�n.");
                yield break;  // Si el objeto est� siendo sujetado, se detiene la corutina
            }

            // Incrementar el tiempo de colisi�n
            collisionTime += Time.deltaTime;

            // Esperar al siguiente frame
            yield return null;
        }

        // Una vez que se ha mantenido la colisi�n por el tiempo requerido y no est� siendo sujetado
        Debug.Log("El objeto ha colisionado por suficiente tiempo, destruyendo e instanciando el prefab.");
        Destroy(obj);

        // Instanciar el prefab en la posici�n del objeto destruido
        if (replacementPrefab != null)
        {
            Instantiate(replacementPrefab, obj.transform.position, obj.transform.rotation);
        }
    }

    // M�todo para verificar si el objeto est� siendo sujetado por el jugador
    private bool IsObjectHeldByPlayer(GameObject obj)
    {
        // Si el jugador tiene un objeto, verificamos si el objeto en la mano es el mismo que colision�
        ObjectGrabbable heldObject = playerPickUpDrop.GetHeldObject();

        if (heldObject != null && heldObject.gameObject == obj)
        {
            Debug.Log("El jugador est� sujetando el objeto, no se destruir�.");
            return true;  // El jugador est� sujetando el objeto
        }

        return false;  // El jugador no est� sujetando el objeto
    }
}

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RecipeCrafting : Interactable
{
    public Image craftImage;
    [SerializeField] private BoxCollider placeItemsAreaCollider;
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private Transform vfxSpawnItem;
    [HideInInspector] public CraftingRecipeSO craftingRecipeSO;

    // Variable para almacenar el �ltimo objeto instanciado
    private WeaponOrToolSO lastCreatedItemSO;

    public void Craft()
    {
        // Obtener los colliders de los objetos en el �rea de crafting
        Collider[] colliderArray = Physics.OverlapBox(transform.position + placeItemsAreaCollider.center,
            placeItemsAreaCollider.size, placeItemsAreaCollider.transform.rotation);

        // Lista para almacenar los objetos que ser�n consumidos (eliminados)
        List<GameObject> consumeItemGameObjectList = new List<GameObject>();

        // Lista para almacenar los ItemSO detectados en el �rea
        List<ItemSO> detectedItems = new List<ItemSO>();

        foreach (Collider collider in colliderArray)
        {
            // Verificar si el collider tiene un componente ItemSOHolder
            if (collider.TryGetComponent(out ItemSOHolder itemSOHolder))
            {
                detectedItems.Add(itemSOHolder.itemSO);
                consumeItemGameObjectList.Add(collider.gameObject); // Agregar el objeto a eliminar si coincide con la receta
            }
        }

        // Verificar si los items detectados coinciden exactamente con alguna de las combinaciones posibles
        foreach (var combination in craftingRecipeSO.materialCombinations)
        {
            // Crear una copia de la lista de materiales para hacer la verificaci�n
            List<ItemSO> requiredItems = new List<ItemSO>(combination.materials);

            // Eliminar de la lista de requeridos los items detectados que coincidan
            foreach (var item in detectedItems)
            {
                if (requiredItems.Contains(item))
                {
                    requiredItems.Remove(item);
                }
            }

            // Si todos los items requeridos fueron eliminados y no hay m�s items detectados, significa que tenemos exactamente los materiales necesarios
            if (requiredItems.Count == 0 && detectedItems.Count == combination.materials.Count)
            {
                // Usar el outputItemSO espec�fico de la combinaci�n
                WeaponOrToolSO outputItem = combination.outputItemSO;

                // Realizar el crafting
                Transform spawnedItemTransform = Instantiate(outputItem.prefab, itemSpawnPoint.position,
                    itemSpawnPoint.rotation);

                // Guardar el ItemSO del objeto instanciado como el �ltimo creado
                lastCreatedItemSO = outputItem;

                // Instanciar efectos visuales
                Instantiate(vfxSpawnItem, itemSpawnPoint.position, itemSpawnPoint.rotation);

                // Destruir los objetos consumidos
                foreach (GameObject consumeItemGameObject in consumeItemGameObjectList)
                {
                    Destroy(consumeItemGameObject);
                }

                return; // Salir del m�todo una vez que el crafting ha sido exitoso
            }
        }

        // Si ninguna combinaci�n coincide o hay materiales extra, mostrar un mensaje o indicar que faltan materiales
        Debug.LogWarning("Faltan materiales o hay materiales extra. La combinaci�n no es v�lida.");
    }


    // Funci�n para obtener el �ltimo ItemSO creado
    public WeaponOrToolSO GetLastCreatedItem()
    {
        return lastCreatedItemSO;
    }
    public List<ItemSO> GetItemsInCollider()
    {
        //Debug.Log("GetItemsInCollider");
        // Usamos Physics.OverlapBox para detectar todos los colliders en el �rea del BoxCollider
        Collider[] colliderArray = Physics.OverlapBox(transform.position + placeItemsAreaCollider.center,
    placeItemsAreaCollider.size, placeItemsAreaCollider.transform.rotation);

        // Lista para almacenar los ItemSO detectados
        List<ItemSO> itemList = new List<ItemSO>();

        // Iteramos sobre los colliders encontrados
        foreach (Collider collider in colliderArray)
        {
            // Comprobamos si el objeto tiene un componente ItemSOHolder
            if (collider.TryGetComponent(out ItemSOHolder itemSOHolder))
            {
                // A�adimos el ItemSO a la lista
                itemList.Add(itemSOHolder.itemSO);
            }
        }

        // Devolvemos la lista de ItemSO
        return itemList;
    }


}

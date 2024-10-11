using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponOrTool", menuName = "ScriptableObjects/WeaponOrTool")]
public class WeaponOrToolSO : ScriptableObject
{
    public enum ItemType
    {
        Weapon,
        Tool
    }

    // Nombre del arma o herramienta
    public string itemName;

    // Tipo del �tem: Arma o Herramienta
    public ItemType itemType;

    // �tem de entrada asociado a la creaci�n de esta arma o herramienta
    public GameObject inputItem;

    // Prefab del arma o herramienta para instanciar en el juego
    public Transform prefab;

    // Valor asociado a este arma o herramienta
    public int value;

    // Materiales utilizados en la creaci�n del arma o herramienta
    //public ItemSO metal;
    //public ItemSO wood;
}

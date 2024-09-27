using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ChangeButtons : Interactable
{
    [SerializeField] private GameObject panelBotones;
    [SerializeField] private GameObject panelBotonesNuevo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeButtonInterface()
    {
        panelBotones.SetActive(false);
        panelBotonesNuevo.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionMessage;
    [SerializeField] private TextMeshProUGUI keyMessage;
    public GameObject actionUI;
    
    

    void Start()
    {
        
    }

    public void UpdateActionText(string actionText, string actionKey)
    {
        actionMessage.text = actionText;
        keyMessage.text = actionKey;
    }

}

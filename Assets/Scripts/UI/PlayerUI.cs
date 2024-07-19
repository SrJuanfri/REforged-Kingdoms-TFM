using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionMessage;
    [SerializeField] private TextMeshProUGUI keyMessage;
    [SerializeField] private TextMeshProUGUI infoName;
    [SerializeField] private TextMeshProUGUI infoValue;
    
    public GameObject actionUI;
    public GameObject infoUI;
    
    void Start()
    {
        
    }

    public void UpdateActionText(string actionText, string actionKey)
    {
        actionMessage.text = actionText;
        keyMessage.text = actionKey;
    }

    public void UpdateInfoText(string nameInfo, int valueInfo)
    {
        infoName.text = nameInfo;
        infoValue.text = valueInfo.ToString();
    }

}

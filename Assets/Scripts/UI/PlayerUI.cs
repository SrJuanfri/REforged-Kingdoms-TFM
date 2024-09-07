using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    // Primer conjunto de prompts (acción principal)
    [SerializeField] private TextMeshProUGUI actionMessage;
    [SerializeField] private TextMeshProUGUI keyMessage;

    // Segundo conjunto de prompts (acción secundaria)
    [SerializeField] private TextMeshProUGUI secondaryActionMessage;
    [SerializeField] private TextMeshProUGUI secondaryKeyMessage;

    // Información adicional
    [SerializeField] private TextMeshProUGUI infoName;
    [SerializeField] private TextMeshProUGUI infoValue;

    // Panel de la acción secundaria
    public GameObject secondaryActionUI;


    // UI Elements
    public GameObject actionUI;
    public GameObject infoUI;

    // Método para actualizar el primer conjunto de prompts (acción principal)
    public void UpdateActionText(string actionText, string actionKey)
    {
        actionMessage.text = actionText;
        keyMessage.text = actionKey;
    }

    // Método para actualizar el segundo conjunto de prompts (acción secundaria)
    public void UpdateSecondaryActionText(string secondaryActionText, string secondaryActionKey)
    {
        secondaryActionMessage.text = secondaryActionText;
        secondaryKeyMessage.text = secondaryActionKey;
        secondaryActionUI.SetActive(true);  // Activar la UI secundaria cuando sea necesario
    }

    // Método para actualizar la información adicional
    public void UpdateInfoText(string nameInfo, int valueInfo)
    {
        infoName.text = nameInfo;
        infoValue.text = valueInfo.ToString();
    }

    // Método para ocultar los prompts secundarios cuando no sean necesarios
    public void ClearSecondaryActionText()
    {
        secondaryActionMessage.text = "";
        secondaryKeyMessage.text = "";
        secondaryActionUI.SetActive(false);  // Desactivar la UI secundaria cuando no se necesite
    }
}

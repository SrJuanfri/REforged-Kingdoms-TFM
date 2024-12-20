using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    // Primer conjunto de prompts (acci�n principal)
    [SerializeField] private TextMeshProUGUI actionMessage;
    [SerializeField] private TextMeshProUGUI keyMessage;

    // Segundo conjunto de prompts (acci�n secundaria)
    [SerializeField] private TextMeshProUGUI secondaryActionMessage;
    [SerializeField] private TextMeshProUGUI secondaryKeyMessage;

    // Informaci�n adicional
    [SerializeField] private TextMeshProUGUI infoName;
    [SerializeField] private TextMeshProUGUI infoValue;

    // Panel de la acci�n secundaria
    public GameObject secondaryActionUI;


    // UI Elements
    public GameObject actionUI;
    public GameObject infoUI;

    // M�todo para actualizar el primer conjunto de prompts (acci�n principal)
    public void UpdateActionText(string actionText, string actionKey)
    {
        actionMessage.text = actionText;
        keyMessage.text = actionKey;
    }

    // M�todo para actualizar el segundo conjunto de prompts (acci�n secundaria)
    public void UpdateSecondaryActionText(string secondaryActionText, string secondaryActionKey)
    {
        secondaryActionMessage.text = secondaryActionText;
        secondaryKeyMessage.text = secondaryActionKey;
        secondaryActionUI.SetActive(true);  // Activar la UI secundaria cuando sea necesario
    }

    // M�todo para actualizar la informaci�n adicional
    public void UpdateInfoText(string nameInfo, int valueInfo)
    {
        // Asignaci�n capitalizando la primera letra
        infoName.text = CapitalizeFirstLetter(nameInfo);

        infoValue.text = valueInfo.ToString();
    }

    // M�todo para capitalizar la primera letra de una cadena
    private string CapitalizeFirstLetter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text; // Retorna el texto sin cambios si es nulo o vac�o

        // Capitaliza la primera letra y concatena el resto de la cadena
        return char.ToUpper(text[0]) + text.Substring(1);
    }


    // M�todo para ocultar los prompts secundarios cuando no sean necesarios
    public void ClearSecondaryActionText()
    {
        secondaryActionMessage.text = "";
        secondaryKeyMessage.text = "";
        secondaryActionUI.SetActive(false);  // Desactivar la UI secundaria cuando no se necesite
    }
}

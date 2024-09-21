using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necesario para usar TextMeshPro

public class CapitalizeFirstLetter : MonoBehaviour
{
    // Referencia al componente TextMeshPro en el objeto
    private TMP_Text textMeshPro;

    void Start()
    {
        // Obtener el componente TextMeshPro al inicio
        textMeshPro = GetComponent<TMP_Text>();

        // Asegurarse de que la primera letra est� en may�scula
        UpdateText();
    }

    void Update()
    {
        // Aqu� puedes actualizar constantemente el texto si es necesario
        // (Por ejemplo, si el texto cambia din�micamente)
        UpdateText();
    }

    // M�todo para capitalizar la primera letra
    private void UpdateText()
    {
        if (textMeshPro != null && textMeshPro.text.Length > 0)
        {
            string text = textMeshPro.text;
            // Convierte la primera letra a may�sculas y concatena con el resto del texto
            textMeshPro.text = char.ToUpper(text[0]) + text.Substring(1);
        }
    }
}

using UnityEngine;

public class TextCorrectionManager : MonoBehaviour
{
    // Funci�n principal para corregir el texto
    public string FixText(string inputText)
    {
        if (string.IsNullOrEmpty(inputText))
            return inputText;

        // Normalizar el texto (convertir a min�sculas y capitalizar la primera letra)
        string correctedText = NormalizeText(inputText);

        return correctedText;
    }

    // M�todo para normalizar el texto (convertir a min�sculas y capitalizar la primera letra de cada oraci�n)
    private string NormalizeText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        // Convertir el texto a min�sculas
        text = text.ToLower();

        // Capitalizar la primera letra de cada oraci�n
        string[] sentences = text.Split(new[] { '.', '!', '?' }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < sentences.Length; i++)
        {
            sentences[i] = sentences[i].Trim();
            if (sentences[i].Length > 0)
            {
                sentences[i] = char.ToUpper(sentences[i][0]) + sentences[i].Substring(1);
            }
        }

        // Unir las oraciones de nuevo en un solo texto
        text = string.Join(". ", sentences);

        // Asegurarse de que el texto final termine con un punto
        if (!text.EndsWith("."))
        {
            text += ".";
        }

        return text;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomerPhraseManager : MonoBehaviour
{
    [System.Serializable]
    public class OrderPhrase
    {
        public CustomerState state;
        public string phrase; // La frase incluye marcadores para el item, metal y madera
    }

    [System.Serializable]
    public class FarewellPhrase
    {
        public CustomerState state;
        public List<string> phrases;
    }

    [SerializeField]
    private List<OrderPhrase> orderPhrases = new List<OrderPhrase>()
    {
        // Frases para clientes muy felices
        new OrderPhrase() { state = CustomerState.MuyFeliz, phrase = "¡Este {item} de {metal} con mango de {wood} es exactamente lo que quería!" },
        new OrderPhrase() { state = CustomerState.MuyFeliz, phrase = "¡Qué alegría! Un {item} de {metal} con un mango de {wood}, perfecto." },
        new OrderPhrase() { state = CustomerState.MuyFeliz, phrase = "¡Me encanta! Un {item} de {metal} con mango de {wood} es justo lo que necesitaba." },
        new OrderPhrase() { state = CustomerState.MuyFeliz, phrase = "¡Increíble! Este {item} de {metal} con mango de {wood} es lo mejor que he visto." },
        new OrderPhrase() { state = CustomerState.MuyFeliz, phrase = "¡Es perfecto! Un {item} de {metal} con mango de {wood}, estoy encantado." },

        // Frases para clientes contentos
        new OrderPhrase() { state = CustomerState.Contento, phrase = "Me gusta este {item} de {metal} con mango de {wood}. ¡Buen trabajo!" },
        new OrderPhrase() { state = CustomerState.Contento, phrase = "Un {item} de {metal} con mango de {wood}, justo lo que buscaba." },
        new OrderPhrase() { state = CustomerState.Contento, phrase = "Este {item} de {metal} con mango de {wood} es muy bueno, gracias." },
        new OrderPhrase() { state = CustomerState.Contento, phrase = "¡Buen trabajo! Este {item} de {metal} con mango de {wood} es de buena calidad." },
        new OrderPhrase() { state = CustomerState.Contento, phrase = "Estoy satisfecho con este {item} de {metal} con mango de {wood}, gracias." },

        // Frases para clientes neutrales
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Este {item} de {metal} con mango de {wood} está bien, creo." },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Un {item} de {metal} con mango de {wood}, es aceptable." },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Este {item} de {metal} con mango de {wood} cumplirá su función." },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "No está mal, este {item} de {metal} con mango de {wood} servirá." },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Podría ser mejor, pero este {item} de {metal} con mango de {wood} es suficiente." },

        // Frases para clientes insatisfechos
        new OrderPhrase() { state = CustomerState.Insatisfecho, phrase = "No es exactamente lo que quería, pero este {item} de {metal} con mango de {wood} servirá." },
        new OrderPhrase() { state = CustomerState.Insatisfecho, phrase = "No estoy muy convencido con este {item} de {metal} con mango de {wood}." },
        new OrderPhrase() { state = CustomerState.Insatisfecho, phrase = "Esperaba más, este {item} de {metal} con mango de {wood} no es lo que quería." },
        new OrderPhrase() { state = CustomerState.Insatisfecho, phrase = "Este {item} de {metal} con mango de {wood} no cumple con mis expectativas." },
        new OrderPhrase() { state = CustomerState.Insatisfecho, phrase = "No estoy satisfecho con este {item} de {metal} con mango de {wood}." },

        // Frases para clientes muy insatisfechos
        new OrderPhrase() { state = CustomerState.MuyInsatisfecho, phrase = "Este {item} de {metal} con mango de {wood} es terrible. ¡Qué decepción!" },
        new OrderPhrase() { state = CustomerState.MuyInsatisfecho, phrase = "¡No puedo creer que me hayas hecho este {item} de {metal} con mango de {wood}!" },
        new OrderPhrase() { state = CustomerState.MuyInsatisfecho, phrase = "Este {item} de {metal} con mango de {wood} es lo peor que he visto." },
        new OrderPhrase() { state = CustomerState.MuyInsatisfecho, phrase = "¡Es inaceptable! Este {item} de {metal} con mango de {wood} no sirve para nada." },
        new OrderPhrase() { state = CustomerState.MuyInsatisfecho, phrase = "¡Qué desastre! Este {item} de {metal} con mango de {wood} es una vergüenza." },
    };

    [SerializeField]
    private List<FarewellPhrase> farewellPhrases = new List<FarewellPhrase>()
    {
        new FarewellPhrase() { state = CustomerState.MuyFeliz, phrases = new List<string>()
        {
            "¡Gracias, fue un placer!",
            "¡Hasta luego, estaré de vuelta!",
            "¡Qué bien me siento, nos vemos pronto!",
            "¡Nos vemos! ¡No puedo esperar a regresar!",
            "¡Maravilloso, gracias por todo!"
        }},
        new FarewellPhrase() { state = CustomerState.Contento, phrases = new List<string>()
        {
            "Gracias, hasta la próxima.",
            "Nos vemos pronto.",
            "Buen trabajo, nos vemos.",
            "¡Gracias, ha sido genial!",
            "¡Hasta luego, volveré pronto!"
        }},
        new FarewellPhrase() { state = CustomerState.Neutral, phrases = new List<string>()
        {
            "Gracias, adiós.",
            "Hasta luego.",
            "Nos vemos.",
            "Bueno, eso es todo, adiós.",
            "Adiós, que tengas un buen día."
        }},
        new FarewellPhrase() { state = CustomerState.Insatisfecho, phrases = new List<string>()
        {
            "Bueno, hasta luego.",
            "No estoy muy contento, pero adiós.",
            "Adiós, espero algo mejor la próxima vez.",
            "Esto no fue lo que esperaba, adiós.",
            "Nos vemos, pero no muy contento."
        }},
        new FarewellPhrase() { state = CustomerState.MuyInsatisfecho, phrases = new List<string>()
        {
            "¡Esto fue terrible, no vuelvo más!",
            "No estoy contento, adiós.",
            "Adiós, fue una mala experiencia.",
            "No volveré después de esto.",
            "¡Qué desastre, no pienso regresar!"
        }},
    };
    public string GetOrderPhrase(CustomerState state, string item, List<string> metals, List<string> woods)
    {
        var matchingPhrases = orderPhrases.FindAll(phrase => phrase.state == state);
        if (matchingPhrases.Count > 0)
        {
            int randomIndex = Random.Range(0, matchingPhrases.Count);

            // Obtener la frase correspondiente
            string phrase = matchingPhrases[randomIndex].phrase;

            // Determinar el artículo correcto según el género del item
            string articuloCorrecto = DeterminarArticulo(item);

            // Asegurarse de que {item} siempre se reemplace correctamente
            phrase = ReemplazarYCorregirArticulo(phrase, item, articuloCorrecto);

            // Eliminar las frases que contienen "un/una metal desconocido" o "un/una madera desconocida"
            phrase = EliminarFrasesDesconocidas(phrase, "metal");
            phrase = EliminarFrasesDesconocidas(phrase, "madera");

            // Reemplazar los placeholders de {metal} y {wood}
            string metalPhrase = FormatMaterialList(metals, "metal");
            string woodPhrase = FormatMaterialList(woods, "wood");

            // Reemplazar {metal} y {wood} en la frase si existen metales o maderas
            phrase = phrase.Replace("{metal}", metalPhrase).Replace("{wood}", woodPhrase);

            // Limpiar espacios en blanco adicionales que puedan quedar
            phrase = phrase.Replace("  ", " ").Trim();

            return phrase;
        }

        return "No hay frases de pedido disponibles para los parámetros especificados.";
    }

    // Función auxiliar para formatear listas de materiales
    private string FormatMaterialList(List<string> materials, string materialType)
    {
        if (materials == null || materials.Count == 0)
        {
            // No hay material, devuelve cadena vacía
            return "";
        }

        if (materials.Count == 1)
        {
            // Solo hay un material
            return materials[0];
        }

        // Formatear la lista separada por comas y agregar "y" antes del último material
        string formattedList = string.Join(", ", materials.Take(materials.Count - 1)) + " y " + materials.Last();
        return formattedList;
    }


    // Método para reemplazar y corregir el artículo antes de {item}, si es necesario
    private string ReemplazarYCorregirArticulo(string phrase, string item, string articuloCorrecto)
    {
        // Crear una lista de posibles combinaciones de "un/una" y el item
        string[] posiblesPlaceholders = { "{item}" };

        foreach (string placeholder in posiblesPlaceholders)
        {
            if (phrase.Contains(placeholder))
            {
                // Verificar si ya existe un artículo antes del {item} y corregirlo si es incorrecto
                phrase = CorregirArticulo(phrase, item, articuloCorrecto);

                // Reemplazar el placeholder {item} con el artículo correcto y el item
                phrase = phrase.Replace(placeholder, $"{articuloCorrecto} {item}");
            }
        }

        return phrase;
    }

    // Método auxiliar para verificar y corregir el artículo antes del {item}
    private string CorregirArticulo(string phrase, string item, string articuloCorrecto)
    {
        // Verificar si ya existe un artículo antes del {item} y si es incorrecto
        string[] articulosIncorrectos = { "un", "una", "Un", "Una" };

        foreach (string articulo in articulosIncorrectos)
        {
            // Si encuentra el artículo antes del {item}, lo reemplaza con el correcto
            if (phrase.Contains($"{articulo} {item}"))
            {
                phrase = phrase.Replace($"{articulo} {item}", $"{articuloCorrecto} {item}");
            }
        }

        return phrase;
    }

    // Método auxiliar para eliminar frases que contengan "un/una metal desconocido" o "un/una madera desconocida"
    private string EliminarFrasesDesconocidas(string phrase, string material)
    {
        // Crear una lista de combinaciones posibles, incluyendo los casos solicitados
        string[] placeholders = new string[]
        {
        $"un {material} desconocido",
        $"una {material} desconocida",
        $"Un {material} desconocido",
        $"Una {material} desconocida",
        $"de un {material} desconocido",
        $"de una {material} desconocida",
        $"De un {material} desconocido",
        $"De una {material} desconocida",
        "de desconocido",
        "de desconocida",
        "De desconocido",
        "De desconocida",
        "un desconocido",
        "una desconocida",
        "Un desconocido",
        "Una desconocida",
        "un desconocida", // Errores tipográficos
        "una desconocido",
        "Un desconocida",
        "Una desconocido"
        };

        // Reemplazar todas las combinaciones encontradas
        foreach (string placeholder in placeholders)
        {
            phrase = phrase.Replace(placeholder, "");
        }

        // Limpiar los espacios en blanco adicionales que puedan quedar
        return phrase.Replace("  ", " ").Trim();
    }




    // Método para obtener una frase de despedida basada en el estado del cliente
    public string GetFarewellPhrase(CustomerState state)
    {
        var farewellPhraseSet = farewellPhrases.Find(phraseSet => phraseSet.state == state);
        if (farewellPhraseSet != null && farewellPhraseSet.phrases.Count > 0)
        {
            int randomIndex = Random.Range(0, farewellPhraseSet.phrases.Count);
            string generatedText = farewellPhraseSet.phrases[randomIndex];

            return generatedText;
        }
        return "No hay frases de despedida disponibles para el estado especificado.";
    }

    public string DeterminarArticulo(string item)
    {
        // Regla básica: Si termina en 'a', usamos "una", si no, usamos "un".
        if (item.EndsWith("a"))
        {
            return "una";
        }
        else
        {
            return "un";
        }
    }

}

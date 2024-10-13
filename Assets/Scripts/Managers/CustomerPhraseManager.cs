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
        new OrderPhrase() { state = CustomerState.MuyFeliz, phrase = "�Este {item} de {metal} con mango de {wood} es exactamente lo que quer�a!" },
        new OrderPhrase() { state = CustomerState.MuyFeliz, phrase = "�Qu� alegr�a! Un {item} de {metal} con un mango de {wood}, perfecto." },
        new OrderPhrase() { state = CustomerState.MuyFeliz, phrase = "�Me encanta! Un {item} de {metal} con mango de {wood} es justo lo que necesitaba." },
        new OrderPhrase() { state = CustomerState.MuyFeliz, phrase = "�Incre�ble! Este {item} de {metal} con mango de {wood} es lo mejor que he visto." },
        new OrderPhrase() { state = CustomerState.MuyFeliz, phrase = "�Es perfecto! Un {item} de {metal} con mango de {wood}, estoy encantado." },

        // Frases para clientes contentos
        new OrderPhrase() { state = CustomerState.Contento, phrase = "Me gusta este {item} de {metal} con mango de {wood}. �Buen trabajo!" },
        new OrderPhrase() { state = CustomerState.Contento, phrase = "Un {item} de {metal} con mango de {wood}, justo lo que buscaba." },
        new OrderPhrase() { state = CustomerState.Contento, phrase = "Este {item} de {metal} con mango de {wood} es muy bueno, gracias." },
        new OrderPhrase() { state = CustomerState.Contento, phrase = "�Buen trabajo! Este {item} de {metal} con mango de {wood} es de buena calidad." },
        new OrderPhrase() { state = CustomerState.Contento, phrase = "Estoy satisfecho con este {item} de {metal} con mango de {wood}, gracias." },

        // Frases para clientes neutrales
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Este {item} de {metal} con mango de {wood} est� bien, creo." },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Un {item} de {metal} con mango de {wood}, es aceptable." },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Este {item} de {metal} con mango de {wood} cumplir� su funci�n." },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "No est� mal, este {item} de {metal} con mango de {wood} servir�." },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Podr�a ser mejor, pero este {item} de {metal} con mango de {wood} es suficiente." },

        // Frases para clientes insatisfechos
        new OrderPhrase() { state = CustomerState.Insatisfecho, phrase = "No es exactamente lo que quer�a, pero este {item} de {metal} con mango de {wood} servir�." },
        new OrderPhrase() { state = CustomerState.Insatisfecho, phrase = "No estoy muy convencido con este {item} de {metal} con mango de {wood}." },
        new OrderPhrase() { state = CustomerState.Insatisfecho, phrase = "Esperaba m�s, este {item} de {metal} con mango de {wood} no es lo que quer�a." },
        new OrderPhrase() { state = CustomerState.Insatisfecho, phrase = "Este {item} de {metal} con mango de {wood} no cumple con mis expectativas." },
        new OrderPhrase() { state = CustomerState.Insatisfecho, phrase = "No estoy satisfecho con este {item} de {metal} con mango de {wood}." },

        // Frases para clientes muy insatisfechos
        new OrderPhrase() { state = CustomerState.MuyInsatisfecho, phrase = "Este {item} de {metal} con mango de {wood} es terrible. �Qu� decepci�n!" },
        new OrderPhrase() { state = CustomerState.MuyInsatisfecho, phrase = "�No puedo creer que me hayas hecho este {item} de {metal} con mango de {wood}!" },
        new OrderPhrase() { state = CustomerState.MuyInsatisfecho, phrase = "Este {item} de {metal} con mango de {wood} es lo peor que he visto." },
        new OrderPhrase() { state = CustomerState.MuyInsatisfecho, phrase = "�Es inaceptable! Este {item} de {metal} con mango de {wood} no sirve para nada." },
        new OrderPhrase() { state = CustomerState.MuyInsatisfecho, phrase = "�Qu� desastre! Este {item} de {metal} con mango de {wood} es una verg�enza." },
    };

    [SerializeField]
    private List<FarewellPhrase> farewellPhrases = new List<FarewellPhrase>()
    {
        new FarewellPhrase() { state = CustomerState.MuyFeliz, phrases = new List<string>()
        {
            "�Gracias, fue un placer!",
            "�Hasta luego, estar� de vuelta!",
            "�Qu� bien me siento, nos vemos pronto!",
            "�Nos vemos! �No puedo esperar a regresar!",
            "�Maravilloso, gracias por todo!"
        }},
        new FarewellPhrase() { state = CustomerState.Contento, phrases = new List<string>()
        {
            "Gracias, hasta la pr�xima.",
            "Nos vemos pronto.",
            "Buen trabajo, nos vemos.",
            "�Gracias, ha sido genial!",
            "�Hasta luego, volver� pronto!"
        }},
        new FarewellPhrase() { state = CustomerState.Neutral, phrases = new List<string>()
        {
            "Gracias, adi�s.",
            "Hasta luego.",
            "Nos vemos.",
            "Bueno, eso es todo, adi�s.",
            "Adi�s, que tengas un buen d�a."
        }},
        new FarewellPhrase() { state = CustomerState.Insatisfecho, phrases = new List<string>()
        {
            "Bueno, hasta luego.",
            "No estoy muy contento, pero adi�s.",
            "Adi�s, espero algo mejor la pr�xima vez.",
            "Esto no fue lo que esperaba, adi�s.",
            "Nos vemos, pero no muy contento."
        }},
        new FarewellPhrase() { state = CustomerState.MuyInsatisfecho, phrases = new List<string>()
        {
            "�Esto fue terrible, no vuelvo m�s!",
            "No estoy contento, adi�s.",
            "Adi�s, fue una mala experiencia.",
            "No volver� despu�s de esto.",
            "�Qu� desastre, no pienso regresar!"
        }},
    };
    public string GetOrderPhrase(CustomerState state, string item, List<string> metals, List<string> woods)
    {
        // Buscar las frases que coincidan con el estado del cliente
        var matchingPhrases = orderPhrases.FindAll(phrase => phrase.state == state);

        if (matchingPhrases.Count > 0)
        {
            // Seleccionar una frase aleatoria
            int randomIndex = Random.Range(0, matchingPhrases.Count);
            string phrase = matchingPhrases[randomIndex].phrase;

            // Reemplazar los placeholders de {metal} y {wood} primero
            string metalPhrase = FormatMaterialList(metals, "metal");
            string woodPhrase = FormatMaterialList(woods, "wood");
            phrase = phrase.Replace("{metal}", metalPhrase).Replace("{wood}", woodPhrase);

            // Determinar el art�culo correcto seg�n el g�nero del item
            string articuloCorrecto = DeterminarArticulo(item);

            // Reemplazar {item} por el nombre del item con su art�culo correspondiente despu�s
            phrase = ReemplazarYCorregirArticulo(phrase, item, articuloCorrecto);

            // Limpiar espacios en blanco adicionales que puedan quedar
            phrase = phrase.Replace("  ", " ").Trim();


            // Eliminar las frases que contienen "un/una metal desconocido" o "un/una madera desconocida" al final
            phrase = EliminarFrasesDesconocidas(phrase, "metal");
            phrase = EliminarFrasesDesconocidas(phrase, "madera");

            phrase = SustituirFrasesConMangoMetalMaderaPorPunto(phrase);
            phrase = ArreglarPuntuacion(phrase);

            // Corregir las may�sculas incorrectas en medio de la frase
            phrase = CorrectCapitalization(phrase);

            return phrase;
        }

        return "No hay frases de pedido disponibles para los par�metros especificados.";
    }

    private string ArreglarPuntuacion(string phrase)
    {
        // Expresi�n regular que encuentra casos de ". ," o ", ." con o sin espacios entre medias
        phrase = System.Text.RegularExpressions.Regex.Replace(phrase, @"\s*(\.\s*,|,\s*\.)\s*", ".", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

        // Limpiar espacios adicionales alrededor de los puntos
        phrase = System.Text.RegularExpressions.Regex.Replace(phrase, @"\s*\.\s*", ". ").Trim();

        return phrase;
    }


    private string SustituirFrasesConMangoMetalMaderaPorPunto(string phrase)
    {
        // Crear una lista de patrones a sustituir por un punto, incluyendo sus variaciones
        string[] patrones = new string[]
        {
        "con mango", "de mango", "de metal", "de madera",
        "Con mango", "De mango", "De metal", "De madera",
        "con Mango", "de Mango", "de Metal", "de Madera",
        "Con Mango", "De Mango", "De Metal", "De Madera",
        "con metal", "con madera",
        "Con metal", "Con madera",
        "con Metal", "con Madera",
        "Con Metal", "Con Madera"
        };

        // Reemplazar solo si no est� seguido por " de " (ignorar si va acompa�ado de "de algo")
        foreach (string patron in patrones)
        {
            // Reemplazar solo si no est� seguido por " de " o m�s contenido
            phrase = System.Text.RegularExpressions.Regex.Replace(phrase, @"\b" + System.Text.RegularExpressions.Regex.Escape(patron) + @"(?!\s+de\s+)", ".", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();
        }

        // Asegurar que si hay espacios o m�ltiples puntos, los limpie correctamente
        phrase = System.Text.RegularExpressions.Regex.Replace(phrase, @"(\s*\.\s*)+", ". ").Trim();

        // Asegurar que los puntos finales sean correctos
        phrase = System.Text.RegularExpressions.Regex.Replace(phrase, @"\s*\.\s*$", ".").Trim();

        return phrase;
    }


    private string CorrectCapitalization(string input)
    {
        char[] delimiters = { '.', '!', '?' }; // Delimitadores que marcan el final de una oraci�n
        string result = "";
        bool capitalizeNext = true; // Controla si la siguiente letra debe ser may�scula

        for (int i = 0; i < input.Length; i++)
        {
            char currentChar = input[i];

            if (char.IsWhiteSpace(currentChar))
            {
                // Si encontramos un espacio, lo agregamos pero mantenemos la decisi�n de capitalizaci�n
                result += currentChar;
            }
            else if (capitalizeNext && char.IsLetter(currentChar))
            {
                // Capitalizar la primera letra encontrada si `capitalizeNext` est� habilitado
                result += char.ToUpper(currentChar);
                capitalizeNext = false; // Desactivar capitalizaci�n hasta encontrar un nuevo delimitador
            }
            else if (!capitalizeNext && char.IsLetter(currentChar))
            {
                // Convertir a min�scula cualquier letra que no deba estar en may�scula
                result += char.ToLower(currentChar);
            }
            else
            {
                // Agregar el car�cter tal cual si no es una letra que deba ser capitalizada o convertida
                result += currentChar;

                // Verificar si el car�cter actual es un delimitador (punto, exclamaci�n, interrogaci�n)
                if (delimiters.Contains(currentChar))
                {
                    capitalizeNext = true; // La pr�xima letra debe ser may�scula despu�s del delimitador
                }
            }
        }

        return result.Trim();
    }


    // Funci�n auxiliar para formatear listas de materiales
    private string FormatMaterialList(List<string> materials, string materialType)
    {
        if (materials == null || materials.Count == 0)
        {
            // No hay material, devuelve cadena vac�a
            return "";
        }

        if (materials.Count == 1)
        {
            // Solo hay un material
            return materials[0];
        }

        // Formatear la lista separada por comas y agregar "y" antes del �ltimo material
        string formattedList = string.Join(", ", materials.Take(materials.Count - 1)) + " y " + materials.Last();
        return formattedList;
    }


    // M�todo para reemplazar y corregir el art�culo antes de {item}, si es necesario
    private string ReemplazarYCorregirArticulo(string phrase, string item, string articuloCorrecto)
    {
        // Crear una lista de posibles combinaciones de "un/una" y el item
        string[] posiblesPlaceholders = { "{item}" };

        foreach (string placeholder in posiblesPlaceholders)
        {
            if (phrase.Contains(placeholder))
            {
                // Verificar si ya existe un art�culo antes del {item} y corregirlo si es incorrecto
                phrase = CorregirArticulo(phrase, item, articuloCorrecto);

                // Reemplazar el placeholder {item} con el art�culo correcto y el item
                phrase = phrase.Replace(placeholder, $"{articuloCorrecto} {item}");
            }
        }

        return phrase;
    }

    // M�todo auxiliar para verificar y corregir el art�culo antes del {item}
    private string CorregirArticulo(string phrase, string item, string articuloCorrecto)
    {
        // Verificar si ya existe un art�culo antes del {item} y si es incorrecto
        string[] articulosIncorrectos = { "un", "una", "Un", "Una" };

        foreach (string articulo in articulosIncorrectos)
        {
            // Si encuentra el art�culo antes del {item}, lo reemplaza con el correcto
            if (phrase.Contains($"{articulo} {item}"))
            {
                phrase = phrase.Replace($"{articulo} {item}", $"{articuloCorrecto} {item}");
            }
        }

        return phrase;
    }

    // M�todo auxiliar para eliminar frases que contengan "un/una metal desconocido" o "un/una madera desconocida"
    private string EliminarFrasesDesconocidas(string phrase, string material)
    {
        // Crear una lista de palabras clave y combinaciones comunes
        string[] palabrasClave = new string[] { material, "metal", "mango", "madera" };

        // Crear una lista de combinaciones posibles
        List<string> placeholders = new List<string>();

        foreach (string palabra in palabrasClave)
        {
            // A�adir todas las combinaciones, permitiendo duplicados similares
            placeholders.AddRange(new string[]
            {
            // Casos con art�culo
            $"un {palabra} desconocido", $"una {palabra} desconocida", $"Un {palabra} desconocido", $"Una {palabra} desconocida",
            $"de un {palabra} desconocido", $"de una {palabra} desconocida", $"De un {palabra} desconocido", $"De una {palabra} desconocida",
            $"unos {palabra}s desconocidos", $"unas {palabra}s desconocidas", $"Unos {palabra}s desconocidos", $"Unas {palabra}s desconocidas",
            $"con un {palabra} de desconocido", $"con una {palabra} de desconocida", $"con un {palabra} desconocido", $"con una {palabra} desconocida",
            $"con unos {palabra}s de desconocido", $"con unas {palabra}s de desconocida", $"con unos {palabra}s desconocidos", $"con unas {palabra}s desconocidas",
            $"sin un {palabra} desconocido", $"sin una {palabra} desconocida", $"por un {palabra} desconocido", $"para un {palabra} desconocido",
            $"todo el {palabra} desconocido", $"toda la {palabra} desconocida", $"todos los {palabra}s desconocidos", $"todas las {palabra}s desconocidas",
            $"el {palabra} desconocido", $"la {palabra} desconocida", $"los {palabra}s desconocidos", $"las {palabra}s desconocidas",
            $"con su {palabra} desconocido", $"con su {palabra} desconocida", $"con este {palabra} desconocido", $"con estos {palabra}s desconocidos",
            $"con alg�n {palabra} desconocido", $"con algunos {palabra}s desconocidos", $"sin ning�n {palabra} desconocido", $"sin ningunos {palabra}s desconocidos",
            $"entre {palabra} desconocido y {palabra} desconocida", $"ning�n {palabra} desconocido", $"ninguna {palabra} desconocida",
            $"{palabra} desconocido", $"{palabra} desconocida", $"{palabra}s desconocidos", $"{palabra}s desconocidas",

            // Nuevas combinaciones sin art�culos
            $"con {palabra} de desconocido", $"con {palabra} desconocido",
            $"de {palabra} desconocido", $"de {palabra} desconocida",

            // Nuevas combinaciones con otras preposiciones
            $"sobre {palabra} de desconocido", $"sobre {palabra} desconocido",
            $"bajo {palabra} de desconocido", $"bajo {palabra} desconocido",
            $"junto a un {palabra} de desconocido", $"junto a una {palabra} de desconocida",

            // Combinaciones con direcciones y posesiones
            $"hacia un {palabra} desconocido", $"hacia una {palabra} desconocida",
            $"al {palabra} desconocido", $"a la {palabra} desconocida",
            $"del {palabra} de desconocido", $"del {palabra} desconocido",
            $"por el {palabra} desconocido", $"por la {palabra} desconocida",

            // **Ampliaci�n de combinaciones con verbos**
            $"hecho de {palabra} desconocido", $"hecha de {palabra} desconocida",
            $"fabricado con {palabra} desconocido", $"fabricada con {palabra} desconocida",
            $"compuesto de {palabra} desconocido", $"compuesta de {palabra} desconocida",
            $"forjado con {palabra} desconocido", $"forjada con {palabra} desconocida",
            $"construido con {palabra} desconocido", $"construida con {palabra} desconocida",
            $"producido con {palabra} desconocido", $"producida con {palabra} desconocida",
            $"creado con {palabra} desconocido", $"creada con {palabra} desconocida",
            $"dise�ado con {palabra} desconocido", $"dise�ada con {palabra} desconocida",
            $"tallado en {palabra} desconocido", $"tallada en {palabra} desconocida",
            $"modelado con {palabra} desconocido", $"modelada con {palabra} desconocida",
            $"fundido en {palabra} desconocido", $"fundida en {palabra} desconocida",
            $"labrado con {palabra} desconocido", $"labrada con {palabra} desconocida",
            $"esculpido en {palabra} desconocido", $"esculpida en {palabra} desconocida",
            $"mezclado con {palabra} desconocido", $"mezclada con {palabra} desconocida",
            $"reparado con {palabra} desconocido", $"reparada con {palabra} desconocida",
            $"ensamblado con {palabra} desconocido", $"ensamblada con {palabra} desconocida",
            $"modificado con {palabra} desconocido", $"modificada con {palabra} desconocida",

            // Negaciones y frases m�s complejas
            $"sin ning�n rastro de {palabra} desconocido", $"sin ninguna pista de {palabra} desconocida",
            $"cuando {palabra} sea desconocido", $"cuando {palabra} sea de desconocido",
            $"si el {palabra} es desconocido", $"si la {palabra} es desconocida",
            $"antes de {palabra} desconocido", $"antes de {palabra} de desconocido",
            $"despu�s de {palabra} desconocido", $"despu�s de {palabra} de desconocido",

            // Casos de errores comunes o variantes simples
            "un desconocida", "una desconocido", "Un desconocida", "Una desconocido",
            "de desconocida", "de desconocido", "De desconocida", "De desconocido"
            });
        }

        // Reemplazar todas las combinaciones encontradas de forma insensible a may�sculas/min�sculas
        foreach (string placeholder in placeholders)
        {
            phrase = ReplaceIgnoreCase(phrase, placeholder, "").Trim();
        }

        // Limpiar espacios en blanco adicionales
        phrase = System.Text.RegularExpressions.Regex.Replace(phrase, @"\s+", " ").Trim();

        return phrase;
    }

    // M�todo auxiliar para reemplazar ignorando may�sculas/min�sculas
    private string ReplaceIgnoreCase(string input, string search, string replacement)
    {
        return System.Text.RegularExpressions.Regex.Replace(input,
            System.Text.RegularExpressions.Regex.Escape(search),
            replacement.Replace("$", "$$"),
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }

    // M�todo para obtener una frase de despedida basada en el estado del cliente
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
        // Regla b�sica: Si termina en 'a', usamos "una", si no, usamos "un".
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

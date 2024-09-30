using System.Collections;
using System.Collections.Generic;
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

    // Método para obtener una frase de pedido basada en el estado del cliente, el ítem, el metal y la madera
    public string GetOrderPhrase(CustomerState state, string item, string metal, string wood)
    {
        var matchingPhrases = orderPhrases.FindAll(phrase => phrase.state == state);
        if (matchingPhrases.Count > 0)
        {
            int randomIndex = Random.Range(0, matchingPhrases.Count);

            // Obtener la frase correspondiente
            string phrase = matchingPhrases[randomIndex].phrase;

            // Determinar el artículo correcto según el género del item
            string articulo = DeterminarArticulo(item);

            // Reemplazar cualquier "un" o "una" en la frase con el artículo correcto antes del {item}
            // Esto asume que las frases tienen "un {item}" o "una {item}"
            if (phrase.Contains("un {item}"))
            {
                phrase = phrase.Replace("un {item}", $"{articulo} {item}");
            }
            else if (phrase.Contains("una {item}"))
            {
                phrase = phrase.Replace("una {item}", $"{articulo} {item}");
            }
            else
            {
                // Si no tiene un artículo explícito, solo reemplazamos el {item}
                phrase = phrase.Replace("{item}", $"{articulo} {item}");
            }

            // Reemplazar los marcadores de {metal} y {wood}
            phrase = phrase.Replace("{metal}", metal);
            phrase = phrase.Replace("{wood}", wood);

            return phrase;
        }

        return "No hay frases de pedido disponibles para los parámetros especificados.";
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

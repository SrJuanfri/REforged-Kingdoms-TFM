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

    // M�todo para obtener una frase de pedido basada en el estado del cliente, el �tem, el metal y la madera
    public string GetOrderPhrase(CustomerState state, string item, string metal, string wood)
    {
        var matchingPhrases = orderPhrases.FindAll(phrase => phrase.state == state);
        if (matchingPhrases.Count > 0)
        {
            int randomIndex = Random.Range(0, matchingPhrases.Count);

            // Obtener la frase correspondiente
            string phrase = matchingPhrases[randomIndex].phrase;

            // Determinar el art�culo correcto seg�n el g�nero del item
            string articulo = DeterminarArticulo(item);

            // Reemplazar cualquier "un" o "una" en la frase con el art�culo correcto antes del {item}
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
                // Si no tiene un art�culo expl�cito, solo reemplazamos el {item}
                phrase = phrase.Replace("{item}", $"{articulo} {item}");
            }

            // Reemplazar los marcadores de {metal} y {wood}
            phrase = phrase.Replace("{metal}", metal);
            phrase = phrase.Replace("{wood}", wood);

            return phrase;
        }

        return "No hay frases de pedido disponibles para los par�metros especificados.";
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

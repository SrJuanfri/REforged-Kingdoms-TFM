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
        // Frases para clientes felices
        new OrderPhrase() { state = CustomerState.Happy, phrase = "¡Hola! Me gustaría un {item} de {metal} con mango de {wood}, por favor." },
        new OrderPhrase() { state = CustomerState.Happy, phrase = "¡Qué alegría verte! Quiero un {item} de {metal} con un mango de {wood}." },
        new OrderPhrase() { state = CustomerState.Happy, phrase = "Un {item} de {metal} con mango de {wood} suena perfecto, ¿verdad?" },
        new OrderPhrase() { state = CustomerState.Happy, phrase = "¡Maravilloso día! ¿Podrías hacerme un {item} de {metal} con mango de {wood}?" },
        new OrderPhrase() { state = CustomerState.Happy, phrase = "Necesito un {item} de {metal} con un robusto mango de {wood}, por favor." },
        new OrderPhrase() { state = CustomerState.Happy, phrase = "Un {item} de {metal} con mango de {wood} sería genial." },
        new OrderPhrase() { state = CustomerState.Happy, phrase = "¿Tienes {item}s de {metal} con mango de {wood}? Me encantaría uno." },
        new OrderPhrase() { state = CustomerState.Happy, phrase = "Me gustaría un {item} de {metal} con mango de {wood}, gracias." },
        new OrderPhrase() { state = CustomerState.Happy, phrase = "¿Podrías hacerme un {item} de {metal} con mango de {wood}?" },

        // Frases para clientes enojados
        new OrderPhrase() { state = CustomerState.Angry, phrase = "Rápido, dame un {item} de {metal} con mango de {wood}. No tengo todo el día." },
        new OrderPhrase() { state = CustomerState.Angry, phrase = "Necesito un {item} de {metal} con mango de {wood}, ¡y que sea rápido!" },
        new OrderPhrase() { state = CustomerState.Angry, phrase = "¡Oye! Un {item} de {metal} con mango de {wood}, ahora." },
        new OrderPhrase() { state = CustomerState.Angry, phrase = "Dame un {item} de {metal} con mango de {wood}, y hazlo rápido." },
        new OrderPhrase() { state = CustomerState.Angry, phrase = "Quiero un {item} de {metal} con mango de {wood}, ¡de inmediato!" },
        new OrderPhrase() { state = CustomerState.Angry, phrase = "Un {item} de {metal} con mango de {wood}, y date prisa." },
        new OrderPhrase() { state = CustomerState.Angry, phrase = "Necesito un {item} de {metal} con mango de {wood}, ¡ahora mismo!" },
        new OrderPhrase() { state = CustomerState.Angry, phrase = "¡Vamos! Un {item} de {metal} con mango de {wood}." },
        new OrderPhrase() { state = CustomerState.Angry, phrase = "Rápido, un {item} de {metal} con mango de {wood}." },

        // Frases para clientes neutrales
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Quisiera pedir un {item} de {metal} con mango de {wood}." },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Voy a necesitar un {item} de {metal} con un mango de {wood}, por favor." },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "¿Podrías prepararme un {item} de {metal} con mango de {wood}?" },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Un {item} de {metal} con mango de {wood}, por favor." },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Me gustaría un {item} de {metal} con mango de {wood}." },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Por favor, un {item} de {metal} con mango de {wood}." },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Voy a necesitar un {item} de {metal} con mango de {wood}." },
        new OrderPhrase() { state = CustomerState.Neutral, phrase = "Quisiera un {item} de {metal} con mango de {wood}, gracias." },
    };

    [SerializeField]
    private List<FarewellPhrase> farewellPhrases = new List<FarewellPhrase>()
    {
        new FarewellPhrase() { state = CustomerState.Happy, phrases = new List<string>()
        {
            "¡Gracias y hasta luego!",
            "Nos vemos pronto.",
            "¡Que tengas un excelente día!",
            "¡Fue un placer verte! Hasta la próxima.",
            "Cuídate y que tengas un gran día.",
            "¡Hasta luego! Espero verte pronto.",
            "Gracias por tu ayuda. ¡Nos vemos!",
            "¡Disfruta tu día! Hasta luego."
        }},
        new FarewellPhrase() { state = CustomerState.Angry, phrases = new List<string>()
        {
            "Adiós.",
            "Hasta nunca.",
            "¡Vete ya!",
            "No vuelvas.",
            "Esto fue un desastre. Adiós.",
            "No estoy contento, pero adiós.",
            "Largo de aquí.",
            "Me voy, y no me verás de nuevo."
        }},
        new FarewellPhrase() { state = CustomerState.Neutral, phrases = new List<string>()
        {
            "Gracias, hasta luego.",
            "Nos vemos.",
            "Que tengas un buen día.",
            "Hasta la próxima.",
            "Cuídate.",
            "Espero que todo salga bien. Adiós.",
            "Hasta luego, gracias por todo.",
            "Nos vemos pronto."
        }},
    };

    // Método para obtener una frase de pedido basada en el estado del cliente, el ítem, el metal y la madera
    public string GetOrderPhrase(CustomerState state, string item, string metal, string wood)
    {
        var matchingPhrases = orderPhrases.FindAll(phrase => phrase.state == state);
        if (matchingPhrases.Count > 0)
        {
            int randomIndex = Random.Range(0, matchingPhrases.Count);
            string generatedText = matchingPhrases[randomIndex].phrase
                .Replace("{item}", item)
                .Replace("{metal}", metal)
                .Replace("{wood}", wood);

            // Corregir el texto generado
            return generatedText;
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

            // Corregir el texto generado
            return generatedText;
        }
        return "No hay frases de despedida disponibles para el estado especificado.";
    }
}

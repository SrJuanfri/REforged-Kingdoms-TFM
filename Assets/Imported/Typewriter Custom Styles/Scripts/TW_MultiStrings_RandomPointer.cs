using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TW_MultiStrings_RandomPointer : MonoBehaviour
{

    public bool LaunchOnStart = true;
    public int timeOut = 1;
    public RandomCharsType Charstype = RandomCharsType.UpperCase;
    public enum RandomCharsType { LowerCase, UpperCase, LowerUpperCase, Digits, Symbols, All };
    public string[] MultiStrings = new string[1];
    public string ORIGINAL_TEXT;

    public AudioClip typingSound; // Sonido a reproducir
    private AudioSource audioSource; // Fuente de audio

    private float time = 0f;
    private int сharIndex = 0;
    private int index_of_string = 0;
    private bool start;
    private bool hasFinished = false; // Para saber si ha terminado de escribir
    private List<int> n_l_list;

    private static System.Random random = new System.Random();
    private static string lowerCase = "abcdefghijklmnopqrstuvwxyz";
    private static string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static string lowerupperCase = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static string digits = "0123456789";
    private static string symbols = "#@$^*?~&";
    private static string all = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789#@$^*?~&";

    void Start()
    {
        MultiStrings[0] = gameObject.GetComponent<Text>().text;
        ORIGINAL_TEXT = gameObject.GetComponent<Text>().text;
        gameObject.GetComponent<Text>().text = "";
        audioSource = gameObject.AddComponent<AudioSource>(); // Asignar fuente de audio
        audioSource.clip = typingSound; // Asignar el sonido de typing
        audioSource.loop = false; // No repetir el sonido, lo controlamos manualmente
        if (LaunchOnStart)
        {
            StartTypewriter();
        }
    }

    void Update()
    {
        if (start == true)
        {
            NewLineCheck(ORIGINAL_TEXT);
        }
    }

    public void StartTypewriter()
    {
        start = true;
        hasFinished = false;
        сharIndex = 0;
        time = 0f;

        // Comenzar a reproducir el sonido si no está sonando
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void SkipTypewriter()
    {
        сharIndex = ORIGINAL_TEXT.Length - 1;
    }

    public void NextString()
    {
        // Si aún no se ha terminado de escribir la frase actual, la completamos
        if (сharIndex < ORIGINAL_TEXT.Length)
        {
            CompleteCurrentPhrase();
            // Después de completar la frase, volvemos a llamar a NextString para procesar la siguiente
            //NextString();
        }
        else
        {
            // Si ya está completa, seguimos con el comportamiento habitual de pasar a la siguiente frase
            start = true;
            hasFinished = false;
            сharIndex = 0;
            time = 0f;
            if (index_of_string + 1 < MultiStrings.Length)
            {
                index_of_string++;
            }
            else
            {
                index_of_string = 0;
            }
            ORIGINAL_TEXT = MultiStrings[index_of_string];

            // Si el audio ha terminado y aún hay texto, lo reiniciamos
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    // Método para completar la frase actual instantáneamente
    private void CompleteCurrentPhrase()
    {
        // Configuramos el índice del carácter al final del texto original para completar la frase
        сharIndex = ORIGINAL_TEXT.Length;

        // Mostrar inmediatamente la frase completa
        gameObject.GetComponent<Text>().text = ORIGINAL_TEXT;

        // Parar el sonido de escribir, ya que hemos terminado
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

    }

    public void LastString()
    {
        start = true;
        hasFinished = false;
        ORIGINAL_TEXT = MultiStrings[MultiStrings.Length - 1];
        сharIndex = ORIGINAL_TEXT.Length - 1;

        // Si el audio ha terminado, lo reiniciamos
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void NewLineCheck(string S)
    {
        if (S.Contains("\n"))
        {
            StartCoroutine(MakeRandomTypewriterTextWithNewLine(S, MakeList(S)));
        }
        else
        {
            StartCoroutine(MakeRandomTypewriterText(S));
        }
    }

    private IEnumerator MakeRandomTypewriterText(string ORIGINAL)
    {
        start = false;
        if (сharIndex != ORIGINAL.Length + 1)
        {
            string emptyString = new string(' ', ORIGINAL.Length - 1);
            string TEXT = ORIGINAL.Substring(0, сharIndex) + RandomChar(ORIGINAL, сharIndex);
            if (сharIndex < ORIGINAL.Length) TEXT = TEXT + emptyString.Substring(сharIndex);
            gameObject.GetComponent<Text>().text = TEXT;

            time += 1f;
            yield return new WaitForSeconds(0.01f);
            CharIndexPlus();
            start = true;
        }
        else
        {
            if (index_of_string == MultiStrings.Length - 1) // Solo marca como finalizado si es el último string
            {
                hasFinished = true;
            }

            // Parar el audio cuando termine de escribir
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private IEnumerator MakeRandomTypewriterTextWithNewLine(string ORIGINAL, List<int> List)
    {
        start = false;
        if (сharIndex != ORIGINAL.Length + 1)
        {
            string emptyString = new string(' ', ORIGINAL.Length - 1);
            string TEXT = ORIGINAL.Substring(0, сharIndex) + RandomChar(ORIGINAL, сharIndex);
            if (сharIndex < ORIGINAL.Length) TEXT = TEXT + emptyString.Substring(сharIndex);
            TEXT = InsertNewLine(TEXT, List);
            gameObject.GetComponent<Text>().text = TEXT;

            time += 1f;
            yield return new WaitForSeconds(0.01f);
            CharIndexPlus();
            start = true;
        }
        else
        {
            if (index_of_string == MultiStrings.Length - 1) // Solo marca como finalizado si es el último string
            {
                hasFinished = true;
            }

            // Parar el audio cuando termine de escribir
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private string RandomChar(string ORIGINAL, int currentCharIndex)
    {
        string randomChar;
        if (currentCharIndex != ORIGINAL.Length)
        {
            string chars = GetCharsType(Charstype);
            randomChar = new string(chars[random.Next(0, chars.Length)], 1).ToString();
        }
        else
        {
            randomChar = "";
        }
        return randomChar;
    }

    private List<int> MakeList(string S)
    {
        n_l_list = new List<int>();
        for (int i = 0; i < S.Length; i++)
        {
            if (S[i] == '\n')
            {
                n_l_list.Add(i);
            }
        }
        return n_l_list;
    }

    private string InsertNewLine(string _TEXT, List<int> _List)
    {
        for (int index = 0; index < _List.Count; index++)
        {
            if (сharIndex - 1 < _List[index])
            {
                _TEXT = _TEXT.Insert(_List[index], "\n");
            }
        }
        return _TEXT;
    }

    private string GetCharsType(RandomCharsType T)
    {
        string s = "";
        if (T == RandomCharsType.LowerCase)
            s = lowerCase;
        if (T == RandomCharsType.UpperCase)
            s = upperCase;
        if (T == RandomCharsType.LowerUpperCase)
            s = lowerupperCase;
        if (T == RandomCharsType.Digits)
            s = digits;
        if (T == RandomCharsType.Symbols)
            s = symbols;
        if (T == RandomCharsType.All)
            s = all;
        return s;
    }

    private void CharIndexPlus()
    {
        if (time == timeOut)
        {
            time = 0f;
            сharIndex += 1;
        }
    }

    // Método para verificar si ha terminado de escribir el último item de la lista
    public bool HasFinishedWriting()
    {
        return hasFinished;
    }
}

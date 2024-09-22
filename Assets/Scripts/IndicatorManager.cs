using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IndicatorManager : MonoBehaviour
{
    [Header("Initial Values")]
    [SerializeField] private int initialSatisfaction = 50;
    [SerializeField] private int initialDanger = 50;
    [SerializeField] private int initialOpinion = 50;

    [Header("Indicator Ranges")]
    [SerializeField] private int minIndicatorValue = 0;
    [SerializeField] private int maxIndicatorValue = 100;

    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    public int Satisfaction { get; private set; }
    public int Danger { get; private set; }
    public int Opinion { get; private set; }

    private void Start()
    {
        // Initialize indicators
        Satisfaction = initialSatisfaction;
        Danger = initialDanger;
        Opinion = initialOpinion;
    }

    private void Update()
    {
        // Check for critical conditions
        CheckForBadEnding();

        DisplayIndicators(textMeshProUGUI);
    }

    public void ProcessItem(string itemType, string quality)
    {
        switch (itemType)
        {
            case "Arma":
                UpdateIndicatorsForWeapon(quality);
                break;
            case "Herramienta":
                UpdateIndicatorsForTool(quality);
                break;
            default:
                Debug.LogWarning("Unknown item type.");
                break;
        }
    }

    private void UpdateIndicatorsForWeapon(string quality)
    {
        switch (quality)
        {
            case "Rechazada":
                AdjustSatisfaction(-25);
                AdjustDanger(0);
                AdjustOpinion(-20);
                break;
            case "Mal Hecha (Diseño)":
                AdjustSatisfaction(-15);
                AdjustDanger(0);
                AdjustOpinion(-20);
                break;
            case "Mal Hecha (Material)":
                AdjustSatisfaction(-20);
                AdjustDanger(0);
                AdjustOpinion(-15);
                break;
            case "Mal Hecha (Diseño y Material)":
                AdjustSatisfaction(-35);
                AdjustDanger(0);
                AdjustOpinion(-25);
                break;
            case "Bien Hecha":
                AdjustSatisfaction(30);
                AdjustDanger(0);
                AdjustOpinion(30);
                break;
            default:
                Debug.LogWarning("Unknown weapon quality.");
                break;
        }
    }

    private void UpdateIndicatorsForTool(string quality)
    {
        switch (quality)
        {
            case "Rechazada":
                AdjustSatisfaction(-25);
                AdjustDanger(0);
                AdjustOpinion(-20);
                break;
            case "Mal Hecha (Diseño)":
                AdjustSatisfaction(-15);
                AdjustDanger(0);
                AdjustOpinion(-20);
                break;
            case "Mal Hecha (Material)":
                AdjustSatisfaction(-20);
                AdjustDanger(0);
                AdjustOpinion(-15);
                break;
            case "Mal Hecha (Diseño y Material)":
                AdjustSatisfaction(-35);
                AdjustDanger(0);
                AdjustOpinion(-25);
                break;
            case "Bien Hecha":
                AdjustSatisfaction(30);
                AdjustDanger(0);
                AdjustOpinion(30);
                break;
            default:
                Debug.LogWarning("Unknown tool quality.");
                break;
        }
    }

    private void AdjustSatisfaction(int amount)
    {
        Satisfaction = Mathf.Clamp(Satisfaction + amount, minIndicatorValue, maxIndicatorValue);
        CheckForBadEnding();
    }

    private void AdjustDanger(int amount)
    {
        Danger = Mathf.Clamp(Danger + amount, minIndicatorValue, maxIndicatorValue);
        CheckForBadEnding();
    }

    private void AdjustOpinion(int amount)
    {
        Opinion = Mathf.Clamp(Opinion + amount, minIndicatorValue, maxIndicatorValue);
        CheckForBadEnding();
    }

    private void CheckForBadEnding()
    {
        if (Satisfaction <= minIndicatorValue || Opinion <= minIndicatorValue)
        {
            TriggerBadEnding(DetermineBadEndingType());
        }

        if (Danger >= maxIndicatorValue || Danger <= minIndicatorValue)
        {
            TriggerBadEnding(DetermineBadEndingType());
        }
    }

    private void TriggerBadEnding(BadEndingType endingType)
    {
        switch (endingType)
        {
            case BadEndingType.LowSatisfaction:
                Debug.Log("Satisfaction has reached a critical low. Triggering bad ending: Low Satisfaction.");
                // Implement bad ending logic for low satisfaction
                SceneManager.LoadScene("FinalFelicidad0");
                break;
            case BadEndingType.LowOpinion:
                Debug.Log("Opinion has reached a critical low. Triggering bad ending: Low Opinion.");
                // Implement bad ending logic for low opinion
                SceneManager.LoadScene("FinalOpinion0");
                break;
            case BadEndingType.HighDanger:
                Debug.Log("Danger level has reached a critical high. Triggering bad ending: High Danger.");
                // Implement bad ending logic for high danger
                SceneManager.LoadScene("FinalPeligrosidad100");
                break;
            case BadEndingType.LowDanger:
                Debug.Log("Danger level has reached a critical low. Triggering bad ending: Low Danger.");
                // Implement bad ending logic for low danger
                SceneManager.LoadScene("FinalPeligrosidad0");
                break;
            default:
                Debug.LogWarning("Unknown bad ending type.");
                break;
        }
    }

    private BadEndingType DetermineBadEndingType()
    {
        if (Satisfaction <= minIndicatorValue)
        {
            return BadEndingType.LowSatisfaction;
        }

        if (Opinion <= minIndicatorValue)
        {
            return BadEndingType.LowOpinion;
        }

        if (Danger >= maxIndicatorValue)
        {
            return BadEndingType.HighDanger;
        }

        if (Danger <= minIndicatorValue)
        {
            return BadEndingType.LowDanger;
        }

        return BadEndingType.None;
    }

    private enum BadEndingType
    {
        None,
        LowSatisfaction,
        LowOpinion,
        HighDanger,
        LowDanger
    }

    // Método para actualizar un TextMeshPro con los valores de los indicadores
    public void DisplayIndicators(TextMeshProUGUI textMeshPro)
    {
        if (textMeshPro != null)
        {
            textMeshPro.text = $"Satisfacción: {Satisfaction}\n" +
                               $"Peligrosidad: {Danger}\n" +
                               $"Opinión: {Opinion}";
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI is not assigned.");
        }
    }
}

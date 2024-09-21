using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class DayCycleManager : MonoBehaviour
{
    public CustomerSelector customerSelector;
    public float customerShoppingDuration = 180f; // Duración de la fase de clientes en segundos, ajustable desde el Inspector
    public float transitionTime = 2f;
    public GameObject transitionCanvas;
    public GameObject transitionTextDay;

    private float dayTimer;
    private DayNightController dayNightController;
    private int dayNumber = 0;
    private List<GameObject> selectedCustomersToday = new List<GameObject>();

    private bool isDayNightCycleActive = true;

    [SerializeField] private Newspaper newspaper;  // Referencia al script del periódico
    [SerializeField] private IndicatorManager indicatorManager;  // Referencia al gestor de indicadores
    [SerializeField] private TextMeshProUGUI dayTextPauseMenu;

    private enum DayPhase
    {
        MerchantArrival,
        CustomerShopping,
        DayTransition
    }

    private DayPhase currentPhase;
    private MerchantController merchantController;

    private void Start()
    {
        dayTextPauseMenu.text = "Día: " + dayNumber;

        dayTimer = 0;
        transitionTextDay.SetActive(false);

        merchantController = FindObjectOfType<MerchantController>();
        merchantController.OnMerchantLeft += StartCustomerShoppingPhase;

        dayNightController = GetComponent<DayNightController>();
        dayNightController.SecondsInFullDay = (int)customerShoppingDuration;

        currentPhase = DayPhase.MerchantArrival;
        StartCoroutine(DayCycleRoutine());
    }

    private IEnumerator DayCycleRoutine()
    {
        while (true)
        {
            switch (currentPhase)
            {
                case DayPhase.MerchantArrival:
                    yield return StartCoroutine(MerchantArrivalPhase());
                    break;
                case DayPhase.CustomerShopping:
                    yield return StartCoroutine(CustomerShoppingPhase());
                    break;
                case DayPhase.DayTransition:
                    yield return StartCoroutine(DayTransitionPhase());
                    break;
            }
        }
    }

    private IEnumerator MerchantArrivalPhase()
    {
        dayNightController.stop = !isDayNightCycleActive;
        merchantController.setStateGoToShop();
        customerSelector.selectedCustomer = null;

        while (currentPhase == DayPhase.MerchantArrival)
        {
            yield return null;
        }
    }

    private void StartCustomerShoppingPhase()
    {
        currentPhase = DayPhase.CustomerShopping;

        if (isDayNightCycleActive && !dayNightController.dayStarted)
        {
            dayNightController.StartDayCycle();
        }

        StartCoroutine(CustomerShoppingPhase());
    }

    private IEnumerator CustomerShoppingPhase()
    {
        dayTimer = customerShoppingDuration;

        while (dayTimer > 0)
        {
            if (!customerSelector.selectedCustomer)
            {
                SelectAndSendCustomer();
            }

            dayTimer -= Time.deltaTime;

            CheckIfCustomerLeaving();

            yield return null;
        }

        yield return StartCoroutine(WaitForCustomersToFinish());

        currentPhase = DayPhase.DayTransition;
    }

    private void CheckIfCustomerLeaving()
    {
        if (customerSelector.selectedCustomer != null)
        {
            CustomerController customerController = customerSelector.selectedCustomer.GetComponent<CustomerController>();
            if (customerController.currentState == CustomerController.State.ReturnToStart)
            {
                customerSelector.selectedCustomer = null;
            }
        }
    }

    private IEnumerator WaitForCustomersToFinish()
    {
        while (true)
        {
            bool allCustomersIdle = true;

            foreach (var customer in FindObjectsOfType<CustomerController>())
            {
                if (customer.currentState != CustomerController.State.Idle)
                {
                    allCustomersIdle = false;
                    break;
                }
            }

            if (allCustomersIdle)
            {
                yield break;
            }

            yield return null;
        }
    }

    private IEnumerator DayTransitionPhase()
    {
        dayNumber++;
        dayTextPauseMenu.text = "Día: " + dayNumber.ToString();
        transitionTextDay.SetActive(true);
        transitionTextDay.GetComponent<TextMeshProUGUI>().text = "Día " + dayNumber.ToString();
        GameObject fade = Initiate.Fade(Color.black, 1.0f);
        fade.transform.parent = transitionCanvas.transform;
        fade.transform.SetSiblingIndex(0);

        // Detener el ciclo de día y noche al final del día
        dayNightController.stop = !isDayNightCycleActive;

        yield return new WaitForSeconds(transitionTime);

        // Reiniciar el ciclo de día y noche
        dayNightController.ResetDayCycle();

        // Actualizar el periódico y restablecerlo para el nuevo día
        ResetAndUpdateNewspaper();

        Initiate.DoneFading();
        fade.GetComponent<Fader>().OnLevelFinishedLoading();
        transitionTextDay.SetActive(false);

        dayTimer = 0;
        currentPhase = DayPhase.MerchantArrival;

        // Reiniciar la lista de clientes seleccionados para el nuevo día
        selectedCustomersToday.Clear();
    }

    private void SelectAndSendCustomer()
    {
        customerSelector.SelectRandomCustomer();

        while (selectedCustomersToday.Contains(customerSelector.selectedCustomer) && customerSelector.customers.Count > selectedCustomersToday.Count)
        {
            customerSelector.SelectRandomCustomer();
        }

        GameObject selectedCustomer = customerSelector.selectedCustomer;
        if (selectedCustomer != null)
        {
            CustomerController customerController = selectedCustomer.GetComponent<CustomerController>();
            if (customerController != null)
            {
                customerController.shouldGoToShop = true;
                customerController.shouldReturnToStart = false;

                selectedCustomersToday.Add(selectedCustomer);

                // Pausar el ciclo día-noche mientras el cliente se dirige a la tienda
                ToggleDayNightCycle(false);

                // Suscribirse al evento cuando el cliente llegue a la tienda
                customerController.OnCustomerReachedShop += OnCustomerReachedShop;
            }
        }
    }

    private void OnCustomerReachedShop(CustomerController customerController)
    {
        // Reanudar el ciclo día-noche cuando el cliente llegue a la tienda
        ToggleDayNightCycle(true);

        // Desuscribirse del evento para evitar múltiples llamadas
        customerController.OnCustomerReachedShop -= OnCustomerReachedShop;
    }

    public void ToggleDayNightCycle(bool isActive)
    {
        isDayNightCycleActive = isActive;
        dayNightController.stop = !isActive; // Actualizar el estado del ciclo día-noche
    }

    // Método para reiniciar y actualizar el periódico con indicadores implícitos
    private void ResetAndUpdateNewspaper()
    {
        newspaper.ResetToOriginalTransform(); // Restablecer el transform del periódico a su posición original
        string implicitIndicators = GenerateImplicitIndicatorsText();
        newspaper.UpdateEventInfo(implicitIndicators);
    }

    // Método para generar texto implícito basado en los indicadores sin números literales, con variación diaria
    private string GenerateImplicitIndicatorsText()
    {
        // Introducciones posibles
        string[] introductions = {
        "Las noticias del día revelan el estado del pueblo.",
        "Los rumores del día comienzan a circular entre los ciudadanos.",
        "Los eventos recientes han dejado una huella en la población."
    };

        // Conclusiones posibles
        string[] conclusions = {
        "El día termina con incertidumbre en el aire.",
        "El futuro parece incierto, todos están atentos a lo que vendrá.",
        "Parece que la situación no cambiará pronto, pero todo puede suceder."
    };

        // Descripciones posibles para Satisfacción
        string[] highSatisfactionDescriptions = {
        "La felicidad del pueblo se siente en cada rincón.",
        "El pueblo está contento y el ambiente es alegre.",
        "La satisfacción es palpable, los ciudadanos están muy complacidos."
    };

        string[] neutralSatisfactionDescriptions = {
        "El pueblo sigue su ritmo habitual, sin grandes cambios.",
        "La estabilidad reina en el ánimo de los ciudadanos.",
        "No parece haber cambios significativos en la satisfacción general."
    };

        string[] lowSatisfactionDescriptions = {
        "El malestar comienza a extenderse entre los habitantes.",
        "Hay descontento en el aire, algo no va bien.",
        "La insatisfacción está creciendo, algunos murmuran que no están felices."
    };

        // Descripciones posibles para Opinión Pública
        string[] highOpinionDescriptions = {
        "La gente confía en tu trabajo, tu reputación está en su punto más alto.",
        "Los habitantes tienen una opinión muy favorable sobre ti.",
        "Tu prestigio entre la gente es excelente, te consideran un héroe local."
    };

        string[] neutralOpinionDescriptions = {
        "Los ciudadanos mantienen una opinión neutral sobre ti.",
        "Tu reputación se mantiene estable, ni muy alta ni baja.",
        "El pueblo parece indeciso sobre su opinión, algunos te apoyan, otros dudan."
    };

        string[] lowOpinionDescriptions = {
        "La opinión pública no es favorable, muchos comienzan a cuestionar tu labor.",
        "Tu reputación ha caído, los habitantes empiezan a perder la confianza en ti.",
        "La gente empieza a hablar mal de ti, la confianza en tu trabajo está en declive."
    };

        // Descripciones posibles para el Nivel de Peligro
        string[] highDangerDescriptions = {
        "El peligro en la región está aumentando rápidamente. Los ciudadanos están asustados.",
        "Los rumores sobre extraños peligros circulan por el pueblo. La tensión es palpable.",
        "El ambiente es cada vez más peligroso, algunos ya no se atreven a salir de casa."
    };

        string[] neutralDangerDescriptions = {
        "El nivel de peligro no parece haber cambiado, la vida sigue con normalidad.",
        "Todo parece en calma por ahora, aunque algunos mantienen la guardia alta.",
        "El pueblo sigue tranquilo, sin señales de peligro inminente."
    };

        string[] lowDangerDescriptions = {
        "Los peligros han disminuido considerablemente. La seguridad del pueblo mejora.",
        "El peligro parece haber desaparecido, los ciudadanos se sienten más seguros.",
        "La calma ha regresado, no hay amenazas visibles en el horizonte."
    };

        // Selección de las descripciones dependiendo del valor de los indicadores
        string satisfactionText = (indicatorManager.Satisfaction > 75)
            ? highSatisfactionDescriptions[Random.Range(0, highSatisfactionDescriptions.Length)]
            : (indicatorManager.Satisfaction < 25)
            ? lowSatisfactionDescriptions[Random.Range(0, lowSatisfactionDescriptions.Length)]
            : neutralSatisfactionDescriptions[Random.Range(0, neutralSatisfactionDescriptions.Length)];

        string opinionText = (indicatorManager.Opinion > 75)
            ? highOpinionDescriptions[Random.Range(0, highOpinionDescriptions.Length)]
            : (indicatorManager.Opinion < 25)
            ? lowOpinionDescriptions[Random.Range(0, lowOpinionDescriptions.Length)]
            : neutralOpinionDescriptions[Random.Range(0, neutralOpinionDescriptions.Length)];

        string dangerText = (indicatorManager.Danger > 75)
            ? highDangerDescriptions[Random.Range(0, highDangerDescriptions.Length)]
            : (indicatorManager.Danger < 25)
            ? lowDangerDescriptions[Random.Range(0, lowDangerDescriptions.Length)]
            : neutralDangerDescriptions[Random.Range(0, neutralDangerDescriptions.Length)];

        // Seleccionar una introducción y conclusión aleatoriamente
        string introduction = introductions[Random.Range(0, introductions.Length)];
        string conclusion = conclusions[Random.Range(0, conclusions.Length)];

        // Retornar el texto generado con introducción, indicadores y conclusión
        return $"{introduction}\n{satisfactionText}\n{opinionText}\n{dangerText}\n{conclusion}";
    }

}

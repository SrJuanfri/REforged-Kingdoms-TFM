using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class MerchantController : Interactable
{
    public Transform shopDestination; // Destino del mercader
    private Transform player; // Referencia al jugador
    public Transform exitPoint; // Punto de salida del mercader
    [SerializeField] private GameObject materialsMachine;
    [SerializeField] private GameObject bellObject; // Objeto Bell para controlar su BoxCollider

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    [HideInInspector]
    public State currentState;

    public delegate void MerchantEventHandler();
    public event MerchantEventHandler OnMerchantLeft;

    public string sellText;
    public string endText;

    private Outline outline;  // Referencia al componente Outline
    private bool outlineEnabled = false;

    // Referencia al DayCycleManager para obtener el número de día
    [SerializeField] private DayCycleManager dayCycleManager;

    // Lista pública de frases para el primer día
    public List<string> firstDayPhrases = new List<string>();
    public float timeBetweenPhrases = 1f; // Tiempo entre cada frase (modificable)

    private ChatBubble currentChatBubble;

    [Header("Character Voice")]
    private CharacterVoice characterVoice;  // Referencia al script CharacterVoice para reproducir el sonido

    // Referencia al CapsuleCollider del mercader
    private CapsuleCollider capsuleCollider;

    // Nuevo bool que controla si se salta la explicación del primer día
    [SerializeField] private bool skipFirstDayExplanation = false;


    public enum State
    {
        GoToShop,
        ShowProducts,
        Leave
    }

    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        materialsMachine.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").transform; // Buscar el jugador por el tag

        // No activamos el Outline aquí, lo activamos cuando el mercader llegue
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.enabled = false;  // Desactivado por defecto
        }

        // Obtener la referencia al script CharacterVoice
        characterVoice = GetComponent<CharacterVoice>();

        // Obtener el CapsuleCollider del mercader
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Desactivar el BoxCollider del objeto "Bell" si es el primer día
        if (IsFirstDay() && bellObject != null && !skipFirstDayExplanation)
        {
            bellObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
    void Update()
    {
        switch (currentState)
        {
            case State.GoToShop:
                UpdateGoToShop();
                break;
            case State.ShowProducts:
                UpdateShowProducts();
                break;
            case State.Leave:
                UpdateLeave();
                break;
        }
    }

    public void setStateGoToShop()
    {
        SetState(State.GoToShop);
    }

    private void SetState(State newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case State.GoToShop:
                EnterGoToShop();
                break;
            case State.ShowProducts:
                EnterShowProducts();
                break;
            case State.Leave:
                EnterLeave();
                break;
        }
    }

    private void EnterGoToShop()
    {
        navMeshAgent.destination = shopDestination.position;
        animator.SetBool("Walk", true);
    }

    private void UpdateGoToShop()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            SetState(State.ShowProducts);
        }
    }

    private void EnterShowProducts()
    {
        animator.SetBool("Walk", false);
        materialsMachine.SetActive(true);
        StartCoroutine(ShowProductsRoutine());

        // Activar el Outline ahora que el mercader ha llegado
        ActivateOutline();
    }

    private void UpdateShowProducts()
    {
        if (player)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private IEnumerator ShowProductsRoutine()
    {
        while (currentState == State.ShowProducts)
        {
            yield return null;
        }
    }

    public void Leave()
    {
        if (currentState == State.ShowProducts)
        {
            SetState(State.Leave);
        }
    }

    private void EnterLeave()
    {
        navMeshAgent.destination = exitPoint.position;
        animator.SetBool("Walk", true);
        materialsMachine.SetActive(false);

        // Desactivar el Outline cuando el mercader se va
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    private void UpdateLeave()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.5f)
        {
            if (animator.GetBool("Walk"))
            {
                animator.SetBool("Walk", false);
                OnMerchantLeft?.Invoke();
            }
        }
    }

    public void InteractNPC()
    {
        // Activar la animación de hablar
        animator.SetTrigger("Talk");

        // Verificar si se debe saltar la explicación del primer día
        if (!skipFirstDayExplanation && IsFirstDay() && firstDayPhrases.Count > 0)
        {
            StartCoroutine(DisplayFirstDayPhrases());
        }
        else
        {
            // Interactúa con el NPC (mercader) y muestra el texto de venta normal
            PlayVoice();  // Reproduce la voz al inicio de cada frase
            ChatBubble.Create(transform, new Vector3(-0.6f, 1.7f, 0f), sellText);
        }

        // Si el outline está habilitado, desactívalo
        if (outlineEnabled && outline != null)
        {
            outline.enabled = false;  // Desactivar el outline cuando el jugador interactúe
            outlineEnabled = false;
        }
    }


    // Método para activar el Outline cuando el mercader llega a la tienda
    private void ActivateOutline()
    {
        if (outline != null)
        {
            outline.OutlineColor = new Color(0.4f, 0.7f, 1f);  // Color azul más claro
            outline.OutlineWidth = 5f;  // Ajustar el ancho del Outline
            outline.enabled = true;  // Activar el Outline
            outlineEnabled = true;
        }
    }

    // Verificar si es el primer día basado en el DayCycleManager
    private bool IsFirstDay()
    {
        // Comprobar si el número del día en el DayCycleManager es igual al día inicial
        return dayCycleManager != null && dayCycleManager.IsFirstDay();
    }

    // Corrutina para mostrar las frases del primer día con un retardo entre ellas y evitar superposición
    // Corrutina para mostrar las frases del primer día con un retardo entre ellas y evitar superposición
    private IEnumerator DisplayFirstDayPhrases()
    {
        // Desactivar el CapsuleCollider mientras se muestran las frases
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = false;
        }

        for (int i = 0; i < firstDayPhrases.Count; i++)
        {
            string phrase = firstDayPhrases[i];

            // Esperar hasta que la burbuja actual desaparezca antes de crear una nueva
            while (currentChatBubble != null)
            {
                yield return null;
            }

            // Si es la última frase, destruir en 6 segundos (por defecto), de lo contrario usar el tiempo entre frases
            float destroyTime = (i == firstDayPhrases.Count - 1) ? 6f : timeBetweenPhrases;

            // Reproduce la voz al inicio de cada frase
            PlayVoice();

            // Activar la animación de hablar
            animator.SetTrigger("Talk");

            // Mostrar cada frase en un chat bubble y guardar la referencia
            currentChatBubble = ChatBubble.Create(transform, new Vector3(-0.6f, 1.7f, 0f), phrase, destroyTime);

            // Esperar el tiempo configurado entre cada frase
            yield return new WaitForSeconds(timeBetweenPhrases);

            // Solo eliminar la referencia a la burbuja si no es la última frase
            if (i != firstDayPhrases.Count - 1)
            {
                currentChatBubble = null;
            }
        }

        // Para la última burbuja, espera a que se destruya completamente (después de 6 segundos)
        yield return new WaitForSeconds(6f);
        currentChatBubble = null;

        // Reactivar el CapsuleCollider después de que terminen las frases
        if (capsuleCollider != null)
        {
            capsuleCollider.enabled = true;
        }

        // Desactivar la animación de hablar después de que se hayan mostrado todas las frases
        animator.ResetTrigger("Talk");

        // Activar el BoxCollider del objeto "Bell" después de mostrar todas las frases
        if (bellObject != null)
        {
            bellObject.GetComponent<BoxCollider>().enabled = true;
        }
    }

    // Método para reproducir la voz antes de cada frase
    private void PlayVoice()
    {
        if (characterVoice != null)
        {
            characterVoice.PlayVoice();  // Usar el método de CharacterVoice para reproducir el sonido correspondiente
        }
    }
}

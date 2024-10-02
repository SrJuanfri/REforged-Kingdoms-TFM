using System;
using UnityEngine;

public class DayProgressManager : MonoBehaviour
{
    private int clientsAttended;
    private int correctOrdersDelivered;
    private int incorrectOrdersDelivered;

    public event Action<int, int, int> OnProgressUpdated;

    private void Awake()
    {
        // Inicializa el progreso del día
        ResetProgress();
    }

    public void ResetProgress()
    {
        clientsAttended = 0;
        correctOrdersDelivered = 0;
        incorrectOrdersDelivered = 0;
    }

    public void RecordClientAttendance()
    {
        clientsAttended++;
        UpdateProgress();
    }

    public void RecordCorrectOrder()
    {
        correctOrdersDelivered++;
        UpdateProgress();
    }

    public void RecordIncorrectOrder()
    {
        incorrectOrdersDelivered++;
        UpdateProgress();
    }

    private void UpdateProgress()
    {
        // Notifica a los suscriptores sobre el progreso actualizado
        OnProgressUpdated?.Invoke(clientsAttended, correctOrdersDelivered, incorrectOrdersDelivered);
    }

    public (int, int, int) GetCurrentProgress()
    {
        return (clientsAttended, correctOrdersDelivered, incorrectOrdersDelivered);
    }
}
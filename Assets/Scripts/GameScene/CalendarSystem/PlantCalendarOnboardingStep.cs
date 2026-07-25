using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// shown right after a plant's location is confirmed during onboarding. Lets the player log their
// care routine straight away, or skip it and log it later from the plant card instead.
public class PlantCalendarOnboardingStep : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TMP_Text promptText;

    [Header("References")]
    [SerializeField] private PlantDatabase plantDatabase;
    [SerializeField] private List<PlantCalendarController> allCalendars; // one entry per plant type - only the matching one is opened.

    private string currentPlantInstanceId;
    private string currentPlantId;
    private Action<string, string> onStepFinished;

    public void BeginStep(string uniquePlantInstanceId, string plantId, Action<string, string> onFinished)
    {
        currentPlantInstanceId = uniquePlantInstanceId;
        currentPlantId = plantId;
        onStepFinished = onFinished;

        if (promptText != null)
        {
            PlantData plant = plantDatabase != null ? plantDatabase.GetById(plantId) : null;
            string plantName = plant != null ? plant.displayName : plantId;

            // TODO: route through LocalizedText once translations are set up for the calendar system.
            promptText.text = "Want to log " + plantName + "'s care routine now?";
        }

        if (promptRoot != null)
            promptRoot.SetActive(true);
    }

    public void OnLogNowPressed()
    {
        if (promptRoot != null)
            promptRoot.SetActive(false);

        PlantCalendarController calendar = FindCalendar(currentPlantId);

        if (calendar != null)
            calendar.OpenForOnboarding(currentPlantId, FinishStep);
        else
            FinishStep(); // no matching calendar screen wired up - don't block onboarding on it.
    }

    public void OnSkipPressed()
    {
        if (promptRoot != null)
            promptRoot.SetActive(false);

        FinishStep(); // skipping is allowed - the player can still log their routine later from the plant card.
    }

    private PlantCalendarController FindCalendar(string plantId)
    {
        if (allCalendars == null)
            return null;

        foreach (PlantCalendarController calendar in allCalendars)
        {
            if (calendar != null && calendar.GetPlantId() == plantId)
                return calendar;
        }

        return null;
    }

    private void FinishStep()
    {
        onStepFinished?.Invoke(currentPlantInstanceId, currentPlantId);
    }
}

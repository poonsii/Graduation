using System.Collections.Generic;
using UnityEngine;

public class PlantOnboardingFlowController : MonoBehaviour
{
    [SerializeField] private PlantLocationSelector locationSelector;
    [SerializeField] private PlantCalendarOnboardingStep calendarOnboardingStep; // optional routine-logging step shown right after the location is picked.
    [SerializeField] private GameBootstrap gameBootstrap;

    private Queue<SavedPlantState> pendingPlants;
    private string currentPlantId;

    public void BeginOnboarding(List<SavedPlantState> plantsNeedingOnboarding)
    {
        Debug.Log("[Onboarding] BeginOnboarding called with " + plantsNeedingOnboarding.Count + " plant(s).");
        pendingPlants = new Queue<SavedPlantState>(plantsNeedingOnboarding);
        AdvanceToNextPlant();
    }

    private void AdvanceToNextPlant()
    {
        if (pendingPlants == null || pendingPlants.Count == 0)
        {
            Debug.Log("[Onboarding] Queue empty - onboarding flow finished (or never started).");
            return; // every plant has been placed.
        }

        SavedPlantState next = pendingPlants.Dequeue();
        Debug.Log("[Onboarding] Advancing to plant '" + next.plantId + "' (" + next.uniquePlantInstanceId + "). Location selector assigned: " + (locationSelector != null));

        currentPlantId = next.plantId;

        if (gameBootstrap != null)
            gameBootstrap.SetPlantCardInteractable(currentPlantId, false); // lock the card while this plant is being onboarded.

        if (next.selectedLightLocation != LightLocationType.Unknown)
        {
            // this plant already has a location from a previous session - don't make the player redo it.
            OnPlantLocationConfirmed(next.uniquePlantInstanceId, next.plantId);
            return;
        }

        if (locationSelector != null)
            locationSelector.BeginSelection(next.uniquePlantInstanceId, next.plantId, OnPlantLocationConfirmed);
        else
            AdvanceToNextPlant(); // no location selector wired up, skip straight to the next plant.
    }

    private void OnPlantLocationConfirmed(string uniquePlantInstanceId, string plantId)
    {
        if (gameBootstrap != null)
            gameBootstrap.SetPlantCardInteractable(plantId, true); // unlock now that the location step is done.

        if (calendarOnboardingStep != null)
            calendarOnboardingStep.BeginStep(uniquePlantInstanceId, plantId, OnCalendarStepFinished);
        else
            AdvanceToNextPlant(); // no calendar step wired up, skip straight to the next plant.
    }

    private void OnCalendarStepFinished(string uniquePlantInstanceId, string plantId)
    {
        if (gameBootstrap != null)
            gameBootstrap.MarkCalendarIntroSeen(uniquePlantInstanceId); // this plant's onboarding is fully done now, whether it was logged or skipped.

        AdvanceToNextPlant();
    }
}

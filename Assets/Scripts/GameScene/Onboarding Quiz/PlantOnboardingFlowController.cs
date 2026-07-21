using System.Collections.Generic;
using UnityEngine;

public class PlantOnboardingFlowController : MonoBehaviour
{
    [SerializeField] private PlantLocationSelector locationSelector;
    [SerializeField] private PlantCareQuizPanel careQuizPanel;

    private Queue<SavedPlantState> pendingPlants;

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
            return; // every plant has been placed and quizzed.
        }

        SavedPlantState next = pendingPlants.Dequeue();
        Debug.Log("[Onboarding] Advancing to plant '" + next.plantId + "' (" + next.uniquePlantInstanceId + "). Location selector assigned: " + (locationSelector != null));

        if (locationSelector != null)
            locationSelector.BeginSelection(next.uniquePlantInstanceId, next.plantId, OnPlantLocationConfirmed);
        else
            AdvanceToNextPlant(); // no location selector wired up, skip straight to the next plant.
    }

    private void OnPlantLocationConfirmed(string uniquePlantInstanceId, string plantId)
    {
        if (careQuizPanel != null)
            careQuizPanel.Show(uniquePlantInstanceId, plantId, OnPlantCareQuizComplete);
        else
            AdvanceToNextPlant();
    }

    private void OnPlantCareQuizComplete()
    {
        AdvanceToNextPlant();
    }
}

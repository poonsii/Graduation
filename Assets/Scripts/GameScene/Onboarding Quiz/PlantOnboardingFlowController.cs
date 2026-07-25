using System.Collections.Generic;
using UnityEngine;

public class PlantOnboardingFlowController : MonoBehaviour
{
    [SerializeField] private PlantLocationSelector locationSelector;
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
            gameBootstrap.SetPlantCardInteractable(plantId, true); // unlock now that this plant's onboarding is done.

        AdvanceToNextPlant();
    }
}

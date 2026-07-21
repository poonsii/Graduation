using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private PlayerProfileManager profileManager; // save/load system.
    [SerializeField] private PlantDatabase plantDatabase;
    [SerializeField] private InventoryManager inventoryManager; // owned plants list.
    [SerializeField] private PlantCardSpawner cardSpawner; // creates plant cards.
    [SerializeField] private PlantRegistrationUI registrationUI; // registration popup.
    [SerializeField] private PlantWorldDisplay plantWorldDisplay;
    [SerializeField] private PlantOnboardingFlowController onboardingFlowController; // walks the player through each plant one by one.

    private bool registrationRequired; // checks if registration is needed.
    private PlayerData currentData; // keeps the current save data in memory.

    private void Start()
    {
        currentData = profileManager.Load(); // load saved data.

        if (currentData == null)
            currentData = new PlayerData(); // make new save data if nothing exists yet.

        if (!currentData.hasFinishedPlantRegistration || currentData.ownedPlantIds == null || currentData.ownedPlantIds.Count == 0)
        {
            ForcePlantRegistration(); // show popup if no plants are saved.
        }
        else
        {
            LoadOwnedPlants(currentData); // load saved plants.
            BeginOnboardingIfNeeded();
        }
    }

    private void OnPlantsChosen(List<string> selectedPlantIds)
    {
        if (selectedPlantIds == null || selectedPlantIds.Count == 0)
        {
            Debug.LogWarning("You must select at least one plant.");
            ForcePlantRegistration();
            return;
        }

        currentData = new PlayerData(); // fresh save data.
        currentData.hasFinishedPlantRegistration = true; // mark registration done.
        currentData.ownedPlantIds = new List<string>(selectedPlantIds); // save chosen plant ids.

        currentData.savedPlants = new List<SavedPlantState>(); // create the saved plant states list.

        foreach (string plantId in selectedPlantIds)
        {
            SavedPlantState plantState = new SavedPlantState();
            plantState.uniquePlantInstanceId = System.Guid.NewGuid().ToString(); // create a unique id for this exact plant.
            plantState.plantId = plantId; // save which plant type it is.
            plantState.lastCareTime = "";
            plantState.isUnhealthy = false;
            plantState.hasCompletedOnboarding = false; // onboarding is not done yet.

            currentData.savedPlants.Add(plantState); // add this plant state to the save data.
        }

        profileManager.Save(currentData); // write save file.
        registrationRequired = false; // registration not needed anymore.
        LoadOwnedPlants(currentData); // load the plants.
        BeginOnboardingIfNeeded();
    }

    private void BeginOnboardingIfNeeded()
    {
        if (currentData == null || currentData.savedPlants == null || onboardingFlowController == null)
        {
            Debug.LogWarning("[Onboarding] Stopped early - currentData null: " + (currentData == null)
                + ", savedPlants null: " + (currentData?.savedPlants == null)
                + ", onboardingFlowController assigned: " + (onboardingFlowController != null));
            return;
        }

        List<SavedPlantState> pending = currentData.savedPlants.FindAll(p => !p.hasCompletedOnboarding); // plants still missing location or care info.

        Debug.Log("[Onboarding] Plants still needing onboarding: " + pending.Count);

        if (pending.Count > 0)
            onboardingFlowController.BeginOnboarding(pending); // walk the player through them one by one.
    }

    public void UpdatePlantLocation(
    string uniquePlantInstanceId,
    LightLocationType selectedLightLocation,
    bool playerAcceptedMismatch)
    {
        SavedPlantState plantState = FindPlantState(uniquePlantInstanceId);
        if (plantState == null)
            return;

        PlantData plant = plantDatabase.GetById(plantState.plantId); // get the plant data for this saved plant.

        if (plant == null)
        {
            Debug.LogWarning("Could not find plant data for id: " + plantState.plantId);
            return;
        }

        plantState.selectedLightLocation = selectedLightLocation; // save chosen light location.
        plantState.lightAdviceResult = PlantLocationAdvisor.GetAdvice(plant.requiredLight, selectedLightLocation); // calculate location advice from plant need.
        plantState.playerAcceptedMismatch = playerAcceptedMismatch; // save if the player ignored the advice.

        RecalculateOnboardingComplete(plantState);
        profileManager.Save(currentData); // save updated plant state.
        inventoryManager.RefreshPlantCards(); // update the plant card with the new location info.
    }

    public void UpdatePlantCare(
    string uniquePlantInstanceId,
    PotSoilType potSoilType,
    HumidityLevel humidityLevel,
    bool playerAcceptedSoilMismatch,
    bool playerAcceptedHumidityMismatch)
    {
        SavedPlantState plantState = FindPlantState(uniquePlantInstanceId);
        if (plantState == null)
            return;

        PlantData plant = plantDatabase.GetById(plantState.plantId); // get the plant data for this saved plant.

        if (plant == null)
        {
            Debug.LogWarning("Could not find plant data for id: " + plantState.plantId);
            return;
        }

        plantState.potSoilType = potSoilType; // save chosen pot soil.
        plantState.potSoilAdviceResult = PlantSoilAdvisor.GetAdvice(plant.recommendedPotSoilType, potSoilType);
        plantState.playerAcceptedSoilMismatch = playerAcceptedSoilMismatch;

        plantState.humidityLevel = humidityLevel; // save chosen humidity.
        plantState.humidityAdviceResult = PlantHumidityAdvisor.GetAdvice(plant.recommendedHumidityLevel, humidityLevel);
        plantState.playerAcceptedHumidityMismatch = playerAcceptedHumidityMismatch;

        RecalculateOnboardingComplete(plantState);
        profileManager.Save(currentData); // save updated plant state.
        inventoryManager.RefreshPlantCards(); // update the plant card with the new soil/humidity info.
    }

    private void RecalculateOnboardingComplete(SavedPlantState plantState)
    {
        plantState.hasCompletedOnboarding =
            plantState.selectedLightLocation != LightLocationType.Unknown &&
            plantState.potSoilType != PotSoilType.Unknown &&
            plantState.humidityLevel != HumidityLevel.Unknown;
    }

    private SavedPlantState FindPlantState(string uniquePlantInstanceId)
    {
        if (currentData == null || currentData.savedPlants == null)
        {
            Debug.LogWarning("Could not find saved plant state for id: " + uniquePlantInstanceId);
            return null; // stop if there is no loaded save data.
        }

        SavedPlantState plantState = currentData.savedPlants.Find(p => p.uniquePlantInstanceId == uniquePlantInstanceId);

        if (plantState == null)
            Debug.LogWarning("Could not find saved plant state for id: " + uniquePlantInstanceId);

        return plantState;
    }

    public SavedPlantState GetSavedPlantState(string uniquePlantInstanceId)
    {
        if (currentData == null || currentData.savedPlants == null)
            return null; // stop if there is no loaded save data.

        return currentData.savedPlants.Find(p => p.uniquePlantInstanceId == uniquePlantInstanceId); // return the exact saved plant.
    }

    public SavedPlantState GetSavedPlantStateForPlant(string plantId)
    {
        if (currentData == null || currentData.savedPlants == null)
            return null; // stop if there is no loaded save data.

        return currentData.savedPlants.Find(p => p.plantId == plantId); // return the saved state for this plant type (used by the plant cards).
    }

    private void LoadOwnedPlants(PlayerData data)
    {
        inventoryManager.Clear(); // clear old owned plants.

        foreach (string plantId in data.ownedPlantIds)
        {
            PlantData plant = plantDatabase.GetById(plantId); // find plant by id.
            if (plant != null)
            {
                inventoryManager.AddPlant(plant); // add to owned list.
            }
        }

        inventoryManager.RefreshRoomPlants(); // update room plants
        inventoryManager.RefreshPlantCards(); // update cards
        cardSpawner.SpawnCards(data.ownedPlantIds); // spawn owned cards

        if (plantWorldDisplay != null)
            plantWorldDisplay.ShowOwnedPlants(data.ownedPlantIds); // show owned plants in world
    }

    public void ResetPlantsAndRequireRegistration()
    {
        profileManager.DeleteSave(); // delete save file.
        inventoryManager.Clear(); // clear owned plants.
        registrationRequired = true; // require registration again.
        currentData = new PlayerData(); // reset current data too.

        ForcePlantRegistration(); // open popup again.
    }

    private void ForcePlantRegistration()
    {
        registrationRequired = true; // mark registration needed.
        registrationUI.Show(OnPlantsChosen); // show popup and wait for choice
    }
}
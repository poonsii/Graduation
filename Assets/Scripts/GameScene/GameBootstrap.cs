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
    }

    public void UpdatePlantOnboardingState(
    string uniquePlantInstanceId,
    LightLocationType selectedLightLocation,
    bool playerAcceptedMismatch,
    PotSoilType potSoilType,
    HumidityLevel humidityLevel)
    {
        if (currentData == null || currentData.savedPlants == null)
            return; // stop if there is no loaded save data.

        SavedPlantState plantState = currentData.savedPlants.Find(p => p.uniquePlantInstanceId == uniquePlantInstanceId); // find the exact saved plant.

        if (plantState == null)
        {
            Debug.LogWarning("Could not find saved plant state for id: " + uniquePlantInstanceId);
            return;
        }

        PlantData plant = plantDatabase.GetById(plantState.plantId); // get the plant data for this saved plant.

        if (plant == null)
        {
            Debug.LogWarning("Could not find plant data for id: " + plantState.plantId);
            return;
        }

        plantState.selectedLightLocation = selectedLightLocation; // save chosen light location.
        plantState.lightAdviceResult = PlantLocationAdvisor.GetAdvice(plant.requiredLight, selectedLightLocation); // calculate location advice from plant need.
        plantState.playerAcceptedMismatch = playerAcceptedMismatch; // save if the player ignored the advice.
        plantState.potSoilType = potSoilType; // save chosen pot soil.
        plantState.humidityLevel = humidityLevel; // save chosen humidity.
        plantState.hasCompletedOnboarding =
    selectedLightLocation != LightLocationType.Unknown &&
    potSoilType != PotSoilType.Unknown &&
    humidityLevel != HumidityLevel.Unknown;
        profileManager.Save(currentData); // save updated plant state.
    }

    public SavedPlantState GetSavedPlantState(string uniquePlantInstanceId)
    {
        if (currentData == null || currentData.savedPlants == null)
            return null; // stop if there is no loaded save data.

        return currentData.savedPlants.Find(p => p.uniquePlantInstanceId == uniquePlantInstanceId); // return the exact saved plant.
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
using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private PlayerProfileManager profileManager;
    [SerializeField] private PlantDatabase plantDatabase;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private PlantCardSpawner cardSpawner;
    [SerializeField] private PlantRegistrationUI registrationUI;
    [SerializeField] private PlantWorldDisplay plantWorldDisplay;

    private bool registrationRequired;

    private void Start()
    {
        PlayerData data = profileManager.Load();

        if (!data.hasFinishedPlantRegistration || data.ownedPlantIds == null || data.ownedPlantIds.Count == 0)
        {
            ForcePlantRegistration();
        }
        else
        {
            LoadOwnedPlants(data);
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

        PlayerData data = new PlayerData();
        data.hasFinishedPlantRegistration = true;
        data.ownedPlantIds = selectedPlantIds;

        profileManager.Save(data);
        registrationRequired = false;
        LoadOwnedPlants(data);
    }

    private void LoadOwnedPlants(PlayerData data)
    {
        inventoryManager.Clear();

        foreach (string plantId in data.ownedPlantIds)
        {
            PlantData plant = plantDatabase.GetById(plantId);
            if (plant != null)
            {
                inventoryManager.AddPlant(plant);
            }
        }

        inventoryManager.RefreshRoomPlants();
        inventoryManager.RefreshPlantCards();
        cardSpawner.SpawnCards(data.ownedPlantIds);

        if (plantWorldDisplay != null)
            plantWorldDisplay.ShowOwnedPlants(data.ownedPlantIds);
    }

    public void ResetPlantsAndRequireRegistration()
    {
        profileManager.DeleteSave();
        inventoryManager.Clear();
        registrationRequired = true;

        ForcePlantRegistration();
    }

    private void ForcePlantRegistration()
    {
        registrationRequired = true;
        registrationUI.Show(OnPlantsChosen);
    }
}
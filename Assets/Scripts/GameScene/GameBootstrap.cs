using System.Collections.Generic;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [SerializeField] private PlayerProfileManager profileManager;
    [SerializeField] private PlantDatabase plantDatabase;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private PlantCardSpawner cardSpawner;
    [SerializeField] private PlantRegistrationUI registrationUI;

    private void Start()
    {
        PlayerData data = profileManager.Load();

        if (!data.hasFinishedPlantRegistration)
        {
            registrationUI.Show(OnPlantsChosen);
        }
        else
        {
            LoadOwnedPlants(data);
        }
    }

    private void OnPlantsChosen(List<string> selectedPlantIds)
    {
        PlayerData data = new PlayerData();
        data.hasFinishedPlantRegistration = true;
        data.ownedPlantIds = selectedPlantIds;

        profileManager.Save(data);
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

        cardSpawner.SpawnCards(data.ownedPlantIds);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            profileManager.DeleteSave();
            Debug.Log("Save reset. Press Play again.");
        }
    }
}
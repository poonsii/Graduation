using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Owned Data")]
    public List<PlantData> ownedPlants = new List<PlantData>();

    [Header("Scene Plants")]
    [SerializeField] private List<RoomPlant> roomPlants = new List<RoomPlant>();

    [Header("Plant Cards UI")]
    [SerializeField] private List<OwnedPlantCardUI> plantCards = new List<OwnedPlantCardUI>();

    public void Clear()
    {
        ownedPlants.Clear();
    }

    public void AddPlant(PlantData plant)
    {
        if (plant != null && !ownedPlants.Contains(plant))
            ownedPlants.Add(plant);
    }

    public bool HasPlant(string plantId)
    {
        foreach (PlantData plant in ownedPlants)
        {
            if (plant != null && plant.id == plantId)
                return true;
        }

        return false;
    }

    public void RefreshRoomPlants()
    {
        Debug.Log("Owned plants count: " + ownedPlants.Count);

        foreach (PlantData ownedPlant in ownedPlants)
        {
            if (ownedPlant != null)
                Debug.Log("Owned plant ID: " + ownedPlant.id);
        }

        foreach (RoomPlant roomPlant in roomPlants)
        {
            if (roomPlant == null)
                continue;

            string roomPlantId = roomPlant.GetPlantId();
            bool isOwned = HasPlant(roomPlantId);

            Debug.Log("RoomPlant ID: " + roomPlantId + " | isOwned: " + isOwned, roomPlant);

            roomPlant.SetOwned(isOwned);
        }
    }

    public void RefreshPlantCards()
    {
        Debug.Log("Refreshing plant cards. Total cards: " + plantCards.Count);

        foreach (OwnedPlantCardUI card in plantCards)
        {
            if (card == null)
                continue;

            string cardPlantId = card.GetPlantId();
            bool isOwned = HasPlant(cardPlantId);

            Debug.Log("Card Plant ID: " + cardPlantId + " | isOwned: " + isOwned, card);

            card.SetOwned(isOwned);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("Owned Data")]
    public List<PlantData> ownedPlants = new List<PlantData>(); // plants owned by the player.

    [Header("Scene Plants")]
    [SerializeField] private List<RoomPlant> roomPlants = new List<RoomPlant>(); // plants in the room.

    [Header("Plant Cards UI")]
    [SerializeField] private List<OwnedPlantCardUI> plantCards = new List<OwnedPlantCardUI>(); // card ui objects.

    public void Clear()
    {
        ownedPlants.Clear(); // remove all owned plants.
    }

    public void AddPlant(PlantData plant)
    {
        if (plant != null && !ownedPlants.Contains(plant))
            ownedPlants.Add(plant); // add plant if it is not already owned.
    }

    public bool HasPlant(string plantId)
    {
        foreach (PlantData plant in ownedPlants)
        {
            if (plant != null && plant.id == plantId)
                return true; // plant belongs to the player.
        }

        return false;
    }

    public void RefreshRoomPlants()
    {
        Debug.Log("Owned plants count: " + ownedPlants.Count); // check owned count.

        foreach (PlantData ownedPlant in ownedPlants)
        {
            if (ownedPlant != null)
                Debug.Log("Owned plant ID: " + ownedPlant.id);
        }

        foreach (RoomPlant roomPlant in roomPlants)
        {
            if (roomPlant == null)
                continue; // skip missing objects.

            string roomPlantId = roomPlant.GetPlantId(); // get plant id.
            bool isOwned = HasPlant(roomPlantId); // check ownership.

            Debug.Log("RoomPlant ID: " + roomPlantId + " | isOwned: " + isOwned, roomPlant); // debug info.

            roomPlant.SetOwned(isOwned); // update plant state.
        }
    }

    public void SetRoomPlantVisible(string plantId, bool visible)
    {
        foreach (RoomPlant roomPlant in roomPlants)
        {
            if (roomPlant != null && roomPlant.GetPlantId() == plantId)
            {
                roomPlant.SetOwned(visible);
                return;
            }
        }
    }

    public void MovePlantToSpot(string plantId, Transform spotTransform)
    {
        foreach (RoomPlant roomPlant in roomPlants)
        {
            if (roomPlant != null && roomPlant.GetPlantId() == plantId)
            {
                roomPlant.MoveToSpot(spotTransform);
                return;
            }
        }

        Debug.LogWarning("Could not find RoomPlant to move for plant id: " + plantId);
    }

    public void RefreshPlantCards()
    {
        Debug.Log("Refreshing plant cards. Total cards: " + plantCards.Count); // check card count.

        foreach (OwnedPlantCardUI card in plantCards)
        {
            if (card == null)
                continue; // skip missing cards.

            string cardPlantId = card.GetPlantId(); // get card id.
            bool isOwned = HasPlant(cardPlantId); // check ownership.

            Debug.Log("Card Plant ID: " + cardPlantId + " | isOwned: " + isOwned, card);

            card.SetOwned(isOwned);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<PlantData> ownedPlants = new List<PlantData>();

    public void Clear()
    {
        ownedPlants.Clear();
    }

    public void AddPlant(PlantData plant)
    {
        if (plant != null && !ownedPlants.Contains(plant))
            ownedPlants.Add(plant);
    }
}
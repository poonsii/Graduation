using System.Collections.Generic;
using UnityEngine;

public class PlantDatabase : MonoBehaviour
{
    [SerializeField] private List<PlantData> allPlants; // all plant data

    private Dictionary<string, PlantData> plantLookup; // quick lookup by id

    private void Awake()
    {
        plantLookup = new Dictionary<string, PlantData>(); // make the lookup table.

        foreach (var plant in allPlants)
        {
            if (!plantLookup.ContainsKey(plant.id))
                plantLookup.Add(plant.id, plant); // store each plant by id.
        }
    }

    public PlantData GetById(string id)
    {
        if (plantLookup.TryGetValue(id, out var plant))
            return plant; // return the plant if found.

        Debug.LogWarning("Plant ID not found: " + id); // warning if missing.
        return null;
    }

    public List<PlantData> GetAllPlants()
    {
        return allPlants; // return the full list.
    }
}
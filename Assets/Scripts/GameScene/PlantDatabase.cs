using System.Collections.Generic;
using UnityEngine;

public class PlantDatabase : MonoBehaviour
{
    [SerializeField] private List<PlantData> allPlants;

    private Dictionary<string, PlantData> plantLookup;

    private void Awake()
    {
        plantLookup = new Dictionary<string, PlantData>();

        foreach (var plant in allPlants)
        {
            if (!plantLookup.ContainsKey(plant.id))
                plantLookup.Add(plant.id, plant);
        }
    }

    public PlantData GetById(string id)
    {
        if (plantLookup.TryGetValue(id, out var plant))
            return plant;

        Debug.LogWarning("Plant ID not found: " + id);
        return null;
    }
}
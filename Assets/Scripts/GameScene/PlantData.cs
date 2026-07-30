using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Plants/Plant Data")]
public class PlantData : ScriptableObject
{
    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject cardPrefab;
    public GameObject roomItemPrefab;

    public GameObject healthyModel;
    public GameObject unhealthyModel;

    public LightLocationType requiredLight = LightLocationType.Unknown; // light need for this specific plant.

    [Header("Calendar - Watering")]
    public int wateringMinDays = 7; // ideal watering window, in days since the last watering.
    public int wateringMaxDays = 10;

    [Header("Calendar - Fertilizing")]
    public int fertilizingMinDays = 14; // ideal fertilizing window, only counted during the fertilizing season below.
    public int fertilizingMaxDays = 30;

    [Header("Calendar - Seasons")]
    [Tooltip("Months (1-12) where this plant needs extra humidity.")]
    public List<int> humiditySeasonMonths = new List<int>();

    [Tooltip("Months (1-12) where this plant should be fertilized.")]
    public List<int> fertilizingSeasonMonths = new List<int>();
}
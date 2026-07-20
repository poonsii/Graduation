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
    public PotSoilType recommendedPotSoilType = PotSoilType.Unknown; // recommended pot soil for this specific plant.
    public HumidityLevel recommendedHumidityLevel = HumidityLevel.Unknown; // recommended humidity for this specific plant.
}
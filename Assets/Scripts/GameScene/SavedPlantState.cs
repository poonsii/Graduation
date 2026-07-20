using System;

[Serializable]
public class SavedPlantState
{
    public string uniquePlantInstanceId;
    public string plantId;
    public string lastCareTime;
    public bool isUnhealthy;

    public LightLocationType selectedLightLocation = LightLocationType.Unknown;
    public LightAdviceResult lightAdviceResult = LightAdviceResult.Unknown;
    public bool playerAcceptedMismatch = false;
    public PotSoilType potSoilType = PotSoilType.Unknown;
    public HumidityLevel humidityLevel = HumidityLevel.Unknown;
    public bool hasCompletedOnboarding = false;
}
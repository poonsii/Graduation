using System;

[Serializable]
public class SavedPlantState
{
    public string uniquePlantInstanceId;
    public string plantId;
    public string lastCareTime;
    public bool isUnhealthy;

    public LightLocationType selectedLightLocation = LightLocationType.Unknown;
    public string selectedSpotId = ""; // which exact spot, since multiple spots can share the same LightLocationType.
    public LightAdviceResult lightAdviceResult = LightAdviceResult.Unknown;
    public bool playerAcceptedMismatch = false;

    public bool hasCompletedOnboarding = false;
}
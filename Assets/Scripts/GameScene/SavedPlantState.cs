using System;

[Serializable]
public class SavedPlantState
{
    public string uniquePlantInstanceId;
    public string plantId;
    public string lastCareTime;
    public bool isUnhealthy;
}
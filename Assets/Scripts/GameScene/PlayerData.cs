using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public bool hasFinishedPlantRegistration;
    public List<string> ownedPlantIds = new List<string>();

    public List<SavedPlantState> savedPlants = new List<SavedPlantState>();
}
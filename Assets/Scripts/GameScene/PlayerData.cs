using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public int saveVersion = 1;

    public bool hasFinishedPlantRegistration;
    public List<string> ownedPlantIds = new List<string>();
    public List<SavedPlantState> savedPlants = new List<SavedPlantState>();
}
using System.Collections.Generic;
using UnityEngine;

public class PlantWorldDisplay : MonoBehaviour
{
    [System.Serializable]
    public class PlantWorldBinding
    {
        public string plantId; 
        public GameObject worldObject; 
    }

    [SerializeField] private List<PlantWorldBinding> plantBindings = new List<PlantWorldBinding>(); // all plant and world object pairs.

    public void ShowOwnedPlants(List<string> ownedPlantIds)
    {
        foreach (PlantWorldBinding binding in plantBindings)
        {
            if (binding.worldObject != null)
                binding.worldObject.SetActive(false); // hide all plants first.
        }

        if (ownedPlantIds == null)
            return; // stop if there are no owned plants.

        foreach (string ownedPlantId in ownedPlantIds)
        {
            foreach (PlantWorldBinding binding in plantBindings)
            {
                if (binding.plantId == ownedPlantId && binding.worldObject != null)
                {
                    binding.worldObject.SetActive(true); // show the plant if the player owns it.
                }
            }
        }
    }
}
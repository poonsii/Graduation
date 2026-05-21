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

    [SerializeField] private List<PlantWorldBinding> plantBindings = new List<PlantWorldBinding>();

    public void ShowOwnedPlants(List<string> ownedPlantIds)
    {
        foreach (PlantWorldBinding binding in plantBindings)
        {
            if (binding.worldObject != null)
                binding.worldObject.SetActive(false);
        }

        if (ownedPlantIds == null)
            return;

        foreach (string ownedPlantId in ownedPlantIds)
        {
            foreach (PlantWorldBinding binding in plantBindings)
            {
                if (binding.plantId == ownedPlantId && binding.worldObject != null)
                {
                    binding.worldObject.SetActive(true);
                }
            }
        }
    }
}
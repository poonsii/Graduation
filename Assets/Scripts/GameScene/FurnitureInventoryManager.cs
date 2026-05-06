using System.Collections.Generic;
using UnityEngine;

public class FurnitureInventoryManager : MonoBehaviour
{
    [SerializeField] private int maxItems = 3;
    public List<FurnitureData> ownedFurniture = new List<FurnitureData>();

    public bool AddFurniture(FurnitureData furniture)
    {
        if (ownedFurniture.Count >= maxItems) return false;
        if (ownedFurniture.Contains(furniture)) return false;

        ownedFurniture.Add(furniture);
        return true;
    }

    public void RemoveFurniture(FurnitureData furniture)
    {
        if (ownedFurniture.Contains(furniture))
            ownedFurniture.Remove(furniture);
    }
}
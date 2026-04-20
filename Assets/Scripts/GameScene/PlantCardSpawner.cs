using System.Collections.Generic;
using UnityEngine;

public class PlantCardSpawner : MonoBehaviour
{
    [SerializeField] private PlantDatabase plantDatabase;
    [SerializeField] private Transform cardParent;
    [SerializeField] private GameObject cardPrefab;

    public void SpawnCards(List<string> ownedPlantIds)
    {
        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject);
        }

        foreach (string plantId in ownedPlantIds)
        {
            PlantData plant = plantDatabase.GetById(plantId);
            if (plant == null) continue;

            GameObject cardObj = Instantiate(cardPrefab, cardParent, false);

            PlantCardUI cardUI = cardObj.GetComponent<PlantCardUI>();
            if (cardUI != null)
            {
                cardUI.Setup(plant);
            }
        }
    }
}
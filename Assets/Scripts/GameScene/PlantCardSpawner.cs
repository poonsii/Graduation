using System.Collections.Generic;
using UnityEngine;

public class PlantCardSpawner : MonoBehaviour
{
    [SerializeField] private PlantDatabase plantDatabase; // plant data source.
    [SerializeField] private Transform cardParent; // parent object for cards
    [SerializeField] private GameObject cardPrefab; 

    public void SpawnCards(List<string> ownedPlantIds)
    {
        if (cardParent == null)
            return; // not set up yet - the static owned-plant cards handle display instead.

        foreach (Transform child in cardParent)
        {
            Destroy(child.gameObject); // remove old cards
        }

        foreach (string plantId in ownedPlantIds)
        {
            PlantData plant = plantDatabase.GetById(plantId); // find plant data.
            if (plant == null) continue; 

            GameObject cardObj = Instantiate(cardPrefab, cardParent, false); // create new card.

         //   PlantCardUI cardUI = cardObj.GetComponent<PlantCardUI>();
          //  if (cardUI != null)
         //   {
         //       cardUI.Setup(plant); // fill card with plant info
         //   }
        }
    }
}
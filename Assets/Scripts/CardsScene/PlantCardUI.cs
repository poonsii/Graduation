using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantCardUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text plantName;

    public void Setup(PlantData plant)
    {
        icon.sprite = plant.icon;
        plantName.text = plant.displayName;
    }
}

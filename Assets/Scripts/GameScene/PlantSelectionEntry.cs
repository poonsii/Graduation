using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantSelectionEntry : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text plantNameText;
    [SerializeField] private Toggle toggle;

    private string plantId;

    public void Setup(PlantData plant)
    {
        plantId = plant.id;
        plantNameText.text = plant.displayName;
        icon.sprite = plant.icon;
        toggle.isOn = false;

        Debug.Log("Created row for: " + plant.displayName);
    }

    public bool IsSelected()
    {
        return toggle.isOn;
    }

    public string GetPlantId()
    {
        return plantId;
    }
}
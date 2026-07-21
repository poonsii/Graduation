using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlantLocationConfirmPanel : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text plantNameText;
    [SerializeField] private TMP_Text locationNameText;
    [SerializeField] private TMP_Text adviceText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button chooseAnotherButton;

    [Header("References")]
    [SerializeField] private PlantDatabase plantDatabase;
    [SerializeField] private GameBootstrap gameBootstrap;
    [SerializeField] private PlantLocationSelector selector;

    private string currentPlantInstanceId;
    private string currentPlantId;
    private PlantLocationSpot currentSpot;
    private LightAdviceResult currentAdviceResult;

    public void Show(string uniquePlantInstanceId, string plantId, PlantLocationSpot spot)
    {
        currentPlantInstanceId = uniquePlantInstanceId;
        currentPlantId = plantId;
        currentSpot = spot;

        PlantData plant = plantDatabase.GetById(plantId);

        plantNameText.text = plant != null ? plant.displayName : plantId;
        locationNameText.text = spot.GetDisplayName();

        if (plant != null)
        {
            currentAdviceResult = PlantLocationAdvisor.GetAdvice(
                plant.requiredLight,
                spot.GetLocationType()
            );

            adviceText.text = GetAdviceText(currentAdviceResult);
        }
        else
        {
            currentAdviceResult = LightAdviceResult.Unknown;
            adviceText.text = "Could not read plant data.";
        }

        root.SetActive(true);
    }

    public void Hide()
    {
        root.SetActive(false);
    }

    public void OnConfirmPressed()
    {
        if (currentSpot == null)
            return;

        bool acceptedMismatch = currentAdviceResult == LightAdviceResult.Bad;

        gameBootstrap.UpdatePlantLocation(
            currentPlantInstanceId,
            currentSpot.GetLocationType(),
            acceptedMismatch
        );

        selector.FinishSelection();
        Hide();
    }

    public void OnChooseAnotherPressed()
    {
        Hide();
        selector.EnableSelectionAgain();
    }

    private string GetAdviceText(LightAdviceResult result)
    {
        switch (result)
        {
            case LightAdviceResult.Good:
                return "This location fits the plant well.";
            case LightAdviceResult.Warning:
                return "This location may work, but it is not ideal.";
            case LightAdviceResult.Bad:
                return "This location does not match the plant's needs very well.";
            default:
                return "No advice available.";
        }
    }
}
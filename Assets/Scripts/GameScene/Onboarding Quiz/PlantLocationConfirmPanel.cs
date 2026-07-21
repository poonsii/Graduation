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
        string plantName = plant != null ? plant.displayName : plantId;
        string locationName = spot.GetDisplayName();

        if (plant != null)
        {
            currentAdviceResult = PlantLocationAdvisor.GetAdvice(
                plant.requiredLight,
                spot.GetLocationType()
            );

            plantNameText.text = $"Your {plantName} is placed in {locationName}, which {GetAdviceFragment(currentAdviceResult)}.";
        }
        else
        {
            currentAdviceResult = LightAdviceResult.Unknown;
            plantNameText.text = "Could not read plant data.";
        }

        if (locationNameText != null)
            locationNameText.text = "";

        if (adviceText != null)
            adviceText.text = "";

        gameBootstrap.PreviewPlantLocation(plantId, spot.transform); // show the plant at this spot while the player decides.

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
            acceptedMismatch,
            currentSpot.transform
        );

        selector.FinishSelection();
        Hide();
    }

    public void OnChooseAnotherPressed()
    {
        gameBootstrap.HidePlantPreview(currentPlantId); // hide it again until a new spot is picked.
        Hide();
        selector.EnableSelectionAgain();
    }

    private string GetAdviceFragment(LightAdviceResult result)
    {
        switch (result)
        {
            case LightAdviceResult.Good:
                return "fits it well";
            case LightAdviceResult.Warning:
                return "may work, but is not ideal";
            case LightAdviceResult.Bad:
                return "does not match its needs very well";
            default:
                return "has no advice available";
        }
    }
}
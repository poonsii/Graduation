using TMPro;
using UnityEngine;

public class OwnedPlantCardUI : MonoBehaviour
{
    [SerializeField] private string plantId;
    [SerializeField] private GameObject cardRoot;

    [Header("References")]
    [SerializeField] private GameBootstrap gameBootstrap;
    [SerializeField] private PlantDatabase plantDatabase;

    [Header("Onboarding Info")]
    [SerializeField] private TMP_Text locationInfoText;
    [SerializeField] private TMP_Text soilInfoText;
    [SerializeField] private TMP_Text humidityInfoText;

    public string GetPlantId() // get the plant id.
    {
        return plantId;
    }

    public void SetOwned(bool owned) // show if it is owned.
    {
        if (cardRoot != null)
            cardRoot.SetActive(owned);
        else
            gameObject.SetActive(owned);

        if (owned)
            Refresh();
    }

    public void Refresh()
    {
        if (gameBootstrap == null || plantDatabase == null)
            return;

        SavedPlantState state = gameBootstrap.GetSavedPlantStateForPlant(plantId);
        PlantData plant = plantDatabase.GetById(plantId);

        if (state == null || plant == null)
            return;

        UpdateLocationText(state);
        UpdateSoilText(state);
        UpdateHumidityText(state);
    }

    private void UpdateLocationText(SavedPlantState state)
    {
        if (locationInfoText == null)
            return;

        if (state.selectedLightLocation == LightLocationType.Unknown)
        {
            locationInfoText.text = "Location: not placed yet.";
            return;
        }

        string ignoredNote = state.lightAdviceResult == LightAdviceResult.Bad && state.playerAcceptedMismatch
            ? " (kept anyway)"
            : "";

        locationInfoText.text = $"Location: {state.selectedLightLocation} - {PlantAdviceText.GetLabel(state.lightAdviceResult)}{ignoredNote}";
    }

    private void UpdateSoilText(SavedPlantState state)
    {
        if (soilInfoText == null)
            return;

        if (state.potSoilType == PotSoilType.Unknown)
        {
            soilInfoText.text = "Soil: not set yet.";
            return;
        }

        string ignoredNote = state.potSoilAdviceResult == PotSoilAdviceResult.Bad && state.playerAcceptedSoilMismatch
            ? " (kept anyway)"
            : "";

        soilInfoText.text = $"Soil: {state.potSoilType} - {PlantAdviceText.GetLabel(state.potSoilAdviceResult)}{ignoredNote}";
    }

    private void UpdateHumidityText(SavedPlantState state)
    {
        if (humidityInfoText == null)
            return;

        if (state.humidityLevel == HumidityLevel.Unknown)
        {
            humidityInfoText.text = "Humidity: not set yet.";
            return;
        }

        string ignoredNote = state.humidityAdviceResult == HumidityAdviceResult.Bad && state.playerAcceptedHumidityMismatch
            ? " (kept anyway)"
            : "";

        humidityInfoText.text = $"Humidity: {state.humidityLevel} - {PlantAdviceText.GetLabel(state.humidityAdviceResult)}{ignoredNote}";
    }
}
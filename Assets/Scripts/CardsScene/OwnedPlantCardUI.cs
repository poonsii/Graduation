using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class OwnedPlantCardUI : MonoBehaviour
{
    [SerializeField] private string plantId;
    [SerializeField] private GameObject cardRoot;

    [Header("References")]
    [SerializeField] private GameBootstrap gameBootstrap;
    [SerializeField] private PlantDatabase plantDatabase;

    [Header("Onboarding Info")]
    [SerializeField] private TMP_Text locationInfoText;

    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        Refresh(); // re-run so the card's text updates immediately on a language switch.
    }

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
    }

    private void UpdateLocationText(SavedPlantState state)
    {
        if (locationInfoText == null)
            return;

        if (state.selectedLightLocation == LightLocationType.Unknown)
        {
            locationInfoText.text = LocalizedText.Get("card_location_not_set");
            return;
        }

        string ignoredNote = state.lightAdviceResult == LightAdviceResult.Bad && state.playerAcceptedMismatch
            ? LocalizedText.Get("card_kept_anyway")
            : "";

        locationInfoText.text = LocalizedText.Get(
            "card_location", PlantAdviceText.GetLabel(state.selectedLightLocation), PlantAdviceText.GetLabel(state.lightAdviceResult)) + ignoredNote;
    }
}

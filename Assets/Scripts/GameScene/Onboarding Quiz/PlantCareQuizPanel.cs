using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SoilOptionButton
{
    public PotSoilType soilType;
    public Button button;
}

[Serializable]
public class HumidityOptionButton
{
    public HumidityLevel humidityLevel;
    public Button button;
}

public class PlantCareQuizPanel : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text plantNameText;

    [Header("Soil Step")]
    [SerializeField] private GameObject soilStepRoot;
    [SerializeField] private List<SoilOptionButton> soilOptions;
    [SerializeField] private GameObject soilAdviceRoot;
    [SerializeField] private TMP_Text soilAdviceText;
    [SerializeField] private Button soilAcceptButton;
    [SerializeField] private Button soilChooseAnotherButton;

    [Header("Humidity Step")]
    [SerializeField] private GameObject humidityStepRoot;
    [SerializeField] private List<HumidityOptionButton> humidityOptions;
    [SerializeField] private GameObject humidityAdviceRoot;
    [SerializeField] private TMP_Text humidityAdviceText;
    [SerializeField] private Button humidityAcceptButton;
    [SerializeField] private Button humidityChooseAnotherButton;

    [Header("References")]
    [SerializeField] private PlantDatabase plantDatabase;
    [SerializeField] private GameBootstrap gameBootstrap;

    private string currentPlantInstanceId;
    private PlantData currentPlant;

    private PotSoilType selectedSoil = PotSoilType.Unknown;
    private PotSoilAdviceResult soilAdviceResult = PotSoilAdviceResult.Unknown;

    private HumidityLevel selectedHumidity = HumidityLevel.Unknown;
    private HumidityAdviceResult humidityAdviceResult = HumidityAdviceResult.Unknown;

    private Action onQuizComplete;

    private void Awake()
    {
        foreach (SoilOptionButton option in soilOptions)
        {
            if (option == null || option.button == null)
                continue;

            PotSoilType capturedType = option.soilType; // avoid the classic loop-variable capture bug.
            option.button.onClick.AddListener(() => OnSoilOptionPicked(capturedType));
        }

        foreach (HumidityOptionButton option in humidityOptions)
        {
            if (option == null || option.button == null)
                continue;

            HumidityLevel capturedLevel = option.humidityLevel;
            option.button.onClick.AddListener(() => OnHumidityOptionPicked(capturedLevel));
        }

        if (soilAcceptButton != null)
            soilAcceptButton.onClick.AddListener(OnSoilAcceptPressed);

        if (soilChooseAnotherButton != null)
            soilChooseAnotherButton.onClick.AddListener(OnSoilChooseAnotherPressed);

        if (humidityAcceptButton != null)
            humidityAcceptButton.onClick.AddListener(OnHumidityAcceptPressed);

        if (humidityChooseAnotherButton != null)
            humidityChooseAnotherButton.onClick.AddListener(OnHumidityChooseAnotherPressed);
    }

    public void Show(string uniquePlantInstanceId, string plantId, Action onComplete)
    {
        currentPlantInstanceId = uniquePlantInstanceId;
        onQuizComplete = onComplete;
        currentPlant = plantDatabase != null ? plantDatabase.GetById(plantId) : null;

        selectedSoil = PotSoilType.Unknown;
        soilAdviceResult = PotSoilAdviceResult.Unknown;
        selectedHumidity = HumidityLevel.Unknown;
        humidityAdviceResult = HumidityAdviceResult.Unknown;

        if (plantNameText != null)
            plantNameText.text = currentPlant != null ? currentPlant.displayName : plantId;

        ShowSoilStep();

        if (root != null)
            root.SetActive(true);
    }

    private void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }

    private void ShowSoilStep()
    {
        if (soilStepRoot != null) soilStepRoot.SetActive(true);
        if (humidityStepRoot != null) humidityStepRoot.SetActive(false);
        if (soilAdviceRoot != null) soilAdviceRoot.SetActive(false);
    }

    private void ShowHumidityStep()
    {
        if (soilStepRoot != null) soilStepRoot.SetActive(false);
        if (humidityStepRoot != null) humidityStepRoot.SetActive(true);
        if (humidityAdviceRoot != null) humidityAdviceRoot.SetActive(false);
    }

    private void OnSoilOptionPicked(PotSoilType type)
    {
        selectedSoil = type;

        PotSoilType recommended = currentPlant != null ? currentPlant.recommendedPotSoilType : PotSoilType.Unknown;
        soilAdviceResult = PlantSoilAdvisor.GetAdvice(recommended, type);

        if (soilAdviceText != null)
            soilAdviceText.text = LocalizedText.Get("care_soil_advice", PlantAdviceText.GetLabel(soilAdviceResult));

        if (soilAdviceRoot != null)
            soilAdviceRoot.SetActive(true);
    }

    private void OnHumidityOptionPicked(HumidityLevel level)
    {
        selectedHumidity = level;

        HumidityLevel recommended = currentPlant != null ? currentPlant.recommendedHumidityLevel : HumidityLevel.Unknown;
        humidityAdviceResult = PlantHumidityAdvisor.GetAdvice(recommended, level);

        if (humidityAdviceText != null)
            humidityAdviceText.text = LocalizedText.Get("care_humidity_advice", PlantAdviceText.GetLabel(humidityAdviceResult));

        if (humidityAdviceRoot != null)
            humidityAdviceRoot.SetActive(true);
    }

    private void OnSoilAcceptPressed()
    {
        if (selectedSoil == PotSoilType.Unknown)
            return;

        ShowHumidityStep();
    }

    private void OnSoilChooseAnotherPressed()
    {
        selectedSoil = PotSoilType.Unknown;
        soilAdviceResult = PotSoilAdviceResult.Unknown;

        if (soilAdviceRoot != null)
            soilAdviceRoot.SetActive(false);
    }

    private void OnHumidityAcceptPressed()
    {
        if (selectedHumidity == HumidityLevel.Unknown)
            return;

        bool acceptedSoilMismatch = soilAdviceResult == PotSoilAdviceResult.Bad;
        bool acceptedHumidityMismatch = humidityAdviceResult == HumidityAdviceResult.Bad;

        if (gameBootstrap != null)
        {
            gameBootstrap.UpdatePlantCare(
                currentPlantInstanceId,
                selectedSoil,
                selectedHumidity,
                acceptedSoilMismatch,
                acceptedHumidityMismatch
            );
        }

        Hide();

        Action callback = onQuizComplete;
        onQuizComplete = null;
        callback?.Invoke(); // tell the onboarding flow this plant is fully done, so it can move to the next one.
    }

    private void OnHumidityChooseAnotherPressed()
    {
        selectedHumidity = HumidityLevel.Unknown;
        humidityAdviceResult = HumidityAdviceResult.Unknown;

        if (humidityAdviceRoot != null)
            humidityAdviceRoot.SetActive(false);
    }
}

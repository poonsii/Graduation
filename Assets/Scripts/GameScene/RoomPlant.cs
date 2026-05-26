using System;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

public class RoomPlant : MonoBehaviour
{
    [Header("Save")]
    [SerializeField] private string plantId = "RoomPlant_1";

    [Header("Plant Setup")]
    [SerializeField] private PlantVisualController visualController;
    [SerializeField] private float unhealthyAfterSeconds = 20f;

    [Header("Day Settings")]
    [SerializeField] private float secondsPerDay = 60f;

    [Header("UI")]
    [SerializeField] private GameObject plantUiPanel;
    [SerializeField] private TMP_Text[] dayTexts;
    [SerializeField] private ClickObject[] clickObjects;

    [Header("Localization")]
    [SerializeField] private LocalizedString dayLocalizedString;

    private float neglectTimer = 0f;
    private bool isUnhealthy = false;
    private int currentDay = 1;
    private bool isOwned = true;

    private string TimerKey => plantId + "_NeglectTimer";
    private string UnhealthyKey => plantId + "_IsUnhealthy";
    private string SaveTimeKey => plantId + "_LastSaveTime";

    private void Awake()
    {
        LoadPlantState();
        UpdatePlantState();
    }

    private void OnEnable()
    {
        if (dayLocalizedString != null)
            dayLocalizedString.StringChanged += UpdateLocalizedDayText;
    }

    private void OnDisable()
    {
        if (dayLocalizedString != null)
            dayLocalizedString.StringChanged -= UpdateLocalizedDayText;

        SavePlantState();
    }

    private void Start()
    {
        RefreshDayString();
    }

    private void Update()
    {
        if (!isOwned)
            return;

        neglectTimer += Time.deltaTime;
        UpdatePlantState();
    }

    public string GetPlantId()
    {
        return plantId;
    }

    public void SetOwned(bool owned)
    {
        isOwned = owned;

        Debug.Log($"{plantId} SetOwned called: {owned}", this);

        gameObject.SetActive(owned);

        if (plantUiPanel != null)
        {
            plantUiPanel.SetActive(owned);
            Debug.Log($"{plantId} UI panel set active: {owned} -> {plantUiPanel.name}", plantUiPanel);
        }
        else
        {
            Debug.LogWarning($"{plantId} has no plantUiPanel assigned.", this);
        }

        if (owned)
        {
            LoadPlantState();
            UpdatePlantState();
        }
    }

    public void CareForPlant()
    {
        if (!isOwned)
            return;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        Debug.Log("CareForPlant called on: " + plantId, gameObject);

        bool wasUnhealthy = isUnhealthy;

        neglectTimer = 0f;
        currentDay = 1;
        isUnhealthy = false;

        if (visualController != null)
            visualController.SetHealthy(true);

        RefreshDayString();
        SavePlantState();

        if (PointsManager.Instance != null)
        {
            PointsManager.Instance.AddPlantCareReward(wasUnhealthy);
        }
        else
        {
            Debug.LogWarning("PointsManager.Instance is null");
        }

        CloseAllPlantCards();

        Debug.Log(plantId + " is healthy again.");
    }

    private void CloseAllPlantCards()
    {
        bool closedAny = false;

        if (clickObjects != null)
        {
            foreach (ClickObject clickObject in clickObjects)
            {
                if (clickObject != null)
                {
                    clickObject.ClosePlantCard();
                    closedAny = true;
                }
            }
        }

        if (!closedAny && plantUiPanel != null)
        {
            plantUiPanel.SetActive(false);
        }
    }

    public void ResetPlantState()
    {
        neglectTimer = 0f;
        currentDay = 1;
        isUnhealthy = false;

        PlayerPrefs.DeleteKey(TimerKey);
        PlayerPrefs.DeleteKey(UnhealthyKey);
        PlayerPrefs.DeleteKey(SaveTimeKey);
        PlayerPrefs.Save();

        if (visualController != null)
            visualController.SetHealthy(true);

        RefreshDayString();

        Debug.Log($"{plantId} state reset.");
    }

    private void UpdatePlantState()
    {
        currentDay = Mathf.FloorToInt(neglectTimer / secondsPerDay) + 1;
        isUnhealthy = neglectTimer >= unhealthyAfterSeconds;

        if (visualController != null)
        {
            visualController.SetHealthy(!isUnhealthy);
        }

        RefreshDayString();
    }

    private void RefreshDayString()
    {
        if (dayLocalizedString != null)
        {
            dayLocalizedString.Arguments = new object[] { currentDay };
            dayLocalizedString.RefreshString();
        }
        else
        {
            UpdateFallbackDayText();
        }
    }

    private void UpdateLocalizedDayText(string value)
    {
        if (dayTexts == null) return;

        foreach (TMP_Text text in dayTexts)
        {
            if (text != null)
                text.text = value;
        }
    }

    private void UpdateFallbackDayText()
    {
        if (dayTexts == null) return;

        foreach (TMP_Text text in dayTexts)
        {
            if (text != null)
                text.text = "Day " + currentDay;
        }
    }

    private void SavePlantState()
    {
        if (!isOwned) return;

        PlayerPrefs.SetFloat(TimerKey, neglectTimer);
        PlayerPrefs.SetInt(UnhealthyKey, isUnhealthy ? 1 : 0);
        PlayerPrefs.SetString(SaveTimeKey, DateTime.Now.Ticks.ToString());
        PlayerPrefs.Save();

        Debug.Log($"Saved {plantId} | Timer={neglectTimer} | Unhealthy={isUnhealthy}");
    }

    private void LoadPlantState()
    {
        neglectTimer = PlayerPrefs.GetFloat(TimerKey, 0f);
        isUnhealthy = PlayerPrefs.GetInt(UnhealthyKey, 0) == 1;

        if (PlayerPrefs.HasKey(SaveTimeKey))
        {
            string ticksString = PlayerPrefs.GetString(SaveTimeKey, "0");

            if (long.TryParse(ticksString, out long savedTicks) && savedTicks > 0)
            {
                DateTime savedTime = new DateTime(savedTicks);
                TimeSpan elapsed = DateTime.Now - savedTime;
                neglectTimer += (float)elapsed.TotalSeconds;
            }
        }

        Debug.Log($"Loaded {plantId} | Timer={neglectTimer} | Unhealthy={isUnhealthy}");
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SavePlantState();
    }

    private void OnApplicationQuit()
    {
        SavePlantState();
    }
}
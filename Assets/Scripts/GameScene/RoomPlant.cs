using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class RoomPlant : MonoBehaviour
{
    [Header("Save")]
    [SerializeField] private string plantId = "Plant_01";

    [Header("Plant Setup")]
    [SerializeField] private PlantVisualController visualController;
    [SerializeField] private float unhealthyAfterSeconds = 20f;

    [Header("Day Settings")]
    [SerializeField] private float secondsPerDay = 5f;

    [Header("UI")]
    [SerializeField] private GameObject plantUiPanel;
    [SerializeField] private TMP_Text[] dayTexts;

    private float neglectTimer = 0f;
    private bool isUnhealthy = false;
    private int currentDay = 1;
    private bool isOwned = true;

    private string TimerKey => plantId + "_NeglectTimer";
    private string UnhealthyKey => plantId + "_IsUnhealthy";
    private string SaveTimeKey => plantId + "_LastSaveTime";

    private void Start()
    {
        LoadPlantState();
        UpdatePlantState();
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
            UpdatePlantState();
    }

    public void CareForPlant()
    {
        if (!isOwned)
            return;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        Debug.Log("CareForPlant called on: " + gameObject.name, gameObject);

        bool wasUnhealthy = isUnhealthy;

        neglectTimer = 0f;
        currentDay = 1;
        isUnhealthy = false;

        visualController.SetHealthy(true);
        UpdateDayText();
        SavePlantState();

        if (PointsManager.Instance != null)
        {
            PointsManager.Instance.AddPlantCareReward(wasUnhealthy);
        }
        else
        {
            Debug.LogWarning("PointsManager.Instance is null");
        }

        Debug.Log("Plant is healthy again.");
    }

    private void UpdatePlantState()
    {
        currentDay = Mathf.FloorToInt(neglectTimer / secondsPerDay) + 1;

        if (neglectTimer >= unhealthyAfterSeconds)
        {
            isUnhealthy = true;
        }

        if (visualController != null)
        {
            visualController.SetHealthy(!isUnhealthy);
        }

        UpdateDayText();
    }

    private void UpdateDayText()
    {
        foreach (TMP_Text text in dayTexts)
        {
            if (text != null)
            {
                text.text = "Day " + currentDay;
            }
        }
    }

    private void SavePlantState()
    {
        PlayerPrefs.SetFloat(TimerKey, neglectTimer);
        PlayerPrefs.SetInt(UnhealthyKey, isUnhealthy ? 1 : 0);
        PlayerPrefs.SetString(SaveTimeKey, DateTime.Now.Ticks.ToString());
        PlayerPrefs.Save();
    }

    private void LoadPlantState()
    {
        neglectTimer = PlayerPrefs.GetFloat(TimerKey, 0f);
        isUnhealthy = PlayerPrefs.GetInt(UnhealthyKey, 0) == 1;

        if (PlayerPrefs.HasKey(SaveTimeKey))
        {
            long savedTicks = Convert.ToInt64(PlayerPrefs.GetString(SaveTimeKey));
            DateTime savedTime = new DateTime(savedTicks);
            TimeSpan elapsed = DateTime.Now - savedTime;
            neglectTimer += (float)elapsed.TotalSeconds;
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && isOwned)
        {
            SavePlantState();
        }
    }

    private void OnApplicationQuit()
    {
        if (isOwned)
        {
            SavePlantState();
        }
    }

    private void OnDisable()
    {
        if (isOwned)
        {
            SavePlantState();
        }
    }
}
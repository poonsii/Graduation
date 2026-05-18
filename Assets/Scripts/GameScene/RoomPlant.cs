using UnityEngine;
using TMPro;

public class RoomPlant : MonoBehaviour
{
    [Header("Plant Setup")]
    [SerializeField] private PlantVisualController visualController;
    [SerializeField] private float unhealthyAfterSeconds = 20f;

    [Header("Day Settings")]
    [SerializeField] private float secondsPerDay = 5f;

    [Header("UI")]
    [SerializeField] private TMP_Text[] dayTexts;

    private float neglectTimer = 0f;
    private bool isUnhealthy = false;
    private int currentDay = 1;

    private void Start()
    {
        UpdateDayText();
        visualController.SetHealthy(true);
    }

    private void Update()
    {
        neglectTimer += Time.deltaTime;

        int newDay = Mathf.FloorToInt(neglectTimer / secondsPerDay) + 1;
        if (newDay != currentDay)
        {
            currentDay = newDay;
            UpdateDayText();
        }

        if (neglectTimer >= unhealthyAfterSeconds)
        {
            isUnhealthy = true;
        }

        visualController.SetHealthy(!isUnhealthy);
    }

    public void CareForPlant()
    {
        neglectTimer = 0f;
        currentDay = 1;
        isUnhealthy = false;

        visualController.SetHealthy(true);
        UpdateDayText();

        Debug.Log("Plant is healthy again.");
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
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Localization;

public class RoomPlant : MonoBehaviour
{
    [Header("Save")]
    [SerializeField] private string plantId = "RoomPlant_1";

    [Header("Plant Setup")]
    [SerializeField] private PlantVisualController visualController; // switches between healthy and unhealthy looks.
    [SerializeField] private float unhealthyAfterSeconds = 20f; // fallback only - used if the calendar isn't wired up.
    [SerializeField] private PlantData plantData; // expand this to see/edit the watering & fertilizing days used by the calendar.

    [Header("UI")]
    [SerializeField] private GameObject plantUiPanel;
    [SerializeField] private TMP_Text[] dayTexts;
    [SerializeField] private ClickObject[] clickObjects;

    [Header("Reminder UI")]
    [SerializeField] private GameObject reminderPopup; // reminder message box
    [SerializeField] private TMP_Text reminderText;
    [SerializeField] private float reminderBeforeUnhealthySeconds = 5f; // when the reminder should appear
    [SerializeField] private float reminderPopupDuration = 3f;

    [Header("Plant Display")]
    [SerializeField] private string plantDisplayName = "plant_1";

    [Header("Localization")]
    [SerializeField] private LocalizedString dayLocalizedString;
    [SerializeField] private LocalizedString reminderLocalizedString;

    [Header("Calendar Info")]
    [SerializeField] private GameBootstrap gameBootstrap; // used to read the calendar's saved plant state (last watered date, etc).
    [SerializeField] private TMP_Text[] lastWateredTexts;
    [SerializeField] private PlantCalendarController calendarController; // this plant's calendar screen, if one exists.

    [Header("Calendar Week Preview")]
    [SerializeField] private RectTransform weekPreviewContainer; // needs a Grid Layout Group, fixed column count 7.
    [SerializeField] private CalendarDayCell weekPreviewDayCellPrefab; // can be a smaller variant of the main calendar's day cell.

    [Header("Light Location Info")]
    [SerializeField] private TMP_Text[] lightLocationTexts; // shows the chosen location, and a short warning if it isn't a good fit.

    [Header("Plant Health Status")]
    [SerializeField] private TMP_Text[] plantHealthStatusTexts; // "Good" / "Needs attention" / "Doing bad".

    private readonly List<CalendarDayCell> weekPreviewCells = new List<CalendarDayCell>();

    private float neglectTimer = 0f;
    private bool isUnhealthy = false;
    private int currentDay = 1;
    private bool isOwned = true;

    private bool reminderShown = false;
    private Coroutine reminderCoroutine;

    private string TimerKey => plantId + "_NeglectTimer";
    private string UnhealthyKey => plantId + "_IsUnhealthy";
    private string SaveTimeKey => plantId + "_LastSaveTime";


    private void Awake()
    {
        LoadPlantState();
        UpdatePlantState();
        HideReminderPopupImmediate();
    }

    private void OnEnable()
    {
        if (dayLocalizedString != null)
            dayLocalizedString.StringChanged += UpdateLocalizedDayText; // update day text when language changes.

        if (reminderLocalizedString != null)
            reminderLocalizedString.StringChanged += UpdateLocalizedReminderText; // update reminder text when language changes.
    }

    private void OnDisable()
    {
        if (dayLocalizedString != null)
            dayLocalizedString.StringChanged -= UpdateLocalizedDayText; // stop listening for day text changes.

        if (reminderLocalizedString != null)
            reminderLocalizedString.StringChanged -= UpdateLocalizedReminderText; // stop listening for reminder text changes.

        SavePlantState();
    }

    private void Start()
    {
        RefreshDayString(); // show the correct day text.
    }

    private void Update()
    {
        if (!isOwned)
            return; // do nothing if the plant is not owned.

        neglectTimer += Time.deltaTime; // count time passing.

        UpdatePlantState(); // check if the plant is still healthy.
        CheckReminder(); // see if the reminder should show.
    }

    public string GetPlantId()
    {
        return plantId; // give back the plant id
    }

    public void RefreshCardInfo() // pulls the calendar/location info onto this plant's card - called by InventoryManager whenever it changes.
    {
        if (gameBootstrap == null)
            return;

        SavedPlantState state = gameBootstrap.GetSavedPlantStateForPlant(plantId);

        RefreshLastWateredText(state);
        RefreshWeekPreview(state);
        RefreshLightLocationText(state);
        RefreshPlantHealthStatusText(state);
    }

    private void RefreshPlantHealthStatusText(SavedPlantState state)
    {
        if (plantHealthStatusTexts == null)
            return;

        PlantHealthStatus status = PlantHealthAdvisor.GetStatus(state, ResolvePlantData(), CalendarClock.Now);
        string label = PlantHealthAdvisor.GetLabel(status);

        foreach (TMP_Text statusText in plantHealthStatusTexts)
        {
            if (statusText != null)
                statusText.text = label;
        }
    }

    private void RefreshLastWateredText(SavedPlantState state)
    {
        if (lastWateredTexts == null)
            return;

        string text = state != null && !string.IsNullOrEmpty(state.lastWateredDate)
            ? LocalizedText.Get("card_last_watered", state.lastWateredDate)
            : LocalizedText.Get("card_last_watered_unknown");

        foreach (TMP_Text lastWateredText in lastWateredTexts)
        {
            if (lastWateredText != null)
                lastWateredText.text = text;
        }
    }

    private void RefreshLightLocationText(SavedPlantState state)
    {
        if (lightLocationTexts == null)
            return;

        string text;

        if (state == null || state.selectedLightLocation == LightLocationType.Unknown)
        {
            text = LocalizedText.Get("card_light_location_unknown");
        }
        else
        {
            text = LocalizedText.Get("card_light_location", PlantAdviceText.GetLabel(state.selectedLightLocation));

            if (state.lightAdviceResult == LightAdviceResult.Warning || state.lightAdviceResult == LightAdviceResult.Bad)
                text += "\n" + LocalizedText.Get("card_light_location_advice"); // not the best spot for this plant.
        }

        foreach (TMP_Text lightLocationText in lightLocationTexts)
        {
            if (lightLocationText != null)
                lightLocationText.text = text;
        }
    }

    private void RefreshWeekPreview(SavedPlantState state)
    {
        if (weekPreviewContainer == null || weekPreviewDayCellPrefab == null)
            return;

        foreach (CalendarDayCell cell in weekPreviewCells)
        {
            if (cell != null)
                Destroy(cell.gameObject);
        }

        weekPreviewCells.Clear();

        HashSet<string> wateredDates = new HashSet<string>();
        HashSet<string> fertilizedDates = new HashSet<string>();

        if (state != null)
        {
            foreach (CalendarLogEntry entry in state.careLog)
            {
                if (entry.actionType == CareActionType.Watered)
                    wateredDates.Add(entry.date);
                else
                    fertilizedDates.Add(entry.date);
            }
        }

        DateTime today = CalendarClock.Now.Date;
        int daysSinceMonday = ((int)today.DayOfWeek + 6) % 7; // Monday = 0 .. Sunday = 6
        DateTime monday = today.AddDays(-daysSinceMonday);

        for (int i = 0; i < 7; i++)
        {
            DateTime day = monday.AddDays(i);
            string dayString = day.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

            CalendarDayCell cell = Instantiate(weekPreviewDayCellPrefab, weekPreviewContainer);
            cell.Setup(day.Day, day.Date == today, wateredDates.Contains(dayString), fertilizedDates.Contains(dayString));
            weekPreviewCells.Add(cell);
        }
    }

    public void OnOpenCalendarPressed() // lets the player log care later even if they skipped it during onboarding.
    {
        if (calendarController != null)
            calendarController.Open();
    }

    public void MoveToSpot(Transform spotTransform)
    {
        if (spotTransform == null)
            return;

        transform.position = spotTransform.position;
        transform.rotation = spotTransform.rotation;
    }

    public void SetCardInteractable(bool interactable)
    {
        if (clickObjects == null)
            return;

        foreach (ClickObject clickObject in clickObjects)
        {
            if (clickObject != null)
                clickObject.enabled = interactable;
        }
    }

    public void SetOwned(bool owned)
    {
        isOwned = owned; // remember if the player owns it.

        gameObject.SetActive(owned); // hide or show the plant.

        if (plantUiPanel != null)
        {
            plantUiPanel.SetActive(owned); // hide or show the ui panel too.
        }

        if (owned)
        {
            LoadPlantState();
            UpdatePlantState();
            HideReminderPopupImmediate();
        }
    }

    public void CareForPlant()
    {
        if (!isOwned)
            return;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null); // clear selected ui.

        bool wasUnhealthy = isUnhealthy; // remember if the plant was bad before caring.

        neglectTimer = 0f;
        currentDay = 1;
        isUnhealthy = false;
        reminderShown = false;

        if (visualController != null)
            visualController.SetHealthy(true); // switch to healthy loo

        RefreshDayString();
        HideReminderPopupImmediate();
        SavePlantState();

        if (PointsManager.Instance != null)
        {
            PointsManager.Instance.AddPlantCareReward(wasUnhealthy); // give points for caring
        }

        CloseAllPlantCards(); // close plant cards.
    }

    public void ResetPlantState()
    {
        neglectTimer = 0f;
        currentDay = 1;
        isUnhealthy = false;
        reminderShown = false;

        PlayerPrefs.DeleteKey(TimerKey);
        PlayerPrefs.DeleteKey(UnhealthyKey);
        PlayerPrefs.DeleteKey(SaveTimeKey);
        PlayerPrefs.Save();

        if (visualController != null)
            visualController.SetHealthy(true); // show healthy plant.

        RefreshDayString(); // update text
        HideReminderPopupImmediate(); // hide reminder popup
    }

    private void UpdatePlantState()
    {
        currentDay = ComputeCurrentDayFromCalendar(); // real days since it was last watered, instead of a compressed timer.
        isUnhealthy = IsOverdueByCalendar(); // unhealthy once the calendar says care is overdue (e.g. water not logged within its window).

        if (visualController != null)
            visualController.SetHealthy(!isUnhealthy); // switch plant look.

        RefreshDayString();
    }

    private PlantData ResolvePlantData()
    {
        if (plantData != null)
            return plantData;

        return gameBootstrap != null ? gameBootstrap.GetPlantData(plantId) : null;
    }

    private bool IsOverdueByCalendar()
    {
        if (gameBootstrap == null)
            return neglectTimer >= unhealthyAfterSeconds; // fall back to the old timer-based check if the calendar isn't wired up.

        SavedPlantState state = gameBootstrap.GetSavedPlantStateForPlant(plantId);

        return CalendarBadgeManager.IsOverdueForCare(state, ResolvePlantData(), CalendarClock.Now);
    }

    private int ComputeCurrentDayFromCalendar()
    {
        if (gameBootstrap == null)
            return currentDay; // calendar isn't wired up - leave the day counter as is.

        SavedPlantState state = gameBootstrap.GetSavedPlantStateForPlant(plantId);

        if (state == null || string.IsNullOrEmpty(state.lastWateredDate))
            return 1; // no watering logged yet.

        if (!DateTime.TryParseExact(state.lastWateredDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastWatered))
            return 1;

        return (CalendarClock.Now.Date - lastWatered.Date).Days + 1; // day 1 = the day it was watered.
    }

    private void CheckReminder()
    {
        if (!isOwned || !PlantReminderSettings.RemindersEnabled || isUnhealthy || reminderShown)
            return; // stop if reminder should not show.

        float timeLeft = unhealthyAfterSeconds - neglectTimer; // check how much time is left.

        if (timeLeft <= reminderBeforeUnhealthySeconds && timeLeft > 0f)
        {
            reminderShown = true; // remember that i already showed it.
            ShowReminderPopup(); // show reminder popup.
        }
    }

    private void ShowReminderPopup()
    {
        if (reminderPopup == null)
            return;

        string shownName = string.IsNullOrWhiteSpace(plantDisplayName) ? plantId : plantDisplayName; // pick the name to show.

        if (reminderLocalizedString != null)
        {
            reminderLocalizedString.Arguments = new object[] { shownName }; // give the name to the translation text.
            reminderLocalizedString.RefreshString(); // refresh the localized message.
        }
        else if (reminderText != null)
        {
            reminderText.text = $"Don't forget to tend to your \"{shownName}\".";
        }

        if (reminderCoroutine != null)
            StopCoroutine(reminderCoroutine); // stop old popup timer if one exists.

        reminderCoroutine = StartCoroutine(ReminderPopupRoutine());
    }

    private IEnumerator ReminderPopupRoutine()
    {
        reminderPopup.SetActive(true); // show the reminder.

        yield return new WaitForSeconds(reminderPopupDuration); // wait a little while.

        reminderPopup.SetActive(false); // hide the reminder again.
        reminderCoroutine = null;
    }

    private void HideReminderPopupImmediate()
    {
        if (reminderCoroutine != null)
        {
            StopCoroutine(reminderCoroutine);
            reminderCoroutine = null; // clear timer reference.
        }

        if (reminderPopup != null)
            reminderPopup.SetActive(false); // hide popup now.
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
                    clickObject.ClosePlantCard(); // close the card.
                    closedAny = true;
                }
            }
        }

        if (!closedAny && plantUiPanel != null)
        {
            plantUiPanel.SetActive(false); // hide the ui panel 
        }
    }

    private void RefreshDayString()
    {
        if (dayLocalizedString != null)
        {
            dayLocalizedString.Arguments = new object[] { currentDay }; // give the day number to the localization text.
            dayLocalizedString.RefreshString(); // refresh the text.
        }
        else
        {
            UpdateFallbackDayText(); // use normal text if localization is missing.
        }
    }

    private void UpdateLocalizedDayText(string value)
    {
        if (dayTexts == null)
            return;

        foreach (TMP_Text text in dayTexts)
        {
            if (text != null)
                text.text = value; // show the localized day text
        }
    }

    private void UpdateLocalizedReminderText(string value)
    {
        if (reminderText != null)
            reminderText.text = value; // show the localized reminder text
    }

    private void UpdateFallbackDayText()
    {
        if (dayTexts == null)
            return;

        foreach (TMP_Text text in dayTexts)
        {
            if (text != null)
                text.text = "Day " + currentDay; // show simple day text
        }
    }

    private void SavePlantState()
    {
        if (!isOwned)
            return;

        PlayerPrefs.SetFloat(TimerKey, neglectTimer); // save timer
        PlayerPrefs.SetInt(UnhealthyKey, isUnhealthy ? 1 : 0); // save unhealthy state
        PlayerPrefs.SetString(SaveTimeKey, DateTime.Now.Ticks.ToString()); // save current time
        PlayerPrefs.Save(); // write data to disk
    }

    private void LoadPlantState()
    {
        neglectTimer = PlayerPrefs.GetFloat(TimerKey, 0f); // load time
        isUnhealthy = PlayerPrefs.GetInt(UnhealthyKey, 0) == 1; // load unhealthy state
        reminderShown = false; // reset reminder flag.

        if (PlayerPrefs.HasKey(SaveTimeKey))
        {
            string ticksString = PlayerPrefs.GetString(SaveTimeKey, "0"); // get saved time

            if (long.TryParse(ticksString, out long savedTicks) && savedTicks > 0)
            {
                DateTime savedTime = new DateTime(savedTicks); // turn saved ticks into time.
                TimeSpan elapsed = DateTime.Now - savedTime; // work out how much time passed.
                neglectTimer += (float)elapsed.TotalSeconds; // add missed time to the timer.
            }
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            SavePlantState(); // save when the app is paused.
    }

    private void OnApplicationQuit()
    {
        SavePlantState(); // save when the app closes.
    }

    public void HideReminderPopupFromSettings()
    {
        HideReminderPopupImmediate(); // hide the reminder from settings.
    }
}
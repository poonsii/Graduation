using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

// drives a single plant's calendar screen (e.g. PlantCalendarMonstera). One instance per plant type,
// same pattern as OwnedPlantCardUI - each screen knows which plantId it belongs to.
public class PlantCalendarController : MonoBehaviour
{
    [Header("Plant")]
    [SerializeField] private string plantId; // which plant this calendar screen belongs to.

    [Header("Screen")]
    [SerializeField] private GameObject screenRoot; // whole panel - shown/hidden as one.

    [Header("References")]
    [SerializeField] private GameBootstrap gameBootstrap;
    [SerializeField] private PlantDatabase plantDatabase;

    [Header("Month")]
    [SerializeField] private TMP_Text monthText; // "Month text" placeholder - current month name plus season tags.
    [SerializeField] private TMP_Text monthInformationText; // watering window + next watering estimate.
    [SerializeField] private TMP_Text seasonInfoText; // the separate "Month information text" box - short seasonal name + tips.

    [Header("Calendar Grid")]
    [SerializeField] private RectTransform calendarGridContainer; // empty rect over the grid area, needs a GridLayoutGroup.
    [SerializeField] private CalendarDayCell dayCellPrefab;

    [Header("Icons")]
    [SerializeField] private Button waterIconButton; // OnClick -> OnWaterIconPressed (wire up in the inspector).
    [SerializeField] private Button fertilizeIconButton; // OnClick -> OnFertilizeIconPressed (wire up in the inspector).

    [Header("Badges")]
    [SerializeField] private Image plantPlannerBadgeImage;
    [SerializeField] private Image weekStreakBadgeImage;

    [Header("Popups")]
    [SerializeField] private EarlyCareConfirmPanel earlyCareConfirmPanel;
    [SerializeField] private CalendarPointsNotificationUI pointsNotification;

    private const string DateFormat = "yyyy-MM-dd";

    private readonly List<CalendarDayCell> spawnedCells = new List<CalendarDayCell>();
    private SavedPlantState currentState;
    private PlantData currentPlantData;
    private Action onOnboardingStepFinished; // only set while this screen is shown as the onboarding step.

    public string GetPlantId() => plantId;

    private void OnEnable()
    {
        Refresh();
    }

    public void Open()
    {
        if (screenRoot != null)
            screenRoot.SetActive(true);

        Refresh();
    }

    public void OpenForOnboarding(string targetPlantId, Action onFinished)
    {
        plantId = targetPlantId;
        onOnboardingStepFinished = onFinished;
        Open();
    }

    public void OnBackPressed()
    {
        if (screenRoot != null)
            screenRoot.SetActive(false);

        if (onOnboardingStepFinished != null)
        {
            Action callback = onOnboardingStepFinished;
            onOnboardingStepFinished = null;
            callback.Invoke(); // let the onboarding flow know this step is done, whether anything got logged or not.
        }
    }

    private void Refresh()
    {
        if (gameBootstrap == null || plantDatabase == null || string.IsNullOrEmpty(plantId))
            return; // not fully wired up yet.

        currentState = gameBootstrap.GetSavedPlantStateForPlant(plantId);
        currentPlantData = plantDatabase.GetById(plantId);

        if (currentState == null || currentPlantData == null)
        {
            Debug.LogWarning("[Calendar] Missing saved state or plant data for plant id: " + plantId);
            return;
        }

        gameBootstrap.RefreshCalendarBadges(plantId); // catch up the streak even if nothing was logged just now, e.g. after being away for a while.

        DateTime today = CalendarClock.Now;

        RefreshMonthTexts(today);
        RefreshCalendarGrid(today);
        RefreshBadgeIcons();
    }

    private void RefreshMonthTexts(DateTime today)
    {
        CultureInfo culture = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.CultureInfo
            : CultureInfo.CurrentCulture;

        string monthName = today.ToString("MMMM", culture); // reuses the game's selected locale instead of needing separate month translations.

        bool humiditySeason = PlantSeasonAdvisor.IsHumiditySeason(currentPlantData, today.Month);
        bool fertilizingSeason = PlantSeasonAdvisor.IsFertilizingSeason(currentPlantData, today.Month);

        if (monthText != null)
        {
            string seasonSuffix = humiditySeason && fertilizingSeason ? LocalizedText.Get("calendar_season_suffix_both")
                : humiditySeason ? LocalizedText.Get("calendar_season_suffix_humidity")
                : fertilizingSeason ? LocalizedText.Get("calendar_season_suffix_fertilizing")
                : "";

            monthText.text = monthName + seasonSuffix;
        }

        if (monthInformationText != null)
        {
            string careWindow = LocalizedText.Get("calendar_care_window", currentPlantData.wateringMinDays, currentPlantData.wateringMaxDays);
            string estimate = BuildNextWateringEstimateText(today);

            monthInformationText.text = careWindow + "\n" + estimate;
        }

        if (seasonInfoText != null)
        {
            seasonInfoText.text = MonthlyCareAdvisor.GetSeasonName(today.Month)
                + "\n" + MonthlyCareAdvisor.GetShortTips(today.Month)
                + "\n" + BuildNextFertilizingEstimateText(today);
        }
    }

    private string BuildNextWateringEstimateText(DateTime today)
    {
        if (string.IsNullOrEmpty(currentState.lastWateredDate))
            return LocalizedText.Get("calendar_water_unknown"); // nothing logged yet, so no estimate to give.

        if (!DateTime.TryParseExact(currentState.lastWateredDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastWatered))
            return LocalizedText.Get("calendar_water_unknown");

        DateTime estimate = lastWatered.AddDays(currentPlantData.wateringMinDays); // earliest day of the ideal window.
        int daysUntil = (estimate.Date - today.Date).Days;

        if (daysUntil <= 0)
            return LocalizedText.Get("calendar_water_due_now");

        return LocalizedText.Get("calendar_water_estimate", daysUntil);
    }

    private string BuildNextFertilizingEstimateText(DateTime today)
    {
        if (string.IsNullOrEmpty(currentState.lastFertilizedDate))
            return LocalizedText.Get("calendar_fertilize_unknown"); // nothing logged yet, so no estimate to give.

        if (!DateTime.TryParseExact(currentState.lastFertilizedDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastFertilized))
            return LocalizedText.Get("calendar_fertilize_unknown");

        DateTime estimate = lastFertilized.AddDays(currentPlantData.fertilizingMinDays); // earliest day of the ideal window.
        int daysUntil = (estimate.Date - today.Date).Days;

        if (daysUntil <= 0)
            return LocalizedText.Get("calendar_fertilize_due_now");

        return LocalizedText.Get("calendar_fertilize_estimate", daysUntil);
    }

    private void RefreshCalendarGrid(DateTime today)
    {
        if (calendarGridContainer == null || dayCellPrefab == null)
            return; // grid area not wired up yet.

        foreach (CalendarDayCell cell in spawnedCells)
        {
            if (cell != null)
                Destroy(cell.gameObject);
        }

        spawnedCells.Clear();

        HashSet<int> wateredDays = new HashSet<int>();
        HashSet<int> fertilizedDays = new HashSet<int>();

        foreach (CalendarLogEntry entry in currentState.careLog)
        {
            if (!DateTime.TryParseExact(entry.date, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime entryDate))
                continue;

            if (entryDate.Year != today.Year || entryDate.Month != today.Month)
                continue; // only show logs from the month being displayed.

            if (entry.actionType == CareActionType.Watered)
                wateredDays.Add(entryDate.Day);
            else
                fertilizedDays.Add(entryDate.Day);
        }

        // blank filler cells so day 1 lines up under the correct weekday column (Mo..Sun).
        int firstWeekday = ((int)new DateTime(today.Year, today.Month, 1).DayOfWeek + 6) % 7;

        for (int i = 0; i < firstWeekday; i++)
        {
            CalendarDayCell filler = Instantiate(dayCellPrefab, calendarGridContainer);
            filler.SetupEmpty();
            spawnedCells.Add(filler);
        }

        int daysInMonth = DateTime.DaysInMonth(today.Year, today.Month);

        for (int day = 1; day <= daysInMonth; day++)
        {
            CalendarDayCell cell = Instantiate(dayCellPrefab, calendarGridContainer);
            cell.Setup(day, day == today.Day, wateredDays.Contains(day), fertilizedDays.Contains(day));
            spawnedCells.Add(cell);
        }
    }

    private void RefreshBadgeIcons()
    {
        SetBadgeVisual(plantPlannerBadgeImage, currentState.earnedBadgeIds.Contains(CalendarBadgeIds.PlantPlanner));
        SetBadgeVisual(weekStreakBadgeImage, currentState.earnedBadgeIds.Contains(CalendarBadgeIds.WeekStreak));
    }

    private void SetBadgeVisual(Image badgeImage, bool earned)
    {
        if (badgeImage == null)
            return;

        badgeImage.enabled = earned; // only show the badge once it's earned - leaves its own artwork colors untouched.
    }

    public void OnWaterIconPressed()
    {
        ToggleCare(CareActionType.Watered);
    }

    public void OnFertilizeIconPressed()
    {
        ToggleCare(CareActionType.Fertilized);
    }

    private void ToggleCare(CareActionType actionType)
    {
        if (currentState == null || currentPlantData == null)
            return;

        if (IsLoggedToday(actionType))
            UnlogCare(actionType); // pressed again - undo today's log instead of logging a second time.
        else
            TryLogCare(actionType);
    }

    private bool IsLoggedToday(CareActionType actionType)
    {
        string todayString = CalendarClock.Now.ToString(DateFormat, CultureInfo.InvariantCulture);
        return currentState.careLog.Exists(entry => entry.date == todayString && entry.actionType == actionType);
    }

    private void UnlogCare(CareActionType actionType)
    {
        gameBootstrap.UnlogPlantCare(plantId, actionType);
        Refresh();
    }

    private void TryLogCare(CareActionType actionType)
    {
        string lastDateString = actionType == CareActionType.Watered ? currentState.lastWateredDate : currentState.lastFertilizedDate;
        int minDays = actionType == CareActionType.Watered ? currentPlantData.wateringMinDays : currentPlantData.fertilizingMinDays;
        int maxDays = actionType == CareActionType.Watered ? currentPlantData.wateringMaxDays : currentPlantData.fertilizingMaxDays;

        int? daysSinceLastCare = null;

        if (!string.IsNullOrEmpty(lastDateString)
            && DateTime.TryParseExact(lastDateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastDate))
        {
            daysSinceLastCare = (CalendarClock.Now.Date - lastDate.Date).Days;
        }

        CareTimingResult timing = CarePointsCalculator.ClassifyTiming(daysSinceLastCare, minDays, maxDays);

        if (timing == CareTimingResult.Early && earlyCareConfirmPanel != null)
        {
            // ask the player before awarding points - logging early only earns full points if it was actually needed.
            earlyCareConfirmPanel.Show(actionType, wasNeeded => FinishLogCare(actionType, CarePointsCalculator.GetEarlyPoints(wasNeeded)));
            return;
        }

        int points = timing == CareTimingResult.Early
            ? CarePointsCalculator.GetEarlyPoints(true) // no confirm panel wired up - default to full points rather than blocking the action.
            : CarePointsCalculator.GetPoints(timing, daysSinceLastCare ?? 0, maxDays);

        FinishLogCare(actionType, points);
    }

    private void FinishLogCare(CareActionType actionType, int points)
    {
        CareLogResult result = gameBootstrap.LogPlantCare(plantId, actionType, points);

        Refresh();

        if (pointsNotification != null)
        {
            pointsNotification.ShowPoints(actionType, result.pointsAwarded); // reflects the 24-hour cooldown, not the theoretical amount.

            foreach (string badgeId in result.newlyEarnedBadges)
                pointsNotification.ShowBadgeEarned(badgeId);
        }
    }
}

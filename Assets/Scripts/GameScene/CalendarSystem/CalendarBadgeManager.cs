using System;
using System.Collections.Generic;
using System.Globalization;

// works out which badges a plant has earned. Called after every logged care action, and also
// whenever the calendar screen is opened, so a badge streak can still break even if the player
// never opens the app on the day it happens.
public static class CalendarBadgeManager
{
    private const string DateFormat = "yyyy-MM-dd";
    private const int StreakDaysNeeded = 7;

    public static List<string> RefreshBadges(SavedPlantState state, PlantData plant, DateTime today)
    {
        List<string> newlyEarned = new List<string>();

        if (state == null)
            return newlyEarned;

        if (state.careLog.Count > 0 && !state.earnedBadgeIds.Contains(CalendarBadgeIds.PlantPlanner))
        {
            state.earnedBadgeIds.Add(CalendarBadgeIds.PlantPlanner); // earned the first time anything gets logged.
            newlyEarned.Add(CalendarBadgeIds.PlantPlanner);
        }

        RefreshStreak(state, plant, today, newlyEarned);

        return newlyEarned;
    }

    private static void RefreshStreak(SavedPlantState state, PlantData plant, DateTime today, List<string> newlyEarned)
    {
        if (IsOverdueForCare(state, plant, today))
        {
            state.streakStartDate = ""; // the streak breaks as soon as care is overdue.
            return;
        }

        if (string.IsNullOrEmpty(state.streakStartDate))
            state.streakStartDate = today.ToString(DateFormat, CultureInfo.InvariantCulture); // streak just started.

        if (!DateTime.TryParseExact(state.streakStartDate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime streakStart))
            return;

        int streakDays = (today.Date - streakStart.Date).Days;

        if (streakDays >= StreakDaysNeeded && !state.earnedBadgeIds.Contains(CalendarBadgeIds.WeekStreak))
        {
            state.earnedBadgeIds.Add(CalendarBadgeIds.WeekStreak);
            newlyEarned.Add(CalendarBadgeIds.WeekStreak);
        }
    }

    public static bool IsOverdueForCare(SavedPlantState state, PlantData plant, DateTime today)
    {
        if (state == null || plant == null)
            return false;

        bool wateringOverdue = IsActionOverdue(state.lastWateredDate, plant.wateringMaxDays, today);

        bool fertilizingOverdue = PlantSeasonAdvisor.IsFertilizingSeason(plant, today.Month)
            && IsActionOverdue(state.lastFertilizedDate, plant.fertilizingMaxDays, today); // only counts outside of fertilizing season if it was never started.

        return wateringOverdue || fertilizingOverdue;
    }

    private static bool IsActionOverdue(string lastDateString, int maxDays, DateTime today)
    {
        if (string.IsNullOrEmpty(lastDateString))
            return false; // nothing logged yet, so nothing to be overdue on.

        if (!DateTime.TryParseExact(lastDateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastDate))
            return false;

        return (today.Date - lastDate.Date).Days > maxDays;
    }
}

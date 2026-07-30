using System;
using System.Globalization;

public enum PlantHealthStatus
{
    Good = 0,
    NeedsAttention = 1,
    Bad = 2
}

// a friendlier, 3-tier read on how overdue a plant's care is, for a status text on the card.
// separate from CalendarBadgeManager.IsOverdueForCare, which stays a simple yes/no for the
// streak badge and RoomPlant's healthy/unhealthy visual.
public static class PlantHealthAdvisor
{
    private const string DateFormat = "yyyy-MM-dd";
    private const int NeedsAttentionBufferDays = 3; // how many days overdue before it counts as "bad" instead of "needs attention".

    public static PlantHealthStatus GetStatus(SavedPlantState state, PlantData plant, DateTime today)
    {
        if (state == null || plant == null)
            return PlantHealthStatus.Good; // no data yet - nothing to worry about.

        int wateringDaysOverdue = DaysOverdue(state.lastWateredDate, plant.wateringMaxDays, today);

        int fertilizingDaysOverdue = PlantSeasonAdvisor.IsFertilizingSeason(plant, today.Month)
            ? DaysOverdue(state.lastFertilizedDate, plant.fertilizingMaxDays, today)
            : 0;

        int worstOverdue = Math.Max(wateringDaysOverdue, fertilizingDaysOverdue);

        if (worstOverdue <= 0)
            return PlantHealthStatus.Good;

        return worstOverdue > NeedsAttentionBufferDays ? PlantHealthStatus.Bad : PlantHealthStatus.NeedsAttention;
    }

    public static string GetLabel(PlantHealthStatus status)
    {
        switch (status)
        {
            case PlantHealthStatus.Good: return LocalizedText.Get("plant_health_good");
            case PlantHealthStatus.NeedsAttention: return LocalizedText.Get("plant_health_needs_attention");
            default: return LocalizedText.Get("plant_health_bad");
        }
    }

    private static int DaysOverdue(string lastDateString, int maxDays, DateTime today)
    {
        if (string.IsNullOrEmpty(lastDateString))
            return 0; // nothing logged yet - treated the same as being on schedule.

        if (!DateTime.TryParseExact(lastDateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastDate))
            return 0;

        return (today.Date - lastDate.Date).Days - maxDays; // positive = days past the window.
    }
}

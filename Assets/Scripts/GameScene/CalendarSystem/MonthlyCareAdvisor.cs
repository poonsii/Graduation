// short, generic month-by-month indoor plant care advice - not tied to a specific plant,
// same idea as a typical "month-by-month plant care" reference chart.
public static class MonthlyCareAdvisor
{
    public static string GetSeasonName(int month)
    {
        switch (month)
        {
            case 1: case 2: return "Winter rest";
            case 3: case 4: return "Spring wake-up";
            case 5: case 6: return "Active growth";
            case 7: case 8: return "Peak summer";
            case 9: case 10: return "Fall transition";
            case 11: case 12: return "Winter prep";
            default: return "";
        }
    }

    public static string GetShortTips(int month)
    {
        switch (month)
        {
            case 1: case 2: return "Water sparingly, skip fertilizer, maximize light.";
            case 3: case 4: return "Increase watering, start feeding, repot if needed.";
            case 5: case 6: return "Water more often, feed every 2-3 weeks.";
            case 7: case 8: return "Water frequently, keep feeding, watch for heat stress.";
            case 9: case 10: return "Reduce watering, taper fertilizer, watch for pests.";
            case 11: case 12: return "Water much less, stop fertilizer, boost humidity.";
            default: return "";
        }
    }
}

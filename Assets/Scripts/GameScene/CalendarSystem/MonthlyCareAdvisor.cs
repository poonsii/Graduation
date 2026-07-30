// short, generic month-by-month indoor plant care advice - not tied to a specific plant,
// same idea as a typical "month-by-month plant care" reference chart.
public static class MonthlyCareAdvisor
{
    public static string GetSeasonName(int month)
    {
        switch (month)
        {
            case 1: case 2: return LocalizedText.Get("calendar_season_name_winter_rest");
            case 3: case 4: return LocalizedText.Get("calendar_season_name_spring");
            case 5: case 6: return LocalizedText.Get("calendar_season_name_growth");
            case 7: case 8: return LocalizedText.Get("calendar_season_name_summer");
            case 9: case 10: return LocalizedText.Get("calendar_season_name_fall");
            case 11: case 12: return LocalizedText.Get("calendar_season_name_winter_prep");
            default: return "";
        }
    }

    public static string GetShortTips(int month)
    {
        switch (month)
        {
            case 1: case 2: return LocalizedText.Get("calendar_season_tips_winter_rest");
            case 3: case 4: return LocalizedText.Get("calendar_season_tips_spring");
            case 5: case 6: return LocalizedText.Get("calendar_season_tips_growth");
            case 7: case 8: return LocalizedText.Get("calendar_season_tips_summer");
            case 9: case 10: return LocalizedText.Get("calendar_season_tips_fall");
            case 11: case 12: return LocalizedText.Get("calendar_season_tips_winter_prep");
            default: return "";
        }
    }
}

using System;
using UnityEngine;

// tracks reward history in PlayerPrefs, separate from the resettable save file, so resetting
// plants can't be used to keep re-earning calendar points for the same care action or badge.
public static class CalendarRewardTracker
{
    public const int BadgeBonusPoints = 100; // one-time bonus for earning a badge, ever.
    private const int CooldownHours = 24; // how often watering/fertilizing points can be earned again.

    public static bool CanAwardCarePoints(string plantId, CareActionType actionType, DateTime now)
    {
        string key = CareTimeKey(plantId, actionType);

        if (!PlayerPrefs.HasKey(key))
            return true;

        if (!long.TryParse(PlayerPrefs.GetString(key), out long savedTicks))
            return true;

        DateTime lastAwarded = new DateTime(savedTicks);
        return (now - lastAwarded).TotalHours >= CooldownHours;
    }

    public static void MarkCarePointsAwarded(string plantId, CareActionType actionType, DateTime now)
    {
        PlayerPrefs.SetString(CareTimeKey(plantId, actionType), now.Ticks.ToString());
        PlayerPrefs.Save();
    }

    public static bool HasEverAwardedBadgeBonus(string plantId, string badgeId)
    {
        return PlayerPrefs.GetInt(BadgeBonusKey(plantId, badgeId), 0) == 1;
    }

    public static void MarkBadgeBonusAwarded(string plantId, string badgeId)
    {
        PlayerPrefs.SetInt(BadgeBonusKey(plantId, badgeId), 1);
        PlayerPrefs.Save();
    }

    private static string CareTimeKey(string plantId, CareActionType actionType)
    {
        return "calendar_reward_time_" + plantId + "_" + actionType;
    }

    private static string BadgeBonusKey(string plantId, string badgeId)
    {
        return "calendar_badge_bonus_" + plantId + "_" + badgeId;
    }
}

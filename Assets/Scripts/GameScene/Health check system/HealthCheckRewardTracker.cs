using System;
using UnityEngine;

// tracks health check point-earning history in PlayerPrefs, separate from the resettable save
// file, mirroring CalendarRewardTracker's 24-hour cooldown so resetting plants can't be used to
// keep re-earning health check points.
public static class HealthCheckRewardTracker
{
    public const int StartPoints = 10; // awarded as soon as the player begins a health check.
    public const int TaskPoints = 15; // awarded per finished (non-skipped) check.
    private const int CooldownHours = 24;

    public static bool CanAwardPoints(string plantId, DateTime now)
    {
        string key = TimeKey(plantId);

        if (!PlayerPrefs.HasKey(key))
            return true;

        if (!long.TryParse(PlayerPrefs.GetString(key), out long savedTicks))
            return true;

        DateTime lastAwarded = new DateTime(savedTicks);
        return (now - lastAwarded).TotalHours >= CooldownHours;
    }

    public static void MarkPointsAwarded(string plantId, DateTime now)
    {
        PlayerPrefs.SetString(TimeKey(plantId), now.Ticks.ToString());
        PlayerPrefs.Save();
    }

    private static string TimeKey(string plantId)
    {
        return "healthcheck_reward_time_" + plantId;
    }
}

using UnityEngine;

// works out how many points a logged watering/fertilizing counts for, based on how many days
// passed since the plant was last cared for.
public static class CarePointsCalculator
{
    public const int MaxPoints = 60; // full points - matches the reward RoomPlant already gives for healthy, on-time care.
    public const int MinPoints = 10; // points never drop below this floor.
    public const int PointsLostPerLateDay = 5;
    public const int EarlyDeclinedPoints = 20; // logged early and the player said it wasn't actually needed.

    public static CareTimingResult ClassifyTiming(int? daysSinceLastCare, int minDays, int maxDays)
    {
        if (daysSinceLastCare == null)
            return CareTimingResult.FirstTime; // nothing to compare against yet.

        int days = daysSinceLastCare.Value;

        int goodRangeStart = Mathf.Max(0, minDays - 1); // one day of buffer on both sides of the ideal window,
        int goodRangeEnd = maxDays + 1;                 // e.g. a 7-10 day window still counts as on time at 6-11 days.

        if (days < goodRangeStart)
            return CareTimingResult.Early;

        if (days > goodRangeEnd)
            return CareTimingResult.Late;

        return CareTimingResult.OnTime;
    }

    public static int GetPoints(CareTimingResult timing, int daysSinceLastCare, int maxDays)
    {
        switch (timing)
        {
            case CareTimingResult.FirstTime:
            case CareTimingResult.OnTime:
                return MaxPoints;

            case CareTimingResult.Late:
                int daysLate = daysSinceLastCare - (maxDays + 1);
                return Mathf.Max(MinPoints, MaxPoints - daysLate * PointsLostPerLateDay); // the later it is, the fewer points it earns.

            default:
                return MinPoints; // Early is resolved through GetEarlyPoints once the player answers the confirm popup.
        }
    }

    public static int GetEarlyPoints(bool wasNeeded)
    {
        return wasNeeded ? MaxPoints : EarlyDeclinedPoints; // full points if it really was needed, otherwise a reduced amount.
    }
}

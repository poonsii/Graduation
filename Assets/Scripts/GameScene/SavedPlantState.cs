using System;
using System.Collections.Generic;

[Serializable]
public class SavedPlantState
{
    public string uniquePlantInstanceId;
    public string plantId;
    public string lastCareTime;
    public bool isUnhealthy;

    public LightLocationType selectedLightLocation = LightLocationType.Unknown;
    public string selectedSpotId = ""; // which exact spot, since multiple spots can share the same LightLocationType.
    public LightAdviceResult lightAdviceResult = LightAdviceResult.Unknown;
    public bool playerAcceptedMismatch = false;

    public bool hasCompletedOnboarding = false;

    // calendar system - routine logging, points and badges.
    public bool hasCompletedCalendarIntro = false; // has the player seen (or skipped) the calendar onboarding step.
    public List<CalendarLogEntry> careLog = new List<CalendarLogEntry>();
    public string lastWateredDate = ""; // yyyy-MM-dd, empty if never logged.
    public string lastFertilizedDate = ""; // yyyy-MM-dd, empty if never logged.
    public string streakStartDate = ""; // yyyy-MM-dd - when the current unbroken care streak began, empty if there is none.
    public List<string> earnedBadgeIds = new List<string>();
}
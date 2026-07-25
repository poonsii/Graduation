using System;

[Serializable]
public class CalendarLogEntry
{
    public string date; // yyyy-MM-dd - the day this action was logged for.
    public CareActionType actionType;
    public int pointsEarned;
}

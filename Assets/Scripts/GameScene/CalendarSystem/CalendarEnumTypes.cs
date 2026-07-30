public enum CareActionType
{
    Watered = 0,
    Fertilized = 1
}

public enum CareTimingResult
{
    FirstTime = 0, // nothing logged before - always counts as good timing.
    Early = 1,
    OnTime = 2,
    Late = 3
}

using System;

// centralizes "what day is it" for the whole calendar system, using the Netherlands' timezone
// instead of trusting whatever timezone the device happens to be set to.
public static class CalendarClock
{
    private static readonly TimeZoneInfo AmsterdamTimeZone = ResolveAmsterdamTimeZone();

    public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, AmsterdamTimeZone);

    private static TimeZoneInfo ResolveAmsterdamTimeZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam"); } // Linux/macOS/Android/iOS
        catch (TimeZoneNotFoundException) { }

        try { return TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time"); } // Windows
        catch (TimeZoneNotFoundException) { }

        return TimeZoneInfo.Local; // last resort - falls back to the device's own timezone.
    }
}

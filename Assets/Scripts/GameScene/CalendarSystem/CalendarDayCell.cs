using TMPro;
using UnityEngine;

// one cell inside the calendar grid. Spawned at runtime by PlantCalendarController, one per day of the
// current month (plus a few empty filler cells so day 1 lines up under the correct weekday column).
public class CalendarDayCell : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text dayNumberText;
    [SerializeField] private GameObject todayHighlight; // shown only on the current day.
    [SerializeField] private GameObject wateredIcon; // small marker shown when this day was logged as watered.
    [SerializeField] private GameObject fertilizedIcon; // small marker shown when this day was logged as fertilized.

    public void Setup(int dayNumber, bool isToday, bool wasWatered, bool wasFertilized)
    {
        if (dayNumberText != null)
            dayNumberText.text = dayNumber.ToString();

        if (todayHighlight != null)
            todayHighlight.SetActive(isToday);

        if (wateredIcon != null)
            wateredIcon.SetActive(wasWatered);

        if (fertilizedIcon != null)
            fertilizedIcon.SetActive(wasFertilized);
    }

    public void SetupEmpty()
    {
        if (dayNumberText != null)
            dayNumberText.text = ""; // blank filler cell, used before day 1 to line the grid up with the weekday headers.

        if (todayHighlight != null)
            todayHighlight.SetActive(false);

        if (wateredIcon != null)
            wateredIcon.SetActive(false);

        if (fertilizedIcon != null)
            fertilizedIcon.SetActive(false);
    }
}

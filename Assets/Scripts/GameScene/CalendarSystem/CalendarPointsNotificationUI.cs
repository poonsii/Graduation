using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// small popup that tells the player how many points a logged care action earned, and shows a
// message when a badge gets earned. Messages are queued so a badge earned at the same time as a
// points message doesn't cut the first one off.
public class CalendarPointsNotificationUI : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float popupDuration = 3f;

    private readonly Queue<string> queuedMessages = new Queue<string>();
    private Coroutine popupCoroutine;

    public void ShowPoints(CareActionType actionType, int points)
    {
        // TODO: route through LocalizedText once translations are set up for the calendar system.
        string actionLabel = actionType == CareActionType.Watered ? "Watered" : "Fertilized";

        string message = points >= CarePointsCalculator.MaxPoints
            ? actionLabel + "! +" + points + " points - great timing."
            : actionLabel + ". +" + points + " points - try to log closer to the ideal window next time."; // nudges the player towards logging closer to the ideal window next time.

        QueueMessage(message);
    }

    public void ShowBadgeEarned(string badgeId)
    {
        string badgeName = badgeId == CalendarBadgeIds.WeekStreak ? "1 Week Streak" : "Plant Planner";

        QueueMessage("Badge earned: " + badgeName + "!");
    }

    private void QueueMessage(string message)
    {
        if (popupRoot == null)
            return;

        queuedMessages.Enqueue(message);

        if (popupCoroutine == null)
            popupCoroutine = StartCoroutine(PopupRoutine());
    }

    private IEnumerator PopupRoutine()
    {
        while (queuedMessages.Count > 0)
        {
            string message = queuedMessages.Dequeue();

            if (messageText != null)
                messageText.text = message;

            popupRoot.SetActive(true);

            yield return new WaitForSeconds(popupDuration);
        }

        popupRoot.SetActive(false);
        popupCoroutine = null;
    }
}

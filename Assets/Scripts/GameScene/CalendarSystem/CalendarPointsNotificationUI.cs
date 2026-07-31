using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// small popup that shows "+N points" when a care action is logged, and a short message when a
// badge gets earned. Pops in, holds, then fades out. Messages are queued so a badge earned at
// the same time as a points message doesn't cut the first one off.
public class CalendarPointsNotificationUI : MonoBehaviour
{
    [Header("Popup")]
    [SerializeField] private GameObject popupRoot;
    [SerializeField] private CanvasGroup popupCanvasGroup; // put this on the same object as popupRoot - controls the fade.
    [SerializeField] private TMP_Text messageText;

    [Header("Timing")]
    [SerializeField] private float popDuration = 0.15f; // quick scale-in when it appears.
    [SerializeField] private float holdDuration = 1f; // fully visible for this long.
    [SerializeField] private float fadeDuration = 0.6f; // then fades out over this long.

    private readonly Queue<string> queuedMessages = new Queue<string>();
    private Coroutine popupCoroutine;

    public void ShowPoints(CareActionType actionType, int points)
    {
        ShowPoints(points);
    }

    public void ShowPoints(int points)
    {
        string sign = points >= 0 ? "+" : ""; // an unlog (or a health check retraction) can pass a negative amount and still read correctly.
        QueueMessage(LocalizedText.Get("calendar_points_popup", sign + points));
    }

    public void ShowBadgeEarned(string badgeId)
    {
        string badgeName = badgeId == CalendarBadgeIds.WeekStreak
            ? LocalizedText.Get("calendar_badge_name_week_streak")
            : LocalizedText.Get("calendar_badge_name_plant_planner");

        QueueMessage(LocalizedText.Get("calendar_badge_earned_popup", badgeName));
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

            yield return PopIn();
            yield return new WaitForSeconds(holdDuration);
            yield return FadeOut();
        }

        popupRoot.SetActive(false);
        popupCoroutine = null;
    }

    private IEnumerator PopIn()
    {
        if (popupCanvasGroup != null)
            popupCanvasGroup.alpha = 1f;

        Transform popupTransform = popupRoot.transform;
        float elapsed = 0f;

        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(0.7f, 1f, elapsed / popDuration);
            popupTransform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }

        popupTransform.localScale = Vector3.one;
    }

    private IEnumerator FadeOut()
    {
        if (popupCanvasGroup == null)
            yield break; // no canvas group wired up - skip straight to hiding, no fade.

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            popupCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            yield return null;
        }

        popupCanvasGroup.alpha = 0f;
    }
}

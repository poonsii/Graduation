using System;
using TMPro;
using UnityEngine;

// asks the player "was this actually needed?" when they log care earlier than the plant's usual
// window. Answering yes still earns full points, answering no earns a reduced amount.
public class EarlyCareConfirmPanel : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text questionText;

    private Action<bool> onAnswered;

    public void Show(CareActionType actionType, Action<bool> onAnswerGiven)
    {
        onAnswered = onAnswerGiven;

        if (questionText != null)
        {
            string actionLabel = actionType == CareActionType.Watered
                ? LocalizedText.Get("calendar_action_watered")
                : LocalizedText.Get("calendar_action_fertilized");

            questionText.text = LocalizedText.Get("calendar_early_confirm_question", actionLabel);
        }

        if (root != null)
            root.SetActive(true);
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }

    public void OnYesPressed()
    {
        Answer(true);
    }

    public void OnNoPressed()
    {
        Answer(false);
    }

    private void Answer(bool wasNeeded)
    {
        Hide();

        Action<bool> callback = onAnswered;
        onAnswered = null;
        callback?.Invoke(wasNeeded);
    }
}

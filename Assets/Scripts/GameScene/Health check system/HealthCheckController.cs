using System;
using TMPro;
using UnityEngine;

// drives a single plant's health check flow (e.g. HealthCheckMonstera). One instance per plant
// type, same pattern as PlantCalendarController. Walks the player through an intro, then a leaf,
// pest and root check - each shown as a 3D model with an inspect prompt, a yes/no question and an
// advice screen. Every step can be skipped.
public class HealthCheckController : MonoBehaviour
{
    [Header("Plant")]
    [SerializeField] private string plantId;

    [Header("References")]
    [SerializeField] private PlantDatabase plantDatabase;
    [SerializeField] private HealthCheckCameraMover cameraMover;
    [SerializeField] private Transform cameraStageSpot; // where the camera moves to for this plant's health check.
    [SerializeField] private CalendarPointsNotificationUI pointsNotification;

    [Header("Screen")]
    [SerializeField] private GameObject screenRoot; // whole health check panel - shown/hidden as one.
    [SerializeField] private GameObject backgroundRoot; // the camera-space background canvas behind the 3D model - shown/hidden alongside the screen.
    [SerializeField] private GameObject[] otherUiToHide; // persistent UI (top bar, bottom plant switcher, etc.) that isn't covered by the background anymore and needs hiding explicitly while a check is open.

    [Header("Intro")]
    [SerializeField] private GameObject introRoot;
    [SerializeField] private TMP_Text introText;

    [Header("Leaf Check")]
    [SerializeField] private GameObject leafCheckRoot;
    [SerializeField] private GameObject leafCheckModel; //MDL_MonsteraLeafCheck.
    [SerializeField] private TMP_Text leafInstructionText;
    [SerializeField] private TMP_Text leafQuestionText;
    [SerializeField] private GameObject leafAdviceRoot;
    [SerializeField] private TMP_Text leafAdviceText;

    [Header("Pest Check")]
    [SerializeField] private GameObject pestCheckRoot;
    [SerializeField] private GameObject pestCheckModel; // MDL_MonsteraPestsCheck.
    [SerializeField] private TMP_Text pestInstructionText;
    [SerializeField] private TMP_Text pestQuestionText;
    [SerializeField] private GameObject pestAdviceRoot;
    [SerializeField] private TMP_Text pestAdviceText;

    [Header("Root Check")]
    [SerializeField] private GameObject rootCheckRoot;
    [SerializeField] private GameObject rootCheckModel; // e.g. MDL_MonsteraRootCheck.
    [SerializeField] private TMP_Text rootInstructionText;
    [SerializeField] private TMP_Text rootQuestionText;
    [SerializeField] private GameObject rootAdviceRoot;
    [SerializeField] private TMP_Text rootAdviceText;

    private bool pointsEnabledThisRun; // false while the 24-hour cooldown from a previous run is still active.
    private bool startPointsAwarded;
    private bool anyTaskCompleted; // true once at least one check was answered instead of skipped.
    private DateTime sessionNow;
    private bool[] otherUiPreviousStates; // each entry's own state right before it was hidden, so closing restores it instead of forcing everything back on.

    public string GetPlantId() => plantId;

    public void Open()
    {
        sessionNow = CalendarClock.Now;
        pointsEnabledThisRun = HealthCheckRewardTracker.CanAwardPoints(plantId, sessionNow);
        startPointsAwarded = false;
        anyTaskCompleted = false;

        if (introText != null)
            introText.text = LocalizedText.Get("healthcheck_intro_text", ResolvePlantName());

        if (cameraMover != null && cameraStageSpot != null)
            cameraMover.MoveTo(cameraStageSpot);

        if (screenRoot != null)
            screenRoot.SetActive(true);

        if (backgroundRoot != null)
            backgroundRoot.SetActive(true);

        HideOtherUi();

        ShowOnly(introRoot);
    }

    public void OnStartPressed() // "Yes" button on the intro screen.
    {
        if (pointsEnabledThisRun)
        {
            AwardPoints(HealthCheckRewardTracker.StartPoints);
            startPointsAwarded = true;
        }

        ShowLeafCheck();
    }

    public void OnSkipHealthCheckPressed() // "Skip for now" on the intro screen - leaves without starting.
    {
        CloseScreen();
    }

    private void ShowLeafCheck()
    {
        if (leafInstructionText != null)
            leafInstructionText.text = LocalizedText.Get("healthcheck_leaf_instruction");

        if (leafQuestionText != null)
            leafQuestionText.text = LocalizedText.Get("healthcheck_leaf_question");

        ShowOnly(leafCheckRoot);
    }

    public void OnLeafSkipPressed() => ShowPestCheck();
    public void OnLeafYesPressed() => AnswerLeaf(true);
    public void OnLeafNoPressed() => AnswerLeaf(false);
    public void OnLeafAdviceContinuePressed() => ShowPestCheck();

    private void AnswerLeaf(bool hasSymptom)
    {
        MarkTaskCompleted();
        ShowAdvice(leafAdviceRoot, leafAdviceText, hasSymptom ? "healthcheck_leaf_advice_yes" : "healthcheck_leaf_advice_no");
    }

    private void ShowPestCheck()
    {
        if (pestInstructionText != null)
            pestInstructionText.text = LocalizedText.Get("healthcheck_pest_instruction");

        if (pestQuestionText != null)
            pestQuestionText.text = LocalizedText.Get("healthcheck_pest_question");

        ShowOnly(pestCheckRoot);
    }

    public void OnPestSkipPressed() => ShowRootCheck();
    public void OnPestYesPressed() => AnswerPest(true);
    public void OnPestNoPressed() => AnswerPest(false);
    public void OnPestAdviceContinuePressed() => ShowRootCheck();

    private void AnswerPest(bool hasPests)
    {
        MarkTaskCompleted();
        ShowAdvice(pestAdviceRoot, pestAdviceText, hasPests ? "healthcheck_pest_advice_yes" : "healthcheck_pest_advice_no");
    }

    private void ShowRootCheck()
    {
        if (rootInstructionText != null)
            rootInstructionText.text = LocalizedText.Get("healthcheck_root_instruction");

        if (rootQuestionText != null)
            rootQuestionText.text = LocalizedText.Get("healthcheck_root_question");

        ShowOnly(rootCheckRoot);
    }

    public void OnRootSkipPressed() => Finish();
    public void OnRootYesPressed() => AnswerRoot(true);
    public void OnRootNoPressed() => AnswerRoot(false);
    public void OnRootAdviceContinuePressed() => Finish();

    private void AnswerRoot(bool rootsGrowingOut)
    {
        MarkTaskCompleted();
        ShowAdvice(rootAdviceRoot, rootAdviceText, rootsGrowingOut ? "healthcheck_root_advice_yes" : "healthcheck_root_advice_no");
    }

    private void MarkTaskCompleted()
    {
        anyTaskCompleted = true;

        if (pointsEnabledThisRun)
            AwardPoints(HealthCheckRewardTracker.TaskPoints);
    }

    private void ShowAdvice(GameObject adviceRoot, TMP_Text adviceText, string localizationKey)
    {
        if (adviceText != null)
            adviceText.text = LocalizedText.Get(localizationKey);

        ShowOnly(adviceRoot);
    }

    private void AwardPoints(int amount)
    {
        if (PointsManager.Instance != null)
            PointsManager.Instance.AddPoints(amount);

        if (pointsNotification != null)
            pointsNotification.ShowPoints(amount);
    }

    private void Finish()
    {
        if (pointsEnabledThisRun)
        {
            if (anyTaskCompleted)
                HealthCheckRewardTracker.MarkPointsAwarded(plantId, sessionNow); // start the 24-hour cooldown now that points were actually kept.
            else if (startPointsAwarded)
                AwardPoints(-HealthCheckRewardTracker.StartPoints); // every task was skipped - take the starting points back.
        }

        CloseScreen();
    }

    private void CloseScreen()
    {
        if (screenRoot != null)
            screenRoot.SetActive(false);

        if (backgroundRoot != null)
            backgroundRoot.SetActive(false);

        RestoreOtherUi();

        if (cameraMover != null)
            cameraMover.MoveToOriginalPose();
    }

    private void HideOtherUi()
    {
        if (otherUiToHide == null)
            return;

        otherUiPreviousStates = new bool[otherUiToHide.Length];

        for (int i = 0; i < otherUiToHide.Length; i++)
        {
            if (otherUiToHide[i] == null)
                continue;

            otherUiPreviousStates[i] = otherUiToHide[i].activeSelf; // remember what it was before hiding it.
            otherUiToHide[i].SetActive(false);
        }
    }

    private void RestoreOtherUi()
    {
        if (otherUiToHide == null || otherUiPreviousStates == null)
            return;

        for (int i = 0; i < otherUiToHide.Length; i++)
        {
            if (otherUiToHide[i] != null)
                otherUiToHide[i].SetActive(otherUiPreviousStates[i]); // back to whatever it was before, not forced on.
        }
    }

    // shows exactly one of this plant's health check screens (and its matching 3D model), hides the rest.
    private void ShowOnly(GameObject target)
    {
        SetActive(introRoot, introRoot == target);
        SetActive(leafCheckRoot, leafCheckRoot == target);
        SetActive(leafAdviceRoot, leafAdviceRoot == target);
        SetActive(pestCheckRoot, pestCheckRoot == target);
        SetActive(pestAdviceRoot, pestAdviceRoot == target);
        SetActive(rootCheckRoot, rootCheckRoot == target);
        SetActive(rootAdviceRoot, rootAdviceRoot == target);

        SetActive(leafCheckModel, target == leafCheckRoot);
        SetActive(pestCheckModel, target == pestCheckRoot);
        SetActive(rootCheckModel, target == rootCheckRoot);
    }

    private static void SetActive(GameObject target, bool active)
    {
        if (target != null)
            target.SetActive(active);
    }

    private string ResolvePlantName()
    {
        PlantData plant = plantDatabase != null ? plantDatabase.GetById(plantId) : null;
        return plant != null ? plant.displayName : plantId;
    }
}

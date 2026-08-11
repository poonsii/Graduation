using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PlayerProfileManager profileManager;

    private Transform screens;
    private int currentScreenIndex;
    private bool isShowingTutorial;

    private void Awake()
    {
        screens = transform.Find("TutorialScreens");
    }

    private void Start()
    {
        WireSkipButton();

        PlayerData data = profileManager != null ? profileManager.Load() : null;

        if (data != null && data.hasSeenTutorial)
        {
            gameObject.SetActive(false);
            return;
        }

        ShowScreen(0);
    }

    private void WireSkipButton()
    {
        if (screens == null || screens.childCount == 0)
            return;

        Transform skip = screens.GetChild(0).Find("Skip for now");
        Button skipButton = skip != null ? skip.GetComponent<Button>() : null;

        if (skipButton != null)
            skipButton.onClick.AddListener(SkipTutorial);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isShowingTutorial)
            AdvanceScreen();
    }

    private void AdvanceScreen()
    {
        ShowScreen(currentScreenIndex + 1);
    }

    private void ShowScreen(int index)
    {
        if (screens == null || screens.childCount == 0)
            return;

        if (index >= screens.childCount)
        {
            CompleteTutorial();
            return;
        }

        for (int i = 0; i < screens.childCount; i++)
            screens.GetChild(i).gameObject.SetActive(i == index);

        currentScreenIndex = index;
        isShowingTutorial = true;
        gameObject.SetActive(true);
    }

    public void SkipTutorial()
    {
        CompleteTutorial();
    }

    public void ReplayTutorial()
    {
        ShowScreen(0);
    }

    private void CompleteTutorial()
    {
        isShowingTutorial = false;
        gameObject.SetActive(false);
        MarkTutorialSeen();
    }

    private void MarkTutorialSeen()
    {
        if (profileManager == null)
            return;

        PlayerData data = profileManager.Load();
        if (data == null)
            data = new PlayerData();

        data.hasSeenTutorial = true;
        profileManager.Save(data);
    }
}

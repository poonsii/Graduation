using UnityEngine;
using TMPro;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance;

    [Header("Current Points")]
    [SerializeField] private int points = 0;

    [Header("UI Text")]
    [SerializeField] private TMP_Text[] pointTexts;

    private const string PointsKey = "Points";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadPoints();
    }

    public void RegisterUI(TMP_Text[] newPointTexts)
    {
        pointTexts = newPointTexts;
        Debug.Log("RegisterUI on manager: " + gameObject.name + " | refs = " + pointTexts.Length, this);
        UpdateUI();
    }

    public void AddPlantCareReward(bool wasUnhealthy)
    {
        points += wasUnhealthy ? 15 : 60;
        SavePoints();
        Debug.Log("AddPlantCareReward on manager: " + gameObject.name + " | points = " + points, this);
        UpdateUI();
    }

    public void AddPoints(int amount)
    {
        points += amount;
        SavePoints();
        UpdateUI();
    }

    public void ResetPoints()
    {
        points = 0;
        SavePoints();
        UpdateUI();
    }

    public int GetPoints()
    {
        return points;
    }

    public void UpdateUI()
    {
        if (pointTexts == null || pointTexts.Length == 0)
        {
            Debug.Log("UpdateUI skipped, no point text references registered.", this);
            return;
        }

        string value = points + " points";

        foreach (TMP_Text text in pointTexts)
        {
            if (text != null)
            {
                text.text = value;
                text.ForceMeshUpdate();
                Debug.Log("Updated text: " + text.gameObject.name + " -> " + value, text);
            }
        }
    }

    private void SavePoints()
    {
        PlayerPrefs.SetInt(PointsKey, points);
        PlayerPrefs.Save();
    }

    private void LoadPoints()
    {
        points = PlayerPrefs.GetInt(PointsKey, 0);
    }
}
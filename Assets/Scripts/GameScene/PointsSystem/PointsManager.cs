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
            Destroy(gameObject); // destroy duplicate managers.
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // keep this manager alive between scenes.
        LoadPoints(); // load saved points on startup.
    }

    public void RegisterUI(TMP_Text[] newPointTexts) //UI text
    {
        pointTexts = newPointTexts; 
        Debug.Log("RegisterUI on manager: " + gameObject.name + " | refs = " + pointTexts.Length, this);
        UpdateUI(); // immediately update the text with the current points.
    }

    public void AddPlantCareReward(bool wasUnhealthy)
    {
        points += wasUnhealthy ? 15 : 60; // give more points if the plant was already unhealthy.
        SavePoints();
        Debug.Log("AddPlantCareReward on manager: " + gameObject.name + " | points = " + points, this);
        UpdateUI();
    }

    public void AddPoints(int amount)
    {
        points += amount; // add the given amount of points.
        SavePoints();
        UpdateUI();
    }

    public bool TrySpendPoints(int amount) // money spending
    {
        if (points < amount)
            return false; 

        points -= amount; // subtract the points.
        SavePoints();
        UpdateUI();
        return true;
    }

    public void ResetPoints() // set points back to zero.
    {
        points = 0; 
        SavePoints();
        UpdateUI();
    }

    public int GetPoints()
    {
        return points; // return the current point total.
    }

    public void UpdateUI()
    {
        if (pointTexts == null || pointTexts.Length == 0)
        {
            Debug.Log("UpdateUI skipped, no point text references registered.", this);
            return; // stop if there are no text objects to update.
        }

        string value = points + " points"; // format the display text.

        foreach (TMP_Text text in pointTexts)
        {
            if (text != null)
            {
                text.text = value; 
                text.ForceMeshUpdate(); // force the text to redraw.
                Debug.Log("Updated text: " + text.gameObject.name + " -> " + value, text);
            }
        }
    }

    private void SavePoints()
    {
        PlayerPrefs.SetInt(PointsKey, points); // save points to device storage.
        PlayerPrefs.Save();
    }

    private void LoadPoints()
    {
        points = PlayerPrefs.GetInt(PointsKey, 0); // load saved points, default to 0 if none found.
    }
}
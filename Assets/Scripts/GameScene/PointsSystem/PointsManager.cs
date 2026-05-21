using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PointsManager : MonoBehaviour
{
    public static PointsManager Instance;

    [Header("Current Points")]
    [SerializeField] private int leafPoints = 0;
    [SerializeField] private int decorationPoints = 0;

    [Header("UI Text")]
    [SerializeField] private TMP_Text[] leafTexts;
    [SerializeField] private TMP_Text[] decorationTexts;

    private const string LeafKey = "LeafPoints";
    private const string DecorationKey = "DecorationPoints";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPoints();
            Debug.Log("PointsManager created.");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);
        UpdateUI();
    }

    public void RegisterUI(TMP_Text[] newLeafTexts, TMP_Text[] newDecorationTexts)
    {
        leafTexts = newLeafTexts;
        decorationTexts = newDecorationTexts;

        Debug.Log("PointsManager RegisterUI called. Leaf texts: " +
                  (leafTexts != null ? leafTexts.Length : 0) +
                  ", Decoration texts: " +
                  (decorationTexts != null ? decorationTexts.Length : 0));

        UpdateUI();
    }

    public void AddPlantCareReward(bool wasUnhealthy)
    {
        if (wasUnhealthy)
        {
            leafPoints += 5;
            decorationPoints += 10;
        }
        else
        {
            leafPoints += 10;
            decorationPoints += 50;
        }

        SavePoints();
        UpdateUI();
    }

    public void UpdateUI()
    {
        Debug.Log("UpdateUI called. Leafs: " + leafPoints + ", Decoration: " + decorationPoints);

        if (leafTexts != null)
        {
            foreach (TMP_Text text in leafTexts)
            {
                if (text != null)
                    text.text = leafPoints + " leafs";
            }
        }

        if (decorationTexts != null)
        {
            foreach (TMP_Text text in decorationTexts)
            {
                if (text != null)
                    text.text = decorationPoints + " points";
            }
        }
    }

    private void SavePoints()
    {
        PlayerPrefs.SetInt(LeafKey, leafPoints);
        PlayerPrefs.SetInt(DecorationKey, decorationPoints);
        PlayerPrefs.Save();
    }

    private void LoadPoints()
    {
        leafPoints = PlayerPrefs.GetInt(LeafKey, 0);
        decorationPoints = PlayerPrefs.GetInt(DecorationKey, 0);
    }

    public void ResetPoints()
    {
        leafPoints = 0;
        decorationPoints = 0;

        PlayerPrefs.SetInt(LeafKey, 0);
        PlayerPrefs.SetInt(DecorationKey, 0);
        PlayerPrefs.Save();

        Debug.Log("Points reset.");

        UpdateUI();
    }
}
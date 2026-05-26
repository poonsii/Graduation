using UnityEngine;
using TMPro;

public class PointsUIBinder : MonoBehaviour
{
    private TMP_Text pointsText;

    private void Awake()
    {
        pointsText = GetComponent<TMP_Text>();

        if (pointsText == null)
        {
            Debug.LogError("PointsUIBinder must be on the same object as the TMP_Text.", this);
        }
    }

    private void Start()
    {
        Register();
    }

    public void Register()
    {
        if (PointsManager.Instance != null && pointsText != null)
        {
            PointsManager.Instance.RegisterUI(new TMP_Text[] { pointsText });
            Debug.Log("Registered points text in Start: " + pointsText.gameObject.name, this);
        }
        else
        {
            Debug.LogWarning("PointsManager or pointsText missing in Register().", this);
        }
    }
}
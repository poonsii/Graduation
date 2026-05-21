using UnityEngine;
using TMPro;

public class PointsUIBinder : MonoBehaviour
{
    [SerializeField] private TMP_Text[] leafTexts;
    [SerializeField] private TMP_Text[] decorationTexts;

    private void OnEnable()
    {
        if (PointsManager.Instance != null)
        {
            PointsManager.Instance.RegisterUI(leafTexts, decorationTexts);
            Debug.Log("GameScenePointsUI registered UI.");
        }
        else
        {
            Debug.LogWarning("PointsManager.Instance is null.");
        }
    }
}
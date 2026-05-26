using UnityEngine;

public class GameSceneActions : MonoBehaviour
{
    public void ResetPoints()
    {
        if (PointsManager.Instance != null)
            PointsManager.Instance.ResetPoints();
    }

    public void AddTestPoints()
    {
        if (PointsManager.Instance != null)
            PointsManager.Instance.AddPoints(10);
    }
}
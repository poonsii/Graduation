using UnityEngine;

public class GameSceneActions : MonoBehaviour
{
    public void ResetPoints()
    {
        if (PointsManager.Instance != null)
            PointsManager.Instance.ResetPoints(); // reset all points through the manager.
    }

    public void AddTestPoints()
    {
        if (PointsManager.Instance != null)
            PointsManager.Instance.AddPoints(10); // add 10 test point.
    }
}
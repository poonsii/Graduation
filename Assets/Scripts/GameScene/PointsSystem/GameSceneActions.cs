using UnityEngine;

public class GameSceneActions : MonoBehaviour
{
    public void ResetPoints()
    {
        if (PointsManager.Instance != null)
        {
            PointsManager.Instance.ResetPoints();
        }
    }
}
using UnityEngine;

public class OptionsActions : MonoBehaviour
{
    public void ResetPoints()
    {
        if (PointsManager.Instance != null)
        {
            PointsManager.Instance.ResetPoints(); // reset all points from the options menu.
        }
    }
}
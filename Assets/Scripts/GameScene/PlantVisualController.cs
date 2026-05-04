using UnityEngine;

public class PlantVisualController : MonoBehaviour
{
    [SerializeField] private GameObject healthyModel;
    [SerializeField] private GameObject unhealthyModel;

    public void SetHealthy(bool isHealthy)
    {
        healthyModel.SetActive(isHealthy);
        unhealthyModel.SetActive(!isHealthy);
    }
}
using UnityEngine;

public class PlantVisualController : MonoBehaviour
{
    [SerializeField] private GameObject healthyRoot;
    [SerializeField] private GameObject unhealthyRoot;

    public void SetHealthy(bool isHealthy)
    {
        if (healthyRoot != null)
            healthyRoot.SetActive(isHealthy);

        if (unhealthyRoot != null)
            unhealthyRoot.SetActive(!isHealthy);
    }
}
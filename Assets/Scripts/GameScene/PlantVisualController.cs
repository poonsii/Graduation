using UnityEngine;

public class PlantVisualController : MonoBehaviour
{
    [SerializeField] private GameObject healthyRoot; // healthy version of the plant
    [SerializeField] private GameObject unhealthyRoot; // unhealthy version of the plant

    public void SetHealthy(bool isHealthy)
    {
        if (healthyRoot != null)
            healthyRoot.SetActive(isHealthy); // show healthy version

        if (unhealthyRoot != null)
            unhealthyRoot.SetActive(!isHealthy); // show unhealthy version
    }
}
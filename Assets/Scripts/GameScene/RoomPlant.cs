using UnityEngine;

public class RoomPlant : MonoBehaviour
{
    [SerializeField] private PlantVisualController visualController;
    [SerializeField] private float unhealthyAfterSeconds = 60f;

    private float neglectTimer = 0f;
    private bool isUnhealthy = false;

    private void Update()
    {
        neglectTimer += Time.deltaTime;

        if (neglectTimer >= unhealthyAfterSeconds)
        {
            isUnhealthy = true;
        }

        visualController.SetHealthy(!isUnhealthy);
    }

    public void CareForPlant()
    {
        neglectTimer = 0f;
        isUnhealthy = false;
        visualController.SetHealthy(true);

        Debug.Log("Plant is healthy again.");
    }
}
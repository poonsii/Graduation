using UnityEngine;

public class PlacementManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private float gridSize = 1f;

    private FurnitureData selectedFurniture;
    private GameObject previewObject;

    public void StartPlacement(FurnitureData furniture)
    {
        CancelPlacement();

        selectedFurniture = furniture;
        previewObject = Instantiate(furniture.prefab);
    }

    private void Update()
    {
        if (previewObject == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, floorLayer))
        {
            Vector3 snappedPosition = SnapToGrid(hit.point);
            previewObject.transform.position = snappedPosition;

            if (Input.GetMouseButtonDown(0))
            {
                PlaceFurniture(snappedPosition);
            }

            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
    }

    private Vector3 SnapToGrid(Vector3 position)
    {
        float x = Mathf.Round(position.x / gridSize) * gridSize;
        float z = Mathf.Round(position.z / gridSize) * gridSize;
        return new Vector3(x, 0f, z);
    }

    private void PlaceFurniture(Vector3 position)
    {
        Instantiate(selectedFurniture.prefab, position, Quaternion.identity);
        CancelPlacement();
    }

    public void CancelPlacement()
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = null;
        selectedFurniture = null;
    }
}
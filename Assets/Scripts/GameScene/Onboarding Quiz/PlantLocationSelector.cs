using UnityEngine;
using UnityEngine.EventSystems;

public class PlantLocationSelector : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask locationLayerMask;
    [SerializeField] private PlantLocationConfirmPanel confirmPanel;

    private PlantLocationSpot currentSelectedSpot;
    private string currentPlantInstanceId;
    private string currentPlantId;
    private bool selectionActive = false;

    public void BeginSelection(string uniquePlantInstanceId, string plantId)
    {
        currentPlantInstanceId = uniquePlantInstanceId;
        currentPlantId = plantId;
        selectionActive = true;

        if (confirmPanel != null)
            confirmPanel.Hide();

        ClearCurrentSelection();
    }

    private void Update()
    {
        if (!selectionActive)
            return;

#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            TrySelectAtScreenPosition(Input.mousePosition);
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    return;

                TrySelectAtScreenPosition(touch.position);
            }
        }
#endif
    }

    private void TrySelectAtScreenPosition(Vector2 screenPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, locationLayerMask))
        {
            PlantLocationSpot spot = hit.collider.GetComponent<PlantLocationSpot>();
            if (spot != null)
            {
                SelectSpot(spot);
            }
        }
    }

    private void SelectSpot(PlantLocationSpot spot)
    {
        if (currentSelectedSpot != null)
            currentSelectedSpot.SetSelected(false);

        currentSelectedSpot = spot;
        currentSelectedSpot.SetSelected(true);

        if (confirmPanel != null)
        {
            confirmPanel.Show(
                currentPlantInstanceId,
                currentPlantId,
                currentSelectedSpot
            );
        }
    }

    public void EnableSelectionAgain()
    {
        selectionActive = true;

        if (confirmPanel != null)
            confirmPanel.Hide();
    }

    public void FinishSelection()
    {
        selectionActive = false;
    }

    private void ClearCurrentSelection()
    {
        if (currentSelectedSpot != null)
        {
            currentSelectedSpot.SetSelected(false);
            currentSelectedSpot = null;
        }
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlantLocationSelector : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask locationLayerMask;
    [SerializeField] private PlantLocationConfirmPanel confirmPanel;

    [Header("Choose-a-location Prompt")]
    [SerializeField] private GameObject promptRoot;
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private PlantDatabase plantDatabase;

    private PlantLocationSpot currentSelectedSpot;
    private string currentPlantInstanceId;
    private string currentPlantId;
    private bool selectionActive = false;
    private System.Action<string, string> onSelectionFinished;

    public void BeginSelection(string uniquePlantInstanceId, string plantId, System.Action<string, string> onFinished = null)
    {
        currentPlantInstanceId = uniquePlantInstanceId;
        currentPlantId = plantId;
        onSelectionFinished = onFinished;
        selectionActive = true;

        Debug.Log("[Onboarding] Selection is now active for '" + plantId + "'. Tap/click one of the location spots in the room. Main camera assigned: " + (mainCamera != null) + ", layer mask value: " + locationLayerMask.value);

        if (confirmPanel != null)
            confirmPanel.Hide();

        ShowPrompt(plantId);
        ClearCurrentSelection();
    }

    private void ShowPrompt(string plantId)
    {
        if (promptRoot == null)
            return;

        if (promptText != null)
        {
            PlantData plant = plantDatabase != null ? plantDatabase.GetById(plantId) : null;
            string plantName = plant != null ? plant.displayName : plantId;
            promptText.text = $"Choose a location in the room for your {plantName}.";
        }

        promptRoot.SetActive(true);
    }

    private void HidePrompt()
    {
        if (promptRoot != null)
            promptRoot.SetActive(false);
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

        HidePrompt(); // the confirm panel takes over from here.

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

        ShowPrompt(currentPlantId);
    }

    public void FinishSelection()
    {
        selectionActive = false;
        HidePrompt();
        onSelectionFinished?.Invoke(currentPlantInstanceId, currentPlantId); // let a listener (e.g. the onboarding flow) know this plant's location step is done.
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
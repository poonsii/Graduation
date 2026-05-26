using UnityEngine;
using UnityEngine.EventSystems;

public class ClickObject : MonoBehaviour
{
    public GameObject plantCardUI;
    public LayerMask tapMask; // Set this in Inspector to include Walls + Plants
    public float maxDistance = 100f;

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;

        if (plantCardUI != null)
            plantCardUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            TryOpen(Input.mousePosition);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            TryOpen(touch.position);
        }
    }

    private void TryOpen(Vector3 screenPosition)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, tapMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                if (plantCardUI != null)
                    plantCardUI.SetActive(true);
            }
        }
    }

    public void ClosePlantCard()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (plantCardUI != null)
            plantCardUI.SetActive(false);
    }
}
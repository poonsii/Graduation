using UnityEngine;
using UnityEngine.EventSystems;

public class ClickObject : MonoBehaviour
{
    public GameObject plantCardUI; 
    public LayerMask tapMask; 
    public float maxDistance = 100f;

    private Camera mainCam; 

    private void Start()
    {
        mainCam = Camera.main; // get the main camera.

        if (plantCardUI != null)
            plantCardUI.SetActive(false); // hide the card on start.
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return; 

            TryOpen(Input.mousePosition); //pc
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            TryOpen(touch.position); //phone
        }
    }

    private void TryOpen(Vector3 screenPosition)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPosition); // cast a ray from the screen position.

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, tapMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
            {
                if (plantCardUI != null)
                    plantCardUI.SetActive(true); // open the card if this object was hit.
            }
        }
    }

    public void ClosePlantCard()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null); // clear selected UI object.

        if (plantCardUI != null)
            plantCardUI.SetActive(false); // hide the plant card.
    }
}
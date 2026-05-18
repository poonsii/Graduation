using UnityEngine;

public class ClickObject : MonoBehaviour
{
    public GameObject plantCardUI;

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
            TryOpen(Input.mousePosition);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            TryOpen(Input.GetTouch(0).position);
        }
    }

    private void TryOpen(Vector3 screenPosition)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (plantCardUI != null)
                    plantCardUI.SetActive(true);
            }
        }
    }

    public void ClosePlantCard()
    {
        if (plantCardUI != null)
            plantCardUI.SetActive(false);
    }
}
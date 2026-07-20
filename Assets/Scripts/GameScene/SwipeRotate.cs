using UnityEngine;

public class SwipeRotate : MonoBehaviour
{
    private Vector2 lastPointerPos; 
    private bool isDragging; 

    private float targetX; 
    private float targetY; 

    [Header("Rotation Speeds")]
    [Range(0.01f, 2f)]
    public float horizontalSpeed = 0.25f; 

    [Range(0.01f, 1f)]
    public float verticalSpeed = 0.08f; 

    [Header("Rotation Smoothing")]
    [Range(0.1f, 20f)]
    public float smoothDamp = 8f; 

    [Header("Zoom")]
    public Camera targetCamera; 
    public float zoomSpeedMouse = 0.5f; 
    public float zoomSpeedTouch = 0.005f; 
    public float minDistance = 0.2f; 
    public float maxDistance = 20f;

    [Header("Pan")]
    public float panSpeedMouse = 0.002f; 
    public float panSpeedTouch = 0.002f;

    [Header("Invert Controls")]
    public bool invertMovement = false; 

    private float targetDistance; 
    private Vector3 focusPoint; 
    private Vector3 cameraDirection; 

    private float startX; 
    private float startY; 
    private float startDistance; 
    private Vector3 startFocusPoint; 
    private Vector3 startCameraDirection; 
    private Vector3 startCameraPosition; 
    private Quaternion startCameraRotation; 

    private float InputDirection => invertMovement ? -1f : 1f; 

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main; // use the main camera if none is assigned.

        focusPoint = transform.position; // set  focus point to this object's position.

        Vector3 startRot = transform.eulerAngles;
        targetX = startRot.x;
        targetY = startRot.y;

        startX = targetX; // save starting x rotation.
        startY = targetY; // save starting y rotation.

        if (targetCamera != null)
        {
            startCameraPosition = targetCamera.transform.position; // save starting camera position.
            startCameraRotation = targetCamera.transform.rotation; // save starting camera rotation.

            Vector3 offset = startCameraPosition - focusPoint;
            targetDistance = offset.magnitude; // calculate initial distance from focus point.
            cameraDirection = offset.normalized; // calculate initial direction from focus point.

            startFocusPoint = focusPoint; // save starting focus point.
            startDistance = targetDistance; // save starting distance.
            startCameraDirection = cameraDirection; // save starting camera direction.
        }
    }

    void Update()
    {
        HandleTouchInput();
        HandleMouseInput();
        UpdateRotation();
        UpdateCameraPosition();
    }

    void UpdateRotation()
    {
        Quaternion targetRotation = Quaternion.Euler(targetX, targetY, 0f);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * smoothDamp // smoothly rotate towards the target rotation.
        );
    }

    void UpdateCameraPosition()
    {
        if (targetCamera == null) return;

        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance); // keep zoom within allowed range
        Vector3 desiredPosition = focusPoint + cameraDirection * targetDistance;
        targetCamera.transform.position = desiredPosition; // move camera to the correct position.
    }

    void HandleTouchInput() // phone
    {
        if (Input.touchCount == 0) return; // stop if there are no touches.

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastPointerPos = touch.position;
                isDragging = true; // start dragging.
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touch.position - lastPointerPos;
                lastPointerPos = touch.position;

                targetY -= delta.x * horizontalSpeed * InputDirection; // rotate horizontally.
                targetX -= delta.y * verticalSpeed * InputDirection; // rotate vertically.
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false; 
            }
        }
        else if (Input.touchCount == 2)
        {
            isDragging = false; // stop single finger drag when two fingers are used.

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;
            float pinchDelta = currentMagnitude - prevMagnitude;

            targetDistance -= pinchDelta * zoomSpeedTouch; // zoom based on pinch distance change.

            Vector2 prevCenter = (touch0PrevPos + touch1PrevPos) * 0.5f;
            Vector2 currentCenter = (touch0.position + touch1.position) * 0.5f;
            Vector2 centerDelta = currentCenter - prevCenter;

            PanFocus(centerDelta, panSpeedTouch); // pan based on two finger center movement.
        }
    }

    void HandleMouseInput() //pc
    {
        if (Input.touchCount > 0) return; // skip mouse input if touch is being used.

        if (Input.GetMouseButtonDown(0))
        {
            lastPointerPos = Input.mousePosition;
            isDragging = true; // start dragging.
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 delta = mousePos - lastPointerPos;
            lastPointerPos = mousePos;

            targetY -= delta.x * horizontalSpeed * InputDirection; // rotate horizontally.
            targetX -= delta.y * verticalSpeed * InputDirection; // rotate vertically.
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false; 
        }

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetDistance -= scroll * zoomSpeedMouse; // zoom in or out using the scroll wheel.
        }

        if (Input.GetMouseButton(1))
        {
            Vector2 panDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            PanFocus(panDelta, panSpeedMouse); // pan using the right mouse button.
        }
    }

    void PanFocus(Vector2 screenDelta, float speed)
    {
        if (targetCamera == null) return;

        Vector3 right = targetCamera.transform.right;
        Vector3 up = targetCamera.transform.up;

        float zoomBoost = 1f + (1f / Mathf.Max(targetDistance, 0.1f)) * 2f; // increase pan speed when zoomed in.
        float panScale = Mathf.Max(targetDistance, 1f) * zoomBoost;

        Vector3 move =
            (-right * screenDelta.x - up * screenDelta.y) *
            speed *
            panScale *
            InputDirection;

        focusPoint += move; // move the focus point to pan the view.
    }

    public void ResetView()
    {
        isDragging = false;

        targetX = startX; 
        targetY = startY; 
        focusPoint = startFocusPoint; 
        targetDistance = startDistance; 
        cameraDirection = startCameraDirection; 
    }

    public void ResetViewInstant()
    {
        ResetView(); // reset the view

        transform.rotation = Quaternion.Euler(targetX, targetY, 0f); // snap rotation.

        if (targetCamera != null)
        {
            targetCamera.transform.position = startCameraPosition; // snap camera position.
            targetCamera.transform.rotation = startCameraRotation; // snap camera rotation,

            Vector3 offset = startCameraPosition - focusPoint;
            targetDistance = offset.magnitude;
            cameraDirection = offset.normalized;
        }
    }

    public void ToggleInvertMovement()
    {
        invertMovement = !invertMovement; // flip the invert setting.
    }

    public void SetInvertMovement(bool value)
    {
        invertMovement = value; // set the invert setting.
    }
}
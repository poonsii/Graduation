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

    private float targetDistance;
    private Vector3 focusPoint;
    private Vector3 cameraDirection;

    void Start()
    {
        Vector3 startRot = transform.eulerAngles;
        targetX = startRot.x;
        targetY = startRot.y;

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null)
        {
            focusPoint = transform.position;
            Vector3 offset = targetCamera.transform.position - focusPoint;
            targetDistance = offset.magnitude;
            cameraDirection = offset.normalized;
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
            Time.deltaTime * smoothDamp
        );
    }

    void UpdateCameraPosition()
    {
        if (targetCamera == null) return;

        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);

        Vector3 desiredPosition = focusPoint + cameraDirection * targetDistance;

        targetCamera.transform.position = desiredPosition;
        targetCamera.transform.LookAt(focusPoint);
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastPointerPos = touch.position;
                isDragging = true;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 delta = touch.position - lastPointerPos;
                lastPointerPos = touch.position;

                targetY -= delta.x * horizontalSpeed;
                targetX -= delta.y * verticalSpeed;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
        else if (Input.touchCount == 2)
        {
            isDragging = false;

            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;
            float pinchDelta = currentMagnitude - prevMagnitude;

            targetDistance -= pinchDelta * zoomSpeedTouch;

            Vector2 prevCenter = (touch0PrevPos + touch1PrevPos) * 0.5f;
            Vector2 currentCenter = (touch0.position + touch1.position) * 0.5f;
            Vector2 centerDelta = currentCenter - prevCenter;

            PanFocus(centerDelta, panSpeedTouch);
        }
    }

    void HandleMouseInput()
    {
        if (Input.touchCount > 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            lastPointerPos = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 mousePos = Input.mousePosition;
            Vector2 delta = mousePos - lastPointerPos;
            lastPointerPos = mousePos;

            targetY -= delta.x * horizontalSpeed;
            targetX -= delta.y * verticalSpeed;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetDistance -= scroll * zoomSpeedMouse;
        }

        if (Input.GetMouseButton(1))
        {
            Vector2 panDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            PanFocus(panDelta, panSpeedMouse);
        }
    }

    void PanFocus(Vector2 screenDelta, float speed)
    {
        if (targetCamera == null) return;

        Vector3 right = targetCamera.transform.right;
        Vector3 up = targetCamera.transform.up;

        float zoomBoost = 1f + (1f / Mathf.Max(targetDistance, 0.1f)) * 2f;
        float panScale = Mathf.Max(targetDistance, 1f) * zoomBoost;

        Vector3 move =
            (-right * screenDelta.x - up * screenDelta.y) *
            speed *
            panScale;

        focusPoint += move;
    }
}
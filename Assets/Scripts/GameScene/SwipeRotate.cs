using UnityEngine;

public class SwipeRotate : MonoBehaviour
{
    private Vector2 lastPointerPos;
    private bool isDragging;

    private float targetX;
    private float targetY;

    [Header("Rotation Speeds")]
    [Range(0.01f, 2f)]
    public float horizontalSpeed = 0.25f;   // strong Y rotation

    [Range(0.01f, 1f)]
    public float verticalSpeed = 0.08f;     // weak X rotation

    [Header("Smoothing")]
    [Range(0.1f, 20f)]
    public float smoothDamp = 8f;

    void Start()
    {
        Vector3 startRot = transform.eulerAngles;
        targetX = startRot.x;
        targetY = startRot.y;
    }

    void Update()
    {
        HandleTouchInput();
        HandleMouseInput();

        Quaternion targetRotation = Quaternion.Euler(targetX, targetY, 0f);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * smoothDamp
        );
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;

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
            targetX += delta.y * verticalSpeed;
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isDragging = false;
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
            targetX += delta.y * verticalSpeed;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }
}
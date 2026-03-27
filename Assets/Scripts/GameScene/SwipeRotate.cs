using UnityEngine;

public class SwipeRotate : MonoBehaviour
{
    private Touch touch;
    private Vector2 lastMousePos;

    [Range(0.1f, 2f)]
    public float rotateSpeedModifier = 0.3f;

    [Range(0.1f, 10f)]
    public float smoothDamp = 5f;  // higher = faster; lower = smoother

    private Quaternion targetRotation;

    void Start()
    {
        targetRotation = transform.rotation;
    }

    void Update()
    {
        // TOUCH (phone)
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float rotY = -touch.deltaPosition.x * rotateSpeedModifier;
                float rotX = touch.deltaPosition.y * rotateSpeedModifier;

                Quaternion inputRot = Quaternion.Euler(rotX, rotY, 0f);
                targetRotation = inputRot * targetRotation;
            }
        }
        // Mouse (pc)
        else if (Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastMousePos = Input.mousePosition;
            }

            Vector2 mousePos = Input.mousePosition;
            Vector2 delta = mousePos - lastMousePos;
            lastMousePos = mousePos;

            float rotY = -delta.x * rotateSpeedModifier;
            float rotX = delta.y * rotateSpeedModifier;

            Quaternion inputRot = Quaternion.Euler(rotX, rotY, 0f);
            targetRotation = inputRot * targetRotation;
        }

        // smoothly move toward target rotation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * smoothDamp
        );
    }
}
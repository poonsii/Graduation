using UnityEngine;

// snaps the main camera to a health check "stage" spot and back again, since each check shows the
// plant from a dedicated camera angle instead of the normal room view. Also disables the room's
// swipe/zoom camera control while a check is open - otherwise it overwrites the camera's position
// every frame and fights with this. Shared by every plant's HealthCheckController, since they all
// move the same camera.
public class HealthCheckCameraMover : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private SwipeRotate roomCameraControl; // the room's drag/zoom control - disabled while a health check is open.

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool hasOriginalPose;

    public void MoveTo(Transform target)
    {
        if (cameraTransform == null || target == null)
            return;

        if (!hasOriginalPose)
        {
            // remember where the camera was before the very first move, so it can be restored later.
            originalPosition = cameraTransform.position;
            originalRotation = cameraTransform.rotation;
            hasOriginalPose = true;
        }

        if (roomCameraControl != null)
            roomCameraControl.enabled = false; // stop it from overwriting the camera position every frame.

        cameraTransform.position = target.position;
        cameraTransform.rotation = target.rotation;
    }

    public void MoveToOriginalPose()
    {
        if (cameraTransform == null || !hasOriginalPose)
            return;

        cameraTransform.position = originalPosition;
        cameraTransform.rotation = originalRotation;
        hasOriginalPose = false;

        if (roomCameraControl != null)
            roomCameraControl.enabled = true; // hand control back to the player.
    }
}

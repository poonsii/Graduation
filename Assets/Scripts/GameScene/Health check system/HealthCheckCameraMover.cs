using System;
using System.Collections;
using UnityEngine;

// smoothly moves the main camera to a health check "stage" spot and back again, since each check
// shows the plant from a dedicated camera angle instead of the normal room view. Shared by every
// plant's HealthCheckController, since they all move the same camera.
public class HealthCheckCameraMover : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float moveDuration = 0.75f;

    private Coroutine moveCoroutine;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool hasOriginalPose;

    public void MoveTo(Transform target, Action onComplete = null)
    {
        if (cameraTransform == null || target == null)
        {
            onComplete?.Invoke();
            return;
        }

        if (!hasOriginalPose)
        {
            // remember where the camera was before the very first move, so it can be restored later.
            originalPosition = cameraTransform.position;
            originalRotation = cameraTransform.rotation;
            hasOriginalPose = true;
        }

        StartMove(target.position, target.rotation, onComplete);
    }

    public void MoveToOriginalPose(Action onComplete = null)
    {
        if (cameraTransform == null || !hasOriginalPose)
        {
            onComplete?.Invoke();
            return;
        }

        StartMove(originalPosition, originalRotation, onComplete);
    }

    private void StartMove(Vector3 targetPosition, Quaternion targetRotation, Action onComplete)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveRoutine(targetPosition, targetRotation, onComplete));
    }

    private IEnumerator MoveRoutine(Vector3 targetPosition, Quaternion targetRotation, Action onComplete)
    {
        Vector3 startPosition = cameraTransform.position;
        Quaternion startRotation = cameraTransform.rotation;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / moveDuration));

            cameraTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            cameraTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        cameraTransform.position = targetPosition;
        cameraTransform.rotation = targetRotation;

        moveCoroutine = null;
        onComplete?.Invoke();
    }
}

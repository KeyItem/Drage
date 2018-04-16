using System.Collections;
using UnityEngine;

public class BoxCameraFollow2D : MonoBehaviour
{
    public PlayerController2D targetPlayerController;

    private Collider2D playerCollider;

    [Header("Box Camera Attributes")]
    public float lookAheadDistanceX;

    private float lookAheadDirectionX;

    private float currentLookAheadX;
    private float targetLookAheadX;

    private float horizontalSmoothVelocity;
    private float verticalSmoothVelocity;

    [Space(10)]
    public float horizontalSmoothTime;
    public float verticalSmoothTime;

    private bool isLookAheadStopped;

    [Space(10)]
    public float verticalOffset;
    public float cameraZOffset;

    [Space(10)]
    public Vector2 playerFocusAreaSize;

    private FocusArea focusArea;

    private void Start()
    {
        CameraSetup();
    }

    private void LateUpdate()
    {
        MoveCamera();
    }

    private void CameraSetup()
    {
        if (targetPlayerController != null)
        {
            playerCollider = targetPlayerController.gameObject.GetComponent<BoxCollider2D>();

            focusArea = new FocusArea(playerCollider.bounds, playerFocusAreaSize);
        }
    }

    private void MoveCamera()
    {
        focusArea.UpdateFocusArea(playerCollider.bounds);

        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;

        if (focusArea.focusAreaVelocity.x != 0)
        {
            lookAheadDirectionX = Mathf.Sign(focusArea.focusAreaVelocity.x);

            if (targetPlayerController.playerInputDirection.x == lookAheadDirectionX && targetPlayerController.playerInputDirection.x != 0)
            {
                targetLookAheadX = lookAheadDirectionX * lookAheadDistanceX;
                isLookAheadStopped = false;
            }
            else
            {
                if (!isLookAheadStopped)
                {
                    isLookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirectionX * lookAheadDistanceX - currentLookAheadX) / 4f;
                }
            }
        }

        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref horizontalSmoothVelocity, horizontalSmoothTime);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref verticalSmoothVelocity, verticalSmoothTime);
        focusPosition += Vector2.right * currentLookAheadX;

        transform.position = (Vector3)focusPosition + Vector3.forward * cameraZOffset;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawCube(focusArea.center, playerFocusAreaSize);
    }

    struct FocusArea
    {
        [Header("Focus Area Attributes")]
        public Vector2 focusAreaVelocity;

        [Space(10)]
        public Vector2 center;

        private float left;
        private float right;
        private float top;
        private float bottom;

        public FocusArea (Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            focusAreaVelocity = Vector2.zero;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void UpdateFocusArea(Bounds targetBounds)
        {
            float shiftX = 0;

            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }

            left += shiftX;
            right += shiftX;

            float shiftY = 0;

            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }

            top += shiftY;
            bottom += shiftY;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            focusAreaVelocity = new Vector2(shiftX, shiftY);
        }
    }
}
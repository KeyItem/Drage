using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Camera))]
public class MultipleTargetCamera : MonoBehaviour
{
    private Camera targetCamera;

    [Header("Camera Position Attributes")]
    public List<Transform> targetTransforms;

    [Space(10)]
    public Vector3 cameraOffset = Vector3.zero;

    [Header("Camera Movement Attributes")]
    public float cameraMoveSmoothTime = 0.12f;

    private Vector3 cameraMoveSmoothVelocity;

    [Header("Camera Scrolling Attributes")]
    public float cameraMinZoom = 40f;
    public float cameraMaxZoom = 10f;

    [Space(10)]
    public float cameraZoomLimiter = 50f;

    private void Start()
    {
        InitializeCamera();
    }

    private void LateUpdate()
    {
        MoveCamera();
        ZoomCamera();
    }

    private void InitializeCamera()
    {
        targetCamera = GetComponent<Camera>();
    }

    private void MoveCamera()
    {
        if (targetTransforms.Count == 0)
        {
            return;
        }

        Vector3 cameraCenterPoint = ReturnCenterPointBetweenTargets();
        Vector3 newCameraPosition = cameraCenterPoint + cameraOffset;

        transform.position = Vector3.SmoothDamp(transform.position, newCameraPosition, ref cameraMoveSmoothVelocity, cameraMoveSmoothTime);
    }

    private void ZoomCamera()
    {
        if (targetTransforms.Count == 0)
        {
            return;
        }

        float newPlayerDistanceZoom = ReturnLargestDistanceBetweenPlayers();

        newPlayerDistanceZoom = Mathf.Lerp(cameraMaxZoom, cameraMinZoom, newPlayerDistanceZoom / cameraZoomLimiter);

        targetCamera.fieldOfView = Mathf.Lerp(targetCamera.fieldOfView, newPlayerDistanceZoom, Time.deltaTime);
    }

    private Vector3 ReturnCenterPointBetweenTargets()
    {
        if (targetTransforms.Count == 1)
        {
            return targetTransforms[0].position;
        }

        Bounds newTargetBounds = new Bounds(targetTransforms[0].position, Vector3.zero);

        for (int i = 0; i < targetTransforms.Count; i++)
        {
            newTargetBounds.Encapsulate(targetTransforms[i].position);
        }

        return newTargetBounds.center;
    }

    private float ReturnLargestDistanceBetweenPlayers()
    {
        Bounds newDistanceBounds = new Bounds(targetTransforms[0].position, Vector3.zero);

        for (int i = 0; i < targetTransforms.Count; i++)
        {
            newDistanceBounds.Encapsulate(targetTransforms[i].position);
        }

        return newDistanceBounds.size.x;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthographicSmashCameraController : MonoBehaviour
{
    [Header("Camera Control Values")]
    public float cameraDampTime = 0.2f;
    public float cameraScreenEdgeBuffer = 4f;
    public float cameraMinimumSize = 6.5f;

    public List<Transform> objectsToFollow;

    private Camera mainCam;

    private float cameraZoomSpeed;

    private Vector3 cameraMoveVelocity;
    private Vector3 cameraDesiredPosition;

    private void Awake()
    {
        IntializeCamera();
    }
    
    private void IntializeCamera()
    {
        mainCam = GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        CameraMove();
        CameraZoom();
    }

    private void CameraMove()
    {
        FindAveragePositionOfTargets();

        transform.position = Vector3.SmoothDamp(transform.position, cameraDesiredPosition, ref cameraMoveVelocity, cameraDampTime);
    }

    private void CameraZoom()
    {
        float requiredSize = FindRequiredCameraSize();

        mainCam.orthographicSize = Mathf.SmoothDamp(mainCam.orthographicSize, requiredSize, ref cameraZoomSpeed, cameraDampTime);
    }

    private void FindAveragePositionOfTargets()
    {
        Vector3 averagePosition = new Vector3();

        int numberOfTargets = 0;

        for (int i = 0; i < objectsToFollow.Count; i++)
        {
            if (!objectsToFollow[i].gameObject.activeSelf) //If Not Active, Ignore
            {
                continue;
            }

            averagePosition += objectsToFollow[i].position;

            numberOfTargets++;
        }

        if (numberOfTargets > 0)
        {
            averagePosition /= numberOfTargets;
        }

        averagePosition.y = transform.position.y;

        cameraDesiredPosition = averagePosition;
    }

    private float FindRequiredCameraSize()
    {
        Vector3 desiredLocalPosition = transform.InverseTransformPoint(cameraDesiredPosition);

        float size = 0f;

        for (int i = 0; i < objectsToFollow.Count; i++)
        {
            if (!objectsToFollow[i].gameObject.activeSelf)
            {
                continue;
            }

            Vector3 targetLocalPosition = transform.InverseTransformPoint(objectsToFollow[i].position);

            Vector3 desiredPositionToTarget = targetLocalPosition - desiredLocalPosition;

            size = Mathf.Max(size, Mathf.Abs(desiredPositionToTarget.y));

            size = Mathf.Max(size, Mathf.Abs(desiredPositionToTarget.x), mainCam.aspect);
        }

        size += cameraScreenEdgeBuffer;

        size = Mathf.Max(size, cameraMinimumSize);

        return size;
    }

    private void SetStartPositionAndSize()
    {
        FindAveragePositionOfTargets();

        transform.position = cameraDesiredPosition;

        mainCam.orthographicSize = FindRequiredCameraSize();
    }
}

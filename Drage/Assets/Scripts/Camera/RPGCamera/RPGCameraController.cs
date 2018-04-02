using System.Collections;
using UnityEngine;

public class RPGCameraController : MonoBehaviour
{
    [Header("Camera Attributes")]
    public CameraPoint currentCameraPoint;

    [Space(10)]
    public float cameraTransitionTime;

    private IEnumerator currentCameraTransitionCoroutine = null;

    [Header("Camera Points")]
    public int playerCurrentCameraPointIndex;

    [Space(10)]
    public CameraPoint[] playerCameraPoints;

    [Space(10)]
    public CameraPoint[] enemyCameraPoints;

    [Header("Camera Target")]
    public bool isLookingAtPlayer = false;
    public bool isLookingAtEnemy = false;

    private void Awake()
    {
        CameraSetup();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MoveToNextPlayerCameraPoint();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            MoveToNextEnemyCameraPoint();
        }
    }

    private void CameraSetup()
    {
        currentCameraPoint = playerCameraPoints[0];

        transform.position = currentCameraPoint.transform.position;
        transform.rotation = currentCameraPoint.transform.rotation;
    }

    public void MoveToNextPlayerCameraPoint()
    {     
        if (++playerCurrentCameraPointIndex > playerCameraPoints.Length - 1)
        {
            playerCurrentCameraPointIndex = 0;
        }

        CameraPoint nextCameraPoint = ReturnNextPlayerCameraPoint();

        MoveToCameraPoint(nextCameraPoint);

        isLookingAtPlayer = true;
        isLookingAtEnemy = false;
    }

    public void MoveToNextEnemyCameraPoint()
    {
        CameraPoint nextCameraPoint = ReturnEnemyCameraPoint();

        MoveToCameraPoint(nextCameraPoint);

        isLookingAtEnemy = true;
        isLookingAtPlayer = false;
    }

    private void MoveToCameraPoint(CameraPoint targetCameraPoint)
    {
        if (currentCameraTransitionCoroutine != null)
        {
            StopCoroutine(currentCameraTransitionCoroutine);
        }

        currentCameraTransitionCoroutine = CameraPointTransition(targetCameraPoint);

        StartCoroutine(CameraPointTransition(targetCameraPoint));
    }

    private IEnumerator CameraPointTransition(CameraPoint targetCameraPoint)
    {
        float currentTime = 0f;

        while (currentTime < 1)
        {
            currentTime += Time.deltaTime / cameraTransitionTime;

            if (currentTime > 1)
            {
                currentTime = 1;
            }

            transform.position = Vector3.Slerp(currentCameraPoint.transform.position, targetCameraPoint.transform.position, currentTime);

            transform.LookAt(targetCameraPoint.cameraTargetTransform);

            yield return new WaitForEndOfFrame();
        }

        currentCameraPoint = targetCameraPoint;

        currentCameraTransitionCoroutine = null;

        yield return null;
    }

    private CameraPoint ReturnNextPlayerCameraPoint()
    {      
        return playerCameraPoints[playerCurrentCameraPointIndex];
    }

    private CameraPoint ReturnEnemyCameraPoint()
    {
        return enemyCameraPoints[playerCurrentCameraPointIndex];
    }
}

using System.Collections;
using UnityEngine;
using Rewired;

public class NavigatorCamera : MonoBehaviour
{
    private Player playerInput;

    [Header("Navigator Camera Attributes")]
    public NavigatorCameraAttributes2D navigatorCameraAttributes;

    [Header("Navigator Camera Points")]
    public Transform[] cameraNavigationPoints;

    private Vector3[] cameraWaypoints;

    [Space(10)]
    public Vector3 currentWaypoint;
    public Vector3 previousWaypoint;

    [Space(10)]
    public int cameraWaypointIndex;

    [Space(10)]
    public bool hasReachedEndOfWaypoints = false;

    private void Start()
    {
        CameraNavigationSetup();
        InputSetup();
    }

    private void Update()
    {
        ManageInput();   
    }

    private void InputSetup()
    {
        playerInput = ReInput.players.GetPlayer(0);
    }

    private void CameraNavigationSetup()
    {
        ConvertTransformsToVectors();

        cameraWaypointIndex = 1;

        currentWaypoint = cameraWaypoints[1];
        previousWaypoint = cameraWaypoints[0];

        StartCoroutine(CameraNavigation(navigatorCameraAttributes.cameraWaitTime, navigatorCameraAttributes.cameraPanTime));
    }

    private void ManageInput()
    {
        if (playerInput.GetButtonDown("NextLevel"))
        {
            if (hasReachedEndOfWaypoints)
            {
                LevelManager.Instance.LoadNextLevel();
            }
        }

        if (playerInput.GetButtonDown("RestartLevel"))
        {
            LevelManager.Instance.ReloadCurrentLevel();
        }
    }

    private void MoveToNextPoint()
    {
        if (CanMoveToNextWaypoint())
        {
            StartCoroutine(CameraNavigation(navigatorCameraAttributes.cameraWaitTime, navigatorCameraAttributes.cameraPanTime));
        }
        else
        {
            hasReachedEndOfWaypoints = true;

            Debug.Log("Done all Points");
        }
    }

    private bool CanMoveToNextWaypoint()
    {
        if (cameraWaypointIndex++ < cameraWaypoints.Length - 1)
        {
            previousWaypoint = currentWaypoint;

            currentWaypoint = cameraWaypoints[cameraWaypointIndex];

            Debug.Log("Moving to next point");

            return true;
        }

        return false;
    }

    private void ConvertTransformsToVectors()
    {
        cameraWaypoints = new Vector3[cameraNavigationPoints.Length];

        for (int i = 0; i < cameraWaypoints.Length; i++)
        {
            cameraWaypoints[i] = cameraNavigationPoints[i].position;
        }
    }

    private float ReturnNextWaitTime()
    {
        float newWaitTime = Time.time + navigatorCameraAttributes.cameraWaitTime;

        return newWaitTime;
    }

    private IEnumerator CameraNavigation(float cameraWaitTime, float cameraPanTime)
    {
        if (cameraWaitTime > 0)
        {
            yield return new WaitForSeconds(cameraWaitTime);
        }

        float panProgress = 0;

        while (panProgress < 1f)
        {
            Debug.Log("Moving to :: " + currentWaypoint);

            panProgress += Time.deltaTime / cameraPanTime;

            if (panProgress > 1f)
            {
                panProgress = 1f;
            }

            Vector3 newCameraPosition = Vector3.Lerp(previousWaypoint, currentWaypoint, panProgress);

            transform.position = newCameraPosition;

            yield return new WaitForEndOfFrame();
        }

        MoveToNextPoint();

        yield return null;
    }
}

[System.Serializable]
public struct NavigatorCameraAttributes2D
{
    [Header("Navigator Camera Attributes")]
    public float cameraWaitTime;

    [Space(10)]
    public float cameraPanTime;
}

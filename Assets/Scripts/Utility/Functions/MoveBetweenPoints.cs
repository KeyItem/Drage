using System.Collections;
using UnityEngine;

public class MoveBetweenPoints : MonoBehaviour
{
    [Header("Obstacle Attributes")]
    public GameObject targetObject;

    [Header("Target Point Attributes")]
    public Transform[] targetPoints;

    [Space(10)]
    public Transform currentTargetPoint;

    [Space(10)]
    public int currentTargetPointIndex;

    [Header("Starting Attributes")]
    public int startingPointIndex = 0;

    [Space(10)]
    public bool startAtCustomPoint = false;

    [Header("Movement Attributes")]
    public float moveTime = 1f;

    [Space(10)]
    public float waitTimeBetweenPoints = 0.25f;

    [Space(10)]
    public bool isLerp = true;
    public bool isSlerp = false;

    private void Start()
    {
        MovementSetup();
    }

    private void MovementSetup()
    {
        if (startAtCustomPoint)
        {
            currentTargetPointIndex = startingPointIndex;
        }

        currentTargetPoint = targetPoints[currentTargetPointIndex];

        targetObject.transform.position = currentTargetPoint.transform.position;

        MoveToNextPoint();
    }

    private void MoveToNextPoint()
    {
        Transform nextPoint = GetNextPoint();

        StartCoroutine(MoveToPoint(nextPoint));
    }

    private IEnumerator MoveToPoint(Transform nextPoint)
    {
        float currentTime = 0f;

        while (currentTime < 1)
        {
            currentTime += Time.deltaTime / moveTime;

            if (currentTime > 1)
            {
                currentTime = 1;
            }

            if (isLerp)
            {
                targetObject.transform.position = Vector3.Lerp(currentTargetPoint.transform.position, nextPoint.transform.position, currentTime);
            }
            else if (isSlerp)
            {
                targetObject.transform.position = Vector3.Slerp(currentTargetPoint.transform.position, nextPoint.transform.position, currentTime);
            }

            yield return new WaitForEndOfFrame();
        }

        currentTargetPoint = nextPoint;

        if (waitTimeBetweenPoints > 0)
        {
            yield return new WaitForSeconds(waitTimeBetweenPoints);
        }

        MoveToNextPoint();

        yield return null;
    }

    private Transform GetNextPoint()
    {
        if (++currentTargetPointIndex > targetPoints.Length - 1)
        {
            currentTargetPointIndex = 0;
        }

        return targetPoints[currentTargetPointIndex];
    }
}

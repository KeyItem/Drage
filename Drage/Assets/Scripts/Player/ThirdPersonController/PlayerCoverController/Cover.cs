using System.Collections;
using UnityEngine;

public class Cover : MonoBehaviour
{
    [Header("Cover Attributes")]
    public CoverPoint[] coverPoints;

    [Space(10)]
    public BezierSpline coverSpline;

    [Space(10)]
    public float playerCoverSplineMovePercent = 0f;

    [Space(10)]
    public int playerAttachedCoverPointIndex = 0;

    [Space(10)]
    public bool isPlayerAttachedToCover = false;

    private void Start()
    {
        SetupCoverPoints();
    }

    private void SetupCoverPoints()
    {
        if (coverPoints == null)
        {
            Debug.LogError("CoverPoint Array is null at :: " + gameObject.name);

            return;
        }

        for (int i = 0; i < coverPoints.Length; i++)
        {
            CoverPoint currentCoverPoint = coverPoints[i];

            currentCoverPoint.attachedCover = this;
            currentCoverPoint.coverPointIndex = i;

            currentCoverPoint.name += i;
        }

        coverSpline = GetComponent<BezierSpline>();
    }

    public CoverPoint ReturnClosestCoverPoint (Vector3 playerPosition)
    {
        float closestDistance = float.MaxValue;

        CoverPoint closestCoverPoint = null;

        for (int i = 0; i < coverPoints.Length; i++)
        {
            float distanceFromPlayer = Vector3.Distance(coverPoints[i].transform.position, playerPosition);

            if (distanceFromPlayer < closestDistance)
            {
                closestDistance = distanceFromPlayer;

                closestCoverPoint = coverPoints[i];
            }
        }

        return closestCoverPoint;
    }

    public Vector3 ReturnCoverMovePosition(float moveDirection, float moveAmount)
    {
        playerCoverSplineMovePercent += Time.deltaTime * moveAmount * moveDirection;

        if (playerCoverSplineMovePercent > 1)
        {
            playerCoverSplineMovePercent = 0f;
        }
        else if (playerCoverSplineMovePercent < 0)
        {
            playerCoverSplineMovePercent = 1f;
        }

        Vector3 newPlayerPosition = coverSpline.GetPoint(playerCoverSplineMovePercent);

        return newPlayerPosition;
    }

    public void ResetCover()
    {
        playerCoverSplineMovePercent = 0f;

        isPlayerAttachedToCover = false;
        playerAttachedCoverPointIndex = 0;
    }
}

using System.Collections;
using UnityEngine;

public class PlayerCoverController : MonoBehaviour
{
    [Header("Playert Cover Controller Attributes")]
    public Cover playerCurrentCover = null;

    private PlayerCoverAttributes playerCoverAttributes;

    private ThirdPersonPlayerController playerController;

    [Space(10)]
    public LayerMask coverMask;

    [Space(10)]
    public bool isPlayerInCover = false;

    [Header("DEBUG")]
    public bool canShowDebug = false;

    private void Start()
    {
        InitializePlayerCoverController();
    }

    private void InitializePlayerCoverController()
    {
        playerController = GetComponent<ThirdPersonPlayerController>();

        playerCoverAttributes = playerController.playerCoverAttributes;
    }

    public void PlayerTakeCover()
    {
        if (!isPlayerInCover)
        {
            if (CheckForCoverInPlayerRadius())
            {
                Cover newPlayerCover = ReturnPlayerCover();

                CoverPoint newPlayerCoverPoint = newPlayerCover.ReturnClosestCoverPoint(transform.position);

                float distanceToCoverPoint = Vector3.Distance(transform.position, newPlayerCoverPoint.transform.position);

                if (CheckPlayerPathToCover(newPlayerCoverPoint, distanceToCoverPoint))
                {
                    StartCoroutine(TransitionPlayerToCover(newPlayerCover, newPlayerCoverPoint, distanceToCoverPoint));
                }
            }
        }
        else
        {
            PlayerLeaveCover();
        }
    }

    private IEnumerator TransitionPlayerToCover(Cover targetCover, CoverPoint targetCoverPoint, float distanceToCover)
    {
        float coverSplinePercent = 1f / targetCover.coverPoints.Length;
        coverSplinePercent *= targetCoverPoint.coverPointIndex + 1;

        Vector3 targetCoverHookLocation = targetCover.coverSpline.GetPoint(coverSplinePercent);

        transform.LookAt(targetCoverHookLocation);

        Vector3 playerStartPosition = transform.position;

        float speedMultiplier = playerCoverAttributes.playerCoverDetectionRadius / distanceToCover;

        float currentCoverTransitionTime = 0;

        PlayerStartMovingToCover();

        while (currentCoverTransitionTime < 1)
        {
            currentCoverTransitionTime += (Time.deltaTime * speedMultiplier) / playerCoverAttributes.playerCoverTransitionTime;

            if (currentCoverTransitionTime > 1)
            {
                currentCoverTransitionTime = 1;
            }

            transform.position = Vector3.Lerp(playerStartPosition, targetCoverHookLocation, currentCoverTransitionTime);

            yield return new WaitForEndOfFrame();
        }

        PlayerFinishMovingToCover(targetCover, targetCoverPoint, targetCoverHookLocation, coverSplinePercent);

        yield return null;
    }

    private void PlayerStartMovingToCover()
    {
        playerController.PlayerStartMovingToCover();
    }

    private void PlayerFinishMovingToCover(Cover targetCover, CoverPoint targetCoverPoint, Vector3 coverSplinePercentPosition, float targetCoverSplinePercent)
    {
        transform.position = coverSplinePercentPosition;

        playerController.canPlayerMove = true;
        playerController.canPlayerTakeCover = true;
        isPlayerInCover = true;

        playerCurrentCover = targetCover;
        playerCurrentCover.isPlayerAttachedToCover = true;
        playerCurrentCover.playerAttachedCoverPointIndex = targetCoverPoint.coverPointIndex;
        playerCurrentCover.playerCoverSplineMovePercent = targetCoverSplinePercent;

        playerController.PlayerFinishMovingToCover(targetCover);
    }

    private void PlayerLeaveCover()
    {
        isPlayerInCover = false;

        playerController.PlayerLeaveCover();

        playerCurrentCover.ResetCover();

        playerCurrentCover = null;
    }

    public bool CheckForCoverInPlayerRadius()
    {
        Collider[] hitCoverColliders = Physics.OverlapSphere(transform.position, playerCoverAttributes.playerCoverDetectionRadius, coverMask);

        if (hitCoverColliders.Length > 0)
        {
            return true;
        }

        return false;
    }

    private bool CheckPlayerPathToCover(CoverPoint coverPointToCheck, float distanceToCover)
    {
        float verticalDistanceBetweenCover = Mathf.Abs(coverPointToCheck.transform.position.y - transform.position.y);

        if (verticalDistanceBetweenCover > 0.5f) //Check if Cover is on similar Y coordinates
        {
            return false;
        }

        Vector3 interceptVector = (coverPointToCheck.transform.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, interceptVector, distanceToCover))
        {
            if (canShowDebug)
            {
                Debug.DrawRay(transform.position, interceptVector * distanceToCover, Color.red, 1f);
            }

            return false;
        }
        else
        {
            if (canShowDebug)
            {
                Debug.DrawRay(transform.position, interceptVector * distanceToCover, Color.green, 1f);
            }

            return true;
        }
    }

    private Cover ReturnPlayerCover()
    {
        Cover newPlayerCover = null;

        float closestCoverPoint = float.MaxValue;

        Collider[] hitCoverColliders = Physics.OverlapSphere(transform.position, playerCoverAttributes.playerCoverDetectionRadius, coverMask);

        for (int i = 0; i < hitCoverColliders.Length; i++)
        {
            Cover coverToCheck = hitCoverColliders[i].GetComponent<Cover>();

            float coverDistanceToPlayer = Vector3.Distance(transform.position, coverToCheck.transform.position);

            if (coverDistanceToPlayer < closestCoverPoint)
            {
                closestCoverPoint = coverDistanceToPlayer;

                newPlayerCover = coverToCheck;
            }
        }

        return newPlayerCover;
    }

}

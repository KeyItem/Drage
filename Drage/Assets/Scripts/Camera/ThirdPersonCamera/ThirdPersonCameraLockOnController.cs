using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraLockOnController : MonoBehaviour
{
    private ThirdPersonCameraController thirdPersonCameraController;
    private ThirdPersonCameraLockOnAttributes thirdPersonCameraLockOnAttributes;

    private PlayerUI playerUI;

    [Header("Camera LockOn Attributes")]
    public List<Transform> nearbyEnemyList = new List<Transform>();

    [Space(10)]
    public Transform targetPlayer;
    public Transform currentLockOnTransform;

    [Space(10)]
    public float cameraDistanceFromTarget = 0f;

    [Space(10)]
    public LayerMask enemyMask;

    [Space(10)]
    public bool isCameraLockedOn = false;

    [Header("DEBUG")]
    public bool canShowDebug = false;

    private void Start()
    {
        InitializeLockOnController();
    }

    private void Update()
    {
        ManageEnemiesInRange();

        ManageLockOnDistance();
    }

    private void InitializeLockOnController()
    {
        thirdPersonCameraController = GetComponent<ThirdPersonCameraController>();
        thirdPersonCameraLockOnAttributes = thirdPersonCameraController.thirdPersonCameraLockOnAttributes;

        playerUI = GameObject.FindGameObjectWithTag("PlayerUI").GetComponent<PlayerUI>();

        if (targetPlayer == null)
        {
            targetPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void ManageEnemiesInRange()
    {
        nearbyEnemyList = ReturnEnemiesInRange();
    }

    private void ManageLockOnDistance()
    {
        if (isCameraLockedOn)
        {
            cameraDistanceFromTarget = Vector3.Distance(targetPlayer.position, currentLockOnTransform.position);

            if (cameraDistanceFromTarget > thirdPersonCameraLockOnAttributes.cameraMaxLockOnDistance)
            {
                thirdPersonCameraController.StopLock();

                cameraDistanceFromTarget = 0;
            }
        }
    }

    public bool CheckForEnemiesInRange()
    {
        if (nearbyEnemyList.Count > 0)
        {
            return true;
        }

        return false;
    }

    public void RequestLockOn()
    {
        Transform newLockOnTarget = ReturnClosestVisibleEnemy(nearbyEnemyList);

        if (newLockOnTarget != null)
        {
            SetLockOnTarget(newLockOnTarget);

            thirdPersonCameraController.SetLockOnTarget(newLockOnTarget);

            playerUI.SetNewLockOnIndicatorPosition(newLockOnTarget);
        }
    }

    public void RequestNewLockTarget(DIRECTION newDirection)
    {
        Transform newLockOnTarget = SwitchLockOnTarget(newDirection);

        if (newLockOnTarget != null)
        {
            SetLockOnTarget(newLockOnTarget);

            thirdPersonCameraController.SetLockOnTarget(newLockOnTarget);
        }
    }

    public void RequestStopLockOn()
    {
        currentLockOnTransform = null;

        isCameraLockedOn = false;

        playerUI.DisableLockOnIndicator();
    }

    private void SetLockOnTarget(Transform targetTransform)
    {
        currentLockOnTransform = targetTransform;

        isCameraLockedOn = true;

        playerUI.SetNewLockOnIndicatorPosition(targetTransform);
    }

    private Transform RequestTarget()
    {
        Transform newTargetTransform = ReturnClosestVisibleEnemy(nearbyEnemyList);

        if (newTargetTransform != null)
        {
            SetLockOnTarget(newTargetTransform);

            playerUI.SetNewLockOnIndicatorPosition(newTargetTransform);
        }

        return newTargetTransform;
    }

    private Transform SwitchLockOnTarget(DIRECTION targetDirection)
    {
        Transform newTargetTransform = null;

        List<Transform> targetsInDirection = ReturnEnemiesInDirection(currentLockOnTransform, targetDirection, nearbyEnemyList);

        if (targetsInDirection.Count > 0)
        {
            switch(targetDirection)
            {
                case DIRECTION.LEFT:
                    newTargetTransform = ReturnClosestEnemyInList(currentLockOnTransform, targetsInDirection);
                    break;

                case DIRECTION.RIGHT:
                    newTargetTransform = ReturnClosestEnemyInList(currentLockOnTransform, targetsInDirection);
                    break;

                case DIRECTION.FORWARD:
                    newTargetTransform = ReturnFarthestEnemyInList(currentLockOnTransform, targetsInDirection);
                    break;

                case DIRECTION.BACK:
                    newTargetTransform = ReturnClosestEnemyInList(targetPlayer, targetsInDirection);
                    break;
            }                 
        }

        if (newTargetTransform != null)
        {
            SetLockOnTarget(newTargetTransform);

            playerUI.SetNewLockOnIndicatorPosition(newTargetTransform);
        }

        return newTargetTransform;
    }

    private List<Transform> ReturnEnemiesInRange()
    {
        Collider[] newEnemyColliders = Physics.OverlapSphere(targetPlayer.position, thirdPersonCameraLockOnAttributes.cameraLockOnRadius, enemyMask);

        List<Transform> newEnemiesInRange = new List<Transform>(newEnemyColliders.Length);

        for (int i = 0; i < newEnemyColliders.Length; i++)
        {
            newEnemiesInRange.Add(newEnemyColliders[i].transform);
        }

        return newEnemiesInRange;
    }

    private Transform ReturnClosestEnemyInList(Transform target, List<Transform> enemyList)
    {
        Transform newEnemy = null;

        float closestEnemyDistance = float.MaxValue;

        if (enemyList.Count > 0)
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                float distanceFromTarget = Vector3.Distance(target.position, enemyList[i].position);

                if (distanceFromTarget < closestEnemyDistance)
                {
                    newEnemy = enemyList[i];

                    closestEnemyDistance = distanceFromTarget;
                }

                if (canShowDebug)
                {
                    Debug.DrawLine(targetPlayer.position, enemyList[i].position, Color.white, 1f);
                }
            }

            if (canShowDebug)
            {
                Debug.DrawLine(targetPlayer.position, newEnemy.position, Color.green, 1f);
            }
        }     

        return newEnemy;
    }

    private Transform ReturnFarthestEnemyInList(Transform targetTransform, List<Transform> targetList)
    {
        Transform newTarget = null;

        float farthestEnemyDistance = 0f;

        if (targetList.Count > 0)
        {
            for (int i = 0; i < targetList.Count; i++)
            {
                float distanceToTarget = Vector3.Distance(targetTransform.position, targetList[i].position);

                if (distanceToTarget > farthestEnemyDistance)
                {
                    newTarget = targetList[i];

                    farthestEnemyDistance = distanceToTarget;
                }

                if (canShowDebug)
                {
                    Debug.DrawLine(targetTransform.position, targetList[i].position, Color.white, 1f);
                }
            }

            if (canShowDebug)
            {
                Debug.DrawLine(targetTransform.position, newTarget.position, Color.green, 1f);
            }
        }

        return newTarget;
    }

    private List <Transform> ReturnEnemiesInDirection(Transform targetTransform, DIRECTION targetDirection, List<Transform> targetList)
    {
        List<Transform> newEnemyList = new List<Transform>();

        for (int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i] == targetTransform)
            {
                continue; //Ignore Current Target
            }

            Vector3 interceptBetweenTargetAndCamera = (targetList[i].position - transform.position).normalized;

            Vector3 localTransformPoint = transform.InverseTransformPoint(targetList[i].position);

            float dotProductBetweenTargetAndCamera = Vector3.Dot(interceptBetweenTargetAndCamera, transform.forward);

            switch (targetDirection)
            {
                case DIRECTION.LEFT:
                    if (dotProductBetweenTargetAndCamera > 0)
                    {
                        if (localTransformPoint.x < 0f)
                        {
                            newEnemyList.Add(targetList[i]);
                        }
                    }
                    break;

                case DIRECTION.RIGHT:
                    if (dotProductBetweenTargetAndCamera > 0)
                    {
                        if (localTransformPoint.x > 0f)
                        {
                            newEnemyList.Add(targetList[i]);
                        }
                    }
                    break;

                case DIRECTION.FORWARD:
                    if (dotProductBetweenTargetAndCamera > 0)
                    {
                        float distanceBetweenTargets = Vector3.Distance(targetPlayer.position, targetList[i].position);

                        if (distanceBetweenTargets > cameraDistanceFromTarget)
                        {
                            newEnemyList.Add(targetList[i]);
                        }
                    }
                    break;

                case DIRECTION.BACK:
                    if (dotProductBetweenTargetAndCamera > 0)
                    {
                        float distanceBetweenTargets = Vector3.Distance(targetPlayer.position, targetList[i].position);

                        if (distanceBetweenTargets < cameraDistanceFromTarget)
                        {
                            newEnemyList.Add(targetList[i]);
                        }
                    }
                    break;
            }
        }

        return newEnemyList;
    }

    private Transform ReturnClosestVisibleEnemy(List<Transform> enemyList)
    {
        Transform closestEnemy = null;
        List<Transform> visibleEnemies = new List<Transform>();

        for (int i = 0; i < enemyList.Count; i++)
        {
            Vector3 interceptVector = (enemyList[i].position - transform.position).normalized;

            float dotProduct = Vector3.Dot(interceptVector, transform.forward);

            if (dotProduct > 0.5f)
            {
                visibleEnemies.Add(enemyList[i]);
            }
        }

        if (visibleEnemies.Count > 0)
        {
            float closestEnemyDistance = float.MaxValue;

            for (int o = 0; o < visibleEnemies.Count; o++)
            {
                float distanceFromPlayer = Vector3.Distance(targetPlayer.position, visibleEnemies[o].position);

                if (distanceFromPlayer < closestEnemyDistance)
                {
                    closestEnemy = visibleEnemies[o];

                    closestEnemyDistance = distanceFromPlayer;
                }

                if (canShowDebug)
                {
                    Debug.DrawLine(targetPlayer.position, visibleEnemies[o].position, Color.white, 1f);
                }
            }

            if (canShowDebug)
            {
                Debug.DrawLine(targetPlayer.position, closestEnemy.position, Color.green, 1f);
            }

            return closestEnemy;
        }
        else
        {
            return null;
        }
    }

    public float ReturnCameraLockOnHeight()
    {
        float newCameraLockOnHeight = 0f;

        newCameraLockOnHeight = (cameraDistanceFromTarget - thirdPersonCameraLockOnAttributes.cameraMinLockOnDistance) / (thirdPersonCameraLockOnAttributes.cameraMaxLockOnDistance - thirdPersonCameraLockOnAttributes.cameraMinLockOnDistance);

        newCameraLockOnHeight = Mathf.Clamp(newCameraLockOnHeight, 0, 1f);

        newCameraLockOnHeight = Mathf.Lerp(thirdPersonCameraLockOnAttributes.cameraMinLockOnHeight, thirdPersonCameraLockOnAttributes.cameraMaxLockOnHeight, newCameraLockOnHeight);

        return newCameraLockOnHeight;
    }
}

[System.Serializable]
public class ThirdPersonCameraLockOnAttributes
{
    [Header("Camera LockOn Attributes")]
    public float cameraLockOnRadius = 5f;

    [Space(10)]
    public float cameraMinLockOnDistance = 1f;
    public float cameraMaxLockOnDistance = 5f;

    [Space(10)]
    public float cameraMinLockOnHeight = 1f;
    public float cameraMaxLockOnHeight = 3f;

    [Space(10)]
    public float cameraLockOnMoveSmoothTime = 0.05f;
    public float cameraLockOnRotateSmoothTime = 2f;
}

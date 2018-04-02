using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ActorController : MonoBehaviour
{
    private NavMeshAgent navAgent;

    [Header("Actor Name Attributes")]
    public ACTOR_NAME actorName;

    [Header("Actor Navigation Attributes")]
    public int currentMovementIndex = 0;

    [Space(10)]
    public Transform actorCurrentTargetNavPoint;

    private Transform[] actorTargetPoints;

    [Space(10)]
    public float actorCurrentMoveSpeed = 1f;

    private float[] actorMoveSpeeds;

    [Space(10)]
    public bool canActorMove = true;

    [Space(10)]
    public bool isActorMoving = false;

    private void Start()
    {
        ActorNavigationSetup();
    }

    private void Update()
    {
        ManageNavigation();
    }

    private void ActorNavigationSetup()
    {
        navAgent = GetComponent<NavMeshAgent>();
    }

    private void ManageNavigation()
    {
        if (isActorMoving)
        {
            if (!navAgent.pathPending)
            {
                if (navAgent.remainingDistance <= navAgent.stoppingDistance)
                {
                    if (!navAgent.hasPath || navAgent.velocity.sqrMagnitude == 0f)
                    {
                        ReachedDestination();
                    }
                }
            }
        }
    }

    private void ReachedDestination()
    {
        if (actorTargetPoints.Length > 0)
        {
            if (CanMoveToNextMovementAction())
            {
                actorCurrentMoveSpeed = actorMoveSpeeds[currentMovementIndex];
                navAgent.speed = actorCurrentMoveSpeed;

                actorCurrentTargetNavPoint = actorTargetPoints[currentMovementIndex];
                navAgent.SetDestination(actorCurrentTargetNavPoint.position);
            }
        }
        else
        {
            Debug.Log("Reached Destination");

            isActorMoving = false;

            navAgent.isStopped = true;

            actorCurrentTargetNavPoint = null;
        } 
    }

    public void SetNewNavigationTarget(Transform newActorTargetPoint, float newActorSpeed)
    {
        if (newActorTargetPoint != null)
        {
            actorCurrentMoveSpeed = newActorSpeed;
            navAgent.speed = actorCurrentMoveSpeed;

            actorCurrentTargetNavPoint = newActorTargetPoint;
            navAgent.SetDestination(actorCurrentTargetNavPoint.position);

            isActorMoving = true;
        }
    }

    public void ReceiveNewActorMovementData(ActorMovementData newActorMovementData)
    {
        currentMovementIndex = 0;

        actorTargetPoints = newActorMovementData.actorMovePointTransform;
        actorMoveSpeeds = newActorMovementData.actorMovePointSpeeds;

        SetNewNavigationTarget(actorTargetPoints[0], actorMoveSpeeds[0]);
    }

    public bool CanMoveToNextMovementAction()
    {
        currentMovementIndex++;

        if (currentMovementIndex <= actorTargetPoints.Length - 1)
        {
            return true;
        }

        return false;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MoveToTargetNav : MonoBehaviour
{
    private NavMeshAgent navAgent;

    public Transform targetTransform;

    private void Start()
    {
        AgentSetup();
    }

    private void AgentSetup()
    {
        navAgent = GetComponent<NavMeshAgent>();

        SetNewTarget(targetTransform);
    }

    private void SetNewTarget(Transform target)
    {
        navAgent.SetDestination(target.transform.position);
    }
}

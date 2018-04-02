using System.Collections;
using UnityEngine;

public class CrowdController : MonoBehaviour
{
    [Header("Crowd Member Attributes")]
    public float currentBobTime;

    [Space(10)]
    public float bobHeight = 2f;

    [Space(10)]
    public float crowdBobOffset = 2f;

    [Space(10)]
    public bool canMove = false;

    [Space(10)]
    public bool isMoving = false;

    private void Update()
    {
        MoveCrowd();
    }

    private void MoveCrowd()
    {
        if (canMove)
        {
            transform.position = new Vector3(transform.position.x, crowdBobOffset + Mathf.Sin(Time.time * currentBobTime) * bobHeight, transform.position.z);
        }
    }

    public void StartCrowdBob(float newCrowdSpeed)
    {
        SetNewBobSpeed(newCrowdSpeed);

        canMove = true;
    }

    public void StopCrowBob()
    {    
        canMove = false;

        transform.position = new Vector3(transform.position.x, 2 + Mathf.Sin(Time.time) * currentBobTime, transform.position.z);

        SetNewBobSpeed(0);
    }

    public void SetNewBobSpeed(float newBobTime)
    {
        currentBobTime = newBobTime;
    }
}

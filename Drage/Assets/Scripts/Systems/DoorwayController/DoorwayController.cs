using System.Collections;
using UnityEngine;

public class DoorwayController : MonoBehaviour
{
    private ThirdPersonPlayerController playerController;

    [Header("Doorway Attributes")]
    public Transform targetDoorway;

    [Space(10)]
    public float doorwayForwardPlacement = 1f;

    [Space(10)]
    public float moveWaitTime = 1f;

    [Space(10)]
    public bool canDoorwayBeUsed = true;

    [Header("Game Events")]
    public EventAction[] events;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (canDoorwayBeUsed)
            {
                Transform targetPlayer = other.gameObject.transform;

                playerController = targetPlayer.GetComponent<ThirdPersonPlayerController>();

                PlayEvents();

                StartCoroutine(PositionPlayer(targetPlayer));

                canDoorwayBeUsed = false;
            }
        }
    }

    private void PlayEvents()
    {
        if (events.Length > 0)
        {
            for (int i = 0; i < events.Length; i++)
            {
                events[i].ActivateEvent();
            }
        } 
    }

    private IEnumerator PositionPlayer(Transform targetPlayer)
    {
        if (moveWaitTime > 0)
        {
            playerController.DisablePlayerMovement();

            yield return new WaitForSeconds(moveWaitTime);
        }

        Vector3 newPlayerPosition = targetDoorway.position + (targetDoorway.forward * doorwayForwardPlacement);
        Quaternion newPlayerRotation = targetDoorway.rotation;

        targetPlayer.position = newPlayerPosition;
        targetPlayer.rotation = newPlayerRotation;

        canDoorwayBeUsed = true;

        playerController.EnablePlayerMovement();

        yield return null;
    }
}

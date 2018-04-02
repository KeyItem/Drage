using System.Collections;
using UnityEngine;

public class PlayerPickUpController : MonoBehaviour
{
    [Header("Player Pick Up Attributes")]
    public Transform targetAttachPointOnPlayer;

    [Space(10)]
    public GameObject currentHeldProp;

    [Space(10)]
    public Interactable currentInteractable;

    private Transform currentlyPickedUpObject;

    private Rigidbody currentObjectRigidbody;
    private Collider currentObjectCollider;

    [Space(10)]
    public bool canPlayerPickUpObject = true;

    [Space(10)]
    public bool isPlayerHoldingObject = false;

    public delegate void PlayerPickUp();
    public delegate void PlayerDrop();

    public static event PlayerPickUp OnPlayerPickup;
    public static event PlayerDrop OnPlayerDrop;

    public void PickUpObject(Interactable newObject)
    {
        if (OnPlayerPickup != null)
        {
            OnPlayerPickup();
        }

        currentInteractable = newObject;
        currentHeldProp = newObject.gameObject;
        currentlyPickedUpObject = newObject.transform;

        currentObjectRigidbody = newObject.GetComponent<Rigidbody>();
        currentObjectCollider = newObject.GetComponent<Collider>();

        currentObjectRigidbody.isKinematic = true;
        Physics.IgnoreCollision(currentObjectCollider, GetComponent<Collider>(), true);

        currentlyPickedUpObject.SetParent(targetAttachPointOnPlayer);

        currentlyPickedUpObject.localPosition = Vector3.zero;
        currentlyPickedUpObject.rotation = Quaternion.identity;

        isPlayerHoldingObject = true;
    }

    public void DropObject()
    {
        if (OnPlayerDrop != null)
        {
            OnPlayerDrop();
        }

        Physics.IgnoreCollision(currentObjectCollider, GetComponent<Collider>(), false);

        currentInteractable.StopInteraction();

        currentlyPickedUpObject.SetParent(null);

        currentObjectRigidbody.isKinematic = false;

        currentHeldProp = null;
        currentlyPickedUpObject = null;
        currentObjectRigidbody = null;
        currentObjectCollider = null;

        isPlayerHoldingObject = false;
    }

    public bool CanPlayerPickUpObject()
    {
        if (canPlayerPickUpObject)
        {
            if (!isPlayerHoldingObject)
            {
                return true;
            }
        }

        return false;
    }
}

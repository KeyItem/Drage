using System.Collections;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Header("Base Interactable Object Attributes")]
    public INTERACTABLE_OBJECT_TYPE interactableObjectType;

    [Space(10)]
    public bool isPlaced = true;

    [Space(10)]
    public bool canBeInteractedWith = true;

    [Space(10)]
    public bool isBeingInteractedWith = false;

    public virtual void Interact(GameObject playerInteracting)
    {

    }

    public virtual void InteractWithProp(GameObject objectToInteractWith)
    {

    }

    public virtual void StopInteraction()
    {

    }
}

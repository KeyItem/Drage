using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsInteractable : Interactable
{
    private Rigidbody objectRigidbody;

    [Header("Physics Interactions")]
    public float physicsForceMultiplier = 1f;

    private void Start()
    {
        ObjectSetup();
    }

    private void ObjectSetup()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }

    public override void Interact(GameObject playerInteracting)
    {
        if (isPlaced)
        {
            if (canBeInteractedWith)
            {
                Vector3 interceptVec = (transform.position - playerInteracting.transform.position).normalized;
                interceptVec *= physicsForceMultiplier;

                objectRigidbody.AddForce(interceptVec, ForceMode.Impulse);
            }
        }     
    }
}

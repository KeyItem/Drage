using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairPlayerController : MonoBehaviour
{
    private ChairInputController inputController;

    private Rigidbody chairRigidbody;

    [Header("Player Movement Attributes")]
    public float baseRotationTorque;

    [Space(10)]
    public float baseLiftForce;

    [Space(10)]
    public bool canMove = true;

    [Header("Ground Check Attributes")]
    public float groundRayLength;

    [Space(10)]
    public float groundPadding = 0.5f;

    [Space(10)]
    public LayerMask groundMask;

    private RaycastHit groundRayHit;

    [Space(10)]
    public List<GameObject> groundedChairLegs = new List<GameObject>();

    [Header("Chair Leg Attributes")]
    public List<GameObject> chairLegs = new List<GameObject>(); //[0 - TopLeft, 1 - TopRight, 2 - BottomLeft, 3 - BottomRight]

    [Space(10)]
    private bool isInitialized = false;

    private void Start()
    {
        InitializePlayer();
    }

    private void Update()
    {
        ManageGround();
    }

    private void FixedUpdate()
    {
        ManageMovement();
    }

    private void InitializePlayer() //Initial Player Setup
    {
        inputController = GetComponent<ChairInputController>();

        chairRigidbody = GetComponent<Rigidbody>();

        chairLegs = ReturnChairLegs();

        chairRigidbody.maxAngularVelocity = 1;

        isInitialized = true;
    }

    private void ManageMovement()
    {
        if (isInitialized)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                transform.position = Vector3.zero + Vector3.up;

                transform.rotation = Quaternion.Euler(0, 180, 0);

                chairRigidbody.velocity = Vector3.zero;
                chairRigidbody.angularVelocity = Vector3.zero;
            }

            if (canMove)
            {
                for (int i = 0; i < chairLegs.Count; i++)
                {
                    chairLegs[i].GetComponent<Renderer>().material.color = Color.white;
                }

                if (inputController.topLeftLegAxis > 0)
                {
                    if (groundedChairLegs.Contains(chairLegs[0]))
                    {
                        Vector3 torqueForce = Vector3.up * -baseRotationTorque;

                        Vector3 liftForce = chairLegs[0].transform.position - chairRigidbody.worldCenterOfMass;

                        liftForce = Vector3.Reflect(liftForce, Vector3.up);

                        liftForce.y = baseLiftForce;
                        
                        chairRigidbody.AddForceAtPosition(liftForce * Time.deltaTime, chairLegs[0].transform.position, ForceMode.Impulse);

                        chairRigidbody.AddTorque(torqueForce * Time.deltaTime, ForceMode.Impulse);

                        Debug.DrawRay(chairLegs[0].transform.position, liftForce, Color.yellow);

                        chairLegs[0].GetComponent<Renderer>().material.color = Color.red;
                    }              
                }

                if (inputController.topRightLegAxis > 0)
                {
                    if (groundedChairLegs.Contains(chairLegs[1]))
                    {
                        Vector3 torqueForce = Vector3.up * baseRotationTorque;

                        Vector3 liftForce = chairLegs[1].transform.position - chairRigidbody.worldCenterOfMass;

                        liftForce = Vector3.Reflect(liftForce, Vector3.up);

                        liftForce.y = baseLiftForce;

                        chairRigidbody.AddForceAtPosition(liftForce * Time.deltaTime, chairLegs[1].transform.position, ForceMode.Impulse);

                        chairRigidbody.AddTorque(torqueForce * Time.deltaTime, ForceMode.Impulse);

                        Debug.DrawRay(chairLegs[1].transform.position, liftForce, Color.yellow);

                        chairLegs[1].GetComponent<Renderer>().material.color = Color.red;
                    }         
                }

                if (inputController.bottomLeftLegAxis > 0)
                {
                    if (groundedChairLegs.Contains(chairLegs[2]))
                    {
                        Vector3 torqueForce = Vector3.up * -baseRotationTorque;

                        Vector3 liftForce = chairLegs[2].transform.position - chairRigidbody.worldCenterOfMass;

                        liftForce = Vector3.Reflect(liftForce, Vector3.up);

                        liftForce.y = baseLiftForce;

                        chairRigidbody.AddForceAtPosition(liftForce * Time.deltaTime, chairLegs[2].transform.position, ForceMode.Impulse);

                        chairRigidbody.AddTorque(torqueForce * Time.deltaTime, ForceMode.Impulse);

                        Debug.DrawRay(chairLegs[2].transform.position, liftForce, Color.yellow);

                        chairLegs[2].GetComponent<Renderer>().material.color = Color.red;
                    }
                }

                if (inputController.bottomRightLegAxis > 0)
                {
                    if (groundedChairLegs.Contains(chairLegs[3]))
                    {
                        Vector3 torqueForce = Vector3.up * baseRotationTorque;

                        Vector3 liftForce = chairLegs[3].transform.position - chairRigidbody.worldCenterOfMass;

                        liftForce = Vector3.Reflect(liftForce, Vector3.up);

                        liftForce.y = baseLiftForce;

                        chairRigidbody.AddForceAtPosition(liftForce * Time.deltaTime, chairLegs[3].transform.position, ForceMode.Impulse);

                        chairRigidbody.AddTorque(torqueForce * Time.deltaTime, ForceMode.Impulse);

                        Debug.DrawRay(chairLegs[3].transform.position, liftForce, Color.yellow);

                        chairLegs[3].GetComponent<Renderer>().material.color = Color.red;
                    }                
                }              
            }
        }
    }

    private void ManageGround()
    {
        foreach(GameObject chair in chairLegs)
        {
            if (Physics.Raycast(chair.transform.position + (Vector3.up * groundPadding), -chair.transform.up, out groundRayHit, groundRayLength, groundMask))
            {
                if (!groundedChairLegs.Contains(chair))
                {
                    groundedChairLegs.Add(chair);
                }

                Debug.DrawRay(chair.transform.position + (Vector3.up * groundPadding), -chair.transform.up * groundRayLength, Color.green);
            }
            else
            {
                if (groundedChairLegs.Contains(chair))
                {
                    groundedChairLegs.Remove(chair);
                }

                Debug.DrawRay(chair.transform.position + (Vector3.up * groundPadding), -chair.transform.up * groundRayLength, Color.red);
            }
        }
    }

    private List<GameObject> ReturnChairLegs() //Go through Player looking for chair legs, when found add to List
    {
        List<GameObject> newChairLegs = new List<GameObject>();

        Transform[] allChildObjects = GetComponentsInChildren<Transform>();

        foreach(Transform child in allChildObjects)
        {
            if (child.CompareTag("Leg"))
            {
                newChairLegs.Add(child.gameObject);
            }
        }

        if (newChairLegs.Count == 0)
        {
            Debug.LogError("Error: No Legs found!");
        }

        return newChairLegs;
    }
}

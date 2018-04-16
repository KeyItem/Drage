using System.Collections;
using UnityEngine;

public class ApplyRandomForce : MonoBehaviour, IPooledObject
{
    private Rigidbody objectRigidbody;

    [Header("Random Force Attributes")]
    public float xAxisForce;
    public float yAxisForce;
    public float zAxisForce;

    private void Start()
    {
        InitializeObject();
    }

    public void OnObjectSpawn()
    {
        ApplyRandomForceToObject();
    }

    private void InitializeObject()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }

    private void ApplyRandomForceToObject()
    {
        objectRigidbody = GetComponent<Rigidbody>();

        float newRandomXAxisForce = Random.Range(-xAxisForce, xAxisForce);
        float newRandomYAxisForce = Random.Range(-yAxisForce, yAxisForce);
        float newRandomZAxisForce = Random.Range(-zAxisForce, zAxisForce);

        Vector3 newForceVector = new Vector3(newRandomXAxisForce, newRandomYAxisForce, newRandomZAxisForce);

        objectRigidbody.AddForce(newForceVector, ForceMode.Impulse);
    }
}

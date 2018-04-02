using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    private ObjectPooler objectPooler;

    [Header("Object Spawner Attributes")]
    public string[] requestedObjectTags;

    [Space(10)]
    public bool canSpawnObjects = false;

    private void Start()
    {
        SetupSpawner();
    }

    private void FixedUpdate()
    {
        SpawnObject();
    }

    private void SetupSpawner()
    {
        objectPooler = ObjectPooler.Instance;
    }

    public void SpawnObject()
    {
        if (canSpawnObjects)
        {
            for (int i = 0; i < requestedObjectTags.Length; i++)
            {
                objectPooler.CreateObjectFromPool_Reuseable(requestedObjectTags[i], transform.position, Quaternion.identity);
            }
        }
    }
}

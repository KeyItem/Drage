using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    private static ObjectPooler _instance;

    public static ObjectPooler Instance { get { return _instance; } }

    [Header("Pools")]
    public List<Pool> poolList = new List<Pool>();

    [Header("Pool Dictionaries")]
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        InitializePooler();
    }

    private void Start()
    {
        SetupDictionary();
    }

    private void InitializePooler()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void SetupDictionary()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool targetPool in poolList)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < targetPool.poolSize; i++)
            {
                GameObject newObject = Instantiate(targetPool.poolPrefab, Vector3.zero, Quaternion.identity, transform) as GameObject;
                newObject.SetActive(false);

                objectPool.Enqueue(newObject);
            }

            poolDictionary.Add(targetPool.poolTag, objectPool);
        }
    }
    
    public GameObject CreateObjectFromPool_Reuseable(string objectTag, Vector3 objectPosition, Quaternion objectRotation)
    {
        if (!poolDictionary.ContainsKey(objectTag))
        {
            Debug.LogError("Pool Dictionary Does Not Contain Requested Tag!");

            return null;
        }

        GameObject newObject = poolDictionary[objectTag].Dequeue();

        newObject.transform.position = objectPosition;
        newObject.transform.rotation = objectRotation;

        newObject.SetActive(true);

        IPooledObject pooledObjectInterface = newObject.GetComponent<IPooledObject>();

        if (pooledObjectInterface != null)
        {
            pooledObjectInterface.OnObjectSpawn();
        }

        poolDictionary[objectTag].Enqueue(newObject);

        return newObject;
    }

    public GameObject CreateObjectFromPool(string objectTag, Vector3 objectPosition, Quaternion objectRotation)
    {
        if (!poolDictionary.ContainsKey(objectTag))
        {
            Debug.LogError("Pool Dictionary Does Not Contain Requested Tag!");

            return null;
        }

        if (poolDictionary[objectTag].Count <= 0)
        {
            return null;
        }

        GameObject newObject = poolDictionary[objectTag].Dequeue();

        newObject.transform.position = objectPosition;
        newObject.transform.rotation = objectRotation;

        newObject.SetActive(true);

        IPooledObject pooledObjectInterface = newObject.GetComponent<IPooledObject>();

        if (pooledObjectInterface != null)
        {
            pooledObjectInterface.OnObjectSpawn();
        }

        return newObject;
    }

    public void ReturnObjectToQueue(string objectTag, GameObject targetObject)
    {
        if (!poolDictionary.ContainsKey(objectTag))
        {
            Debug.LogError("Pool Dictionary Does Not Contain Requested Tag!");

            return;
        }

        targetObject.transform.position = Vector3.zero;
        targetObject.transform.rotation = Quaternion.identity;

        targetObject.SetActive(false);

        poolDictionary[objectTag].Enqueue(targetObject);
    }
}

[System.Serializable]
public class Pool
{
    public string poolTag;

    [Space(10)]
    public GameObject poolPrefab;

    [Space(10)]
    public int poolSize;
}


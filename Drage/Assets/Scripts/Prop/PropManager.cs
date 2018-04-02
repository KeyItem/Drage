using System.Collections;
using UnityEngine;

public class PropManager : MonoBehaviour
{
    private static PropManager _instance;
    public static PropManager Instance { get { return _instance; } }

    [Header("Prop Manager Attributes")]
    public PropController[] propsToSetup;

    [Space(10)]
    public PropPoints[] propPoints;

    [Space(10)]
    public Transform propSpawnPoint;

    [Space(10)]
    public bool arePropsSetup = false;

    private void Awake()
    {
        InitializePropManager();
    }

    private void Start()
    {
        PrepareProps();
    }

    private void InitializePropManager()
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

    private void PrepareProps()
    {
        for (int i = 0; i < propsToSetup.Length; i++)
        {
            propsToSetup[i].transform.position = propSpawnPoint.position;

            propsToSetup[i].gameObject.SetActive(false);
        }
    }

    public void StartPropSetup()
    {
        for (int i = 0; i < propsToSetup.Length; i++)
        {
            if (!propsToSetup[i].isPropInitialized)
            {
                SetupProp(propsToSetup[i], propPoints[i]);
            }
        }
    }

    public void SetupTargetProp(int propIndex)
    {
        SetupProp(propsToSetup[propIndex], propPoints[propIndex]);
    }

    private void SetupProp(PropController propController, PropPoints newPropPoints)
    {
        propController.gameObject.SetActive(true);

        propController.StartSetup(newPropPoints);
    }
}

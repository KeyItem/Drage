using System.Collections;
using UnityEngine;

public class MarbleController : MonoBehaviour
{
    private static MarbleController _instance;
    public static MarbleController Instance { get { return _instance; } }

    [Header("Marble Positions")]
    public Transform[] marbleSpawnPoints;

    private int previousIndex = 0;

    private void Awake()
    {
        InitializeMarbleController();
    }

    private void InitializeMarbleController()
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

    public void RequestNewMarbleDrop()
    {
        GameObject newMarble = RequestNewMarble();

        Vector3 newMarblePosition = ReturnNewMarbleSpawnPoint();

        newMarble.transform.position = newMarblePosition;
    }

    private GameObject RequestNewMarble()
    {
        GameObject newMarble = ObjectPooler.Instance.CreateObjectFromPool_Reuseable("Marble", Vector3.zero, Quaternion.identity);

        return newMarble;
    }

    private Vector3 ReturnNewMarbleSpawnPoint()
    {
        int newIndex = Random.Range(0, marbleSpawnPoints.Length);

        if (newIndex == previousIndex)
        {
            while (newIndex == previousIndex)
            {
                newIndex = Random.Range(0, marbleSpawnPoints.Length);
            }
        }

        previousIndex = newIndex;

        return marbleSpawnPoints[previousIndex].position;
    }
}

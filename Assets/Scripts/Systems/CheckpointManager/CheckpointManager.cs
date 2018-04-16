using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private static CheckpointManager _instance;
    public static CheckpointManager Instance { get { return _instance; } }

    [Header("Checkpoint Attributes")]
    public Transform[] checkpoints;

    private Vector2[] checkpointPositions;

    [Space(10)]
    public Vector2 currentCheckpoint;
    public Vector2 nextCheckpoint;

    [Space(10)]
    public int checkpointIndex;
    public int nextCheckpointIndex;

    [Space(10)]
    public bool isAtLastCheckpoint = false;

    [Space(10)]
    public float minTargetDistanceToCheckpoint = 1f;

    [Header("Target Attributes")]
    public Transform targetTransform;

    private void Awake()
    {
        InitializeCheckpointManager();
    }

    private void Start()
    {
        CheckpointsSetup();
    }

    private void Update()
    {
        ManageCheckpoints();
    }

    private void InitializeCheckpointManager()
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

    private void CheckpointsSetup()
    {
        ReadCheckpointData();

        checkpointIndex = 0;
        nextCheckpointIndex = 1;

        currentCheckpoint = checkpointPositions[checkpointIndex];
        nextCheckpoint = checkpointPositions[nextCheckpointIndex];
    }

    private void ManageCheckpoints()
    {
        if (currentCheckpoint != null && nextCheckpoint != null)
        {
            if (!isAtLastCheckpoint)
            {
                float checkpointDistance = ReturnCheckpointDistance(nextCheckpoint);

                Debug.DrawLine(targetTransform.position, nextCheckpoint, Color.yellow);

                if (checkpointDistance < minTargetDistanceToCheckpoint)
                {
                    MoveToNextCheckpoint();
                }
            }
        }
    }

    private void ReadCheckpointData()
    {
        checkpointPositions = new Vector2[checkpoints.Length];

        for (int i = 0; i < checkpointPositions.Length; i++)
        {
            checkpointPositions[i] = checkpoints[i].position;
        }
    }

    private void MoveToNextCheckpoint()
    {
        checkpointIndex++;
        
        if (checkpointIndex < checkpointPositions.Length - 1)
        {
            currentCheckpoint = checkpointPositions[checkpointIndex];

            nextCheckpointIndex = (nextCheckpointIndex + 1);

            if (nextCheckpointIndex < checkpointPositions.Length - 1)
            {
                nextCheckpoint = checkpointPositions[nextCheckpointIndex];
            }
            else
            {
                nextCheckpointIndex = checkpointPositions.Length - 1;

                nextCheckpoint = checkpointPositions[nextCheckpointIndex];
            }
        }
        else
        {
            checkpointIndex = checkpointPositions.Length - 1;
            nextCheckpointIndex = checkpointIndex;

            currentCheckpoint = checkpointPositions[checkpointIndex];
            nextCheckpoint = checkpointPositions[nextCheckpointIndex];

            isAtLastCheckpoint = true;
        }
    }

    private float ReturnCheckpointDirection(Vector2 targetCheckpoint)
    {
        Vector2 interceptVector = ((Vector2)targetTransform.position - targetCheckpoint).normalized;

        return interceptVector.x;
    }

    private float ReturnCheckpointDistance(Vector2 targetCheckpoint)
    {
        float checkpointDistance = Vector2.Distance(targetTransform.position, targetCheckpoint);

        return checkpointDistance;
    }
}

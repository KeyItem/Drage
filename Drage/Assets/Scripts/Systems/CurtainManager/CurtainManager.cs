using System.Collections;
using UnityEngine;

public class CurtainManager : MonoBehaviour
{
    private static CurtainManager _instance;
    public static CurtainManager Instance { get { return _instance; } }

    [Header("Curtain Manager Attributes")]
    public Transform leftCurtainTransform;
    public Transform rightCurtainTransform;

    [Space(10)]
    public Transform[] leftSideCurtainPositions;
    public Transform[] rightSideCurtainPositions;

    [Space(10)]
    public float curtainOpenTime = 1f;
    public float curtainCloseTime = 2f;

    [Space(10)]
    public bool isCurtainOpen = false;

    private IEnumerator CurtainMover = null;

    [Header("DEBUG")]
    public bool isCurtainMoving = false;

    private void Awake()
    {
        InitializeCurtainManager();
    }

    private void InitializeCurtainManager()
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

    public void MoveCurtain()
    {
        if (CurtainMover != null)
        {
            StopCoroutine(CurtainMover);
        }

        if (isCurtainOpen)
        {
            CurtainMover = StartMovingCurtains(curtainCloseTime, true);

            isCurtainOpen = false;
        }
        else
        {
            CurtainMover = StartMovingCurtains(curtainOpenTime, false);

            isCurtainOpen = true;
        }

        StartCoroutine(CurtainMover);
    }

    private IEnumerator StartMovingCurtains(float moveTime, bool isOpen)
    {
        isCurtainMoving = true;

        Vector3 currentLeftCurtainPosition = leftCurtainTransform.position;
        Vector3 currentRightCurtainPosition = rightCurtainTransform.position;

        Vector3 targetLeftCurtainPosition = Vector3.zero;
        Vector3 targetRightCurtainPosition = Vector3.zero;

        if (isOpen) //Get Close Position
        {
            targetLeftCurtainPosition = leftSideCurtainPositions[1].position;
            targetRightCurtainPosition = rightSideCurtainPositions[1].position;
        }
        else //Get Open Position
        {
            targetLeftCurtainPosition = leftSideCurtainPositions[0].position;
            targetRightCurtainPosition = rightSideCurtainPositions[0].position;
        }

        if (moveTime > 0f)
        {
            float newMoveRatio = 0f;

            while(newMoveRatio < 1f)
            {
                newMoveRatio += Time.deltaTime / moveTime;

                if (newMoveRatio > 1f)
                {
                    newMoveRatio = 1f;
                }

                Vector3 newLeftCurtainPosition = Vector3.Lerp(currentLeftCurtainPosition, targetLeftCurtainPosition, newMoveRatio);
                Vector3 newRightCurtainPosition = Vector3.Lerp(currentRightCurtainPosition, targetRightCurtainPosition, newMoveRatio);

                leftCurtainTransform.position = newLeftCurtainPosition;
                rightCurtainTransform.position = newRightCurtainPosition;

                yield return new WaitForEndOfFrame();
            }
        }

        CurtainMover = null;

        isCurtainMoving = false;

        yield return null;
    }
}

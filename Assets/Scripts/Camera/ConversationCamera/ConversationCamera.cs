using System.Collections;
using UnityEngine;

public class ConversationCamera : MonoBehaviour
{
    private Camera conversationCam;

    [Header("Coversation Attributes")]
    public ConversationSystem currentConversation = null;

    [Space(10)]
    public DialogData currentConversationData = null;

    [Header("Conversation Camera Attributes")]
    public ConversationCameraEvent currentCameraAction = null;

    [Space(10)]
    private IEnumerator CurrentCameraActionCoroutine = null;

    private void Start()
    {
        InitializeCamera();
    }

    private void InitializeCamera()
    {
        conversationCam = GetComponent<Camera>();
        conversationCam.enabled = false;
    }

    public void ImportConversation(ConversationSystem newConversation)
    {
        currentConversation = newConversation;
    }

    public void ImportCoversationData(DialogData newConversationData)
    {
        currentConversationData = newConversationData;
    }

    public void ImportConversationCameraAction(ConversationCameraEvent newConversationCameraAction)
    {
        currentCameraAction = newConversationCameraAction;
    }

    public void StartConversation()
    {
        conversationCam.enabled = true;
    }

    public void EndConversation()
    {
        conversationCam.enabled = false;

        currentConversation = null;
        currentConversationData = null;
        currentCameraAction = null;
        CurrentCameraActionCoroutine = null;
    }

    public void StartCameraEffect(int conversationIndex)
    {
        currentCameraAction = currentConversationData.conversationCameraEvents[conversationIndex];

        if (CurrentCameraActionCoroutine != null)
        {
            Debug.Log("Stopping Current Camera Effect");

            StopCoroutine(CurrentCameraActionCoroutine);
        }

        switch (currentCameraAction.conversationCameraAction)
        {
            case CONVERSATION_CAMERA_ACTION.DIRECT:
                CurrentCameraActionCoroutine = MoveCameraDirect(conversationIndex);

                StartCoroutine(CurrentCameraActionCoroutine);
                break;

            case CONVERSATION_CAMERA_ACTION.MOVETO:
                CurrentCameraActionCoroutine = MoveCamera(currentCameraAction.conversationCameraMoveType, conversationIndex);

                StartCoroutine(CurrentCameraActionCoroutine);
                break;

            case CONVERSATION_CAMERA_ACTION.ROTATE:
                CurrentCameraActionCoroutine = RotateCamera(currentCameraAction.conversationCameraMoveType, conversationIndex);

                StartCoroutine(CurrentCameraActionCoroutine);
                break;

            case CONVERSATION_CAMERA_ACTION.MOVE_AND_ROTATE:
                CurrentCameraActionCoroutine = MoveAndRotateCamera(currentCameraAction.conversationCameraMoveType, conversationIndex);

                StartCoroutine(CurrentCameraActionCoroutine);
                break;
        }
    }

    private IEnumerator MoveCameraDirect(int conversationIndex)
    {
        transform.position = currentConversation.conversationCameraTransforms[conversationIndex].position;
        transform.rotation = currentConversation.conversationCameraTransforms[conversationIndex].rotation;

        CurrentCameraActionCoroutine = null;

        yield return null;
    }

    private IEnumerator MoveCamera(CONVERSATION_CAMERA_MOVETYPE cameraMoveType, int conversationIndex)
    {
        Transform targetTransform = currentConversation.conversationCameraTransforms[conversationIndex];

        Vector3 currentPosition = transform.position;

        float currentMoveTime = 0f;
        
        while (currentMoveTime < 1f)
        {
            currentMoveTime += Time.deltaTime / currentCameraAction.cameraMoveTime;

            if (currentMoveTime > 1f)
            {
                currentMoveTime = 1f;
            }

            if (cameraMoveType == CONVERSATION_CAMERA_MOVETYPE.LERP)
            {
                transform.position = Vector3.Lerp(currentPosition, targetTransform.position, currentMoveTime);
            }
            else if (cameraMoveType == CONVERSATION_CAMERA_MOVETYPE.SLERP)
            {
                transform.position = Vector3.Slerp(currentPosition, targetTransform.position, currentMoveTime);
            }

            yield return new WaitForEndOfFrame();
        }

        transform.position = currentConversation.conversationCameraTransforms[conversationIndex].position;

        CurrentCameraActionCoroutine = null;

        yield return null;
    }

    private IEnumerator RotateCamera(CONVERSATION_CAMERA_MOVETYPE cameraMoveType, int conversationIndex)
    {
        Transform targetTransform = currentConversation.conversationCameraTransforms[conversationIndex];

        Quaternion currentRotation = transform.rotation;

        float currentRotateTime = 0f;

        while (currentRotateTime < 1f)
        {
            currentRotateTime += Time.deltaTime / currentCameraAction.cameraRotateTime;

            if (currentRotateTime > 1f)
            {
                currentRotateTime = 1f;
            }

            if (cameraMoveType == CONVERSATION_CAMERA_MOVETYPE.LERP)
            {
                transform.rotation = Quaternion.Lerp(currentRotation, targetTransform.rotation, currentRotateTime);
            }
            else if (cameraMoveType == CONVERSATION_CAMERA_MOVETYPE.SLERP)
            {
                transform.rotation = Quaternion.Slerp(currentRotation, targetTransform.rotation, currentRotateTime);
            }

            yield return new WaitForEndOfFrame();
        }

        transform.rotation = currentConversation.conversationCameraTransforms[conversationIndex].rotation;

        CurrentCameraActionCoroutine = null;

        yield return null;
    }

    private IEnumerator MoveAndRotateCamera(CONVERSATION_CAMERA_MOVETYPE cameraMoveType, int conversationIndex)
    {
        Transform targetTransform = currentConversation.conversationCameraTransforms[conversationIndex];

        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        float currentMoveTime = 0f;
        float currentRotateTime = 0f;

        while (currentMoveTime < 1f && currentRotateTime < 1f)
        {
            currentMoveTime += Time.deltaTime / currentCameraAction.cameraMoveTime;
            currentRotateTime += Time.deltaTime / currentCameraAction.cameraRotateTime;

            if (currentMoveTime > 1f)
            {
                currentMoveTime = 1f;
            }

            if (currentRotateTime > 1f)
            {
                currentRotateTime = 1f;
            }

            if (cameraMoveType == CONVERSATION_CAMERA_MOVETYPE.LERP)
            {
                transform.position = Vector3.Lerp(currentPosition, targetTransform.position, currentMoveTime);
                transform.rotation = Quaternion.Lerp(currentRotation, targetTransform.rotation, currentRotateTime);
            }
            else if (cameraMoveType == CONVERSATION_CAMERA_MOVETYPE.SLERP)
            {
                transform.position = Vector3.Slerp(currentPosition, targetTransform.position, currentMoveTime);
                transform.rotation = Quaternion.Slerp(currentRotation, targetTransform.rotation, currentRotateTime);
            }

            yield return new WaitForEndOfFrame();
        }

        transform.position = currentConversation.conversationCameraTransforms[conversationIndex].position;
        transform.rotation = currentConversation.conversationCameraTransforms[conversationIndex].rotation;

        CurrentCameraActionCoroutine = null;

        yield return null;
    }
}

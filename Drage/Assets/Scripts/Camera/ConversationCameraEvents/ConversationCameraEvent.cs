using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Conversation Camera Event", menuName = "Conversation/Camera Event", order = 1202)]
public class ConversationCameraEvent : ScriptableObject
{
    [Header("Conversation Camera Action Type")]
    public CONVERSATION_CAMERA_ACTION conversationCameraAction;

    [Header("Camera Action Attributes")]
    public CONVERSATION_CAMERA_MOVETYPE conversationCameraMoveType;

    [Space(10)]
    public float cameraMoveTime = 1f;
    public float cameraRotateTime = 1f;
}

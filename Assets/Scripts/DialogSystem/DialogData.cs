using System.Collections;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Conversation Data", menuName = "Conversation/Coversation Data", order = 1201)]
public class DialogData : ScriptableObject
{
    [Header("Conversation Dialog Attributes")]
    public DialogLine[] dialogLines;

    [Header("Helper Dialog Attributes")]
    public DialogHelp[] dialogHelp;

    [Header("Conversation Camera Actions")]
    public ConversationCameraEvent[] conversationCameraEvents;
}

[System.Serializable]
public class DialogLine
{
    public string dialogLine;

    [Space(10)]
    public ACTOR_NAME speakerName;
}

[System.Serializable]
public class DialogHelp
{
    public string[] helperDialog;

    [Space(10)]
    public float[] helperDialogTime;

    [Space(10)]
    public ACTOR_NAME[] speakerNames;
}

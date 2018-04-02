using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ConversationDialogDisplay : MonoBehaviour
{
    [Header("Display Attributes")]
    public GameObject dialogBoxObject;

    [Space(10)]
    public Text dialogueTextUI;
    public Text dialogueSpeakerTextUI;

    private IEnumerator CurrentScrollingText = null;

    [Space(10)]
    public bool isDialogBoxActive = false;

    [Header("Dialogue Attributes")]
    private DialogData currentConversationData;

    [Header("Dialogue Display Attributes")]
    public float dialogueDisplaySpeed = 0.02f;

    [Header("Dialogue Speaker Attributes")]
    public Color[] dialogueSpeakerNameColors;
    public Color[] dialogueSpeakerTextColors;

    public void ActivateDialogBox()
    {
        isDialogBoxActive = true;

        dialogBoxObject.SetActive(true);
    }

    public void DeactivateDialogBox()
    {
        isDialogBoxActive = false;

        dialogBoxObject.SetActive(false);
    }

    public void ImportConversationData(DialogData newConversationData)
    {
        currentConversationData = newConversationData;
    }

    private void SetConversationSpeaker(int conversationIndex)
    {
        string currentSpeaker = ActorInfo.Instance.ReturnActorName(currentConversationData.dialogLines[conversationIndex].speakerName);

        Color[] speakerColors = ReturnSpeakerColor(currentSpeaker);

        dialogueSpeakerTextUI.color = speakerColors[0];
        dialogueTextUI.color = speakerColors[1];

        dialogueSpeakerTextUI.text = currentSpeaker;
    }

    private Color[] ReturnSpeakerColor(string speakerName)
    {
        Color[] newColor = new Color[2] { Color.black, Color.black };

        /*
        switch (speakerName)
        {
            case "Matt":
                newColor[0] = dialogueSpeakerNameColors[0];
                newColor[1] = dialogueSpeakerTextColors[0];
                break;

            case "Ryon":
                newColor[0] = dialogueSpeakerNameColors[1];
                newColor[1] = dialogueSpeakerTextColors[1];
                break;

            case "Van":
                newColor[0] = dialogueSpeakerNameColors[2];
                newColor[1] = dialogueSpeakerTextColors[2];
                break;
        }
        */

        return newColor;
    }

    public void StartConversation()
    {
        ActivateDialogBox();
    }

    public void EndConversation()
    {
        currentConversationData = null;

        DeactivateDialogBox();
    }

    public void ReadConversationDialog(int conversationIndex)
    {
        ActivateDialogBox();

        SetConversationSpeaker(conversationIndex);
        ReadTargetDialogueIndex(conversationIndex);
    }

    private void ReadTargetDialogueIndex(int conversationIndex)
    {
        string newDialogText = currentConversationData.dialogLines[conversationIndex].dialogLine;

        if (CurrentScrollingText != null)
        {
            StopCoroutine(CurrentScrollingText);

            QuickDisplayText(newDialogText);
        }
        else
        {
            CurrentScrollingText = ScrollTargetText(newDialogText);

            StartCoroutine(CurrentScrollingText);
        }
    }

    private void QuickDisplayText(string dialogText)
    {
        dialogueTextUI.text = dialogText;
    }

    public void ClearDialogBox()
    {
        dialogueTextUI.text = string.Empty;
        dialogueSpeakerTextUI.text = string.Empty;
    }

    private IEnumerator ScrollTargetText(string targetText)
    {
        dialogueTextUI.text = "";

        if (dialogueDisplaySpeed > 0)
        {
            foreach (char letter in targetText.ToCharArray())
            {
                dialogueTextUI.text += letter;

                yield return new WaitForSeconds(dialogueDisplaySpeed);
            }
        }
        else
        {
            QuickDisplayText(targetText);
        }
    
        CurrentScrollingText = null;

        yield return null;
    }
}

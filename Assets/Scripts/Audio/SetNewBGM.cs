using System.Collections;
using UnityEngine;

public class SetNewBGM : MonoBehaviour
{
    [Header("BGM Attributes")]
    public AudioClip newBGM;

    private void Start()
    {
        SetNewLevelBGM();
    }

    private void SetNewLevelBGM()
    {
        if (newBGM != null)
        {
            AudioManager.Instance.StopAllAudioSourceOfType(AUDIO_SOURCE_TYPE.MUSIC);

            AudioManager.Instance.RequestNewAudioSource(AUDIO_SOURCE_TYPE.MUSIC, newBGM);
        }
    }
}

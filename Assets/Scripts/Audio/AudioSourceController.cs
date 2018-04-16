using System.Collections;
using UnityEngine;

public class AudioSourceController : MonoBehaviour
{
    [Header("Audio Source Controller Attributes")]
    public AudioSource targetAudioSource;

    private AudioClip targetAudio;

    [Space(10)]
    public AUDIO_SOURCE_TYPE audioSourceType;
    private string audioSourceNameType;

    [Space(10)]
    public AUDIO_SOURCE_STATE audioSourceState;

    private void Start()
    {
        AudioSourceControllerSetup();
    }

    private void Update()
    {
        ManageAudioSourceStatus();
    }

    private void AudioSourceControllerSetup()
    {
        if (targetAudioSource == null)
        {
            targetAudioSource = GetComponent<AudioSource>();
        }
    }

    private void ManageAudioSourceStatus()
    {
        if (audioSourceState == AUDIO_SOURCE_STATE.PLAYING)
        {
            if (!targetAudioSource.isPlaying)
            {
                StopClip();
            }
        }
    }

    public void LoadNewAudioData(AUDIO_SOURCE_TYPE targetAudioSourceType, AudioClip targetAudioClip)
    {
        if (targetAudioClip != null)
        {
            audioSourceType = targetAudioSourceType;

            targetAudio = targetAudioClip;
        }
    }

    public void StartClip()
    {
        if (targetAudio != null)
        {
            audioSourceNameType = ReturnAudioSourceNameType(audioSourceType);

            audioSourceState = AUDIO_SOURCE_STATE.PLAYING;

            targetAudioSource.clip = targetAudio;

            targetAudioSource.Play();
        }       
    }

    public void PauseClip()
    {
        if (targetAudio != null)
        {
            audioSourceState = AUDIO_SOURCE_STATE.PAUSED;

            targetAudioSource.Pause();
        }
    }

    public void StopClip()
    {
        if (targetAudio != null)
        {
            audioSourceState = AUDIO_SOURCE_STATE.WAITING;

            targetAudioSource.Stop();

            targetAudioSource.clip = null;

            targetAudio = null;

            audioSourceType = AUDIO_SOURCE_TYPE.NONE;

            AudioManager.Instance.RemoveActiveAudioSource(this);

            ObjectPooler.Instance.ReturnObjectToQueue(audioSourceNameType, gameObject);

            audioSourceNameType = string.Empty;
        }
    }

    private string ReturnAudioSourceNameType(AUDIO_SOURCE_TYPE audioSourceType)
    {
        string sourceNameType = "";

        switch (audioSourceType)
        {
            case AUDIO_SOURCE_TYPE.MUSIC:
                sourceNameType = "MusicAudioSource";
                break;

            case AUDIO_SOURCE_TYPE.SOUND_EFFECT:
                sourceNameType = "SoundEffectAudioSource";
                break;

            case AUDIO_SOURCE_TYPE.DIALOGUE:
                sourceNameType = "DialogueAudioSource";
                break;

           default:
                Debug.LogError("Incorrect Parameter passed in ReturnAudioSourceTypeName, was :: " + audioSourceType);
                break;
        }

        return sourceNameType;
    }
}

public enum AUDIO_SOURCE_STATE
{
    NONE,
    WAITING,
    PLAYING,
    PAUSED
}

public enum AUDIO_SOURCE_TYPE
{
    NONE,
    MUSIC,
    SOUND_EFFECT,
    DIALOGUE
}

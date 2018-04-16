using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;

    public static AudioManager Instance { get { return _instance; } }

    [Header("Audio Sources Attributes")]
    public List<AudioSourceController> activeMusicAudioSourceControllers = new List<AudioSourceController>();

    [Space(10)]
    public List<AudioSourceController> activeSoundEffectAudioSourceControllers = new List<AudioSourceController>();

    [Space(10)]
    public List<AudioSourceController> activeDialogueAudioSourceControllers = new List<AudioSourceController>();

    private void Awake()
    {
        InitializeAudioManager();
    }

    private void InitializeAudioManager()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PauseAllActiveSources()
    {
        foreach(AudioSourceController audioSourceController in activeMusicAudioSourceControllers)
        {
            audioSourceController.PauseClip();
        }

        foreach (AudioSourceController audioSourceController in activeSoundEffectAudioSourceControllers)
        {
            audioSourceController.PauseClip();
        }

        foreach (AudioSourceController audioSourceController in activeDialogueAudioSourceControllers)
        {
            audioSourceController.PauseClip();
        }
    }

    public void StopAllAudioSourceOfType(AUDIO_SOURCE_TYPE audioSourceType)
    {
        switch(audioSourceType)
        {
            case AUDIO_SOURCE_TYPE.MUSIC:
                foreach (AudioSourceController audioSourceController in activeMusicAudioSourceControllers)
                {
                    audioSourceController.StopClip();
                }
                break;

            case AUDIO_SOURCE_TYPE.SOUND_EFFECT:
                foreach (AudioSourceController audioSourceController in activeSoundEffectAudioSourceControllers)
                {
                    audioSourceController.StopClip();
                }
                break;

            case AUDIO_SOURCE_TYPE.DIALOGUE:
                foreach (AudioSourceController audioSourceController in activeDialogueAudioSourceControllers)
                {
                    audioSourceController.StopClip();
                }
                break;
        }
    }

    public void StopAllActiveSources()
    {
        foreach (AudioSourceController audioSourceController in activeMusicAudioSourceControllers)
        {
            audioSourceController.StopClip();
        }

        foreach (AudioSourceController audioSourceController in activeSoundEffectAudioSourceControllers)
        {
            audioSourceController.StopClip();
        }

        foreach (AudioSourceController audioSourceController in activeDialogueAudioSourceControllers)
        {
            audioSourceController.StopClip();
        }
    }

    public void RequestNewAudioSource(AUDIO_SOURCE_TYPE audioSourceType, AudioClip targetAudioClip)
    {
        if (targetAudioClip != null)
        {
            AudioSourceController newAudioSourceController = ReturnNewAudioSourceController(audioSourceType);

            newAudioSourceController.LoadNewAudioData(audioSourceType, targetAudioClip);

            newAudioSourceController.StartClip();

            AddNewActiveAudioSource(newAudioSourceController);
        }      
    }

    private AudioSourceController ReturnNewAudioSourceController(AUDIO_SOURCE_TYPE audioSourceType)
    {
        GameObject newAudioSource = null;
        AudioSourceController newAudioSourceController = null;

        switch (audioSourceType)
        {
            case AUDIO_SOURCE_TYPE.MUSIC:
                newAudioSource = ObjectPooler.Instance.CreateObjectFromPool_Reuseable("MusicAudioSource", Vector3.zero, Quaternion.identity);
                break;

            case AUDIO_SOURCE_TYPE.SOUND_EFFECT:
                newAudioSource = ObjectPooler.Instance.CreateObjectFromPool_Reuseable("SoundEffectAudioSource", Vector3.zero, Quaternion.identity);

                break;

            case AUDIO_SOURCE_TYPE.DIALOGUE:
                newAudioSource = ObjectPooler.Instance.CreateObjectFromPool_Reuseable("DialogueAudioSource", Vector3.zero, Quaternion.identity);
                break;
        }

        newAudioSourceController = newAudioSource.GetComponent<AudioSourceController>();

        return newAudioSourceController;
    } 

    public void AddNewActiveAudioSource(AudioSourceController targetAudioSourceController)
    {
        switch (targetAudioSourceController.audioSourceType)
        {
            case AUDIO_SOURCE_TYPE.MUSIC:
                activeMusicAudioSourceControllers.Add(targetAudioSourceController);
                break;

            case AUDIO_SOURCE_TYPE.SOUND_EFFECT:
                activeSoundEffectAudioSourceControllers.Add(targetAudioSourceController);
                break;

            case AUDIO_SOURCE_TYPE.DIALOGUE:
                activeDialogueAudioSourceControllers.Add(targetAudioSourceController);
                break;
        }
    }

    public void RemoveActiveAudioSource(AudioSourceController targetAudioSourceController)
    {
        switch (targetAudioSourceController.audioSourceType)
        {
            case AUDIO_SOURCE_TYPE.MUSIC:
                activeMusicAudioSourceControllers.Remove(targetAudioSourceController);
                break;

            case AUDIO_SOURCE_TYPE.SOUND_EFFECT:
                activeSoundEffectAudioSourceControllers.Remove(targetAudioSourceController);
                break;

            case AUDIO_SOURCE_TYPE.DIALOGUE:
                activeDialogueAudioSourceControllers.Remove(targetAudioSourceController);
                break;
        }
    }
}

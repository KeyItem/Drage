using System.Collections;
using UnityEngine;

public class EntityAudioController2D : MonoBehaviour
{
    [Header("Entity Audio Clips")]
    public EntityAudioAttributes2D entityAudioClips;

    public void RequestAudioEvent(string eventName)
    {
        AudioClip targetAudioClip = RetrieveTargetClip(eventName);

        AudioManager.Instance.RequestNewAudioSource(AUDIO_SOURCE_TYPE.SOUND_EFFECT, targetAudioClip);
    }

    private AudioClip RetrieveTargetClip(string clipName)
    {
        for (int i = 0; i < entityAudioClips.entityAudio.Length; i++)
        {
            if (entityAudioClips.entityAudio[i].name == clipName)
            {
                return entityAudioClips.entityAudio[i].clip;
            }
        }

        return null;
    }
}

[System.Serializable]
public struct EntityAudioAttributes2D
{
    [Header("Entity Audio Attributes")]
    public EntityAudioClipAttributes[] entityAudio;
}

[System.Serializable]
public struct EntityAudioClipAttributes
{
    [Header("Entity Audio Clip Attributes")]
    public string name;

    [Space(10)]
    public AudioClip clip;
}

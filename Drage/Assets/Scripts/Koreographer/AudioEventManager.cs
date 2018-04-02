using System.Collections;
using UnityEngine;
using SonicBloom.Koreo;

public class AudioEventManager : MonoBehaviour
{
	private void Awake ()
    {
        AudioEventManagerSetup();
    }

    private void AudioEventManagerSetup()
    {
        Koreographer.Instance.RegisterForEvents("MarbleDropEvent", RequestMarbleDrop);
        Koreographer.Instance.RegisterForEvents("ShakeEvent", RequestNewPlatformColor);
        Koreographer.Instance.RegisterForEvents("SmashEvent", RequestNewScreenShake);
    }

    private void RequestMarbleDrop(KoreographyEvent koreoEvent)
    {
        MarbleController.Instance.RequestNewMarbleDrop();
    }

    private void RequestNewPlatformColor(KoreographyEvent koreoEvent)
    {
        MarblePlatformController.Instance.ChangePlatformColor();
    }

    private void RequestNewScreenShake(KoreographyEvent koreoEvent)
    {
        Camera.main.backgroundColor = Random.ColorHSV();
    }
}

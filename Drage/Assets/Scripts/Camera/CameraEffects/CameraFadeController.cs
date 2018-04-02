using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraFadeController : MonoBehaviour
{
    [Header("Camera Fading Attributes")]
    [Tooltip("The Image that is being faded")]
    public Image cameraFadeImage;

    private Color baseImageColor;

    [Space(10)]
    public bool isFading = false;

    [Header("Camera Fade Settings")]
    public CameraFadeAttributes currentCameraFadeAttributes;

    private IEnumerator currentFadeCoroutine = null;

    private void Start()
    {
        baseImageColor = cameraFadeImage.color;
    }

    public void StartFade(CameraFadeAttributes targetFadeAttributes)
    {
        cameraFadeImage.gameObject.SetActive(true);

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        currentFadeCoroutine = BasicImageFade(targetFadeAttributes);

        StartCoroutine(currentFadeCoroutine);
    }

    private void EndFade()
    {
        cameraFadeImage.gameObject.SetActive(false);
    }

    private IEnumerator BasicImageFade(CameraFadeAttributes targetFadeAttributes)
    {
        currentCameraFadeAttributes = targetFadeAttributes;

        Color currentColor = Color.white; //Setting default values for Fade Colors
        Color targetFadeColor = Color.black;

        float currentFadeValue = 0f; //Setting default values for fade variables
        float fadeDuration = 0f;

        isFading = true;

        switch (targetFadeAttributes.cameraFadeType)
        {
            case CAMERA_FADE_TYPE.IN:
                fadeDuration = targetFadeAttributes.fadeInTime;
                currentColor = baseImageColor;
                targetFadeColor = Color.clear;
                break;

            case CAMERA_FADE_TYPE.OUT:
                fadeDuration = targetFadeAttributes.fadeOutTime;
                currentColor = Color.clear;
                targetFadeColor = baseImageColor;
                break;

            case CAMERA_FADE_TYPE.FADE_IN_THEN_OUT:
                fadeDuration = targetFadeAttributes.fadeInTime;
                currentColor = baseImageColor;
                targetFadeColor = Color.clear;
                break;

            case CAMERA_FADE_TYPE.FADE_OUT_THEN_IN:
                fadeDuration = targetFadeAttributes.fadeOutTime;
                currentColor = Color.clear;
                targetFadeColor = baseImageColor;
                break;

            case CAMERA_FADE_TYPE.NONE:
                yield return null;
                break;
        }

        while(currentFadeValue < 1)
        {
            currentFadeValue += Time.deltaTime / fadeDuration;

            if (currentFadeValue > 1)
            {
                currentFadeValue = 1;
            }

            cameraFadeImage.color = Color.Lerp(currentColor, targetFadeColor, currentFadeValue);

            yield return new WaitForEndOfFrame();
        }

        if (targetFadeAttributes.fadeTransitionWaitTime > 0)
        {
            yield return new WaitForSeconds(targetFadeAttributes.fadeTransitionWaitTime);
        }

        switch(targetFadeAttributes.cameraFadeType)
        {
            case CAMERA_FADE_TYPE.FADE_IN_THEN_OUT:
                currentFadeValue = 0;
                fadeDuration = targetFadeAttributes.fadeOutTime;
                currentColor = Color.clear;
                targetFadeColor = baseImageColor;
                break;

            case CAMERA_FADE_TYPE.FADE_OUT_THEN_IN:
                currentFadeValue = 0;
                fadeDuration = targetFadeAttributes.fadeInTime;
                currentColor = baseImageColor;
                targetFadeColor = Color.clear;
                break;
        }

        if (currentFadeValue == 0)
        {
            while (currentFadeValue < 1)
            {
                currentFadeValue += Time.deltaTime / fadeDuration;

                if (currentFadeValue > 1)
                {
                    currentFadeValue = 1;
                }

                cameraFadeImage.color = Color.Lerp(currentColor, targetFadeColor, currentFadeValue);

                yield return new WaitForEndOfFrame();
            }
        }

        currentFadeCoroutine = null;

        isFading = false;

        EndFade();

        yield return null;
    }
}

[System.Serializable]
public class CameraFadeAttributes
{
    [Header("Camera Fading Attributes")]
    public CAMERA_FADE_TYPE cameraFadeType;

    [Space(10)]
    [Tooltip("The time value for the image fading out")]
    public float fadeOutTime = 1f;
    [Tooltip("The time value for the image fading in")]
    public float fadeInTime = 1f;

    [Space(10)]
    [Tooltip("The time value for to wait between operations")]
    public float fadeTransitionWaitTime = 1f;

    public CameraFadeAttributes(CAMERA_FADE_TYPE cameraFadeType, float cameraFadeOutTime, float cameraFadeInTime, float cameraFadeTransitionTime)
    {
        this.cameraFadeType = cameraFadeType;
        this.fadeOutTime = cameraFadeOutTime;
        this.fadeInTime = cameraFadeInTime;
        this.fadeTransitionWaitTime = cameraFadeTransitionTime;
    }
}

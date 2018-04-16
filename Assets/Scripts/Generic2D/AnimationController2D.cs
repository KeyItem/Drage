using System.Collections;
using UnityEngine;

public class AnimationController2D : MonoBehaviour
{
    [Header("Animation Controller Attributes")]
    public Animator targetAnimator;

    private void Start()
    {
        AnimationControllerSetup();
    }

    private void AnimationControllerSetup()
    {
        if (targetAnimator == null)
        {
            targetAnimator = GetComponent<Animator>();
        }
    }

    public virtual void SetFloat(string floatName, float newSpeed)
    {
        targetAnimator.SetFloat(floatName, newSpeed);
    }

    public virtual void SetBool(string boolName, bool boolState)
    {
        targetAnimator.SetBool(boolName, boolState);
    }

    public virtual void SetTrigger(string triggerName)
    {
        targetAnimator.SetTrigger(triggerName);
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("LockOn UI Attributes")]
    public Image lockOnIndicator;

    private Transform targetLockOnPoint;

    private void Update()
    {
        ManageTargetLockOnIndicator();
    }

    private void ManageTargetLockOnIndicator()
    {
        if (targetLockOnPoint != null)
        {
            Vector3 newTargetScreenPoint = Camera.main.WorldToScreenPoint(targetLockOnPoint.position);

            lockOnIndicator.transform.position = newTargetScreenPoint;
        }
    }

    public void SetNewLockOnIndicatorPosition(Transform targetPoint)
    {
        targetLockOnPoint = targetPoint;

        lockOnIndicator.gameObject.SetActive(true);  
    }

    public void DisableLockOnIndicator()
    {
        targetLockOnPoint = null;

        lockOnIndicator.gameObject.SetActive(false);
    }
}

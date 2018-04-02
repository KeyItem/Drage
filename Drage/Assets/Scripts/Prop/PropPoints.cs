using System.Collections;
using UnityEngine;

[System.Serializable]
public class PropPoints : MonoBehaviour
{
    [Header("Prop Points")]
    public Transform propStartingPosition;
    public Transform propEndingPosition;

    [Header("DEBUG")]
    public bool canShowDebug = false;

    public void OnDrawGizmosSelected()
    {
        if (canShowDebug)
        {
            Gizmos.color = Color.green;

            Gizmos.DrawLine(propStartingPosition.position, propEndingPosition.position);
        }
    }
}

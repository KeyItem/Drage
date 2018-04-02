using System.Collections;
using UnityEngine;

public class MarbleResetter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Marble"))
        {
            other.GetComponent<Rigidbody>().velocity = Vector3.zero;

            ObjectPooler.Instance.ReturnObjectToQueue("Marble", other.gameObject);
        }
    }
}

using System.Collections;
using UnityEngine;

public class Zone : MonoBehaviour
{
	private void Start ()
    {
        ZoneSetup();
	}

    public virtual void ZoneSetup()
    {

    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ZoneEffect(collision.gameObject);
        }
    }

    public virtual void ZoneEffect(GameObject effectedObject)
    {

    }
}

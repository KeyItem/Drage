using System.Collections;
using UnityEngine;

public class DamageZone : Zone
{
    [Header("Zone Attributes")]
    public float zoneDamage;

    public override void ZoneEffect(GameObject effectedObject)
    {
        HealthController2D healthController = effectedObject.GetComponent<HealthController2D>();

        if (healthController != null)
        {
            healthController.TakeDamage(zoneDamage);
        }
    }
}

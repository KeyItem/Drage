using System.Collections;
using UnityEngine;

public class ResetZone : Zone
{
    public override void ZoneEffect(GameObject player)
    {
        PlayerController2D playerController = player.GetComponent<PlayerController2D>();

        playerController.ResetController();
    }
}

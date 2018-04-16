using System.Collections;
using UnityEngine;

public class WinZone : Zone
{
    [Header("Load Level ID")]
    public int levelID;

    public override void ZoneEffect(GameObject effectedObject)
    {
        LevelManager.Instance.LoadNextLevel();
    }
}

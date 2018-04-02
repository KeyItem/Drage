using System.Collections;
using UnityEngine;

public class MarblePlatformController : MonoBehaviour
{
    private static MarblePlatformController _instance;
    public static MarblePlatformController Instance { get { return _instance; } }

    [Header("Marble Positions")]
    public Material platformMaterial;

    private void Awake()
    {
        InitializeMarblePlatformController();
    }

    private void InitializeMarblePlatformController()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void ChangePlatformColor()
    {
        Color newColor = Random.ColorHSV();

        platformMaterial.color = newColor;
    }
}

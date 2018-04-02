using System.Collections;
using UnityEngine;

public class ThirdPersonPlayerToolbox : MonoBehaviour
{
    private static ThirdPersonPlayerToolbox _instance;
    public static ThirdPersonPlayerToolbox Instance { get { return _instance; } }

    public ThirdPersonInputManager ThirdPersonInput;
    public ThirdPersonPlayerController ThirdPersonPlayerController;

    public PlayerClimbController ThirdPersonClimbController;
    public PlayerCoverController ThirdPersonCoverController;
    public PlayerInteractionController ThirdPersonInteraction;
    public PlayerPickUpController ThirdPersonPickUpController;

    public ThirdPersonCameraController ThirdPersonCamera;

    private void Awake()
    {
        ToolboxSetup();
    }

    private void ToolboxSetup()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        ThirdPersonInput = GetComponent<ThirdPersonInputManager>();
        ThirdPersonPlayerController = GetComponent<ThirdPersonPlayerController>();
        ThirdPersonClimbController = GetComponent<PlayerClimbController>();
        ThirdPersonCoverController = GetComponent<PlayerCoverController>();
        ThirdPersonInteraction = GetComponent<PlayerInteractionController>();
        ThirdPersonPickUpController = GetComponent<PlayerPickUpController>();

        ThirdPersonCamera = Camera.main.GetComponent<ThirdPersonCameraController>();
    }
}

using MOBA;
using UnityEngine;

[RequireComponent(typeof(MobaCameraController))]
[RequireComponent(typeof(MobaInputHandler))]
[RequireComponent(typeof(MobaAnimationController))]
[RequireComponent(typeof(MobaMovementController))]
public class MobaPlayer : MonoBehaviour
{
    public MobaCameraController CameraController { get; private set; }
    public MobaInputHandler InputHandler { get; private set; }
    public MobaAnimationController AnimationController { get; private set; }
    public MobaMovementController MovementController { get; private set; }

    public MobaPlayerMovementControllerConfig movementControllerSettings;

    private void Awake()
    {
        CameraController = GetComponent<MobaCameraController>();
        InputHandler = GetComponent<MobaInputHandler>();
        AnimationController = GetComponent<MobaAnimationController>();
        MovementController = GetComponent<MobaMovementController>();
    }
}

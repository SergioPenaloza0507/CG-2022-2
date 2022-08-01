using MOBA;
using UnityEngine;

[RequireComponent(typeof(MobaCameraController))]
[RequireComponent(typeof(MobaInputHandler))]
[RequireComponent(typeof(MobaAnimationController))]
[RequireComponent(typeof(MobaMovementController))]
[RequireComponent(typeof(MobaAttackController))]
public class MobaPlayer : MonoBehaviour
{
    public MobaCameraController CameraController { get; private set; }
    public MobaInputHandler InputHandler { get; private set; }
    public MobaAnimationController AnimationController { get; private set; }
    public MobaMovementController MovementController { get; private set; }

    [Expandable]
    public MobaPlayerMovementControllerConfig movementControllerSettings;
    [Expandable]
    public MobaPlayerCameraControllerConfig cameraControllerSettings;

    public void BroadcastInputToListeners<TAction>(TAction action) where TAction : struct
    {
        IInputListener<TAction>[] listeners = GetComponents<IInputListener<TAction>>();
        foreach (IInputListener<TAction> inputListener in listeners)
        {
            inputListener.Listen(action);
        }
    }
    
    private void Awake()
    {
        CameraController = GetComponent<MobaCameraController>();
        InputHandler = GetComponent<MobaInputHandler>();
        AnimationController = GetComponent<MobaAnimationController>();
        MovementController = GetComponent<MobaMovementController>();
    }
}

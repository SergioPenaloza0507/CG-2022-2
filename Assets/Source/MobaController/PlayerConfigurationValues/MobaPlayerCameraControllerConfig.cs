using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Moba Camera Controller Configuration", menuName = "Player Controller Framework/MOBA/Camera Controller Configuration")]
public class MobaPlayerCameraControllerConfig : ScriptableObject
{
    public Camera camera;
    public Camera cameraRigRoot;
}

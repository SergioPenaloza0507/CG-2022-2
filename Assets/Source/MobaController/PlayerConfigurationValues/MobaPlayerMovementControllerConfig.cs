using UnityEngine;

[CreateAssetMenu(fileName = "Moba Movement Controller Configuration", menuName = "Player Controller Framework/MOBA/Movement Controller Configuration")]
public class MobaPlayerMovementControllerConfig : ScriptableObject
{
    public GameObject locationEffector;
    public Vector3 locationEffectorOffset;
    public Quaternion locationEffectorRotationOffset;
    public Vector3 locationEffectorScale;
}

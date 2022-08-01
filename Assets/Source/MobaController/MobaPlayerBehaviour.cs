using UnityEngine;

[RequireComponent(typeof(MobaPlayer))]
public abstract class MobaPlayerBehaviour : MonoBehaviour
{
    protected MobaPlayer player;

    protected virtual void Awake()
    {
        player = GetComponent<MobaPlayer>();
    }
}

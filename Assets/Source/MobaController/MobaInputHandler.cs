using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobaInputHandler : MobaPlayerBehaviour
{
    [SerializeField] private float startDelay;

    [SerializeField]private bool isEnabled;

    void Enable()
    {
        isEnabled = true;
    }
    protected override void Awake()
    {
        base.Awake();
        Invoke(nameof(Enable), startDelay);
    }

    void Update()
    {
        if(!isEnabled) return;
        if (Input.GetMouseButtonDown(1))
        {
            player.BroadcastInputToListeners( new Vector2Action{contextId = MOBA.ActionContextHolder.SCREEN_POSITION_CONTEXT_ID, value = Input.mousePosition});
        }

        if (Input.GetMouseButtonDown(0))
        {
            player.BroadcastInputToListeners(new ByteAction{contextId = MOBA.ActionContextHolder.ATTACK_CONTEXT_ID, value = 1});
        }
    }
}

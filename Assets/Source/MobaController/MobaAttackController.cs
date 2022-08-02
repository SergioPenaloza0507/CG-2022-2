using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using CONTEXT = MOBA.ActionContextHolder;

public class MobaAttackController : MobaPlayerBehaviour, IInputListener<ByteAction>
{
    [SerializeField] private int comboLength;
    [SerializeField]private int currentAttackIndex;
    
    [SerializeField]private float comboCooldown;
    private float comboCooldownTimer;
    private bool coolDown;

    private void Update()
    {
        if(coolDown)
            comboCooldownTimer += Time.deltaTime;
    }

    public bool ValidateAction(ByteAction action)
    {
        return action.contextId == CONTEXT.ATTACK_CONTEXT_ID;
    }

    public void Listen(ByteAction action)
    {
        // if (coolDown)
        // {
        //     // if (comboCooldownTimer < comboCooldown)
        //     // {
        //     //     return;
        //     // }
        //
        //     comboCooldownTimer = 0;
        //     currentAttackIndex = 0;
        // }

        // if (currentAttackIndex == 0)
        // {
        //     player.AnimationController.StartAttackCombo(action.value);
        // }
        // else
        // {
            player.AnimationController.KeepAttacking();
        // }
        //
        // currentAttackIndex++;
        // if (currentAttackIndex >= comboLength +1)
        // {
        //     coolDown = true;
        // }
    }
}

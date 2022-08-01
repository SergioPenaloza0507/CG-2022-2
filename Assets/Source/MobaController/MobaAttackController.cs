using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using CONTEXT = MOBA.ActionContextHolder;

public class MobaAttackController : MobaPlayerBehaviour, IInputListener<ByteAction>
{
    [SerializeField] private float[] attackComboDelays;

    private bool isAttacking;
    
    [SerializeField]private int currentAttackIndex;
    private float comboDelayTimer;

    private void Update()
    {
        // if (isAttacking && currentAttackIndex < attackComboDelays.Length)
        // {
        //     comboDelayTimer += Time.deltaTime;
        //     if (comboDelayTimer > attackComboDelays[currentAttackIndex])
        //     {
        //         currentAttackIndex = 0;
        //         comboDelayTimer = 0;
        //         player.AnimationController.StopAttacking();
        //         isAttacking = false;
        //     }
        // }
    }

    public bool ValidateAction(ByteAction action)
    {
        return action.contextId == CONTEXT.ATTACK_CONTEXT_ID;
    }

    public void Listen(ByteAction action)
    {
        if (currentAttackIndex >= attackComboDelays.Length)
        {
            currentAttackIndex = 0;
            comboDelayTimer = 0;
            return;
        }

        if (currentAttackIndex == 0)
        {
            player.AnimationController.StartAttackCombo(action.value);
            isAttacking = true;
        }
        else
        {
            player.AnimationController.KeepAttacking();
        }

        currentAttackIndex++;
        comboDelayTimer = 0;
        
    }
}

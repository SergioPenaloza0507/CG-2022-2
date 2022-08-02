using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MobaAnimationController : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.SetTrigger("Intro");
    }

    public void StartRunning()
    {
        anim.SetBool("Moving", true);
    }
    
    public void StopRunning()
    {
        anim.SetBool("Moving", false);
    }

    public void StartAttackCombo(int comboIndex)
    {
        anim.SetTrigger($"Combo {comboIndex} Enter");
    }

    public void KeepAttacking()
    {
        anim.SetTrigger("Combo Follow");
    }
    
    public void StopAttacking()
    {
        anim.ResetTrigger("Combo Follow");
        anim.SetTrigger("Combo Exit");
    }
    
    
}

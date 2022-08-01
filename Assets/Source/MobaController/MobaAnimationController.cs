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
    }

    public void StartRunning()
    {
        anim.SetBool("Moving", true);
    }
    
    public void StopRunning()
    {
        anim.SetBool("Moving", false);
    }
}

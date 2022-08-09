using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AttackController2 : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private ParticleSystem particleSystem;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
            if(particleSystem != null)
                particleSystem.Play(true);
        }
    }
}

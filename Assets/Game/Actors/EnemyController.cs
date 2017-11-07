using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : ActorBehaviour {
    private Animator animator;
    private float animatorFactor = 1.5f;
    
    protected override void OnAwake(params object[] args) {
        speed = 1f;
        animator = GetComponent<Animator>();
        SetDefaultState(State.SeekPlayer);
    }

    protected override void OnStart(params object[] args) {
        
    }

    private void Update() {
        
    }

    protected override void OnStartMove(params object[] args) {
        animator.SetBool("Run", true);
        animator.speed = speed / animatorFactor;
    }

    protected override void OnFinishMove(params object[] args) {
        animator.SetBool("Run", false);
        animator.speed = 1;
    }
}

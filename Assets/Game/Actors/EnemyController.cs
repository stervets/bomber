using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : ActorBehaviour {
    public Animator animator;
    public float animatorFactor = 1.5f;

    protected override void OnAwake(params object[] args) {
        speed = 1f;
        animator = GetComponent<Animator>();
        SetDefaultState(State.SeekPlayer);
    }

    protected override void OnStart(params object[] args) {
        
    }

    protected override bool OnMeetObtacle(ControllerBehaviour obj, CellController lastCell) {
        console.log("meet", obj.GetType());
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ActorBehaviour {
    /*
    protected override void OnAwake(params object[] args) {
        speed = 3.5f;
        ListenTo(g.c, Channel.Map.Loaded, OnMapLoaded);
        animator = GetComponent<Animator>();
    }

    protected void OnMapLoaded(params object[] args) {
        SetCell(g.map.cell[6,5]);
    }


    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            moveToCell(gc.GetCellFromCamera(Input.mousePosition));
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            cell.Blow(4);
        }
    }

    protected override void OnStartMove(params object[] args) {
        animator.SetBool("Run", true);
    }

    protected override void OnFinishMove(params object[] args) {
        animator.SetBool("Run", false);
    }
    */
}

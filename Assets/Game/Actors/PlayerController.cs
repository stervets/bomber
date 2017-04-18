using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ActorBehaviour {
    private Animator animator;
    private float animatorFactor = 2.4f;

    protected override void OnAwake(params object[] args) {
        speed = 2.5f;
        ListenTo(g.c, Channel.Actor.SetPlayerBomb, SetPlayerBomb);
        animator = GetComponent<Animator>();
    }

    protected override void OnStart(params object[] args) {
        g.c.Trigger(Channel.Camera.SetTarget, this);
    }

    private CellController targetCell;

    private int bombPower = 5;

    protected void SetPlayerBomb(params object[] args) {
        var bomb = Instantiate(g.map.bombPrefab, cell.top, Quaternion.identity).GetComponent<BombController>();
        bomb.SetTimer(2000, bombPower);
    }

    //private float animSpeed = 1f;
    private void Update() {
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x>=g.camera.pixelRect.x) {
            targetCell = g.map.GetCellFromCamera(Input.mousePosition);
            if (targetCell != cell) {
                moveToCell(targetCell);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && g.map.obtacles[cell].GetType() != typeof(BombController)) {
            SetPlayerBomb();
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            animatorFactor -= 0.01f;
            console.log(animatorFactor);
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            animatorFactor += 0.01f;
            console.log(animatorFactor);
        }
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

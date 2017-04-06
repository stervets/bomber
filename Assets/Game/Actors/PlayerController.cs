using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ActorBehaviour {
    private Animator animator;
    protected override void OnAwake(params object[] args) {
        speed = 3.5f;
        //ListenTo(g.c, Channel.Map.Loaded, OnMapLoaded);
        animator = GetComponent<Animator>();
    }

    //protected void OnMapLoaded(params object[] args) {
    //        SetCell(g.map.cell[6,5]);
    //  }

    private CellController targetCell;

    private int bombStrength = 3;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            targetCell = g.map.GetCellFromCamera(Input.mousePosition);
            if (targetCell != cell) {
                moveToCell(targetCell);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && g.map.obtacles[cell].GetType() != typeof(BombController)) {
            var bomb = Instantiate(g.map.bombPrefab, cell.top, Quaternion.identity).GetComponent<BombController>();
            bomb.blow(3000, bombStrength);
        }
    }

    protected override void OnStartMove(params object[] args) {
        animator.SetBool("Run", true);
    }

    protected override void OnFinishMove(params object[] args) {
        animator.SetBool("Run", false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : BlockController {
    protected override void OnAwake(params object[] args) {
        //targetDirection = transform.rotation.eulerAngles;
    }

    private bool tween;

    public override void Blow(int directionX, int directionY) {
        if (!tween && cell.lastBlock == this && (directionX!=0 || directionY!=0)) {
            //transform.rotation = targetRotation;
            cell.Blow(true, this);
            cell.RemoveBlock(this, -1);
            var dir = g.map.IsCellAvailToMove(cell, directionX, directionY) ? 1 : -1;
            directionX *= dir;
            directionY *= dir;
            var newCell = g.map.GetCell(cell.x + directionX, cell.y + directionY);
            newCell.AddBlock(this);
            tween = true;
            LeanTween.rotateAround(gameObject, Vector3.back * directionX + Vector3.left * directionY, 90f, 0.5f).setOnComplete(
                _ => { tween = false; });
            LeanTween.move(gameObject, newCell.top + Vector3.down, 0.5f).setEase(LeanTweenType.easeOutQuad);
        }
    }
}

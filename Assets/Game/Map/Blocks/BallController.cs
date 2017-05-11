using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BallController : BlockController {
    protected override void OnAwake(params object[] args) {
        //targetDirection = transform.rotation.eulerAngles;
    }

    private bool tween;

    public override void Blow(int directionX, int directionY) {
        cell.Blow(true, this);
        if (!tween && cell.lastBlock == this && (directionX != 0 || directionY != 0)) {
            cell.RemoveBlock(this, -1);
            var nextCell = g.map.GetCell(cell.x + directionX, cell.y + directionY);
            var cellIsAvail = g.map.IsCellAvailToMove(cell, nextCell);
            var dir = !cellIsAvail && g.map.obtacles[nextCell] == null ? -1 : 1;
            directionX *= dir;
            directionY *= dir;
            var newCell = g.map.GetCell(cell.x + directionX, cell.y + directionY);
            tween = true;
            LeanTween.rotateAround(gameObject, Vector3.back * directionX + Vector3.left * directionY, 90f, 0.5f)
                .setOnComplete(
                    _ => { tween = false; });
            LeanTween.move(gameObject, newCell.top, 0.5f).setEase(LeanTweenType.easeOutQuad);

            Observable.Timer(System.TimeSpan.FromMilliseconds(250))
                .Subscribe(_ => { newCell.AddBlock(this); });
        }
    }
}
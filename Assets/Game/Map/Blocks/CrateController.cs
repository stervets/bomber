using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateController : BlockController {
    //transform.rotation = MapController.GetRotationFromDirection(Random.Range(0, 4));

    private const float blowTime = 0.3f;

    public override void Blow(int directionX, int directionY) {
        cell.Blow(true, this);
        Dissolve(blowTime);
    }
}

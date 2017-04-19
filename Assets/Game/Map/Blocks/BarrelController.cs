using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelController : BlockController {
    private const int power = 3;
    private const float blowTime = 0.3f;

    public override void Blow(int directionX, int directionY) {
        cell.Blow(true, this);
        Dissolve(blowTime);
        g.map.Trigger(200, Channel.Map.BlowCell, cell, power);
    }

}

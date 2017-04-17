using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelController : BlockController {
    private const int power = 3;
    protected override void OnBlow() {
        g.map.Trigger(200, Channel.Map.BlowCell, cell, power);
    }

}

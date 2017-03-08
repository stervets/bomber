﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : BlockController {
    //transform.rotation = MapController.GetRotationFromDirection(Random.Range(0, 4));

    protected override void OnStart(params object[] args) {
        if (imported)return;
        base.OnStart();
        SetDirection((byte)Random.Range(0,4));
    }
}
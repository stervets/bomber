using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCursorController : ControllerBehaviour {
    protected override void OnStart(params object[] args) {
        g.c.Trigger(Channel.Camera.SetTarget, this);
    }
}

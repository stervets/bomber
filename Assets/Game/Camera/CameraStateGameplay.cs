using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateGameplay : StateBehaviour {
    protected override void OnAwake(params object[] args) {
        SetDefaultState(State.GamePlay);
    }
}

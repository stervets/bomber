using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateDefault : StateBehaviour {
    protected override void OnAwake(params object[] args) {
        SetDefaultState(State.Default);
	}
}

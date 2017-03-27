using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateEditor : StateBehaviour {

	// Use this for initialization
    protected override void OnAwake(params object[] args) {
		SetDefaultState(State.Editor);
	}

    protected override void OnStart(params object[] args) {
        console.log("Editor camera started");
    }
}

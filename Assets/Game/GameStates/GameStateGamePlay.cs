using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStateGamePlay : StateBehaviour {
	//private GameController cc{get{return _controller as GameController;}}

	private GameObject map;

	protected override void OnAwake(params object[] args) {
	    //Debug.Log("on state awake: " + this.ToString());
	    SetDefaultState(State.GamePlay);
	}

	protected override void OnStart(params object[] args) {
	    //Debug.Log("on state start: " + this.ToString());
	    //Debug.Log ("Game started");
		//Map map = new Map ("test");
	}

	protected override void OnEnabled(params object[] args) {
	    map = Instantiate(gc.MapPrefab);
	    console.log("game controller enabled");
	}

	protected override void OnDisabled(params object[] args) {
		Destroy (map);
	}
}

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

    protected void OnMapLoaded(params object[] args) {
        console.log("map loaded");
    }

    protected override void OnEnabled(params object[] args) {
	    map = Instantiate(gc.MapPrefab);
	    ListenTo(g.map, Channel.GameObject.Start, objects => {
	        g.map.loadFromFile();
	    });

	    ListenTo(g.map, Channel.Map.Loaded, OnMapLoaded);
	}

	protected override void OnDisabled(params object[] args) {
		Destroy (map);
	}
}

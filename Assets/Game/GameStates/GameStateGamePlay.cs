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
        g.map.CreateActor(g.map.cellItems[CellItem.PlayerRespawn][0], "player", "PlayerController");
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


    // Test isCellAvailToMove
    /*
    private CellController cell;
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (cell == null) {
                cell = g.map.GetCellFromCamera(Input.mousePosition);
                console.log(cell);
            } else {
                console.log(g.map.GetCellFromCamera(Input.mousePosition));
                console.log(g.map.IsCellAvailToMove(cell, g.map.GetCellFromCamera(Input.mousePosition)));
                console.log("blowable", g.map.IsCellAvailToMove(cell, g.map.GetCellFromCamera(Input.mousePosition), true)); // blowable
                cell = null;
            }
        }
    }
    */

    // Test FindPath
    /*
    private CellController cell;

    void ShowPath(List<BlockController> waypoints) {
        foreach (var o in GameObject.FindGameObjectsWithTag("Debug")) {
            Destroy(o, 0.01f);
        }

        waypoints.ForEach(waypoint => {
            Instantiate(g.map.debugCubePrefab,
                waypoint.transform.position + Vector3.up * (waypoint.isLadder ? 0.5f : 1f), Quaternion.identity);
        });
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (cell == null) {
                cell = g.map.GetCellFromCamera(Input.mousePosition);
                console.log(cell);
            } else {
                console.log(g.map.GetCellFromCamera(Input.mousePosition));
                g.map.FindPath(cell, g.map.GetCellFromCamera(Input.mousePosition), ShowPath);
                cell = null;
            }
        }
    }
    */

}

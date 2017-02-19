using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerInputController : ControllerBehaviour {
	protected override void OnAwake(params object[] args) {
		SetDefaultState (State.Default);
	}

	protected override void OnStart(params object[] args) {

	}

	Vector3 mouseCoords = Vector3.zero;

	void OnDrawGizmos() {
		//Gizmos.DrawCube (GetRealPositionFromTable(GetTablePositionFromReal(GetRealPositionFromCamera (mouseCoords))) + Vector3.up*0.5f, Vector3.one);
		//Gizmos.DrawCube (GetRealPositionFromCamera (mouseCoords), Vector3.one*0.3f);
		//Gizmos.DrawCube (Map.GetRealPositionFromTable(Map.GetTablePositionFromReal(GetRealPositionFromCamera (mouseCoords))), Vector3.one * 1f);

		//Gizmos.DrawCube (Map.GetRealPositionFromTable(GetTablePositionFromCamera(mouseCoords)), Vector3.one * 1f);
		//Gizmos.DrawCube (GetCellFromCamera(mouseCoords).realPosition, Vector3.one * 1f);
	}


    private bool isEndPoint;
    private Cell startCell;
    private Cell cell;
    public GameObject pathPrefab;

    private float cameraSpeed = 0.5f;

    void Update () {
        if (Input.GetMouseButtonDown(1)) {
            g.map.BlowCell(gc.GetCellFromCamera(Input.mousePosition), 3);
            //gc.GetCellFromCamera(Input.mousePosition)
        }

        if (Input.GetMouseButtonDown (0)) {
	        mouseCoords = Input.mousePosition;
	        //cursorPoint = GetTablePositionFromReal (GetRealPositionFromCamera (mouseCoords));
	        cell = gc.GetCellFromCamera(mouseCoords);

	        if (isEndPoint) {
	            g.map.FindPath (startCell, cell, waypoints => {
	                foreach (var waypoint in waypoints) {
	                    Instantiate(pathPrefab,
	                        waypoint.realPosition + Vector3.down * 0.25f +
	                        Vector3.up * (waypoint.isLadder ? 0.5f : (waypoint.movable>1 ? 1f : 0f))
	                        , Quaternion.identity);
	                }
	            });
	        } else {
	            startCell = cell;
	            GameObject[] waypoints = GameObject.FindGameObjectsWithTag ("WayPoint");
	            for (int i = 0, j = waypoints.Length; i < j; i++) {
	                Destroy (waypoints [i]);
	            }
	        }
	        isEndPoint = !isEndPoint;
	    }

        if (Input.GetKey(KeyCode.A)) {
            //g.camera.transform.position += Vector3.left * cameraSpeed;
        }

        if (Input.GetKey(KeyCode.D)) {
            //g.camera.transform.position -= Vector3.left * cameraSpeed;
        }
    }
}

using UnityEngine;
using System.Collections.Generic;

public class PlayerController : ControllerBehaviour {
    private CharacterController character;
    private List<Cell> waypoints;
    private Cell waypoint;
    private Vector3 realWaypoint;

    private float speed = 0.1f;
    private Vector3 direction;

    private Animator animator;

	protected override void OnAwake(params object[] args) {
		SetDefaultState (State.Default);
	}

	protected override void OnStart(params object[] args) {
	    character = GetComponent<CharacterController>();
	    animator = GetComponent<Animator>();
	}

    void OnNewPath(List<Cell> _waypoints) {
        waypoints = _waypoints;
        NextPoint();
    }

    void NextPoint() {
        if (waypoints.Count > 0) {
            waypoint = waypoints[0];
            waypoints.RemoveAt(0);

            realWaypoint = waypoint.realPosition;
            realWaypoint.y = transform.position.y;

            direction = (realWaypoint - transform.position).normalized;
            if (direction != Vector3.zero) {
                transform.rotation = Quaternion.LookRotation(direction);
            }
            animator.SetBool("Run", true);
        } else {
            animator.SetBool("Run", false);
            waypoint = null;
        }
    }

    /*
    private void OnDrawGizmos() {
        if (waypoint != null) {
            Gizmos.DrawCube(waypoint.realPosition, Vector3.one*0.2f);
        }
    }
    */

    private void FixedUpdate() {
        if (waypoint != null) {
            realWaypoint.y = transform.position.y;
            if (Vector3.Distance(transform.position, realWaypoint) < speed) {
                NextPoint();
            } else {
                character.Move(transform.forward * speed);
            }
        }
    }


    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            //gc.map.BlowCell(gc.GetCellFromCamera(Input.mousePosition), 3);
            //gc.GetCellFromCamera(Input.mousePosition)
            var startCell = gc.map.GetCellFromReal(transform.position);
            var finishCell = gc.GetCellFromCamera(Input.mousePosition);
            gc.map.FindPath(startCell, finishCell, OnNewPath);
            //rigid.MovePosition();
            //gc.Get
        }

        /*
        if (Input.GetMouseButtonDown (0)) {
            mouseCoords = Input.mousePosition;
            //cursorPoint = GetTablePositionFromReal (GetRealPositionFromCamera (mouseCoords));
            cell = gc.GetCellFromCamera(mouseCoords);

            if (isEndPoint) {
                gc.map.FindPath (startCell, cell, waypoints => {
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
        */
    }
}

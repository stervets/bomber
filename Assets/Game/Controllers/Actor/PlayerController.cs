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


            //Debug.Log(animator.Get);
            //animator.SetBool("eiruwyie1", true);
        } else {
            animator.SetBool("Run", false);
            waypoint = null;
        }
    }


    private void OnDrawGizmos() {
        Gizmos.color = new Color(0.4f, 1f, 0.7f, 1f);
        if (waypoint != null) {
            //Gizmos.DrawCube(waypoint.realPosition, Vector3.one*0.5f);
        }
        //Gizmos.DrawCube(g.map.GetRealPositionFromCellPosition(Map.GetTablePositionFromReal(transform.position)) , Vector3.one);
    }


    private void FixedUpdate() {
        if (waypoint != null) {
            realWaypoint.y = transform.position.y;
            if (Vector3.Distance(transform.position, realWaypoint) < speed) {
                NextPoint();
            } else {
                //character.BroadcastMessage("addsad", 1,2,3);

                character.Move(transform.forward * speed);
            }
        }
    }


    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            //gc.map.BlowCell(gc.GetCellFromCamera(Input.mousePosition), 3);
            //gc.GetCellFromCamera(Input.mousePosition)
            var startCell = g.map.GetCellFromReal(transform.position);
            var finishCell = gc.GetCellFromCamera(Input.mousePosition);
            g.map.FindPath(startCell, finishCell, OnNewPath);
        }
    }
}

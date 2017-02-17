﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorBehaviour : ControllerBehaviour {
    //protected InputBehaviour _input;
    //public abstract InputBehaviour input { get; }
    protected InputBehaviour input;

    public Cell cell;

    public float speed;
    private float squaredSpeed;

    protected Cell waypoint;
    protected List<Cell> waypoints;

    protected CharacterController characterController;

    protected Vector3 direction;
    protected Animator animator;

    protected override void BeforeControllerAwake() {
        squaredSpeed = speed * speed;
    }

    private Vector3 realWaypoint;

    /*
    protected override void OnAwake(params object[] args) {

    }
    */
    public void moveToNextCell(int offsetX, int offsetY, bool checkMapObstacles = true) {
        var nextCell = g.map.GetCell(cell.x + offsetX, cell.y + offsetY);
        if ((nextCell = (nextCell==null || !(checkMapObstacles && g.map.isCellOffsetAvailToMove(cell.x, cell.y, offsetX, offsetY)) ? null : nextCell))!=null) {
            waypoints = new List<Cell> {nextCell};
        }
        NextPoint();
        OnStartMove();
    }

    public void moveToCell(Cell targetCell) {
        g.map.FindPath(cell, targetCell, SetWaypoints);
    }

    public void SetWaypoints(List<Cell> _waypoints) {
        waypoints = _waypoints;
        NextPoint();
        OnStartMove();
    }

    public List<Cell> getFreeCells(bool includeDiagonal = false) {
        var cells = new List<Cell>();
        if (g.map.isCellOffsetAvailToMove(cell.x, cell.y, -1, 0)) {
            cells.Add(g.map.cell[cell.x-1, cell.y]);
        }
        if (g.map.isCellOffsetAvailToMove(cell.x, cell.y, 1, 0)) {
            cells.Add(g.map.cell[cell.x+1, cell.y]);
        }
        if (g.map.isCellOffsetAvailToMove(cell.x, cell.y, 0, -1)) {
            cells.Add(g.map.cell[cell.x, cell.y-1]);
        }
        if (g.map.isCellOffsetAvailToMove(cell.x, cell.y, 0, 1)) {
            cells.Add(g.map.cell[cell.x, cell.y+1]);
        }

        if (!includeDiagonal) return cells;

        if (g.map.isCellOffsetAvailToMove(cell.x, cell.y, -1, -1)) {
            cells.Add(g.map.cell[cell.x-1, cell.y-1]);
        }
        if (g.map.isCellOffsetAvailToMove(cell.x, cell.y, 1, 1)) {
            cells.Add(g.map.cell[cell.x+1, cell.y+1]);
        }
        if (g.map.isCellOffsetAvailToMove(cell.x, cell.y, -1, 1)) {
            cells.Add(g.map.cell[cell.x-1, cell.y+1]);
        }
        if (g.map.isCellOffsetAvailToMove(cell.x, cell.y, 1, -1)) {
            cells.Add(g.map.cell[cell.x+1, cell.y-1]);
        }
        return cells;
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
            OnFinishMove();
        }
    }

    private void FixedUpdate() {
        if (waypoint != null) {
            if (Vector3.Dot(direction, realWaypoint - transform.position) < 0) {
                NextPoint();
            } else {
                characterController.Move(transform.forward * speed * Time.deltaTime);
            }
        }
        OnFixedUpdate();
    }

    public void Fire() {
        Trigger(Channel.Actor.Fire);
    }

    protected virtual void OnFixedUpdate() {
    }

    protected virtual void OnStartMove() {
    }

    protected virtual void OnFinishMove() {
    }
}

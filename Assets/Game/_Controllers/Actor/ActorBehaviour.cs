using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorBehaviour : ControllerBehaviour {
    //protected InputBehaviour _input;
    //public abstract InputBehaviour input { get; }
    /*
    protected InputBehaviour input;

    public Cell cell;
    private Cell oldCell;

    public float speed;
    //private float squaredSpeed;

    protected Cell waypoint;
    protected List<Cell> waypoints;

    protected CharacterController characterController;

    protected Vector3 direction;
    protected Animator animator;

    protected void SetCell(Cell _cell) {
        oldCell = cell = _cell;
        transform.position = cell.realPosition;
    }

    protected override void BeforeControllerAwake() {
        characterController = GetComponent<CharacterController>();
        On(Channel.Actor.StartMove, OnStartMove);
        On(Channel.Actor.FinishMove, OnFinishMove);
        On(Channel.Actor.Fire, OnFire);
    }

    private Vector3 realWaypoint;

    public void moveToNextCell(int offsetX, int offsetY, bool checkMapObstacles = true) {
        var nextCell = g.map.GetCell(cell.x + offsetX, cell.y + offsetY);
        if ((nextCell = (nextCell==null || !(checkMapObstacles && g.map.isCellOffsetAvailToMove(cell.x, cell.y, offsetX, offsetY)) ? null : nextCell))!=null) {
            waypoints = new List<Cell> {nextCell};
        }
        NextPoint();
        Trigger(Channel.Actor.StartMove);
    }

    public void moveToCell(Cell targetCell) {
        g.map.FindPath(cell, targetCell, SetWaypoints);
    }

    public void SetWaypoints(List<Cell> _waypoints) {
        waypoints = _waypoints;
        if (waypoints.Count > 1) {
            if (Vector3.SqrMagnitude(waypoints[1].realPosition-transform.position)<
                Vector3.SqrMagnitude(waypoints[1].realPosition-waypoints[0].realPosition)
            ) {
                waypoints.RemoveAt(0);
            }
        }
        NextPoint();
        Trigger(Channel.Actor.StartMove);
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
        } else {
            waypoint = null;
            Trigger(Channel.Actor.FinishMove);
        }
    }

    private void FixedUpdate() {
        if (waypoint != null) {
            // Здесь желательно учитывать кусочек перепройденного расстояния
            if (Vector3.Dot(direction, realWaypoint - transform.position) <= 0) {
                NextPoint();
            } else {
                characterController.Move(transform.forward * speed * Time.deltaTime);
                cell = g.map.GetCellFromReal(transform.position);
                if (cell != oldCell) {
                    gc.Trigger(Channel.Actor.ChangeLocation, cell, oldCell);
                    oldCell = cell;
                }
            }
        }
        OnFixedUpdate();
    }

    public void Fire() {
        Trigger(Channel.Actor.Fire);
    }

    protected virtual void OnFixedUpdate() {
    }

    protected virtual void OnStartMove(params object[] args) {
    }

    protected virtual void OnFinishMove(params object[] args) {
    }

    protected virtual void OnFire(params object[] args) {
    }
    */
}

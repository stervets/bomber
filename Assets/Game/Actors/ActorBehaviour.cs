using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorBehaviour : ControllerBehaviour {
    //protected InputBehaviour _input;
    //public abstract InputBehaviour input { get; }

    //protected InputController input;

    public CellController cell;
    private CellController oldCell;

    public float speed;
    //private float squaredSpeed;

    protected CellController waypoint;
    protected List<CellController> waypoints;

    //protected CharacterController characterController;

    protected Vector3 direction;
    //protected Animator animator;

    protected void PlaceOnCell(CellController _cell) {
        oldCell = cell = _cell;
        transform.position = cell.top;
    }

    protected override void BeforeControllerAwake() {
        //characterController = GetComponent<CharacterController>();
        On(Channel.Actor.StartMove, OnStartMove);
        On(Channel.Actor.FinishMove, OnFinishMove);
        On(Channel.Actor.Fire, OnFire);
    }

    protected override void OnAwake(params object[] args) {
        PlaceOnCell(g.map.GetCellFromReal(transform.position));
    }

    private Vector3 realWaypoint;

    public void moveToNextCell(int offsetX, int offsetY, bool checkMapObstacles = true) {
        /*
        var nextCell = g.map.GetCell(cell.x + offsetX, cell.y + offsetY);
        if ((nextCell = (nextCell==null || !(checkMapObstacles && g.map.isCellOffsetAvailToMove(cell.x, cell.y, offsetX, offsetY)) ? null : nextCell))!=null) {
            waypoints = new List<Cell> {nextCell};
        }
        NextPoint();
        Trigger(Channel.Actor.StartMove);
        */
    }

    public void moveToCell(Cell targetCell) {
        //g.map.FindPath(cell, targetCell, SetWaypoints);
    }

    public void SetWaypoints(List<CellController> _waypoints) {
        waypoints = _waypoints;
        if (waypoints.Count > 1) {
            if (Vector3.SqrMagnitude(waypoints[1].top-transform.position)<
                Vector3.SqrMagnitude(waypoints[1].top-waypoints[0].top)
            ) {
                waypoints.RemoveAt(0);
            }
        }
        NextPoint();
        Trigger(Channel.Actor.StartMove);
    }

    public List<CellController> getFreeCells(bool includeDiagonal = false) {
        var cells = new List<CellController>();
        BlockController block;
        if ((block = g.map.GetBlockAvailToMove(cell, -1, 0)) != null) cells.Add(block.cell);
        if ((block = g.map.GetBlockAvailToMove(cell, 1, 0)) != null) cells.Add(block.cell);
        if ((block = g.map.GetBlockAvailToMove(cell, 0, -1)) != null) cells.Add(block.cell);
        if ((block = g.map.GetBlockAvailToMove(cell, 0, 1)) != null) cells.Add(block.cell);
        if (!includeDiagonal) return cells;
        if ((block = g.map.GetBlockAvailToMove(cell, -1, -1)) != null) cells.Add(block.cell);
        if ((block = g.map.GetBlockAvailToMove(cell, 1, 1)) != null) cells.Add(block.cell);
        if ((block = g.map.GetBlockAvailToMove(cell, -1, 1)) != null) cells.Add(block.cell);
        if ((block = g.map.GetBlockAvailToMove(cell, 1, -1)) != null) cells.Add(block.cell);
        return cells;
    }

    void NextPoint() {
        if (waypoints.Count > 0) {
            waypoint = waypoints[0];
            waypoints.RemoveAt(0);

            realWaypoint = waypoint.top;
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
                //characterController.Move(transform.forward * speed * Time.deltaTime);
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

}

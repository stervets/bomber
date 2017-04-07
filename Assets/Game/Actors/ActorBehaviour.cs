using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ActorBehaviour : ControllerBehaviour {
    //protected InputBehaviour _input;
    //public abstract InputBehaviour input { get; }

    //protected InputController input;

    public CellController cell;
    private CellController oldCell;

    public float speed;
    //private float squaredSpeed;

    protected BlockController waypoint;
    protected List<BlockController> waypoints;

    protected CharacterController characterController;

    protected Vector3 direction;
    //protected Animator animator;

    protected void PlaceOnCell(CellController _cell) {
        oldCell = cell = _cell;
        transform.position = cell.top;
        g.map.Trigger(Channel.Map.SetObtacle, this, cell);
    }

    protected override void BeforeControllerAwake() {
        characterController = GetComponent<CharacterController>();
        On(Channel.Actor.StartMove, OnStartMove);
        On(Channel.Actor.FinishMove, OnFinishMove);
        //On(Channel.Actor.Fire, OnFire);
        PlaceOnCell(g.map.GetCellFromReal(transform.position));
    }

    private Vector3 realWaypoint;

    public void moveToNextCell(int offsetX, int offsetY) {
        /*
        var nextCell = g.map.GetCell(cell.x + offsetX, cell.y + offsetY);
        if ((nextCell = (nextCell==null || !(checkMapObstacles && g.map.isCellOffsetAvailToMove(cell.x, cell.y, offsetX, offsetY)) ? null : nextCell))!=null) {
            waypoints = new List<Cell> {nextCell};
        }
        NextPoint();
        Trigger(Channel.Actor.StartMove);
        */
    }

    public void moveToCell(CellController targetCell) {
        g.map.FindPath(cell, targetCell, SetWaypoints);
    }

    public void SetWaypoints(List<BlockController> _waypoints) {
        if (_waypoints.Count > 1) {
            waypoints = _waypoints;
            if (Vector3.SqrMagnitude(waypoints[1].cell.top-transform.position)<
                Vector3.SqrMagnitude(waypoints[1].cell.top-waypoints[0].cell.top)
            ) {
                waypoints.RemoveAt(0);
            }
            NextPoint();
            Trigger(Channel.Actor.StartMove);
        }
    }

    public List<CellController> getFreeCells(bool includeDiagonal = false) {
        var cells = new List<CellController>();
        /*
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
        */
        return cells;
    }

    void NextPoint() {
        if (waypoints.Count > 0) {
            waypoint = waypoints[0];
            waypoints.RemoveAt(0);

            if (cell!=waypoint.cell && g.map.obtacles[waypoint.cell]!=null && waypoints.Count > 0) {
                waypoint = null;
                g.map.FindPath(cell, waypoints.Last().cell, SetWaypoints);
                waypoints.Clear();
                Trigger(Channel.Actor.FinishMove);
                return;
            }

            realWaypoint = waypoint.cell.top;
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
                    g.map.Trigger(Channel.Map.SetObtacle, this, cell, g.map.obtacles[oldCell]!=null && g.map.obtacles[oldCell].GetType() == GetType() ? oldCell : null);
                    oldCell = cell;
                }
            }
        }
        OnFixedUpdate();
    }

    /*
    public void Fire() {
        Trigger(Channel.Actor.Fire);
    }
    */

    protected virtual void OnFixedUpdate() {
    }

    protected virtual void OnStartMove(params object[] args) {
    }

    protected virtual void OnFinishMove(params object[] args) {
    }

    /*
    protected virtual void OnFire(params object[] args) {
    }
    */
}

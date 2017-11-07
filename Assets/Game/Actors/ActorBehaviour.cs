using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ActorBehaviour : StateControllerBehaviour
{
    //protected InputBehaviour _input;
    //public abstract InputBehaviour input { get; }

    //protected InputController input;

    public CellController cell;
    private CellController oldCell;

    public float speed;

    private const float gravity = 2f;
    //private float squaredSpeed;

    protected BlockController waypoint;
    protected List<BlockController> waypoints;

    protected CharacterController characterController;

    protected Vector3 direction;

    public int directionX;

    public int directionY;
    //protected Animator animator;

    private Quaternion rotation;

    protected void PlaceOnCell(CellController _cell)
    {
        oldCell = cell = _cell;
        transform.position = cell.top;
        g.map.Trigger(Channel.Map.SetObtacle, this, cell);
    }

    protected override void BeforeControllerAwake()
    {
        characterController = GetComponent<CharacterController>();
        On(Channel.Actor.StartMove, OnStartMove);
        On(Channel.Actor.FinishMove, OnFinishMove);
        PlaceOnCell(g.map.GetCellFromReal(transform.position));
        transform.parent = g.map.gameObject.transform;
    }

    private Vector3 realWaypoint;

    public void moveToNextCell(int offsetX, int offsetY)
    {
        /*
        var nextCell = g.map.GetCell(cell.x + offsetX, cell.y + offsetY);
        if ((nextCell = (nextCell==null || !(checkMapObstacles && g.map.isCellOffsetAvailToMove(cell.x, cell.y, offsetX, offsetY)) ? null : nextCell))!=null) {
            waypoints = new List<Cell> {nextCell};
        }
        NextPoint();
        Trigger(Channel.Actor.StartMove);
        */
    }

    public void moveToCell(CellController targetCell)
    {
        g.map.FindPath(cell, targetCell, SetWaypoints);
    }

    public void SetWaypoints(List<BlockController> _waypoints)
    {
        if (_waypoints.Count > 1)
        {
            waypoints = _waypoints;

            if (Vector3.Dot(waypoints[1].transform.position - waypoints[0].transform.position,
                    transform.position - waypoints[0].transform.position) > 0)
            {
                waypoints.RemoveAt(0);
            }

            Trigger(Channel.Actor.StartMove, waypoints.Last().cell);
            NextPoint();
        }
    }

    public List<CellController> getFreeCells(bool includeDiagonal = false)
    {
        var cells = new List<CellController>();

        BlockController block;
        if (g.map.IsCellAvailToMove(cell, -1, 0)) cells.Add(g.map.GetCell(cell.x - 1, cell.y));
        if (g.map.IsCellAvailToMove(cell, +1, 0)) cells.Add(g.map.GetCell(cell.x + 1, cell.y));
        if (g.map.IsCellAvailToMove(cell, 0, +1)) cells.Add(g.map.GetCell(cell.x, cell.y + 1));
        if (g.map.IsCellAvailToMove(cell, 0, -1)) cells.Add(g.map.GetCell(cell.x, cell.y - 1));
        if (!includeDiagonal) return cells;
        if (g.map.IsCellAvailToMove(cell, -1, -1)) cells.Add(g.map.GetCell(cell.x - 1, cell.y - 1));
        if (g.map.IsCellAvailToMove(cell, -1, +1)) cells.Add(g.map.GetCell(cell.x - 1, cell.y + 1));
        if (g.map.IsCellAvailToMove(cell, +1, +1)) cells.Add(g.map.GetCell(cell.x + 1, cell.y + 1));
        if (g.map.IsCellAvailToMove(cell, +1, -1)) cells.Add(g.map.GetCell(cell.x + 1, cell.y - 1));
        return cells;
    }

    void NextPoint()
    {
        if (waypoints.Count > 0)
        {
            waypoint = waypoints[0];
            waypoints.RemoveAt(0);

            if (cell != waypoint.cell && g.map.obtacles[waypoint.cell] != null && waypoints.Count > 0)
            {
                waypoint = null;
                g.map.FindPath(cell, waypoints.Last().cell, SetWaypoints);
                waypoints.Clear();
                Trigger(Channel.Actor.FinishMove);
                return;
            }

            realWaypoint = waypoint.cell.top;
            realWaypoint.y = transform.position.y;

            direction = (realWaypoint - transform.position).normalized;
            directionX = (int) direction.x;
            directionY = (int) -direction.z;

            if (direction != Vector3.zero)
            {
                rotation = Quaternion.LookRotation(direction);
            }
        }
        else
        {
            waypoint = null;
            Trigger(Channel.Actor.FinishMove);
        }
    }

    private void FixedUpdate()
    {
        if (waypoint != null)
        {
            // Здесь желательно учитывать кусочек перепройденного расстояния
            if (Vector3.Dot(direction, realWaypoint - transform.position) <= 0)
            {
                NextPoint();
            }
            else
            {
                characterController.Move(direction * speed * Time.deltaTime);
                cell = g.map.GetCellFromReal(transform.position);
                if (cell != oldCell)
                {
                    g.map.Trigger(Channel.Map.SetObtacle, this, cell,
                        g.map.obtacles[oldCell] != null && g.map.obtacles[oldCell].GetType() == GetType()
                            ? oldCell
                            : null);
                    oldCell = cell;
                }
            }
        }
        characterController.SimpleMove(Vector3.down * gravity * Time.deltaTime);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 10f * Time.deltaTime);
        OnFixedUpdate();
    }

    /*
    public void Fire() {
        Trigger(Channel.Actor.Fire);
    }
    */

    protected virtual void OnFixedUpdate()
    {
    }

    protected virtual void OnStartMove(params object[] args)
    {
    }

    protected virtual void OnFinishMove(params object[] args)
    {
    }

    /*
    protected virtual void OnFire(params object[] args) {
    }
    */
}
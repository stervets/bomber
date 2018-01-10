using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ActorBehaviour : StateControllerBehaviour {
    public CellController cell, realPositionCell, oldRealPositionCell;
    public float speed;
    private const float gravity = 2f;

    private BlockController waypoint;
    private CellController _waypointCell;
    protected List<BlockController> waypoints;

    protected CharacterController characterController;

    protected Vector3 direction;

    public int directionX;

    public int directionY;
    //protected Animator animator;

    private Quaternion rotation;

    protected void PlaceOnCell(CellController _cell) {
        cell = realPositionCell = _cell;
        transform.position = cell.top;
        g.map.Trigger(Channel.Map.SetObtacle, this, cell);
    }

    protected override void BeforeControllerAwake() {
        characterController = GetComponent<CharacterController>();
        //On(Channel.Actor.StartMove, OnStartMove);
        //On(Channel.Actor.FinishMove, OnFinishMove);
        PlaceOnCell(g.map.GetCellFromReal(transform.position));
        transform.parent = g.map.gameObject.transform;
        ListenTo(g.map, Channel.Actor.NewPosition, actor => {
            if ((ActorBehaviour) actor[0] != this) {
                OnActorNewPosition((ActorBehaviour) actor[0]);
            }
        });
    }

    /*
    public override void OnStart(params object[] args) {
        ListenTo(g.map, Channel.Actor.NewPosition, actor => {
            console.log("Catch");
            OnActorNewPosition((ActorBehaviour) actor[0]);
        });
    }
    */

    private void OnEnable() {
        //g.map.Trigger(Channel.Actor.NewPosition, this);
        GetRealPositionCell();
    }

    public void moveToCell(CellController targetCell) {
        g.map.FindPath(cell, targetCell, SetWaypoints);
    }

    private void SetWaypoints(List<BlockController> _waypoints) {
        if (_waypoints.Count > 1) {
            waypoints = _waypoints;

            if (Vector3.Dot(waypoints[1].transform.position - waypoints[0].transform.position,
                    transform.position - waypoints[0].transform.position) > 0) {
                waypoints.RemoveAt(0);
            }
            Trigger(Channel.Actor.StartMove, waypoints.Last().cell);
            NextPoint();
        }
    }

    public List<CellController> getFreeCells(bool includeDiagonal = false) {
        var cells = new List<CellController>();
        if (g.map.IsCellAvailToMove(cell, -1, 0, false, true)) cells.Add(g.map.GetCell(cell.x - 1, cell.y));
        if (g.map.IsCellAvailToMove(cell, +1, 0, false, true)) cells.Add(g.map.GetCell(cell.x + 1, cell.y));
        if (g.map.IsCellAvailToMove(cell, 0, +1, false, true)) cells.Add(g.map.GetCell(cell.x, cell.y + 1));
        if (g.map.IsCellAvailToMove(cell, 0, -1, false, true)) cells.Add(g.map.GetCell(cell.x, cell.y - 1));
        if (!includeDiagonal) return cells;
        if (g.map.IsCellAvailToMove(cell, -1, -1, false, true)) cells.Add(g.map.GetCell(cell.x - 1, cell.y - 1));
        if (g.map.IsCellAvailToMove(cell, -1, +1, false, true)) cells.Add(g.map.GetCell(cell.x - 1, cell.y + 1));
        if (g.map.IsCellAvailToMove(cell, +1, +1, false, true)) cells.Add(g.map.GetCell(cell.x + 1, cell.y + 1));
        if (g.map.IsCellAvailToMove(cell, +1, -1, false, true)) cells.Add(g.map.GetCell(cell.x + 1, cell.y - 1));
        return cells;
    }

    public List<ControllerBehaviour> getAroundObtacles(bool includeDiagonal = false) {
        var obtacles = new List<ControllerBehaviour>();
        ControllerBehaviour obtacle;
        if (g.map.IsCellAvailToMove(cell, -1, 0) &&
            (obtacle = g.map.obtacles[g.map.GetCell(cell.x - 1, cell.y)]) != null)
            obtacles.Add(obtacle);
        if (g.map.IsCellAvailToMove(cell, 1, 0) &&
            (obtacle = g.map.obtacles[g.map.GetCell(cell.x + 1, cell.y)]) != null)
            obtacles.Add(obtacle);
        if (g.map.IsCellAvailToMove(cell, 0, -1) &&
            (obtacle = g.map.obtacles[g.map.GetCell(cell.x, cell.y - 1)]) != null)
            obtacles.Add(obtacle);
        if (g.map.IsCellAvailToMove(cell, 0, 1) &&
            (obtacle = g.map.obtacles[g.map.GetCell(cell.x, cell.y + 1)]) != null)
            obtacles.Add(obtacle);

        if (!includeDiagonal) return obtacles;

        if (g.map.IsCellAvailToMove(cell, -1, -1) &&
            (obtacle = g.map.obtacles[g.map.GetCell(cell.x - 1, cell.y - 1)]) != null)
            obtacles.Add(obtacle);
        if (g.map.IsCellAvailToMove(cell, 1, 1) &&
            (obtacle = g.map.obtacles[g.map.GetCell(cell.x + 1, cell.y + 1)]) != null)
            obtacles.Add(obtacle);
        if (g.map.IsCellAvailToMove(cell, -1, 1) &&
            (obtacle = g.map.obtacles[g.map.GetCell(cell.x - 1, cell.y + 1)]) != null)
            obtacles.Add(obtacle);
        if (g.map.IsCellAvailToMove(cell, 1, -1) &&
            (obtacle = g.map.obtacles[g.map.GetCell(cell.x + 1, cell.y - 1)]) != null)
            obtacles.Add(obtacle);
        return obtacles;
    }

    public void StopMovement() {
        if (waypoints != null) {
            waypoints.Clear();
        }
    }

    void NextPoint() {
        if (waypoints.Count > 0) {
            waypoint = waypoints[0];
            waypoints.RemoveAt(0);

            if (cell != waypoint.cell) {
                if (g.map.obtacles[waypoint.cell] != null &&
                    g.map.obtacles[waypoint.cell] != this) {
                    if (waypoints.Count > 0) {
                        if (OnMeetObtacle(g.map.obtacles[waypoint.cell], waypoints.Last().cell)) {
                            g.map.FindPath(cell, waypoints.Last().cell, SetWaypoints, true);
                        }
                    }
                    StopMovement();
                    return;
                }

                g.map.Trigger(Channel.Map.SetObtacle, this, waypoint.cell,
                    g.map.obtacles[cell] != null && g.map.obtacles[cell].GetType() == GetType()
                        ? cell
                        : null);
                cell = waypoint.cell;
            }

            _waypointCell = waypoint.cell;
            direction = Vector3.ProjectOnPlane(_waypointCell.top - transform.position, Vector3.up).normalized;

            directionX = (int) direction.x;
            directionY = (int) -direction.z;

            if (direction != Vector3.zero) {
                rotation = Quaternion.LookRotation(direction);
            }
        }
        else {
            _waypointCell = null;
            Trigger(Channel.Actor.FinishMove);
        }
    }

    void GetRealPositionCell() {
        //console.log("rela pos",  Time.time);
        realPositionCell = g.map.GetCellFromReal(transform.position);
        if (oldRealPositionCell != realPositionCell) {
            g.map.Trigger(Channel.Actor.NewPosition, this);
            oldRealPositionCell = realPositionCell;
        }
    }

    private void FixedUpdate() {
        if (_waypointCell != null) {
            //if (GetType() == typeof(EnemyController)) console.log("fixed:", Vector3.Dot(direction, Vector3.ProjectOnPlane(_waypointCell.top - transform.position, Vector3.up)));
            if (Vector3.Dot(direction, _waypointCell.top - transform.position) <= 0.001) {
                NextPoint();
            }
            else {
                characterController.Move(direction * speed * Time.deltaTime);
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 10f * Time.deltaTime);
        }
        characterController.SimpleMove(Vector3.down * gravity * Time.deltaTime);
        if (Time.frameCount % 20 == 0) {
            GetRealPositionCell();
        }
        Trigger(Channel.Actor.FixedUpdate);
    }


    protected virtual void OnActorNewPosition(ActorBehaviour actor) {
        //console.log(actor);
    }

    protected virtual bool OnMeetObtacle(ControllerBehaviour obj, CellController lastCell) {
        return true;
    }
}
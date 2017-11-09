using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyControllerStateSeekPlayer : StateBehaviour
{
    private EnemyController cc
    {
        get { return _controller as EnemyController; }
    }

    private int _directionX = 0;
    private int _directionY = 0;

    protected override void OnAwake(params object[] args)
    {
        SetDefaultState(State.SeekPlayer);
    }

    private void GetRandomDirection(params object[] args)
    {
        var cells = cc.getFreeCells();
        if (cells.Count > 0)
        {
            if (cells.Count > 1)
            {
                cells.Remove(cells.Find(_cell =>_cell.x - cc.cell.x == -cc.directionX && _cell.y - cc.cell.y == -cc.directionY));
            }
            var cell = cells[Random.Range(0, cells.Count)];
            _directionX = cell.x - cc.cell.x;
            _directionY = cell.y - cc.cell.y;
        }
        else
        {
            _directionX = 0;
            _directionY = 0;
        }
        
        //console.log(cells.Count, "x: "+_directionX, "y: "+_directionY);
        MoveToDirection();
    }

    protected override void OnStart(params object[] args)
    {
        //gc.Trigger(0, Channel.Controller.SetState, State.GamePlay);
        ListenTo(cc, Channel.Actor.FinishMove, OnFinishMove);
        On(Channel.Actor.MoveToNextDirection, GetRandomDirection);
    }

    protected void MoveToDirection()
    {
        if (_directionX + _directionY == 0)
        {
            Trigger(1000, Channel.Actor.MoveToNextDirection);
            return;
        }

        if (Random.Range(0, 3) == 0 || !g.map.IsCellAvailToMove(cc.cell, _directionX, _directionY, false, true))
        {
            GetRandomDirection();
        }
        else
        {
            cc.moveToCell(g.map.GetCell(cc.cell.x + _directionX, cc.cell.y+ _directionY));    
        }
    }

    protected void OnFinishMove(params object[] args)
    {
        MoveToDirection();
    }

    protected override void OnEnabled(params object[] args)
    {
        //console.log("playerSeek");
        MoveToDirection();
    }

    protected override void OnDisabled(params object[] args)
    {
    }
}
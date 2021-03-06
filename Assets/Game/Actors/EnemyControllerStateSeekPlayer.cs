﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UniRx;
using UniRx.Triggers;
using Unity.Linq;


public class EnemyControllerStateSeekPlayer : StateBehaviour {
    private EnemyController cc {
        get { return _controller as EnemyController; }
    }

    private int _directionX;
    private int _directionY;

    private PlayerController _player;
    private int delayBeforeAttack = 100;
    private int delayAfterAttack = 2000;

    private Collider radar;


    protected override void OnAwake(params object[] args) {
        SetDefaultState(State.SeekPlayer);
    }

    private void OnRadarCollided(Collider collider) {
        //console.log("Found:", collider.gameObject.Parent().GetComponent<PlayerController>());

        //if (collider.gameObject.Parent().GetType() == typeof(PlayerController)) {
        if ((_player = collider.gameObject.Parent().GetComponent<PlayerController>()) != null) {
            if (g.map.IsCellAvailToMove(cc.cell, _player.cell.x - cc.cell.x, _player.cell.y - cc.cell.y)) {
                console.log("I FOUND YOU!");

                cc.StopMovement();
                //StopMovement();
                
                Observable.Timer(TimeSpan.FromMilliseconds(delayBeforeAttack))
                    .Subscribe(_ => {
                        //console.log("Atttack!");
                        cc.animator.SetTrigger("Attack");
                        Observable.Timer(TimeSpan.FromMilliseconds(delayAfterAttack))
                            .Subscribe(__ => {
                                _player = null;
                                //console.log("Continue");
                                GetRandomDirection();
                            });
                    });
            } else {
                _player = null;
            }
        }
    }

    protected override void OnStart(params object[] args) {
        radar = transform.Find("radar").GetComponent<Collider>();

        radar.OnTriggerEnterAsObservable().Subscribe(OnRadarCollided);

        //gc.Trigger(0, Channel.Controller.SetState, State.GamePlay);
        ListenTo(cc, Channel.Actor.FinishMove, OnFinishMove);
        //ListenTo(cc, Channel.Actor.NewPosition, OnNewPosition);
        ListenTo(cc, Channel.Actor.FixedUpdate, OnFixedUpdate);

        ListenTo(cc, Channel.Actor.StartMove, OnStartMove);
        ListenTo(cc, Channel.Actor.FinishMove, OnFinishMove);

        On(Channel.Actor.MoveToNextDirection, GetRandomDirection);
    }

    private void OnNewPosition(params object[] args) {
        /*
        _player = (PlayerController) cc.getAroundObtacles(true)
            .Find(obtacle => obtacle.GetType() == typeof(PlayerController));
        if (_player == null) return;
        */
    }

    private void OnFixedUpdate(params object[] args) {
        if (_player != null) {
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(_player.transform.position - transform.position),
                10f * Time.deltaTime);
        }
    }

    private void GetRandomDirection(params object[] args) {
        var cells = cc.getFreeCells();
        if (cells.Count > 0) {
            if (cells.Count > 1) {
                cells.Remove(cells.Find(_cell =>
                    _cell.x - cc.cell.x == -cc.directionX && _cell.y - cc.cell.y == -cc.directionY));
            }
            var cell = cells[Random.Range(0, cells.Count)];
            _directionX = cell.x - cc.cell.x;
            _directionY = cell.y - cc.cell.y;
        } else {
            _directionX = 0;
            _directionY = 0;
        }

        //console.log(cells.Count, "x: "+_directionX, "y: "+_directionY);
        MoveToDirection();
    }

    protected void MoveToDirection() {
        if (_directionX + _directionY == 0) {
            Trigger(1000, Channel.Actor.MoveToNextDirection);
            return;
        }

        if (Random.Range(0, 3) == 0 || !g.map.IsCellAvailToMove(cc.cell, _directionX, _directionY, false, true)) {
            GetRandomDirection();
        } else {
            cc.moveToCell(g.map.GetCell(cc.cell.x + _directionX, cc.cell.y + _directionY));
        }
    }

    private void OnStartMove(params object[] args) {
        //OnNewPosition();
        cc.animator.SetBool("Run", true);
        cc.animator.speed = cc.speed / cc.animatorFactor;
    }

    private void StopMovement() {
        cc.animator.SetBool("Run", false);
        cc.animator.speed = 1;
    }

    private void OnFinishMove(params object[] args) {
        StopMovement();
        if (!_player) {
            MoveToDirection();
        }
    }

    protected override void OnEnabled(params object[] args) {
        //console.log("playerSeek");
        MoveToDirection();
    }

    protected override void OnDisabled(params object[] args) {
    }
}
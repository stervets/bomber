using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : ControllerBehaviour {
    private CellController cell;
    private int power;

    protected override void OnAwake(params object[] args) {
        On(Channel.Actor.BlowBomb, OnBlowBomb);
    }

    void OnBlowBomb(params object[] args) {
        g.map.Trigger(Channel.Map.RemoveObtacle, cell);
        g.map.Blow(cell, power);
        Destroy(gameObject);
    }

    protected override void OnStart(params object[] args) {
        LeanTween.scale(gameObject, Vector3.one * 1.4f, 0.3f).setEase( LeanTweenType.pingPong ).setLoopPingPong();
        cell = g.map.GetCellFromReal(transform.position);
        g.map.Trigger(Channel.Map.SetObtacle, this, cell);
    }

    public void SetTimer(int delay = 0, int _power = 1) {
        power = _power;
        Trigger(delay, Channel.Actor.BlowBomb);
    }
}

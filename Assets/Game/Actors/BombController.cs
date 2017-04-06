using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : ControllerBehaviour {
    private CellController cell;
    private int strength;

    protected override void OnAwake(params object[] args) {
        On(Channel.Actor.BlowBomb, OnBlowBomb);
    }

    void OnBlowBomb(params object[] args) {
        g.map.Trigger(Channel.Map.RemoveObtacle, cell);
        cell.Trigger(Channel.Map.BlowCell, strength);
        Destroy(gameObject);
    }

    protected override void OnStart(params object[] args) {
        LeanTween.scale(gameObject, Vector3.one * 1.4f, 0.3f).setEase( LeanTweenType.pingPong ).setLoopPingPong();
        cell = g.map.GetCellFromReal(transform.position);
        g.map.Trigger(Channel.Map.SetObtacle, this, cell);
    }

    public void blow(int delay = 0, int _strength = 1) {
        strength = _strength;
        Trigger(delay, Channel.Actor.BlowBomb);
    }
}

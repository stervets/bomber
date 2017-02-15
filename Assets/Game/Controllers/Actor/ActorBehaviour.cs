using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorController : ControllerBehaviour {
    //protected InputBehaviour _input;
    //public abstract InputBehaviour input { get; }
    protected InputBehaviour input;

    public int x;
    public int y;

    /*
    protected override void OnAwake(params object[] args) {

    }
    */
    void MoveTo(Cell cell, bool checkMapObstacles = false) {

    }
}

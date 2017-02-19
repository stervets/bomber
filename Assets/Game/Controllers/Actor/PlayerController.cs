using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : ActorBehaviour {

    protected override void OnAwake(params object[] args) {
        speed = 0.5f;
        Debug.Log(g.map);
    }

}

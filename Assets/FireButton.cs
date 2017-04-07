using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireButton : MonoBehaviour {

    public void OnFireButtonClick() {
        g.c.Trigger(Channel.Actor.SetPlayerBomb);
    }
}

using UnityEngine;

public class GameControllerStateAwake : StateBehaviour {
	private GameController cc{get{return _controller as GameController;}}

	protected override void OnAwake(params object[] args) {
	    //Debug.Log("on state awake: " + this.ToString());
	    SetDefaultState(State.Awake);
	}

	protected override void OnStart(params object[] args) {
	    //Debug.Log("on state start: " + this.ToString());
	    //cc.SetState (State.GamePlay);
        cc.Trigger(0, Channel.Controller.SetState, State.GamePlay);
	}

	protected override void OnEnabled(params object[] args) {
	}

	protected override void OnDisabled(params object[] args) {
	}
}

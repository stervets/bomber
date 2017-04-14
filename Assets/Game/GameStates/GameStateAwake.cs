using UnityEngine;

public class GameStateAwake : StateBehaviour {
	//private GameController cc{get{return _controller as GameController;}}

	protected override void OnAwake(params object[] args) {
	    SetDefaultState(State.Awake);
	}

	protected override void OnStart(params object[] args) {
	    g.c.write("awake state");
        gc.Trigger(0, Channel.Controller.SetState, State.GamePlay);
	}

	protected override void OnEnabled(params object[] args) {
	}

	protected override void OnDisabled(params object[] args) {
	}
}

using UnityEngine;

public class GameStateAwake : StateBehaviour {
	//private GameController cc{get{return _controller as GameController;}}

	protected override void OnAwake(params object[] args) {
	    SetDefaultState(State.Awake);
	}

	protected override void OnStart(params object[] args) {
	    gc.Trigger(0, Channel.Controller.SetState, State.GamePlay);
		//gc.Trigger(0, Channel.Controller.SetState, State.Editor);
	}

	protected override void OnEnabled(params object[] args) {
	}

	protected override void OnDisabled(params object[] args) {
	}
}

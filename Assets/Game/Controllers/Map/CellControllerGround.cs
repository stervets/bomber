using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellControllerGround : CellController {

	private ParticleSystem ExplosionFX;
	private ParticleSystem sparklesFX;
	private bool sparkles = false;

	protected override void OnAwake(params object[] args) {
	}

	protected override void OnStart(params object[] args) {
		ExplosionFX = transform.FindChild ("ExplosionFX").GetComponent<ParticleSystem> ();

		Transform sparklesChild = transform.FindChild ("sparkles");
		if (sparklesChild == null) {
			sparkles = false;
		} else {
			sparklesFX = transform.FindChild ("sparkles").GetComponent<ParticleSystem> ();
			sparkles = true;
		}

	}


	void OnMakeBlowCell(object[] args){
		if (sparkles && (bool)args [0]) {
			sparklesFX.Play ();
		}
		ExplosionFX.Play ();
	}

	protected override void SetListeners(){
		ListenTo (cell.radio, Channel.Map.MakeBlowCell, OnMakeBlowCell);
	}
}

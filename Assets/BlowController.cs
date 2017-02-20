//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BlowController : MonoBehaviour {

	ParticleSystem particle;
    private Vector3 asd;

	void Awake () {
		particle = GetComponent<ParticleSystem> ();
	}

	void Blow(int milliseconds){
		Observable.Timer (System.TimeSpan.FromMilliseconds(milliseconds)).Subscribe( _ =>{
			particle.Play ();
		});
	}
	void Update () {
		//if (Input.GetKeyDown (KeyCode.Space)) {
		//	Blow (0);
		//}
	}

}

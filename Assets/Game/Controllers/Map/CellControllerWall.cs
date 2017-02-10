using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CellControllerWall : CellController {

	private Transform Explosion;
	private Transform sparkles;

	private Transform cubeParts;
	private Rigidbody[] blownBlocks;
	private BoxCollider[] blownBlocksColliders;

	private Vector3 blowPosition;
	private float variety = 0.1f;

	private ParticleSystem ExplosionFX;
	private ParticleSystem sparklesFX;
	private ParticleSystem bigSparklesFX;
	private GameObject wall;


	protected override void OnAwake(params object[] args) {
	}

	protected override void OnStart(params object[] args) {
		wall = transform.FindChild ("wallBlock").gameObject;
		Explosion = transform.FindChild ("ExplosionFX");
		ExplosionFX = Explosion.GetComponent<ParticleSystem> ();

		sparkles = transform.FindChild ("sparkles");
		sparklesFX = sparkles.GetComponent<ParticleSystem> ();
		bigSparklesFX = transform.FindChild("bigSparkles").GetComponent<ParticleSystem> ();

		cubeParts = transform.FindChild ("cubeParts");
		blownBlocks = cubeParts.GetComponentsInChildren<Rigidbody> ();
		blownBlocksColliders = cubeParts.GetComponentsInChildren<BoxCollider> ();
		blowPosition = transform.position - Vector3.up*0.5f;
	}

	void OnMakeBlowCell(object[] args){
		bool blowInitializer = (bool)args [0];
		bool above = (bool)args [1];

		Explosion.position = transform.position + (above ? Vector3.up : Vector3.zero);
		sparkles.position = Explosion.position;

		if (cell.movable>1 && (blowInitializer || !above)) {
			Destroy (wall);
			cell.blowable = (int)Blowable.Yes;
			cell.movable = (int)Movable.Yes;
			bigSparklesFX.Play ();
			Destroy (bigSparklesFX.gameObject, 1);

			blowPosition = blowPosition + Vector3.left * ((int)args [2]/2f) + Vector3.forward * ((int)args [3]/2f);
			blowPosition = blowPosition + Vector3.forward * Random.Range (-variety, variety) + Vector3.left * Random.Range (-variety, variety) + Vector3.up * Random.Range (-variety, variety);
			//transform.FindChild ("Sphere").position = blowPosition;
			cubeParts.gameObject.SetActive (true);
			foreach (Rigidbody blownBlock in blownBlocks) {
				blownBlock.AddExplosionForce (20f, blowPosition, 1.2f, 0f, ForceMode.VelocityChange);
			}

			Observable.Timer (System.TimeSpan.FromMilliseconds(100)).Subscribe( _ =>{
				foreach (BoxCollider collider in blownBlocksColliders) {
					collider.enabled = false;
				}
			});

			Destroy (cubeParts.gameObject, 2f);
		}

		if (blowInitializer) {
			sparklesFX.Play ();
		}
		ExplosionFX.Play ();
	}

	protected override void SetListeners(){
		ListenTo (cell.radio, Channel.Map.MakeBlowCell, OnMakeBlowCell);
	}
}

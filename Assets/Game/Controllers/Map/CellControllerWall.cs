using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class CellControllerWall : CellController {
    public GameObject wallPrefab;
    private List<WallBlockController> walls;

	private Transform Explosion;
	private Transform sparkles;
    private Transform bigSparkles;

    private ParticleSystem ExplosionFX;
    private ParticleSystem sparklesFX;
    private ParticleSystem bigSparklesFX;

	private Vector3 blowPosition;
	private float variety = 0.1f;

	protected override void OnAwake(params object[] args) {
	    walls = new List<WallBlockController>();
	}

	protected override void OnStart(params object[] args) {
		Explosion = transform.FindChild ("ExplosionFX");
		ExplosionFX = Explosion.GetComponent<ParticleSystem> ();

		sparkles = transform.FindChild ("sparkles");
		sparklesFX = sparkles.GetComponent<ParticleSystem> ();

	    bigSparkles = transform.FindChild ("bigSparkles");
	    bigSparklesFX = bigSparkles.GetComponent<ParticleSystem> ();
	}

	void OnMakeBlowCell(object[] args){
		bool blowInitializer = (bool)args [0];
	    Explosion.position = new Vector3(cell.realPosition.x, (int) args[3], cell.realPosition.z);
	    bigSparkles.position = sparkles.position = Explosion.position;
        /*
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
        */
		if (blowInitializer) {
			sparklesFX.Play ();
		}
		ExplosionFX.Play ();
	}

	protected override void InitCellController(){
		ListenTo (cell.radio, Channel.Map.MakeBlowCell, OnMakeBlowCell);
	    if ((cell.movable = cell.param + 1) == 1) {
	        cell.blowable = 2;
	    }

	    for (var i = 0; i < cell.param; i++) {
	        //Map.GetRealPositionFromTable(x, y, realLevel);
	        var wall = Instantiate(wallPrefab, Map.GetRealPositionFromTable(cell.x, cell.y, cell.level + i),
	            cell.realDirection);
	        wall.transform.parent = transform;
	        walls.Add(wall.GetComponent<WallBlockController>());
	    }
        // walls
	}
}

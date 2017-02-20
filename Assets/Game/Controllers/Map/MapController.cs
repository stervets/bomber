using UnityEngine;

public class MapController : ControllerBehaviour {
	private Map map;
	public GameObject[] mapPrefabs;

	void GenerateMapAndSave(int sizeX, int sizeY, string filename){
		map = new Map(sizeX, sizeY);
		ListenTo (g.c, Channel.Map.Loaded, OnMapLoaded);
		ListenTo (map.radio, Channel.Map.NewCell, OnMapNewCell);
		map.GenerateMap();
		map.saveToFile (filename);
	}

	void LoadFromFile(string filename){
		map = new Map();
		ListenTo (g.c, Channel.Map.Loaded, OnMapLoaded);
		ListenTo (map.radio, Channel.Map.NewCell, OnMapNewCell);
		map.loadFromFile (filename);
	}

	protected override void OnAwake(params object[] args) {
	}

	protected override void OnStart(params object[] args) {
		LoadFromFile ("test");
		//GenerateMapAndSave(20, 15, "test");
	}

	protected void OnMapNewCell(params object[] args) {
		var cell = args [0] as Cell;
	    if (cell == null) return;
	    var cellObject = Instantiate (mapPrefabs [cell.prefab], cell.realPosition, cell.realDirection);
	    cellObject.transform.parent = transform;
	    cellObject.GetComponent<CellController> ().BindCell(cell);
	    cell.setPositions();
	}


	protected void OnMapLoaded(params object[] args) {
		Debug.Log ("Map loaded");
	}

}

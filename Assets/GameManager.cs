using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;
/*
public class PathCell{
	public int x;
	public int y;
	public float g = 0f; // Path length
	public float h = 0f; // Distance to finish
	public float hs = 0f; // Distance to start
	public float hStart {get { return hs + h;}} // Closed list sorter value
	public float f {get { return g + h;}} // Open list sorter value
	public PathCell parent = null;

	public PathCell(int _x, int _y)
	{
		x = _x;
		y = _y;
	}

	public PathCell(int _x, int _y, float _g, float _h, PathCell _parent)
	{
		x = _x;
		y = _y;
		g = _g;
		h = _h;
		parent = _parent;
	}

	public PathCell(Vector2 point)
	{
		x = (int) point.x;
		y = (int) point.y;
	}

	public override string ToString(){
		//return string.Format ("x: {0}, y: {1}, g: {0}, h: {1}, f: {0}", x, y, g, h, f);
		return string.Format ("[{0},{1}] f:{2}, h:{3}", x, y, f, h);
	}
}

public class Waypoint {
	public static Vector3 GetTablePositionFromReal(Vector3 position){
		return new Vector3(Mathf.Round(position.x), -Mathf.Round(position.z), 0);
	}

	public static Vector3 GetRealPositionFromTable(Vector3 position){
		return new Vector3(position.x, 0, -position.y);
	}

	public static Vector3 GetRealPositionFromTable(int x, int y){
		return new Vector3(x, 0, -y);
	}

	private int _tableX;
	private int _tableY;
	private Vector3 _realPoint;

	public int tableX{get{return _tableX;}}
	public int tableY{get{return _tableY;}}
	public Vector3 realPoint{get{return _realPoint;}}

	public Waypoint(int x, int y){
		_tableX = x;
		_tableY = y;
		_realPoint = GetRealPositionFromTable (x, y);
	}

	public override string ToString(){
		return string.Format ("x: {0}, y: {1}", tableX, tableY);
	}
}

public class GameManager : MonoBehaviour {
	private int[,] defaultField = new[,] {
		{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
		{1,0,0,0,1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1},
		{1,0,1,0,1,0,1,1,1,1,1,0,1,0,1,1,1,1,1,1,0,1},
		{1,0,1,0,1,0,1,0,0,0,1,0,1,0,1,0,0,0,0,1,0,1},
		{1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,1,1,1,0,1},
		{1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,0,1,0,1},
		{1,0,1,0,1,0,1,0,1,1,1,0,1,0,0,1,1,1,1,1,0,1},
		{1,0,1,0,1,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0,0,1},
		{1,0,1,0,1,0,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1},
		{1,0,1,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,1},
		{1,0,1,0,1,0,1,0,1,0,1,1,0,0,0,0,0,1,1,1,0,1},
		{1,0,1,0,1,0,1,0,1,1,1,1,1,1,1,1,0,1,0,0,0,1},
		{1,0,1,0,0,0,1,0,0,0,0,0,0,0,0,1,0,1,0,0,0,1},
		{1,0,1,1,1,0,1,0,1,1,1,1,1,0,1,1,1,0,0,0,0,1},
		{1,0,0,0,0,0,1,0,1,0,0,0,1,0,1,0,1,1,0,0,0,1},
		{1,1,1,0,1,0,1,0,1,0,1,0,1,0,1,0,1,0,0,1,0,1},
		{1,1,1,0,1,0,0,0,1,0,1,0,0,0,0,0,1,0,0,1,0,1},
		{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
	};

	public GameObject wallPrefab;
	public GameObject wallPrefab2;
	public GameObject pathPrefab;
	public GameObject blowPrefab;

	public static GameManager instance = null;

	public Field field;

	//private Vector3 pointerPosition = new Vector3(8,0,-8);
	private Camera mainCamera;
	private Vector3 mouseCoords = Vector3.zero;

	private bool isEndPoint = false;
	private Vector2 startPoint;
	private Vector2 cursorPoint;


	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance!=this) {
			Destroy (gameObject);
		}
		field = new Field (defaultField, wallPrefab, wallPrefab2, pathPrefab, blowPrefab);
	}

	Vector3 GetRealPositionFromCamera(Vector3 position){
		if (!mainCamera) {
			return Vector3.zero;
		}
		Ray ray = mainCamera.ScreenPointToRay(position);

		float rayLength = (1f - Vector3.Dot(Vector3.up, ray.origin))/Vector3.Dot(Vector3.up, ray.direction);
		return ray.origin + ray.direction * rayLength;

		//return ray.origin + ray.direction / Vector3.Dot(ray.direction / ray.origin.y, Vector3.down);
	}

	Vector3 GetTablePositionFromReal(Vector3 position){
		//Debug.Log (position);
		return new Vector3(Mathf.Round(position.x), -Mathf.Round(position.z), 0);
	}

	Vector3 GetRealPositionFromTable(Vector3 position){
		return new Vector3(position.x, 0, -position.y);
	}

	void OnDrawGizmos() {
		Gizmos.color = new Color(0.4f, 1, 0.4f, 0.4f);
		Gizmos.DrawCube (GetRealPositionFromTable(GetTablePositionFromReal(GetRealPositionFromCamera (mouseCoords))) + Vector3.up*0.5f, Vector3.one);
		//Gizmos.DrawCube (GetRealPositionFromCamera (mouseCoords), Vector3.one*0.3f);
	}

	// Use this for initialization
	void Start () {
		mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera>();

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			mouseCoords = Input.mousePosition;
			cursorPoint = GetTablePositionFromReal (GetRealPositionFromCamera (mouseCoords));
			if (isEndPoint) {
				field.FindPath (startPoint, cursorPoint, (waypoints) => {
					waypoints.ForEach((waypoint)=>{
						GameObject.Instantiate(pathPrefab, waypoint.realPoint, Quaternion.identity);
						//Debug.Log(waypoint);
					});
				});
			} else {
				startPoint = cursorPoint;
				GameObject[] waypoints = GameObject.FindGameObjectsWithTag ("WayPoint");
				for (int i = 0, j = waypoints.Length; i < j; i++) {
					GameObject.Destroy (waypoints [i]);
					//waypoints [i].Destroy ();
				}
			}
			isEndPoint = !isEndPoint;
			//Debug.Log (cursorPoint);
			//Debug.Log (GetRealPositionFromTable(GetTablePositionFromReal(GetRealPositionFromCamera (mouseCoords))));
			//GetRealXYFromCamera();
		}	
	}
}
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController : StateControllerBehaviour {
    public GameObject MapPrefab;
    public Material dissolveMaterial;
    public GameObject clickPointerPrefab;

    //[HideInInspector] public Map map;
    private Text debugText;

    protected override void Awake() {
        Time.fixedDeltaTime = g.FixedUpdateFrameRate;
        SetDefaultState(State.Awake);
        g.c = this;
        base.Awake();
    }

    protected override void OnAwake(params object[] args) {
        //Debug.Log("on controller awake: " + this.ToString());
    }

    protected override void OnStart(params object[] args) {
        //Debug.Log("on controller start: " + this.ToString());
        g.camera = Camera.main;
        g.cameraController = g.camera.GetComponent<CameraStateController>();

        debugText = GameObject.Find("DebugText").GetComponent<Text>();
    }

    void Update() {
        //Camera.main.transform.position = Camera.main.transform.position +
        //	(Vector3.left * (0.1f * Time.deltaTime));
    }

    public void write(string str) {
        debugText.text += str + "\n";
    }


    /*
    public Cell GetCellFromCamera(Vector3 position) {
        RaycastHit hit;
        var ray = g.camera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out hit, 50, LayerMask.GetMask("Map"))) {
            return (hit.collider.gameObject.CompareTag("MapCell")
                ? hit.collider.gameObject.GetComponent<BlockController>().cell
                : hit.collider.gameObject.GetComponentInParent<BlockController>().cell);
        }
        var rayLength = (-0.5f - Vector3.Dot(Vector3.up, ray.origin)) / Vector3.Dot(Vector3.up, ray.direction);
        var cellPosition = Map.GetTablePositionFromReal(ray.origin + ray.direction * rayLength);
        if (cellPosition.x < 0) {
            cellPosition.x = 0;
        } else {
            if (cellPosition.x >= g.map.countX) {
                cellPosition.x = g.map.countX - 1;
            }
        }

        if (cellPosition.y < 0) {
            cellPosition.y = 0;
        } else {
            if (cellPosition.y >= g.map.countY) {
                cellPosition.y = g.map.countY - 1;
            }
        }
        return g.map.cell[(int) cellPosition.x, (int) cellPosition.y];
    }
    */
}
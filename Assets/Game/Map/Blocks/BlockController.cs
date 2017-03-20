//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
using System;
using System.ComponentModel;
using UnityEngine;

public class BlockController : ControllerBehaviour {
    public bool isFlat;
    public bool isLadder;
    public bool isBlowable;

    [HideInInspector] public CellController cell;
    [HideInInspector] public byte prefabIndex;
    [HideInInspector] public byte direction;

    public string export() {
        return string.Join(".", new string[]{
            prefabIndex.ToString(),
            direction.ToString()
        });
    }


    protected bool imported;

    public void import(string data) {
        var dataItem = data.Split('.');
        prefabIndex = byte.Parse(dataItem[0]);
        SetDirection(byte.Parse(dataItem[1]));
        imported = true;
    }

    public void SetDirection(int _direction) {
        direction = (byte)(_direction < 0 ? 3 : (_direction > 3 ? 0 : _direction));
        transform.rotation = MapController.GetRotationFromDirection(direction);
    }

    public void Remove() {
        Debug.Log(cell);
        cell.RemoveBlock(this);
    }

    private float targetY;
    private float speed;
    private void FixedUpdate() {
        if (speed>0) {
            transform.Translate(0, -speed * Time.deltaTime, 0);
            if (transform.position.y <= targetY) {
                transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
                speed = 0;
            } else {
                speed += 0.5f;
            }
        }
    }


    void OnMoveDown(params object[] args) {
        targetY = (float) args[0];
        speed = 0.1f;
    }

    protected override void OnStart(params object[] args) {
        if ((cell = transform.parent.GetComponent<CellController>()) != null) {
            On("MoveDown", OnMoveDown);
        }

        if (imported)return;
        name = name.Substring(0, name.IndexOf('('));
        for (byte i = 0; i < g.map.blockPrefabs.Length; i++) {
            if (g.map.blockPrefabs[i].name != name) continue;
            prefabIndex = i;
            break;
        }
    }
}
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
using System;
using UnityEngine;

public class BlockController : ControllerBehaviour {
    public bool isFlat;
    public bool isLadder;
    public bool isBlowable;

    [HideInInspector] public CellController cell;
    [HideInInspector] public int prefabIndex;
    [HideInInspector] public byte direction;

    public string export() {
        return string.Join(",", new string[]{
            prefabIndex.ToString(),
            direction.ToString()
        });
    }

    protected bool imported;
    public void import(string data) {
        var dataItem = data.Split(',');
        prefabIndex = int.Parse(dataItem[0]);
        SetDirection(byte.Parse(dataItem[1]));
        imported = true;
    }

    public void SetDirection(int _direction) {
        direction = (byte)(_direction < 0 ? 3 : (_direction > 3 ? 0 : _direction));
        transform.rotation = MapController.GetRotationFromDirection(direction);
    }

    public void Remove() {
        cell.RemoveBlock(this);
    }

    protected override void OnStart(params object[] args) {
        if (imported)return;
        cell = transform.parent.GetComponent<CellController>();
        name = name.Substring(0, name.IndexOf('('));
        for (var i = 0; i < g.map.blockPrefabs.Length; i++) {
            if (g.map.blockPrefabs[i].name != name) continue;
            prefabIndex = i;
            break;
        }
    }
}
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Linq;
using Random = UnityEngine.Random;

public class MapController : ControllerBehaviour {
    //private Map map;
    //public GameObject[] blocks;
    public int seed;

    public List<List<CellController>> cells;
    public ushort width;
    public ushort height;

    public GameObject cellPrefab;
    public GameObject editorCursorPrefab;
    public GameObject editorCellItemPrefab;
    public GameObject[] blockPrefabs;

    public void DestroyField() {
        if (cells == null) return;
        while (cells.Count > 0) {
            while (cells.Last().Count > 0) {
                cells.Last().Last().gameObject.Destroy();
            }
            cells.Remove(cells.Last());
        }
    }

    public void MakeField(ushort _width, ushort _height) {
        DestroyField();
        width = _width;
        height = _height;
        cells = new List<List<CellController>>();
        for (ushort y = 0; y < height; y++) {
            cells.Add(new List<CellController>());
            for (ushort x = 0; x < width; x++) {
                var cell = Instantiate(cellPrefab);
                cell.transform.parent = transform;
                var cellController = cell.GetComponent<CellController>();
                cellController.SetPosition(x, y);
                //cellController.CreateBlock(0);
                cells.Last().Add(cellController);
            }
        }
    }

    public CellController GetCell(ushort x, ushort y) {
        return x < width && y < height ? cells[y][x] : null;
    }

    public CellController GetCell(Vector3 position) {
        return GetCell((ushort) position.x, (ushort) position.y);
    }

    public CellController GetCellFromReal(Vector3 position) {
        return GetCell(GetTablePositionFromReal(position));
    }


    public BlockController GetBlock(Vector3 position) {
        var cell = GetCell(position);
        if (cell == null) return null;
        var z = (int)(position.z - cell.z);
        return z >= 0 && z < cell.blocks.Count ? cell.blocks[z] : null;
    }

    public static Vector3 GetRealPositionFromTable(int x, int y, int z = 0) {
        return new Vector3(x, z, -y);
    }

    public static Vector3 GetRealPositionFromTable(Vector3 position) {
        return GetRealPositionFromTable((int) position.x, (int) position.y, (int) position.z);
    }

    public static Vector3 GetTablePositionFromReal(Vector3 position) {
        return new Vector3(Mathf.Round(position.x), -Mathf.Round(position.z), Mathf.Round(position.y));
    }

    public static Quaternion GetRotationFromDirection(int direction) {
        return Quaternion.AngleAxis(direction * 90, Vector3.up);
    }

    protected override void OnAwake(params object[] args) {
        g.map = this;
        Random.InitState(seed);
    }

    protected override void OnStart(params object[] args) {
        console.log("map controller started");
    }

    public void importCells(string data) {
        var dataItem = data.Split(';');
        var index = 0;

        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                cells[y][x].import(dataItem[index++]);
            }
        }
    }

    public string exportCells() {
        var o = new List<string>();
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                o.Add(cells[y][x].export());
            }
        }
        return string.Join(";", o.ToArray());
    }

    private string filename = "map01";

    public void saveToFile(string _filename = "") {
        if (_filename == "") {
            _filename = filename;
        }
        var ini = new INIParser();
        ini.Open(g.MapPath + _filename + ".ini");
        ini.WriteValue("Map", "width", width);
        ini.WriteValue("Map", "height", height);
        ini.WriteValue("Map", "cells", exportCells());
        ini.Close();
    }

    public void loadFromFile(string _filename = "") {
        if (_filename == "") {
            _filename = filename;
        }
        var ini = new INIParser();
        ini.Open(g.MapPath + _filename + ".ini");
        MakeField((ushort)ini.ReadValue("Map", "width", 0), (ushort)ini.ReadValue("Map", "height", 0));
        importCells(ini.ReadValue("Map", "cells", ""));
        ini.Close();
        Trigger(Channel.Map.Loaded);
    }
}
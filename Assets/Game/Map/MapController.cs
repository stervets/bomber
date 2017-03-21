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
    public int width;
    public int height;

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

    public void MakeField(int _width, int _height) {
        DestroyField();
        width = _width;
        height = _height;
        cells = new List<List<CellController>>();
        for (int y = 0; y < height; y++) {
            cells.Add(new List<CellController>());
            for (int x = 0; x < width; x++) {
                var cell = Instantiate(cellPrefab);
                cell.transform.parent = transform;
                var cellController = cell.GetComponent<CellController>();
                cellController.SetPosition(x, y);
                //cellController.CreateBlock(0);
                cells.Last().Add(cellController);
            }
        }
    }

    public CellController GetCell(int x, int y) {
        return x >= 0 && y >= 0 && x < width && y < height ? cells[y][x] : null;
    }

    public CellController GetCell(Vector3 position) {
        return GetCell((int) position.x, (int) position.y);
    }

    public CellController GetCellFromReal(Vector3 position) {
        return GetCell(GetTablePositionFromReal(position));
    }


    public BlockController GetBlock(Vector3 position) {
        var cell = GetCell(position);
        if (cell == null) return null;
        var z = (int) (position.z - cell.z);
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
        MakeField(ini.ReadValue("Map", "width", 0), ini.ReadValue("Map", "height", 0));
        importCells(ini.ReadValue("Map", "cells", ""));
        ini.Close();
        Trigger(Channel.Map.Loaded);
    }

    public BlockController GetBlockOnSameLevel(BlockController currentBlock, CellController cell, int offset = 0) {
        if (cell == null) return null;
        var nextBlockIndex = currentBlock.cell.blocks.IndexOf(currentBlock) - (cell.z - currentBlock.cell.z - offset);
        return nextBlockIndex < 0 || nextBlockIndex >= cell.blocks.Count ? null : cell.blocks[nextBlockIndex];
    }

    public static readonly int[][] LadderExits = {
        new[] {0, -1},
        new[] {1, 0},
        new[] {0, 1},
        new[] {-1, 0}
    };

    public BlockController[] GetLadderExits(BlockController ladderBlock) {
        var exitTop = LadderExits[ladderBlock.direction];
        var exitBottom = LadderExits[Mathf.Clamp(ladderBlock.direction + 2, 0, 3)];
        var cell = GetCell(ladderBlock.cell.x + exitTop[0], ladderBlock.cell.y + exitTop[1]);
        var exitTopBlock = GetBlockOnSameLevel(ladderBlock, cell);
        if (exitTopBlock == null) {
            exitTopBlock = GetBlockOnSameLevel(ladderBlock, cell, 1);
            if (exitTopBlock != null && exitTopBlock.isLadder) {
                exitTopBlock = exitTopBlock.direction == ladderBlock.direction ? exitTopBlock : null;
            }
        }
        return new[] {
            exitTopBlock,
            GetBlockOnSameLevel(ladderBlock, GetCell(ladderBlock.cell.x + exitBottom[0], ladderBlock.cell.y + exitBottom[1]), -1)
        };
    }

    public bool isCellAvailToMove(CellController currentCell, CellController nextCell, bool passBlowable = false) {
        if (nextCell == null) return false;
        var currentBlock = currentCell.lastBlock;
        var nextBlock = GetBlockOnSameLevel(currentBlock, nextCell);
        var diagonal = currentCell.x != nextCell.x && currentCell.y != nextCell.y;

        if (nextBlock == null) {
            if (diagonal || !currentBlock.isLadder) return false;
            var ladderExits = GetLadderExits(currentBlock);
            return (ladderExits[0] != null && GetBlockOnSameLevel(currentBlock, nextCell, 1) == ladderExits[0]) ||
                   (ladderExits[1] != null && GetBlockOnSameLevel(currentBlock, nextCell, -1) == ladderExits[1]);
        }

        if (nextBlock == nextCell.lastBlock) {
            if (nextBlock.isLadder) {
                if (diagonal) return false;
                if (currentBlock.isLadder && currentBlock.direction == nextBlock.direction) return true;
                return GetLadderExits(nextBlock)[0] == currentBlock;
            }
            if (diagonal) {
                var hCell = GetBlockOnSameLevel(currentBlock, cells[currentCell.y][nextCell.x]);
                if (hCell == null || hCell!=hCell.cell.lastBlock) return false;
                var vCell = GetBlockOnSameLevel(currentBlock, cells[nextCell.y][currentCell.x]);
                if (vCell == null || vCell!=vCell.cell.lastBlock || !hCell.isFlat || !vCell.isFlat || hCell.isLadder || vCell.isLadder) return false;
            }
            return nextBlock.isFlat || (passBlowable && nextBlock.isBlowable);
        }

        var ladder = GetBlockOnSameLevel(currentBlock, nextCell, 1);
        if (ladder == null) return false;
        if (ladder.isLadder) {
            if (diagonal) return false;
            return GetLadderExits(ladder)[1] == currentBlock;
        }
        return passBlowable && nextBlock.isBlowable;
    }

    private readonly LayerMask mapLayer = LayerMask.GetMask("Map");
    private RaycastHit hit;
    public CellController GetCellFromCamera(Vector3 position) {
        var ray = g.camera.ScreenPointToRay(position);
        if (Physics.Raycast(ray, out hit, 50, mapLayer)) {
            return hit.collider.gameObject.GetComponentInParent<BlockController>().cell;
        }
        var rayLength = (-0.5f - Vector3.Dot(Vector3.up, ray.origin)) / Vector3.Dot(Vector3.up, ray.direction);
        var cellPosition = Map.GetTablePositionFromReal(ray.origin + ray.direction * rayLength);
        if (cellPosition.x < 0) {
            cellPosition.x = 0;
        } else {
            if (cellPosition.x >= width) {
                cellPosition.x = width - 1;
            }
        }

        if (cellPosition.y < 0) {
            cellPosition.y = 0;
        } else {
            if (cellPosition.y >= height) {
                cellPosition.y = height - 1;
            }
        }
        return cells[(int) cellPosition.y][(int) cellPosition.x];
    }
}
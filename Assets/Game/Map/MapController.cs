using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Linq;
using Random = UnityEngine.Random;
using UniRx;

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
    public GameObject debugCubePrefab;
    public GameObject[] blockPrefabs;

    public GameObject[] actorPrefabs;
    public GameObject bombPrefab;
    public Dictionary<string, GameObject> actorPrefabsDictionary;

    public Dictionary<CellItem, List<CellController>> cellItems;
    public Dictionary<CellController, ControllerBehaviour> obtacles;

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
        cellItems = new Dictionary<CellItem, List<CellController>>();
        obtacles = new Dictionary<CellController, ControllerBehaviour>();
        for (var y = 0; y < height; y++) {
            cells.Add(new List<CellController>());
            for (var x = 0; x < width; x++) {
                var cell = Instantiate(cellPrefab);
                cell.transform.parent = transform;
                var cellController = cell.GetComponent<CellController>();
                cellController.SetPosition(x, y);
                //cellController.CreateBlock(0);
                cells.Last().Add(cellController);
                obtacles[cellController] = null;
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

    void OnSetCellItem(params object[] args) {
        var cell = (CellController) args[0];
        var oldItem = (CellItem) args[1];

        if (cell.item!=CellItem.Null && !cellItems.ContainsKey(cell.item)) {
            cellItems[cell.item] = new List<CellController>();
        }
        if (oldItem != CellItem.Null) {
            cellItems[oldItem].Remove(cell);
        }
        if (cell.item != CellItem.Null) {
            cellItems[cell.item].Add(cell);
        }
    }

    void OnSetObtacle(params object[] args) {
        if (args.Length > 2 && args[2]!=null) {
            OnRemoveObtacle((CellController) args[2]);
        }
        obtacles[(CellController) args[1]] = (ControllerBehaviour) args[0];
    }

    void OnRemoveObtacle(params object[] args) {
        obtacles[(CellController) args[0]] = null;
    }

    protected override void OnAwake(params object[] args) {
        actorPrefabsDictionary = new Dictionary<string, GameObject>();

        On(Channel.Map.SetCellItem, OnSetCellItem);
        On(Channel.Map.SetObtacle, OnSetObtacle);
        On(Channel.Map.RemoveObtacle, OnRemoveObtacle);
        On(Channel.Map.BlowCell, OnBlowCell);

        foreach (var actorPrefab in actorPrefabs) {
            actorPrefabsDictionary[actorPrefab.name] = actorPrefab;
        }
        mapLayer = LayerMask.GetMask("Map");
        g.map = this;
        Random.InitState(seed);
    }

    protected override void OnStart(params object[] args) {
        //console.log("map controller started");
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
        ini.Open(Application.streamingAssetsPath + g.MapPath + _filename + ".ini");
        ini.WriteValue("Map", "width", width);
        ini.WriteValue("Map", "height", height);
        ini.WriteValue("Map", "cells", exportCells());
        ini.Close();
    }

    public void loadINIFile(string data, bool loadFromFile = false) {
        var ini = new INIParser();
        if (loadFromFile) {
            ini.Open(data);
        } else {
            ini.OpenFromString(data);
        }

        MakeField(ini.ReadValue("Map", "width", 0), ini.ReadValue("Map", "height", 0));
        importCells(ini.ReadValue("Map", "cells", ""));
        ini.Close();
        g.cameraController.SetState(State.GamePlay);
        Trigger(Channel.Map.Loaded);
    }

    public void loadFromFile(string _filename = "") {
        g.c.write("load from file");
        if (_filename == "") {
            _filename = filename;
        }

        var file = Application.streamingAssetsPath + g.MapPath + _filename + ".ini";


        g.c.write(file);
        if (file.IndexOf("jar:", StringComparison.Ordinal) == 0) {
            g.c.write("get file via WWW");
            ObservableWWW.Get(file).Subscribe(
                    data => { loadINIFile(data); }, // onSuccess
                    ex => { g.c.write(ex.Message); }); // onError
        } else {
            g.c.write("get file via local");
            loadINIFile(file, true);
        }
    }

    public BlockController GetBlockOnSameLevel(BlockController currentBlock, CellController cell, int offset = 0) {
        if (cell == null) return null;
        var nextBlockIndex = currentBlock.cell.blocks.IndexOf(currentBlock) - (cell.z - currentBlock.cell.z - offset);
        return cell.GetBlockByIndex(nextBlockIndex);
    }

    public static readonly int[][] LadderExits = {
        new[] {0, -1},
        new[] {1, 0},
        new[] {0, 1},
        new[] {-1, 0}
    };

    public BlockController[] GetLadderExits(BlockController ladderBlock) {
        var exitTop = LadderExits[ladderBlock.direction];
        var exitBottom = LadderExits[(ladderBlock.direction + 2) % 4];
        var cell = GetCell(ladderBlock.cell.x + exitTop[0], ladderBlock.cell.y + exitTop[1]);
        var exitTopBlock = GetBlockOnSameLevel(ladderBlock, cell);

        if (exitTopBlock == null || exitTopBlock.cell.lastBlock != exitTopBlock) {
            exitTopBlock = GetBlockOnSameLevel(ladderBlock, cell, 1);
            exitTopBlock = exitTopBlock != null && exitTopBlock.isLadder && exitTopBlock.direction == ladderBlock.direction ? exitTopBlock : null;
        }
        return new[] {
            exitTopBlock,
            GetBlockOnSameLevel(ladderBlock, GetCell(ladderBlock.cell.x + exitBottom[0], ladderBlock.cell.y + exitBottom[1]), -1)
        };
    }

    public bool IsCellAvailToMove(CellController currentCell, CellController nextCell, bool blowBlock = false) {
        if (nextCell == null) return false;
        if (!blowBlock && obtacles[nextCell]!=null)return false;
        var currentBlock = currentCell.lastBlock;
        var nextBlock = GetBlockOnSameLevel(currentBlock, nextCell);
        var diagonal = currentCell.x != nextCell.x && currentCell.y != nextCell.y;
        if (currentBlock.isLadder) {
            if (diagonal) return false;
            var ladderExits = GetLadderExits(currentBlock);
            if (ladderExits[0] != null && nextCell == ladderExits[0].cell) {
                nextBlock = ladderExits[0];
            } else {
                if (ladderExits[1] != null && nextCell == ladderExits[1].cell) {
                    nextBlock = ladderExits[1];
                } else {
                    if (!blowBlock) {
                        return nextBlock!=null && nextBlock.isLadder && currentBlock.direction == nextBlock.direction;
                    }
                }
            }
        }
        if (nextBlock == null || nextBlock!=nextCell.lastBlock) {
            if (nextBlock != null && nextBlock != nextCell.lastBlock) {
                nextBlock = nextBlock.GetBlockOnTop();
            } else {
                nextBlock = GetBlockOnSameLevel(currentBlock, nextCell, 1);
            }

            if (nextBlock == null || diagonal) return false;
            if (nextBlock.isLadder) {
                return GetLadderExits(nextBlock)[1] == currentBlock;
            }
            if (blowBlock && nextBlock.isBlowable) {
                if (nextBlock.isBlowPass)return true;
                nextBlock.Blow();
            }
            return false;
        }
        if (nextBlock.isLadder) {
            var ladderExits = GetLadderExits(nextBlock);
            return ladderExits[0] == currentBlock || ladderExits[1] == currentBlock;
        }
        if (nextBlock.isFlat) {
            if (diagonal) {
                var hCell = GetBlockOnSameLevel(currentBlock, cells[currentCell.y][nextCell.x]);
                if (hCell == null || hCell != hCell.cell.lastBlock) return false;
                var vCell = GetBlockOnSameLevel(currentBlock, cells[nextCell.y][currentCell.x]);
                if (vCell == null || vCell != vCell.cell.lastBlock || !hCell.isFlat || !vCell.isFlat ||
                    hCell.isLadder || vCell.isLadder) return false;
            }
            return true;
        }
        if (blowBlock && nextBlock.isBlowable) {
            if (nextBlock.isBlowPass)return true;
            nextBlock.Blow();
        }
        return false;
    }

    public bool IsCellAvailToMove(CellController currentCell, int offsetX, int offsetY, bool blowBlock = false) {
        var nextCell = GetCell(currentCell.x + offsetX, currentCell.y + offsetY);
        return nextCell != null && IsCellAvailToMove(currentCell, nextCell, blowBlock);
    }

    private LayerMask mapLayer;
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

    private void PutBlockIntoOpenList(List<PathBlock> open, List<PathBlock> closed, PathBlock currentBlock, CellController finish,
        int offsetX, int offsetY) {
        var nextCell = GetCell(currentBlock.block.cell.x + offsetX, currentBlock.block.cell.y + offsetY);
        if (nextCell == null || !IsCellAvailToMove(currentBlock.block.cell, nextCell)) return;

        var g = currentBlock.g + (currentBlock.block.cell.x != nextCell.x && currentBlock.block.cell.y != nextCell.y ? 141f : 100f);
        var h = GetDistance(nextCell, finish);

        var openBlock = open.Find(pathBlock => pathBlock.block.cell == nextCell);

        if (openBlock == null) {
            if (closed.Find(closedBlock => closedBlock.block.cell == nextCell) != null) return;
            open.Add(new PathBlock(nextCell.lastBlock) {
                g = g,
                h = h,
                parent = currentBlock
            });
        } else {
            if (!(openBlock.f > g + h)) return;
            openBlock.g = g;
            openBlock.h = h;
            openBlock.parent = currentBlock;
        }
    }

    private static int SortPathByF(PathBlock cell1, PathBlock cell2) {
        return cell1.f > cell2.f ? 1 : (cell1.f < cell2.f ? -1 : 0);
    }

    private static int SortPathByH(PathBlock cell1, PathBlock cell2) {
        return cell1.fs > cell2.fs ? 1 : (cell1.fs < cell2.fs ? -1 : 0);
    }

    public List<BlockController> GetPath(PathBlock start, PathBlock finish) {
        var waypoints = new List<BlockController>();
        var currentBlock = finish;
        while (currentBlock != start) {
            waypoints.Insert(0, currentBlock.block);
            currentBlock = currentBlock.parent;
        }
        waypoints.Insert(0, start.block);
        return waypoints;
    }

    private List<BlockController> FindPathThread(CellController start, CellController finish) {
        var open = new List<PathBlock>();
        var closed = new List<PathBlock>();

        var startBlock = new PathBlock(start.lastBlock) {
            h = GetDistance(start, finish)
        };

        var currentBlock = startBlock;
        open.Add(currentBlock);

        while (open.Count > 0) {
            closed.Add(currentBlock = open[0]);
            open.Remove(currentBlock);
            currentBlock.hs = GetDistance(currentBlock.block.cell, start.lastBlock.cell) / 10000F;

            if (currentBlock.block == finish.lastBlock) {
                return GetPath(startBlock, currentBlock);
            }

            PutBlockIntoOpenList(open, closed, currentBlock, finish, -1, 0);
            PutBlockIntoOpenList(open, closed, currentBlock, finish, 1, 0);
            PutBlockIntoOpenList(open, closed, currentBlock, finish, 0, -1);
            PutBlockIntoOpenList(open, closed, currentBlock, finish, 0, 1);

            PutBlockIntoOpenList(open, closed, currentBlock, finish, -1, -1);
            PutBlockIntoOpenList(open, closed, currentBlock, finish, 1, -1);
            PutBlockIntoOpenList(open, closed, currentBlock, finish, -1, 1);
            PutBlockIntoOpenList(open, closed, currentBlock, finish, 1, 1);

            open.Sort(SortPathByF);
        }
        closed.Sort(SortPathByH);
        return GetPath(startBlock, closed[0]);
    }

    private static float GetDistance(CellController a, CellController b) {
        return Vector3.SqrMagnitude(a.top - b.top);
    }

    public void FindPath(CellController start, CellController finish, Action<List<BlockController>> callback) {
        Observable.Start(() => FindPathThread(start, finish))
            //.TakeUntilDestroy(g.c)
            .ObserveOnMainThread()
            .Subscribe(callback);
    }

    public void CreateActor(CellController cell, string prefabName, string controllerName) {
        var actor = Instantiate(actorPrefabsDictionary[prefabName], cell.top+Vector3.up*10f, Quaternion.identity);
        //actor.transform.parent = transform;

        actor.AddComponent(Type.GetType(controllerName));
    }

    public void Blow(CellController cell, int power) {
        Trigger(Channel.Map.BlowCell, cell, power);
    }

    void CheckAndBlowNextCell(CellController currentCell, int power, int directionX, int directionY) {
        var nextCell = GetCell(currentCell.x + directionX, currentCell.y + directionY);
        if (nextCell != null && IsCellAvailToMove(currentCell, nextCell, true)) {
            OnBlowCell(nextCell, power, directionX, directionY);
        }
    }

    void OnBlowCell(params object[] args) {
        // [0] cell
        // [1] power
        // [2] int directionX
        // [3] int directionY
        var cell = (CellController) args[0];
        var power = (int) args[1] - 1;

        var length = args.Length;

        var directionX = length > 2 ? (int) args[2] : 0;
        var directionY = length > 3 ? (int) args[3] : 0;
        var blowInitializer = directionX + directionY == 0;

        Observable.Timer(TimeSpan.FromMilliseconds(blowInitializer ? 0 : g.NextBlowDelay))
            .Subscribe(_ => {
                if (power >= 0) {
                    if (blowInitializer) {
                        CheckAndBlowNextCell(cell, power, 0, -1);
                        CheckAndBlowNextCell(cell, power, 1, 0);
                        CheckAndBlowNextCell(cell, power, 0, 1);
                        CheckAndBlowNextCell(cell, power, -1, 0);
                    } else {
                        CheckAndBlowNextCell(cell, power, directionX, directionY);
                    }
                }
                cell.Blow(blowInitializer);
            });
    }
}


public class PathBlock {
    public BlockController block;

    public float g; // Traveled path length
    public float h; // Distance to finish
    public float hs; // Distance to start

    public float f {
        get { return g + h; }
    } // Open list sorter value

    public float fs {
        get { return hs + h; }
    } // Closed list sorter value

    public PathBlock parent;

    public PathBlock(BlockController _block) {
        block = _block;
    }
    public override string ToString() {
        //return string.Format ("x: {0}, y: {1}, g: {0}, h: {1}, f: {0}", x, y, g, h, f);
        return string.Format("[{0},{1}] f:{2}, h:{3}", block.cell.x, block.cell.y, f, h);
    }
}
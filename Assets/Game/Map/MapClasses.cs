//using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public class Cell {
    public ushort x;
    public ushort y;

    public byte z;
    public List<BlockController> blocks;

    public Radio radio;

    public void AddBlock(BlockController block) {
        blocks.Add(block);
    }

    public void RemoveBlock(byte index) {
        blocks.RemoveAt(index);
    }

    public void RemoveBlock(BlockController block) {
        blocks.Remove(block);
    }

    /*
    public void setPositions() {
        realLevel = level + (movable < 2 ? 0 : movable - 1);
        realPosition = Map.GetRealPositionFromTable(x, y, realLevel);
        tablePosition = new Vector3(x, y, realLevel);
    }
    */
    /*
    void FinalizeInitialization() {
        setPositions();
        realDirection = Map.GetRotationFromDirection(direction);

        radio = new Radio();
        radio.On(Channel.Map.BlowCell, OnBlowCell);
    }
    */
    /*
    public void Blow(int lifeTime = 0) {
        radio.Trigger(Channel.Map.BlowCell, lifeTime);
    }

    public void BreakWall() {
        if (movable > 1) {
            movable--;
            setPositions();
        }
    }

    void BlowNextCell(int lifeTime, int delay, int directionX, int directionY) {
        if (!g.map.isCellOffsetAvailToMove(this, directionX, directionY, true)) return;
        var nextCell = g.map.cell[x + directionX, y + directionY];
        g.map.cell[x + directionX, y + directionY]
            .radio.Trigger(Channel.Map.BlowCell,
                lifeTime, delay + g.NextBlowDelay, directionX, directionY,
                nextCell.movable > 1 && realLevel >= nextCell.level && realLevel <= nextCell.realLevel ?
                    realLevel : nextCell.realLevel);
    }
    */
    /*
    void OnBlowCell(params object[] args) {
        // [0] lifeTime
        // [1] delay
        // [2] int directionX
        // [3] int directionY
        // [4] int blow level
        var lifeTime = (int) args[0];

        var length = args.Length;

        var directionX = length > 2 ? (int) args[2] : 0;
        var directionY = length > 3 ? (int) args[3] : 0;
        var blowInitializer = directionX + directionY == 0;

        if (blowable <= 0 || (blowInitializer && movable <= 0)) {
            return;
        }

        var delay = length > 1 ? (int) args[1] : 0;
        Observable.Timer(TimeSpan.FromMilliseconds(delay))
            .Subscribe(_ => {
                var savedBlowable = blowable;
                radio.Trigger(Channel.Map.MakeBlowCell, blowInitializer, directionX, directionY,
                    length > 4 ? (int) args[4] : realLevel);

                if (blowable != savedBlowable) {
                    setPositions();
                }

                if (savedBlowable <= 1 || --lifeTime < 0) return;
                if (blowInitializer) {
                    BlowNextCell(lifeTime, delay, 0, -1);
                    BlowNextCell(lifeTime, delay, 1, 0);
                    BlowNextCell(lifeTime, delay, 0, 1);
                    BlowNextCell(lifeTime, delay, -1, 0);
                } else {
                    BlowNextCell(lifeTime, delay, directionX, directionY);
                }
            });
    }
    */
    public Cell(ushort _x, ushort _y, byte _z = 0) {
        x = _x;
        y = _y;
        z = _z;


        //FinalizeInitialization();
    }

    /*
    public Cell(string data) {
        string[] dataItem = data.Split(',');
        x = int.Parse(dataItem[0]);
        y = int.Parse(dataItem[1]);
        level = int.Parse(dataItem[2]);
        direction = int.Parse(dataItem[3]);
        prefab = int.Parse(dataItem[4]);
        param = int.Parse(dataItem[5]);
        FinalizeInitialization();
    }
    */
    /*
    public string export() {
        return string.Join(",", new string[] {
            x.ToString(),
            y.ToString(),
            level.ToString(),
            direction.ToString(),
            prefab.ToString(),
            param.ToString()
        });
    }

    public override string ToString() {
        return string.Format("[Cell: x={0}, y={1}, realLevel={2}, prefab={3}]", x, y, realLevel, prefab);
    }
    */
}

public class PathCell {
    public Cell cell;

    public float g; // Traveled path length
    public float h; // Distance to finish
    public float hs; // Distance to start

    public float f {
        get { return g + h; }
    } // Open list sorter value

    public float fs {
        get { return hs + h; }
    } // Closed list sorter value

    public PathCell parent;

    public PathCell(Cell _cell) {
        cell = _cell;
    }

    /*
    public PathCell(Cell _cell, float _g, float _h, PathCell _parent) {
        cell = _cell;
        g = _g;
        h = _h;
        parent = _parent;
    }
    */
    public override string ToString() {
        //return string.Format ("x: {0}, y: {1}, g: {0}, h: {1}, f: {0}", x, y, g, h, f);
        return string.Format("[{0},{1}] f:{2}, h:{3}", cell.x, cell.y, f, h);
    }
}


public class Map {
    public Cell[,] cell { get; private set; }
    public ushort countX { get; private set;}
    public ushort countY { get; private set;}

    public Radio radio;

    public void loadFromFile(string filename) {
        /*
        var ini = new INIParser();
        ini.Open(g.MapPath + filename + ".ini");
        _cell = new Cell[(_countX = ini.ReadValue("Map", "countX", 0)), (_countY = ini.ReadValue("Map", "countY", 0))];
        foreach (var item in ini.ReadValue("Map", "cells", "").Split(' ')) {
            var mapCell = new Cell(item);
            _cell[mapCell.x, mapCell.y] = mapCell;
            radio.Trigger(Channel.Map.NewCell, mapCell);
        }
        //radio.Trigger(Channel.Map.Loaded);
        g.c.Trigger(Channel.Map.Loaded);
        */
    }

    public void saveToFile(string filename) {
        var ini = new INIParser();
        ini.Open(g.MapPath + filename + ".ini");
        ini.WriteValue("Map", "countX", countX);
        ini.WriteValue("Map", "countY", countY);
        exportCells((cells) => {
            ini.WriteValue("Map", "cells", cells);
            ini.Close();
        });
    }

    public void exportCells(Action<string> callback) {
        /*
        Observable.Start(() => {
                string cells = "";
                foreach (Cell item in cell) {
                    cells += item.export() + " ";
                }
                return cells;
            })
            .ObserveOnMainThread()
            .Subscribe(callback);
        */
    }

    void init() {
        radio = new Radio();
        //g.map = this;
    }

    public Map() {
        init();
    }

    public Map(string filename) {
        init();
        loadFromFile(filename);
    }

    public Map(ushort sizeX, ushort sizeY) {
        init();
        cell = new Cell[countX = sizeX, countY = sizeY];
    }

    public void GenerateMap() {
        for (ushort x = 0; x < countX; x++) {
            for (ushort y = 0; y < countY; y++) {
                var newCell = new Cell(x, y, (byte)(y < 6 || x == 4 || x == 6 ? 1 : 0));

                cell[x, y] = newCell;


                radio.Trigger(Channel.Map.NewCell, newCell);
            }
        }

        /*
        for (int x = 0; x < countX; x++) {
            for (int y = 0; y < countY; y++) {
                _cell[x, y] = new Cell(x, y, (y < 6 || x == 4 || x == 6) ? 1 : 0) {
                    prefab = 1
                };


                if (UnityEngine.Random.Range(0, 20) == 0) {
                    _cell[x, y].prefab = 0;
                }

                if (UnityEngine.Random.Range(0, 7) == 0) {
                    _cell[x, y].prefab = 2;
                }

                if (UnityEngine.Random.Range(0, 4) == 0) {
                    _cell[x, y].prefab = 3;
                    _cell[x, y].param = UnityEngine.Random.Range(0, 3);
                }

                if (y == 6 && UnityEngine.Random.Range(0, 5) == 0) {
                    _cell[x, y].prefab = 4;
                    _cell[x, y].direction = 0;
                }


                if (x < 6) {
                    _cell[x, y].prefab = 3;
                    _cell[x, y].param = UnityEngine.Random.Range(0, 3);
                }

                if ((x == 4 || x == 6) && y != 8) {
                    _cell[x, y].prefab = 1;
                }

                radio.Trigger(Channel.Map.NewCell, _cell[x, y]);
            }
        }
        */
    }

    public void BlowCell(Cell cellToBlow, int strength) {
        cellToBlow.radio.Trigger(Channel.Map.BlowCell, strength);
    }

    public Cell GetCellFromReal(Vector3 position) {
        var cellXY = GetTablePositionFromReal(position);
        return cell[(int) cellXY.x, (int) cellXY.y];
    }

    public static Vector3 GetTablePositionFromReal(Vector3 position) {
        return new Vector3(Mathf.Round(position.x), -Mathf.Round(position.z), 0);
    }

    /*
    public static Vector3 GetRealPositionFromTable(Vector3 position) {
        return new Vector3(position.x, position.z, -position.y);
    }
    */

    public static Vector3 GetRealPositionFromTable(int x, int y, int level = 0) {
        return new Vector3(x, level, -y);
    }


    /*
    public Vector3 GetRealPositionFromCellPosition(Vector3 position) {
        return cell[(int) position.x, (int) position.y].realPosition;
    }



    public Vector3 GetRealPositionFromTable(Vector3 position) {
        return GetCellFromReal(position).realPosition;
    }

    public static Quaternion GetRotationFromDirection(int direction) {
        return Quaternion.AngleAxis((direction - 1) * 90, Vector3.up);
    }
    */

    /*
    private static float GetDistance(Cell a, Cell b) {
        return Vector3.SqrMagnitude(a.tablePosition - b.tablePosition);
    }
    */
    public Cell GetCell(int x, int y) {
        if (x >= 0 && x < countX && y >= 0 && y < countY) {
            return cell[x, y];
        }
        return null;
    }

    public static readonly int[][] LadderExits = {
        new[] {0, -1},
        new[] {1, 0},
        new[] {0, 1},
        new[] {-1, 0}
    };
    /*
    public Cell[] GetLadderExits(Cell ladderCell) {
        var exitTop = LadderExits[ladderCell.direction];
        var exitBottom = LadderExits[Mathf.Clamp(ladderCell.direction + 2, 0, 3)];
        return new[] {
            GetCell(ladderCell.x + exitTop[0], ladderCell.y + exitTop[1]),
            GetCell(ladderCell.x + exitBottom[0], ladderCell.y + exitBottom[1])
        };
    }

    public bool isCellAvailToMove(Cell currentCell, Cell nextCell, bool passWalls = false) {
        if (nextCell.movable < 1 && !passWalls) return false;

        var diagonal = currentCell.x != nextCell.x && currentCell.y != nextCell.y;

        var nextCellRealLevel = nextCell.realLevel;
        if (passWalls && nextCell.movable > 1 &&
            currentCell.realLevel >= nextCell.level && currentCell.realLevel <= nextCell.realLevel) {
            nextCellRealLevel = currentCell.realLevel;
        }

        if (diagonal && (currentCell.isLadder || nextCell.isLadder)) return false;
        if (nextCellRealLevel != currentCell.realLevel && !currentCell.isLadder && !nextCell.isLadder) return false;

        if (diagonal) {
            var hCell = cell[nextCell.x, currentCell.y];
            var vCell = cell[currentCell.x, nextCell.y];
            if (hCell.movable < 1 || vCell.movable < 1 || hCell.isLadder || vCell.isLadder ||
                currentCell.realLevel != hCell.realLevel ||
                currentCell.realLevel != vCell.realLevel
            ) return false;
        } else {
            if (currentCell.isLadder && nextCell.isLadder && currentCell.direction == nextCell.direction) return true;
            if (currentCell.isLadder) {
                var exit = GetLadderExits(currentCell);
                if ((nextCell != exit[0] || currentCell.realLevel + 1 != nextCellRealLevel) &&
                    (nextCell != exit[1] || (currentCell.realLevel != nextCellRealLevel &&
                                             (!nextCell.isLadder || currentCell.realLevel != nextCellRealLevel + 1))))
                    return false;
            }

            if (nextCell.isLadder) {
                var exit = GetLadderExits(nextCell);
                if ((currentCell != exit[0] || currentCell.realLevel - 1 != nextCellRealLevel) &&
                    (currentCell != exit[1] || (currentCell.realLevel != nextCellRealLevel &&
                                                (!currentCell.isLadder ||
                                                 currentCell.realLevel != nextCellRealLevel - 1))))
                    return false;
            }
        }
        return true;
    }

    public bool isCellAvailToMove(int currentX, int currentY, int nextX, int nextY, bool passWalls = false) {
        var currentCell = GetCell(currentX, currentY);
        if (currentCell == null) return false;

        var nextCell = GetCell(nextX, nextY);
        return nextCell != null && isCellAvailToMove(currentCell, nextCell, passWalls);
    }

    public bool isCellOffsetAvailToMove(int currentX, int currentY, int offsetX, int offsetY, bool passWalls = false) {
        var currentCell = GetCell(currentX, currentY);
        if (currentCell == null) return false;

        var nextCell = GetCell(currentCell.x + offsetX, currentCell.y + offsetY);
        return nextCell != null && isCellAvailToMove(currentCell, nextCell, passWalls);
    }

    public bool isCellOffsetAvailToMove(Cell currentCell, int offsetX, int offsetY, bool passWalls = false) {
        var nextCell = GetCell(currentCell.x + offsetX, currentCell.y + offsetY);
        return nextCell != null && isCellAvailToMove(currentCell, nextCell, passWalls);
    }

    public bool isCellAvailToMove(int currentX, int currentY, Cell nextCell, bool passWalls = false) {
        var currentCell = GetCell(currentX, currentY);
        return currentCell != null && isCellAvailToMove(currentCell, nextCell, passWalls);
    }

    public bool isCellAvailToMove(Cell currentCell, int nextX, int nextY, bool passWalls = false) {
        var nextCell = GetCell(nextX, nextY);
        return nextCell != null && isCellAvailToMove(currentCell, nextCell, passWalls);
    }


    private void PutCellIntoOpenList(List<PathCell> open, List<PathCell> closed, PathCell currentCell, Cell finish,
        int offsetX, int offsetY) {
        var nextCell = GetCell(currentCell.cell.x + offsetX, currentCell.cell.y + offsetY);
        if (nextCell == null || !isCellAvailToMove(currentCell.cell, nextCell)) return;

        var g = currentCell.g + (currentCell.cell.x != nextCell.x && currentCell.cell.y != nextCell.y ? 141f : 100f);
        var h = GetDistance(nextCell, finish);

        var openCell = open.Find(pathCell => pathCell.cell == nextCell);

        if (openCell == null) {
            if (closed.Find(closedCell => closedCell.cell == nextCell) != null) return;
            open.Add(new PathCell(nextCell) {
                g = g,
                h = h,
                parent = currentCell
            });
        } else {
            if (!(openCell.f > g + h)) return;
            openCell.g = g;
            openCell.h = h;
            openCell.parent = currentCell;
        }
    }

    private static int SortPathByF(PathCell cell1, PathCell cell2) {
        return cell1.f > cell2.f ? 1 : (cell1.f < cell2.f ? -1 : 0);
    }

    private static int SortPathByH(PathCell cell1, PathCell cell2) {
        return cell1.fs > cell2.fs ? 1 : (cell1.fs < cell2.fs ? -1 : 0);
    }

    public List<Cell> GetPath(PathCell start, PathCell finish) {
        var waypoints = new List<Cell>();
        var currentCell = finish;

        while (currentCell != start) {
            waypoints.Insert(0, currentCell.cell);
            //Debug.Log ("Current cell: " + currentCell + " | Parent: " + currentCell.parent);
            currentCell = currentCell.parent;
        }
        waypoints.Insert(0, start.cell);
        return waypoints;
    }

    private List<Cell> FindPathThread(Cell start, Cell finish) {
        var open = new List<PathCell>();
        var closed = new List<PathCell>();

        var startCell = new PathCell(start) {
            h = GetDistance(start, finish)
        };
        var currentCell = startCell;
        open.Add(currentCell);

        while (open.Count > 0) {
            closed.Add(currentCell = open[0]);
            open.Remove(currentCell);
            currentCell.hs = GetDistance(currentCell.cell, start) / 10000F;

            if (currentCell.cell == finish) {
                return GetPath(startCell, currentCell);
            }

            PutCellIntoOpenList(open, closed, currentCell, finish, -1, 0);
            PutCellIntoOpenList(open, closed, currentCell, finish, 1, 0);
            PutCellIntoOpenList(open, closed, currentCell, finish, 0, -1);
            PutCellIntoOpenList(open, closed, currentCell, finish, 0, 1);

            PutCellIntoOpenList(open, closed, currentCell, finish, -1, -1);
            PutCellIntoOpenList(open, closed, currentCell, finish, 1, -1);
            PutCellIntoOpenList(open, closed, currentCell, finish, -1, 1);
            PutCellIntoOpenList(open, closed, currentCell, finish, 1, 1);

            open.Sort(SortPathByF);
        }
        closed.Sort(SortPathByH);
        return GetPath(startCell, closed[0]);
    }

    public void FindPath(Cell start, Cell finish, Action<List<Cell>> callback) {
        Observable.Start(() => FindPathThread(start, finish))
            //.TakeUntilDestroy(g.c)
            .ObserveOnMainThread()
            .Subscribe(callback);
    }
    */
}
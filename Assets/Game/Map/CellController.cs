using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellController : ControllerBehaviour {
    public int x;
    public int y;

    public byte z;
    public List<BlockController> blocks;

    public CellItem item = CellItem.Null;

    public Vector3 top;
    public BlockController lastBlock;

    void CalculateTop() {
        top = transform.position + Vector3.up * blocks.Count;
    }

    public void SetPosition(int _x, int _y, byte _z = 0) {
        x = _x;
        y = _y;
        z = _z;
        transform.position = MapController.GetRealPositionFromTable(x, y, z);
        CalculateTop();
    }

    public void SetItem(CellItem _item) {
        item = _item;
        g.map.Trigger(Channel.Map.SetCellItem, this, item);
    }

    public void ChangeZ(int offset) {
        if (z + offset < 0) return;
        SetPosition(x,y,(byte)(z+offset));
    }

    public BlockController CreateBlock(byte index) {
        var block = Instantiate(g.map.blockPrefabs[index], MapController.GetRealPositionFromTable(x, y, z+blocks.Count), Quaternion.identity);
        block.transform.parent = transform;
        var blockController = block.GetComponent<BlockController>();
        AddBlock(blockController);
        return blockController;
    }

    public void AddBlock(BlockController block) {
        blocks.Add(block);
        lastBlock = block;
        CalculateTop();
    }

    public void RemoveBlock(byte index) {
        var block = blocks[index];
        if (block != null) {
            Destroy(block.gameObject);
            blocks.RemoveAt(index);
            for (var i = index; i < blocks.Count; i++) {
                blocks[i].Trigger((i-index)*50, "MoveDown", (float)i);
            }
            CalculateTop();
        }
    }

    public void RemoveBlock(BlockController block) {
        RemoveBlock((byte)blocks.IndexOf(block));
        lastBlock = blocks.Count > 0 ? blocks.Last() : null;
    }

    public void import(string data) {
        var dataItem = data.Split(',');
        SetPosition(x,y, byte.Parse(dataItem[0]));
        SetItem(dataItem[1] == "" ? CellItem.Null : (CellItem) Enum.Parse(typeof(CellItem), dataItem[1]));
        for (var i = 2; i < dataItem.Length; i++) {
            var blockDataItem = dataItem[i].Split('.');
            var block = CreateBlock(byte.Parse(blockDataItem[0]));
            block.import(dataItem[i]);
        }
    }

    public string export() {
        var o = new string[blocks.Count+2];
        o[0] = z.ToString();
        o[1] = item == CellItem.Null ? "" : item.ToString();
        for (var i = 2; i < blocks.Count+2; i++) {
            o[i] = blocks[i-2].export();
        }
        return string.Join(",", o);
    }

    public override string ToString() {
        return String.Format("cell[{0}, {1}, {2}]", x, y, z);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellController : ControllerBehaviour {
    public ushort x;
    public ushort y;

    public byte z;
    public List<BlockController> blocks;

    public void SetPosition(ushort _x, ushort _y, byte _z = 0) {
        x = _x;
        y = _y;
        z = _z;
        transform.position = MapController.GetRealPositionFromTable(x, y, z);
    }

    public void CreateBlock(byte index) {
        var block = Instantiate(g.map.blockPrefabs[index], MapController.GetRealPositionFromTable(x, y, z+blocks.Count), Quaternion.identity);
        block.transform.parent = transform;
        AddBlock(block.GetComponent<BlockController>());
    }

    public void AddBlock(BlockController block) {
        blocks.Add(block);
    }

    public void RemoveBlock(byte index) {
        var block = blocks[index];
        if (block != null) {
            Destroy(block);
            blocks.RemoveAt(index);
        }
    }

    public void RemoveBlock(BlockController block) {
        blocks.Remove(block);
        Destroy(block.gameObject);
    }
}

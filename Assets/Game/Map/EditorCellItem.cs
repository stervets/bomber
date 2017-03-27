using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorCellItem : MonoBehaviour {
    public string itemName;
    private int width;

    void Start() {
        width = (itemName.Length * 10) / 2;
    }

    void OnGUI() {
        var pos = g.camera.WorldToScreenPoint(transform.position);
        GUI.Box(new Rect(pos.x-width, Screen.height-pos.y, width * 2, 22), itemName);
    }

}

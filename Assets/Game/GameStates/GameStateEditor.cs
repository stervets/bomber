using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine.UI;

public class GameStateEditor : StateBehaviour {
    private GameObject map;
    private GameObject gizmosCamera;
    private GameObject cursor;
    private GameObject blockPreviewContainer;
    private GameObject blockPreview;
    private int blockIndex;

    private Dictionary<KeyCode, Action> keyMap;

    private Vector3 cursorTablePosition = Vector3.zero;

    readonly Rect guiRect = new Rect(0, 0, 170, 30);

    protected override void OnAwake(params object[] args) {
        SetDefaultState(State.Editor);

        keyMap = new Dictionary<KeyCode, Action> {
            {KeyCode.LeftArrow, () => { MoveCursor(Vector3.left); }},
            {KeyCode.RightArrow, () => { MoveCursor(Vector3.right); }},
            {KeyCode.UpArrow, () => { MoveCursor(Vector3.down); }},
            {KeyCode.DownArrow, () => { MoveCursor(Vector3.up); }},
            {KeyCode.W, () => { MoveCursor(Vector3.forward); }},
            {KeyCode.S, () => { MoveCursor(Vector3.back); }},
            {KeyCode.Q, () => { NextBlockPreview(-1); }},
            {KeyCode.E, () => { NextBlockPreview(1); }},
            {KeyCode.LeftBracket, () => {
                var cell = g.map.GetCell(cursorTablePosition);
                if (cell!=null) {
                    cell.ChangeZ(-1);
                }
            }},
            {KeyCode.RightBracket, () => {
                var cell = g.map.GetCell(cursorTablePosition);
                if (cell!=null) {
                    cell.ChangeZ(1);
                }
            }},
            {KeyCode.Comma, () => {
                var cell = g.map.GetCell(cursorTablePosition);
                if (cell!=null) {
                    cell.SetItem((CellItem)((int) cell.item-1));
                }
            }},
            {KeyCode.Period, () => {
                var cell = g.map.GetCell(cursorTablePosition);
                if (cell!=null) {
                    cell.SetItem((CellItem)((int) cell.item+1));
                }
            }},
            {KeyCode.A, () => {
                var block = g.map.GetBlock(cursorTablePosition);
                if (block!=null) {
                    block.SetDirection(block.direction-1);
                }
            }},
            {KeyCode.D, () => {
                var block = g.map.GetBlock(cursorTablePosition);
                if (block!=null) {
                    block.SetDirection(block.direction+1);
                }
            }},
            {
                KeyCode.Backspace, () => {
                    var block = g.map.GetBlock(cursorTablePosition);
                    if (block!=null) {
                        block.Remove();
                    }
                }
            },
            {KeyCode.Return, () => {
                var cell = g.map.GetCell(cursorTablePosition);
                if (cell!=null) {
                    cell.CreateBlock((byte)blockIndex);
                }
            }},
            {KeyCode.P, () => { g.map.saveToFile(); }}
        };
    }

    void NextBlockPreview(int offset) {
        blockIndex += offset;
        blockIndex = blockIndex < 0 ? g.map.blockPrefabs.Length - 1 :
        (blockIndex >= g.map.blockPrefabs.Length ? 0 : blockIndex);
        if (blockPreview != null) {
            Destroy(blockPreview);
        }
        blockPreview = Instantiate(g.map.blockPrefabs[blockIndex]);
        blockPreview.transform.parent = blockPreviewContainer.transform;
        blockPreview.transform.localPosition = Vector3.zero;
        blockPreview.transform.localRotation = Quaternion.identity;
    }

    protected override void OnStart(params object[] args) {
        gizmosCamera = g.camera.transform.Find("GizmosCamera").gameObject;
        blockPreviewContainer = gizmosCamera.transform.Find("PrefabPreview").gameObject;
    }

    protected void OnSetCellItem(params object[] args) {
        var cell = (CellController) args[0];
        //var cellItem = (CellItem) args[1];
        var gizmo = cell.transform.Find("EditorCellItem");
        if (gizmo != null) {
            Destroy(gizmo.gameObject);
        }
        if (cell.item != CellItem.Null) {
            var editorCellItem = Instantiate(g.map.editorCellItemPrefab);
            console.log("instantiate");
            editorCellItem.transform.parent = cell.transform;
            editorCellItem.transform.localPosition = Vector3.up;
            editorCellItem.name = "EditorCellItem";
            editorCellItem.GetComponent<EditorCellItem>().itemName = cell.item.ToString();
        }
    }

    CompositeDisposable disposables;

    protected override void OnEnabled(params object[] args) {
        map = Instantiate(gc.MapPrefab);

        ListenTo(g.map, Channel.GameObject.Start, objects => {
            g.map.loadFromFile();
        });

        ListenTo(g.map, Channel.Map.SetCellItem, OnSetCellItem);

        cursor = Instantiate(g.map.editorCursorPrefab);
        gizmosCamera.SetActive(true);
        g.cameraController.SetState(State.Editor);

        disposables = new CompositeDisposable();
        foreach (var action in keyMap) {
            Observable
                .EveryUpdate()
                .Where(_ => Input.GetKeyDown(action.Key))
                .Subscribe(_ => { action.Value(); })
                .AddTo(disposables);
        }

        NextBlockPreview(0);
    }

    protected override void OnDisabled(params object[] args) {
        Destroy(cursor);
        if (gizmosCamera != null) {
            gizmosCamera.SetActive(false);
        }
        disposables.Clear();
        Destroy(map);
    }


    void MoveCursor(Vector3 offset) {
        cursorTablePosition = MapController.GetTablePositionFromReal(cursor.transform.position) + offset;
        cursor.transform.position = MapController.GetRealPositionFromTable(cursorTablePosition);
    }

    private void OnGUI() {
        //GUI.Box();
        GUI.Box(guiRect, cursorTablePosition.ToString());
    }

    private void FixedUpdate() {
        blockPreviewContainer.transform.LookAt(blockPreviewContainer.transform.position + (blockPreviewContainer.transform.forward - blockPreviewContainer.transform.right*0.05f));
    }
}
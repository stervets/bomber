using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine.UI;

public class GameStateEditor : StateBehaviour {
    private GameObject map;
    private GameObject gizmosCamera;
    private GameObject cursor;

    private Dictionary<KeyCode, Action> keyMap;

    private Vector3 cursorTablePosition = Vector3.zero;

    Rect guiRect = new Rect(0, 0, Screen.width, Screen.height);

    protected override void OnAwake(params object[] args) {
        SetDefaultState(State.Editor);

        keyMap = new Dictionary<KeyCode, Action> {
            {KeyCode.LeftArrow, () => { MoveCursor(Vector3.left); }},
            {KeyCode.RightArrow, () => { MoveCursor(Vector3.right); }},
            {KeyCode.UpArrow, () => { MoveCursor(Vector3.down); }},
            {KeyCode.DownArrow, () => { MoveCursor(Vector3.up); }},
            {KeyCode.W, () => { MoveCursor(Vector3.forward); }},
            {KeyCode.S, () => { MoveCursor(Vector3.back); }},
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
                    cell.CreateBlock(0);
                }
            }}
        };
    }

    protected override void OnStart(params object[] args) {
        gizmosCamera = g.camera.transform.FindChild("GizmosCamera").gameObject;
    }

    CompositeDisposable disposables;

    protected override void OnEnabled(params object[] args) {
        map = Instantiate(gc.MapPrefab);
        map.GetComponent<MapController>().MakeField(10, 5);

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
    }

    protected override void OnDisabled(params object[] args) {
        Destroy(cursor);
        gizmosCamera.SetActive(false);
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
}
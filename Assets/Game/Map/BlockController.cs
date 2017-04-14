//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
using System;
using System.ComponentModel;
using UnityEngine;

public class BlockController : ControllerBehaviour {
    public bool isFlat;
    public bool isLadder;
    public bool isBlowable;

    public int index {
        get { return cell.blocks.IndexOf(this); }
    }

    [HideInInspector] public CellController cell;
    [HideInInspector] public byte prefabIndex;
    [HideInInspector] public byte direction;

    private Collider _collider;
    private Renderer renderer;

    public string export() {
        return string.Join(".", new string[]{
            prefabIndex.ToString(),
            direction.ToString()
        });
    }

    private readonly float blowTime = 0.3f;

    protected bool imported;

    public void import(string data) {
        var dataItem = data.Split('.');
        prefabIndex = byte.Parse(dataItem[0]);
        SetDirection(byte.Parse(dataItem[1]));
        imported = true;
    }

    public void SetDirection(int _direction) {
        direction = (byte)(_direction < 0 ? 3 : (_direction > 3 ? 0 : _direction));
        transform.rotation = MapController.GetRotationFromDirection(direction);
    }

    public void Remove(float removeTime = 0) {
        //Debug.Log(cell);
        cell.RemoveBlock(this, removeTime);
    }

    private float targetY;
    private float speed;
    private void FixedUpdate() {
        if (speed>0) {
            transform.Translate(0, -speed * Time.deltaTime, 0);
            if (transform.position.y <= targetY) {
                transform.position = new Vector3(transform.position.x, targetY, transform.position.z);
                speed = 0;
            } else {
                speed += 0.5f;
            }
        }
    }

    public BlockController GetBlockOnTop() {
        return cell.GetBlockByIndex(index + 1);
    }

    void OnMoveDown(params object[] args) {
        targetY = (float) args[0];
        speed = 0.1f;
    }

    protected override void OnStart(params object[] args) {
        if ((cell = transform.parent.GetComponent<CellController>()) != null) {
            On("MoveDown", OnMoveDown);
        }

        _collider = GetComponent<Collider>();
        renderer = GetComponent<Renderer>();

        //renderer.material = g.c.dissolveMaterial;
        //LeanTween.value(LeanTween.tweenEmpty, SetDissolve, 0.25f, 0.75f, blowTime).setEase(LeanTweenType.easeInQuad).setLoopCount(1000);


        if (imported)return;
        name = name.Substring(0, name.IndexOf('('));
        for (byte i = 0; i < g.map.blockPrefabs.Length; i++) {
            if (g.map.blockPrefabs[i].name != name) continue;
            prefabIndex = i;
            break;
        }
    }

    void SetDissolve(float val) {
        if (renderer == null) return;
        renderer.material.SetFloat(BeautifulDissolves.DissolveHelper.dissolveAmountID, val);
    }

    public void Blow() {
        cell.Blow(true, this);
        _collider.enabled = false;
        renderer.material = g.c.dissolveMaterial;
        LeanTween.value(LeanTween.tweenEmpty, SetDissolve, 0.25f, 0.75f, blowTime).setEase(LeanTweenType.easeInQuad);
        LeanTween.moveY(gameObject, transform.position.y+0.5f, blowTime).setEase(LeanTweenType.easeInQuad);
        Remove(blowTime);

    }

    public override String ToString() {
        return String.Format("Block[{0},{1},{2}] flat:{3}, ladder:{4}, blowable:{5}", cell.x, cell.y, (int)transform.position.y, isFlat, isLadder, isBlowable);
    }
}
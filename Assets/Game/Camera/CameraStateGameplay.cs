using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateGameplay : StateBehaviour {
    private int tweenId;
    private CameraStateController controller;

    protected override void OnAwake(params object[] args) {
        SetDefaultState(State.GamePlay);
        controller = _controller as CameraStateController;
    }

    protected override void OnStart(params object[] args) {
    }

    void Shake(float val) {
        var shake = new Vector3(UnityEngine.Random.Range(-val, val), UnityEngine.Random.Range(-val, val),
            UnityEngine.Random.Range(-val, val));
        transform.position+= shake;
        controller.shakeFactor = shake;
    }

    protected void OnBlow(params object[] args) {
        //LeanTween.moveLocal(gameObject, transform.position, 0.5f).setEase(LeanTweenType.easeShake);
        LeanTween.cancel(tweenId);
        tweenId = LeanTween.value(LeanTween.tweenEmpty, Shake, 0.1f, 0, 0.3f).setEase(LeanTweenType.easeInQuad).id;

        console.log("blow");
    }

    protected override void OnEnabled(params object[] args) {
        ListenTo(g.map, Channel.Map.BlowCell, OnBlow);
    }
}

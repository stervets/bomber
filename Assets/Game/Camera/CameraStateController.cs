using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraStateController : StateControllerBehaviour {
    protected override void OnAwake(params object[] args) {
        SetDefaultState(State.Default);
    }
    private ControllerBehaviour target;

    private Vector3 targetLookPosition;
    private Vector3 targetPosition;
    public Vector3 shakeFactor = Vector3.zero;

    private const float followSpeed = 5f;

    readonly Vector3 cameraOffset = new Vector3(-0.2f, 7f, -2f);

    protected override void OnStart(params object[] args) {
        ListenTo(g.c, Channel.Camera.SetTarget, SetTarget);
    }

    protected void SetTarget(params object[] args) {
        if (args[0] == null) {
            target = null;
        } else {
            target = (ControllerBehaviour) args[0];
            ListenTo(target, Channel.GameObject.Destroy, _=>target = null);
        }
    }

    void LateUpdate() {
        if (target != null) {
            targetLookPosition = Vector3.Lerp(targetLookPosition, target.transform.position, followSpeed * Time.deltaTime) +
                                 shakeFactor;
            targetPosition = targetLookPosition + cameraOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            transform.LookAt(targetLookPosition);
        }
    }

}

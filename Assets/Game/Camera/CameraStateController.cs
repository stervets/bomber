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
    private const float speed = 4.5f;

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
            targetLookPosition = Vector3.Lerp(targetLookPosition, target.transform.position, speed * Time.deltaTime);
            targetPosition = targetLookPosition + cameraOffset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
            transform.LookAt(targetLookPosition);
        }
    }

}

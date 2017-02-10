using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainCameraController : ControllerBehaviour {
    protected override void OnAwake(params object[] args) {
        SetDefaultState (State.Default);
    }

    Vector3 cameraPosition;
    Vector3 cleanPosition;
    Vector3 targetPosition;
    //Vector3 main

    float distance;

    protected override void OnStart(params object[] args) {
        cleanPosition = cameraPosition = transform.position;
        ListenTo (g.c, Channel.Map.MakeBlowCell, OnMakeBlowCell);
    }

    void OnMakeBlowCell(params object[] args){
        cleanPosition = cameraPosition = transform.position;
        cleanPosition -= (((Vector3)args [0]) - cleanPosition).normalized * 0.25f;
    }

    void FixedUpdate(){
        distance = Vector3.Distance (cameraPosition, cleanPosition);

        if (distance > 0.01f) {
            cleanPosition = Vector3.Lerp (cleanPosition, cameraPosition, 0.1f);
            distance /= 4;
            transform.position = cleanPosition + Vector3.forward * Random.Range (-distance, distance) + Vector3.left * Random.Range (-distance, distance) + Vector3.up * Random.Range (-distance, distance);
            //transform.position = cleanPosition;
        }

        //targetPosition += Vector3.left * 0.01f;
        //cleanPosition += Vector3.left * 0.01f;
    }
}

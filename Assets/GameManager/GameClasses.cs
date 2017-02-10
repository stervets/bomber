using System;
using UnityEngine;
using System.Collections.Generic;
using UniRx;

public delegate void Handler(params object[] arg);


public class Radio {
    private Dictionary<object, Handler> handlers;

    public Radio() {
        handlers = new Dictionary<object, Handler>();
    }

    ~Radio() {
        Trigger(Channel.GameObject.Destroy);
        handlers = null;
    }

    public void On(object EventId, Handler handler) {
        if (handlers.ContainsKey(EventId)) {
            handlers[EventId] += handler;
        } else {
            handlers.Add(EventId, handler);
        }
    }

    public void Off(object eventId, Handler handler) {
        if (handlers.ContainsKey(eventId)) {
            handlers[eventId] -= handler;
        }
    }

    public void Trigger(object eventId, params object[] arr) {
        if (handlers.ContainsKey(eventId)) {
            handlers[eventId](arr);
        }
    }

    public void Trigger(int delay, object eventId, params object[] arr) {
        if (handlers.ContainsKey(eventId)) {
            Observable.Timer(TimeSpan.FromMilliseconds(delay))
                .Subscribe(_ => {
                    handlers[eventId](arr);
                });
        }
    }

    public void ListenTo(Radio target, object eventId, Handler handler) {
        target.On(eventId, handler);
        On(Channel.GameObject.Destroy, _ => { StopListenTo(target, eventId, handler); });
    }

    public void StopListenTo(Radio target, object eventId, Handler handler) {
        target.Off(eventId, handler);
    }

    public void ListenTo(EventBehavior target, object eventId, Handler handler) {
        target.On(eventId, handler);
        On(Channel.GameObject.Destroy, _ => { StopListenTo(target, eventId, handler); });
    }

    public void StopListenTo(EventBehavior target, object eventId, Handler handler) {
        target.Off(eventId, handler);
    }
}

/**
 * Event controller
 */
public abstract class EventBehavior : MonoBehaviour {
    protected Dictionary<object, Handler> handlers;

    protected void CreateHandlersDictionary() {
        handlers = new Dictionary<object, Handler>();
    }

    void OnDestroy() {
        Trigger(Channel.GameObject.Destroy);
    }

    public void On(object EventId, Handler handler) {
        if (handlers.ContainsKey(EventId)) {
            handlers[EventId] += handler;
        } else {
            handlers.Add(EventId, handler);
        }
    }

    public void Off(object eventId, Handler handler) {
        if (handlers.ContainsKey(eventId)) {
            handlers[eventId] -= handler;
        }
    }

    public void Trigger(object eventId, params object[] arr) {
        if (handlers.ContainsKey(eventId)) {
            handlers[eventId](arr);
        }
    }

    public void Trigger(int delay, object eventId, params object[] arr) {
        if (handlers.ContainsKey(eventId)) {
            Observable.Timer(TimeSpan.FromMilliseconds(delay))
                .Subscribe(_ => {
                    handlers[eventId](arr);
                });
        }
    }

    public void ListenTo(EventBehavior target, object eventId, Handler handler) {
        target.On(eventId, handler);
        On(Channel.GameObject.Destroy, _ => { StopListenTo(target, eventId, handler); });
    }

    public void StopListenTo(EventBehavior target, object eventId, Handler handler) {
        target.Off(eventId, handler);
    }

    public void ListenTo(Radio target, object eventId, Handler handler) {
        target.On(eventId, handler);
        On(Channel.GameObject.Destroy, _ => { StopListenTo(target, eventId, handler); });
    }

    public void StopListenTo(Radio target, object eventId, Handler handler) {
        target.Off(eventId, handler);
    }
}

/**
 * State controller
 */
public abstract class ControllerBehaviour : EventBehavior {
    private object _currentState = State.Default;

    public object currentState {
        get { return _currentState; }
    }

    protected GameController gc;

    protected virtual void Awake() {
        CreateHandlersDictionary();
        gc = g.c;
        On(Channel.GameObject.Awake, OnAwake);
        On(Channel.GameObject.Start, OnStart);
        On(Channel.GameObject.Destroy, OnDestroyed);
        On(Channel.Controller.SetState, OnSetState);

        Trigger(Channel.GameObject.Awake);

        foreach(var state in GetComponents<StateBehaviour>())
        {
            state.InitState();
        }

    }

    protected void SetDefaultState(object state) {
        _currentState = state;
    }

    void Start() {
        Trigger(Channel.GameObject.Start);
        SetState(currentState);
    }

    public void EnableState(object state) {
        Trigger(Channel.State.Enable, state);
    }

    private void OnSetState(object[] state) {
        SetState(state[0]);
    }

    public void SetState(object state) {
        Trigger(Channel.State.Set, (_currentState = state));
    }

    public void DisableState(object state) {
        Trigger(Channel.State.Disable, state);
    }

    protected virtual void OnAwake(params object[] args) {
    }

    protected virtual void OnStart(params object[] args) {
    }

    protected virtual void OnDestroyed(params object[] args) {
    }
}


/**
 * State
 */
public abstract class StateBehaviour : EventBehavior {
    public object state = State.Default;

    protected ControllerBehaviour _controller;
    protected GameController gc;

    private bool stateStartNeeded = true;

    protected void SetDefaultState(object defaultState) {
        state = defaultState;
    }

    public void InitState() {
        CreateHandlersDictionary();

        gc = g.c;
        enabled = false;

        On(Channel.GameObject.Awake, OnAwake);
        On(Channel.GameObject.Start, OnStart);
        On(Channel.GameObject.Enable, OnEnabled);
        On(Channel.GameObject.Disable, OnDisabled);
        On(Channel.GameObject.Destroy, OnDestroyed);

        _controller = GetComponent<ControllerBehaviour>();
        ListenTo(_controller, Channel.State.Enable, OnStateEnable);
        ListenTo(_controller, Channel.State.Disable, OnStateDisable);
        ListenTo(_controller, Channel.State.Set, OnStateSet);

        Trigger(Channel.GameObject.Awake);
    }

    void OnStateEnable(params object[] args) {
        if (Equals(state, args[0])) {
            enabled = true;
        }
    }

    void OnStateDisable(params object[] args) {
        if (Equals(state, args[0])) {
            enabled = false;
        }
    }

    void OnStateSet(params object[] args) {
        enabled = Equals(state, args[0]);
    }

    void OnEnable() {
        if (stateStartNeeded) {
            stateStartNeeded = false;
            Trigger(Channel.GameObject.Start);
        }
        Trigger(Channel.GameObject.Enable);
    }

    void OnDisable() {
        if (!stateStartNeeded) {
            Trigger(Channel.GameObject.Disable);
        }
    }

    void OnTriggerEnter(Collider targetCollider) {
        if (enabled) {
            TriggerEnter(targetCollider);
        }
    }

    void OnTriggerExit(Collider targetCollider) {
        if (enabled) {
            TriggerExit(targetCollider);
        }
    }

    void OnTriggerStay(Collider targetCollider) {
        if (enabled) {
            TriggerStay(targetCollider);
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (enabled) {
            CollisionEnter(collision);
        }
    }

    void OnCollisionExit(Collision collision) {
        if (enabled) {
            CollisionExit(collision);
        }
    }

    void OnCollisionStay(Collision collision) {
        if (enabled) {
            CollisionStay(collision);
        }
    }

    protected virtual void TriggerEnter(Collider other) {
    }

    protected virtual void TriggerExit(Collider other) {
    }

    protected virtual void TriggerStay(Collider other) {
    }

    protected virtual void CollisionEnter(Collision collision) {
    }

    protected virtual void CollisionExit(Collision collisionInfo) {
    }

    protected virtual void CollisionStay(Collision collisionInfo) {
    }


    protected virtual void OnAwake(params object[] args) {
    }

    protected virtual void OnStart(params object[] args) {
    }

    protected virtual void OnEnabled(params object[] args) {
    }

    protected virtual void OnDisabled(params object[] args) {
    }

    protected virtual void OnDestroyed(params object[] args) {
    }
}
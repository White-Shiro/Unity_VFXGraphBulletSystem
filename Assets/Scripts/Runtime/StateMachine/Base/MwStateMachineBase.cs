using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//where T : System.Enum
public struct TState<T> {
    public T Type;
    public int TypeValue;
}

public abstract class MwStateMachineBase {

    public virtual MwStateBase currentState { get; private set; } = null;
    public MwStateBase prevState { get; private set; } = null;

    protected List<MwStateBase> statelist = null;

    protected virtual void OnConstruct() { }

    public virtual void SetState(MwStateBase state) { OnSetState(state); }

    protected virtual void OnSetState(MwStateBase state) {

        if (currentState) {
            prevState = currentState;
            prevState.OnExit();
        }

        if(state == null) return;

        currentState = state;
        currentState.OnEnter();
    }

    //experiment

    /*
    public virtual void SetTState<T>(TState<T> state) {
        if (!(state is System.Enum)) return;
        OnSetState(state);
    }
    */

    public void OnUpdate() {
        currentState?.OnUpdate();
    }

    public void OnFixedUpdate() {
        currentState?.OnFixedUpdate();
    }

    public void OnLateUpdate() {
        currentState?.OnLateUpdate();
    }

}

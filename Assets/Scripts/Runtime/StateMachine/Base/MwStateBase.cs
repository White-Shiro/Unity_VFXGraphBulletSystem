using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class MwStateBase : MonoBehaviour {

	protected MwStateMachineBase stateMachine = null;

	public virtual void OnConstructor(MwStateMachineBase stateMachine_) {
		stateMachine = stateMachine_;
	}

	public virtual void OnEnter() { }
	public virtual void OnUpdate() { }
	public virtual void OnFixedUpdate() { }
	public virtual void OnLateUpdate() { }
	public virtual void OnExit() { }
}

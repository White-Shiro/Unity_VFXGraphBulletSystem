using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MwCharMovementStateMachine : MwStateMachineBase {

	public MwCharMovementStateMachine(MwCharacterMovementComponent movementComp_) {
		if (movementComp_) { 
			_charMovementComp = movementComp_;
		}

		OnConstruct();
	}

	MwCharacterMovementComponent _charMovementComp = null;
	public MwCharacterMovementComponent charMovementComp => _charMovementComp;
	public Dictionary<MwCharacterMovementStateType, MwCharacterMovementState> stateTable { get; protected set; }
	public MwCharacterMovementStateType currentMoveState { get; protected set; }

	protected override void OnConstruct() {

		if (!_charMovementComp) return;

		var states = _charMovementComp.GetComponentsInChildren<MwCharacterMovementState>();

		if (states == null && states.Length < 1) {
			WxLogger.Warning("Cannot Find Any MovementState instances");
			return;
		}

		statelist = new List<MwStateBase>();
		stateTable = new Dictionary<MwCharacterMovementStateType, MwCharacterMovementState>();

		//Cache State References into List/Dictionary
		foreach (var state in states) {
			statelist.Add(state);
			state.OnConstructor(this);

			if(!stateTable.ContainsKey(state.stateType))
				stateTable[state.stateType] = state;

			WxLogger.Log($"{state.stateType} added to StateList");
		}

		base.OnConstruct();

	}

	public virtual void SetMovementState(MwCharacterMovementStateType statetype) {

		if(!stateTable.ContainsKey(statetype)) return;

        var nextState = stateTable[statetype];
		if (nextState == null) return;

		base.SetState(nextState);
		currentMoveState = nextState.stateType;
	}
}
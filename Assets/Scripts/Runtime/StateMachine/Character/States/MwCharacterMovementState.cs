using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MwCharacterMovementStateType {
	//Idle,
	Gound,
	Air,
	Dash,
	DashRecoil,
	Landing,
	//DashLanding,
	BoostUp,
}


public class MwCharacterMovementState : MwStateBase {
	public override void OnConstructor(MwStateMachineBase stateMachine_) {
		charStateMachine = stateMachine_ as MwCharMovementStateMachine;

		if (charStateMachine != null) {
			moveComp = charStateMachine.charMovementComp;
		}

		base.OnConstructor(stateMachine_);
	}

	protected MwCharacterMovementComponent moveComp;
	protected MwCharMovementStateMachine   charStateMachine; //Ambiguity with Baseclass's stateMachine var, might delete Later
	protected Vector2	rawMoveInput		=> moveComp.movementInput;
	protected float		inputMagnitude		=> moveComp.useAnalogMovement?  moveComp.smoothedMovementMag : moveComp.inputMagnitude;
	protected float		speed				=> moveComp.speedScaler;
	protected float		gravity				=> moveComp.gravity;
	protected Vector3	gravityVel			=> moveComp.gravity * Vector3.down;
	protected Vector3	playerForward		=> moveComp.playerForward;
	protected Vector3	playerRight			=> moveComp.playerRight;
	protected Vector3	inputDirection		=> moveComp.playerViewInputDir;
	protected Vector3	input2DDirection	=> moveComp.useAnalogMovement ? moveComp.player2DSmoothedInputDir: moveComp.player2DInputDir;

	protected Vector3	velocity = Vector3.zero;

	public virtual MwCharacterMovementStateType stateType { get; }

	protected void Move(Vector3 velocity) {
		moveComp?.Move(velocity);
    }

	protected void SetMovementState(MwCharacterMovementStateType stateType) {
		charStateMachine?.SetMovementState(stateType);
	}
}

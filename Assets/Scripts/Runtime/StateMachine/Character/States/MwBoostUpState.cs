using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MwBoostUpState : MwCharacterMovementState {
	public override MwCharacterMovementStateType stateType => MwCharacterMovementStateType.BoostUp;

	public override void OnEnter() {
		
	}

	public override void OnUpdate() {

		velocity = moveComp.currentVelocity;

		//Apply Gravity

		if (velocity.y < moveComp.terminalVelocity.y) {
			//Reached Max Falling Speed
			//DeepFalling = true;
		} else {

			velocity += gravityVel * Time.deltaTime;

        }

		if (velocity.y > 0) {
			//isClimbingUpward
			//TODO Add UpwardState

		} else {
			//isFalling
			//TODO Add FallingState

		}

		//AirControl
		bool noInput = input2DDirection == Vector3.zero;
		var horizontalVelocity = noInput ? Vector3.zero : input2DDirection;
		velocity += horizontalVelocity * (moveComp.airControlScaler* speed * Time.deltaTime);

		//Apply Final Movement
		Move(velocity);

		//CheckState
		if(moveComp.jumpOffTimeout > 0) moveComp.jumpOffTimeout -= Time.deltaTime;

		if (moveComp.CheckIsGround && moveComp.jumpOffTimeout < 0) {
			//SetMovementState to Landing
			SetMovementState(MwCharacterMovementStateType.Gound);
		}
	}

	public override void OnFixedUpdate() {

	}

	public override void OnLateUpdate() { }

    public override void OnExit() {}




}

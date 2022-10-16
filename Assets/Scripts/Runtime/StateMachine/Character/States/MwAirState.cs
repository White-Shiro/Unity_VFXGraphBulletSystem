using UnityEngine;

public class MwAirState : MwCharacterMovementState {
	public override MwCharacterMovementStateType stateType => MwCharacterMovementStateType.Air;

	public override void OnEnter() { }

	public override void OnUpdate() {

		velocity = moveComp.currentVelocity;

		//Apply Gravity

		if (velocity.y < moveComp.terminalVelocity.y) {
			//Reached Max Falling Speed
			moveComp.isTerminalFalling = true;
		} else {

			velocity += gravityVel * Time.deltaTime;

		}


		moveComp.isUpward  = velocity.y > 0;
		moveComp.isFalling = !moveComp.isUpward;

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
		velocity += horizontalVelocity * (moveComp.airControlScaler* moveComp.walkSpeed * Time.deltaTime);

		//Apply Final Movement
		Move(velocity);

		//CheckState
		if(moveComp.jumpOffTimeout > 0) moveComp.jumpOffTimeout -= Time.deltaTime;

		if (moveComp.CheckIsGround && moveComp.jumpOffTimeout < 0) {
			//SetMovementState to Landing
			SetMovementState(MwCharacterMovementStateType.Landing);
		}
	}

	//public override void OnFixedUpdate() { }

	//public override void OnLateUpdate() { }

	public override void OnExit() {
		moveComp.isUpward			= false;
		moveComp.isFalling			= false;
		moveComp.isTerminalFalling	= false;

	}




}

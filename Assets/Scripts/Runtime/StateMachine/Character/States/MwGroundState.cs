using UnityEngine;

public class MwGroundState : MwCharacterMovementState {
	public override MwCharacterMovementStateType stateType => MwCharacterMovementStateType.Gound;

	public override void OnEnter() {
		moveComp.isJumping = false;
		moveComp?.JumpReset();
		moveComp.isDashing = false;
	}

	public override void OnUpdate() {

		//Vector X == Vector.zero is cheaper than compare vector.mag
		bool noInput = input2DDirection == Vector3.zero;

		velocity = noInput? Vector3.zero : moveComp.walkSpeed * input2DDirection;

		var stepDownDir = Vector3.down * moveComp.stepDownSpeed;

		velocity = velocity + stepDownDir;

		Move(velocity);

		if (!moveComp.CheckIsGround) {
			SetMovementState(MwCharacterMovementStateType.Air);
		}
	}

	//public override void OnFixedUpdate() {}

	//public override void OnLateUpdate() {}

	//public override void OnExit() {}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MwLandingState : MwCharacterMovementState {
	public override MwCharacterMovementStateType stateType => MwCharacterMovementStateType.Landing;

    [SerializeField][Range(0,1)] float _landSpeedScaler = 1f;
	[SerializeField][Range(0, 2)] float _stepDownScaler = 2f;
	[SerializeField][Min(0)] float _landVelocityLerpRate = 20f;
	[SerializeField][Min(0)] float _landDuration = 0.5f;
	float _landSpeed => moveComp.speedScaler * _landSpeedScaler;
	float _landTimer = 0f;

	public override void OnEnter() {
		_landTimer = _landDuration;
	}

	public override void OnUpdate() {

		velocity = moveComp.currentVelocity.x0z();
		velocity = Vector3.MoveTowards(velocity, Vector3.zero, _landVelocityLerpRate * Time.deltaTime);

		//TimerCountdown
		_landTimer -= Time.deltaTime;

		var stepDownDir = Vector3.down * (moveComp.stepDownSpeed * _stepDownScaler);

		velocity = (velocity * _landSpeed) + stepDownDir;

		Move(velocity);

		if (_landTimer > 0) return;

		//CheckSwitchState when _landTimeOut
		var stillInAir = !moveComp.CheckIsGround && _landTimer < 0;
		var states = stillInAir ? MwCharacterMovementStateType.Air : MwCharacterMovementStateType.Gound;
		SetMovementState(states);
	}

	public override void OnFixedUpdate() {
	}

	public override void OnLateUpdate() {
	}

	public override void OnExit() {
		moveComp.isDashing = false;
	}

}

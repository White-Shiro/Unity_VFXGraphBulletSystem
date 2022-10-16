using System;
using System.Collections.Generic;
using UnityEngine;

public class MwDashRecoilState : MwCharacterMovementState{

	public override MwCharacterMovementStateType stateType => MwCharacterMovementStateType.DashRecoil;

	[SerializeField] [Range(0,1)] float _dashRecoilGravityRatio = 0.5f;
	[SerializeField] [Min(0)] float _dashRecoilVelocityLerpRate = 10f;
	[SerializeField] [Min(0)] float _dashRecoilDuration = 0.3f;

	float _recoilTimer = 0f;

	public override void OnEnter() {
		_recoilTimer = _dashRecoilDuration;
	}

	public override void OnUpdate() {
		velocity = moveComp.currentVelocity;

		if (velocity.y < moveComp.terminalVelocity.y) {
			//Reached Max Falling Speed
			//DeepFalling = true;
		} else {
			velocity += gravityVel * (_dashRecoilGravityRatio * Time.deltaTime);
		}

		var targetVel	= new Vector3(0f,velocity.y,0f);
		velocity		= Vector3.MoveTowards(velocity, targetVel, _dashRecoilVelocityLerpRate * Time.deltaTime);

		//TimerCountdown
		_recoilTimer -= Time.deltaTime;

		Move(velocity);

		if(_recoilTimer > 0) return;

		//CheckSwitchState when _recoilTimeOut
		var stillInAir = !moveComp.CheckIsGround && _recoilTimer < 0;
		var states = stillInAir ? MwCharacterMovementStateType.Air : MwCharacterMovementStateType.Landing;
		SetMovementState(states);
	}

	//public override void OnFixedUpdate() {}
	//public override void OnLateUpdate()  {}

	public override void OnExit() {
		moveComp.isDashing = false;
	}
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MwDashState : MwCharacterMovementState {
	public override MwCharacterMovementStateType stateType => MwCharacterMovementStateType.Dash;

	//Dash
	[SerializeField] [Range(0,1f)]	float _dashGravityScaler	= 0.1f;
	[SerializeField] [Min(0)]		float _dashSpeedScaler		= 20f;
	[SerializeField] float _dashGravityLerpSpeed				= 20f;

	bool isDashSwitched = false;

	float _dashGravity	=> _dashGravityScaler * moveComp.gravity;
	float _dashSpeed	=> _dashSpeedScaler * moveComp.speedScaler;

	public override void OnEnter() {
		moveComp.isDashing = true;
		moveComp.allowJump = false;
	}

	public override void OnUpdate() {

		velocity = input2DDirection * _dashSpeed;
		velocity.y = moveComp.currentVelocity.y;

		velocity.y = Mathf.MoveTowards(velocity.y, -_dashGravity, _dashGravityLerpSpeed * Time.deltaTime); //LerpGrav

		Move(velocity);

		if (moveComp.CheckIsGround) {
			
		} else {

		}

		/*
		if (m_dashinputintervel < dashinputIntervel) {
			m_dashinputintervel += Time.deltaTime;
		}*/



		//PlayerCancled Dash
		if (!moveComp.isDashing) {
			var nextState = MwCharacterMovementStateType.DashRecoil;
			SetMovementState(nextState);
		}
	}

	public override void OnFixedUpdate() {
	}

	public override void OnLateUpdate() {
	}

	public override void OnExit() {
		moveComp.isDashing = false;
		moveComp.allowJump = true;
	}

}

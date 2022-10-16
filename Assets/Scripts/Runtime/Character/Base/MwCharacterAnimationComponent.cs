using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class MwCharacterAnimationComponent : MwCharacterComponent {

	protected Animator _animator = null;
	public Animator animator => _animator;

	//Cached friendComp
	protected MwCharacterMovementComponent _moveComp = null;
	protected MwCharacterEffectComponent   _effectComp = null;
	#region Anim Param

	protected static readonly int movementInputX	= Animator.StringToHash("a_movementInputX");
	protected static readonly int movementInputY	= Animator.StringToHash("a_movementInputY");
	
	protected static readonly int isGrounded		= Animator.StringToHash("a_isGrounded");
	protected static readonly int isUpward			= Animator.StringToHash("a_isUpward");
	protected static readonly int isFalling			= Animator.StringToHash("a_isFalling");
	protected static readonly int isTerminalFalling	= Animator.StringToHash("a_isTerminalFalling");
	
	protected static readonly int isSprinting		= Animator.StringToHash("a_isSprinting");

	protected static readonly int jumpTrigger		= Animator.StringToHash("a_jumpTrigger");

	protected static readonly int aimPitch			= Animator.StringToHash("a_aimPitch");
	#endregion

	protected override void OnConstruct(MwCharacter character_) {
		gameObject.TryGetComponentInChildrenWithChecking(out _animator);

		_moveComp	= character.movementComp;
		_effectComp = character.effectComp;

		//SetUp Param
		//Init

		if(_moveComp && _animator) BindMovementEvent(_moveComp);
	}

	protected virtual void BindMovementEvent(MwCharacterMovementComponent moveComp_) {
		moveComp_.OnJumpEnterEvent		+= SetJumpTrigger;
	}

	protected override void onUpdate() {
		UpdateAnimParam();
	}

	//protected override void onFixedUpdate() {	}
	//protected override void onLateUpdate() {	}
	void UpdateAnimParam() {
		if (!_animator) return;

		if(_moveComp) {
			var input = _moveComp.useAnalogMovement ? _moveComp.smoothedMovementInput : _moveComp.movementInput;
			_animator.SetFloat(movementInputX, input.x);
			_animator.SetFloat(movementInputY, input.y);

			_animator.SetBool(isGrounded		, _moveComp.isGrounded);
			_animator.SetBool(isUpward			, _moveComp.isUpward);
			_animator.SetBool(isFalling			, _moveComp.isFalling);
			_animator.SetBool(isTerminalFalling	, _moveComp.isTerminalFalling);
		}

		if (character.playerController) {
			_animator.SetFloat(aimPitch,character.playerController.aimAngle.y);
		}

		OnUpdateAnimParam();
	}
	void OnAnim_FootStepL(AnimationEvent animationevent) {
		if (animationevent.animatorClipInfo.weight > 0.5f) {
			OnFootStepLeftNotify(animationevent);
		}
	}
	void OnAnim_FootStepR(AnimationEvent animationevent) {
		if (animationevent.animatorClipInfo.weight > 0.5f) {
			OnFootStepRightNotify(animationevent);
		}
	}


    #region Virtual Func
    protected virtual void OnUpdateAnimParam() { }
	protected virtual void SetJumpTrigger() {
		if (!_animator) return;

		_animator.ResetTrigger(jumpTrigger);
		_animator.SetTrigger(jumpTrigger);
	}
	protected virtual void OnFootStepLeftNotify (AnimationEvent animationevent) { }
	protected virtual void OnFootStepRightNotify(AnimationEvent animationevent) { }
    #endregion

}
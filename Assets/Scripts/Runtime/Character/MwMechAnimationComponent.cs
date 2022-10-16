using System;
using System.Collections.Generic;
using UnityEngine;

public class MwMechAnimationComponent : MwCharacterAnimationComponent{

	//Cache
	MwMechEffectComponent _mechEffectComp = null;
	MwMechCharacter       _mechCharacter  = null;

	protected static readonly int isDashing     = Animator.StringToHash("a_isDashing");
	protected static readonly int isBoostingUp  = Animator.StringToHash("a_isBoostingUp");

	protected static readonly int dashTrigger   = Animator.StringToHash("a_dashTrigger");

	protected override void OnConstruct(MwCharacter character_) {
		base.OnConstruct(character_);
		_mechCharacter  = character		as MwMechCharacter;
		_mechEffectComp = _effectComp	as MwMechEffectComponent;
	}

	protected override void OnUpdateAnimParam() {
		base.OnUpdateAnimParam();

		if (_moveComp) {
			_animator.SetBool(isDashing, _moveComp.isDashing);
		}
	}

	protected override void BindMovementEvent(MwCharacterMovementComponent moveComp_) {
		base.BindMovementEvent(moveComp_);

		moveComp_.OnDashEnterEvent		+= SetDashTrigger;
		moveComp_.OnDashEndEvent		+= ResetDashTrigger;
	}

	protected virtual  void SetDashTrigger(bool isGrounded) {
		if (!_animator) return;

		_animator.ResetTrigger(dashTrigger);
		_animator.SetTrigger(dashTrigger);
	}
	protected virtual  void ResetDashTrigger(bool isGrounded) {
		if (!_animator) return;

		_animator.ResetTrigger(dashTrigger);
	}


	const float _footStepVolume = 0.5f;
	protected override void OnFootStepRightNotify(AnimationEvent animationevent) {
		//MwAudioManager.PlayClipAtPoint(MwAudioCategory.Common_Mech, _animator.GetBoneTransform(HumanBodyBones.RightFoot).position,(byte)_mechEffectComp.seConfig.SE_footStepID);
		MwAudioManager.Play2DSEOnce(MwAudioCategory.Common_Mech, (byte)_mechEffectComp.seConfig.SE_footStepID, _footStepVolume, 0.95f, 1.05f);

	}

	protected override void OnFootStepLeftNotify(AnimationEvent animationevent) {
		//MwAudioManager.PlayClipAtPoint(MwAudioCategory.Common_Mech, _animator.GetBoneTransform(HumanBodyBones.LeftFoot).position, (byte)_mechEffectComp.seConfig.SE_footStepID);
		MwAudioManager.Play2DSEOnce(MwAudioCategory.Common_Mech, (byte)_mechEffectComp.seConfig.SE_footStepID, _footStepVolume, 0.95f,1.05f);

	}
}
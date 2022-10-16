using System;
using System.Collections.Generic;
using UnityEngine;

public class MwMechEffectComponent : MwCharacterEffectComponent {

	[field: SerializeField] public MwMechVFXconfigSO vfxConfig { get; protected set; }
	[field: SerializeField] public MwMechSEConfigSO seConfig { get; protected set; }

	MwCharacterMovementComponent _moveComp;
	MwMechAnimationComponent	 _animComp;

	[Header("Attached Dash Effect")]
	[SerializeField] ParticleSystem _dashSmokeVFX;
	[SerializeField] ParticleSystem _dashEnterthrusterLoopVFX_R;
	[SerializeField] ParticleSystem _dashEnterthrusterLoopVFX_L;

	[SerializeField] ParticleSystem _dashEnterthrusterBurstVFX;


	protected override void OnConstruct(MwCharacter character_) {
		base.OnConstruct(character_);

		_moveComp = character_.movementComp;
		_animComp = (MwMechAnimationComponent)character_.animComp;


		InitVFX();
		BindEffectEvent();
	}
	void InitVFX() {
		_dashSmokeVFX.SetEmissionActive(false);
		SetDashEffectActive(false);

		_dashEnterthrusterBurstVFX.SetEmissionActive(false);
	}

	void BindEffectEvent() {
		_moveComp.OnDashEnterEvent += OnDashEnterEffect;
		_moveComp.OnDashEndEvent += OnDashEndEffect;
	}

	void OnDisable() {
		_moveComp.OnDashEnterEvent -= OnDashEnterEffect;
	}

	void OnDashEnterEffect(bool isGrounded) {
		if (_dashEnterthrusterBurstVFX) _dashEnterthrusterBurstVFX.Emit(1);
		SetDashEffectActive(true);
	}

	void OnDashEndEffect(bool isGrounded) {
		SetDashEffectActive(false);
	}

	void SetDashEffectActive(bool enable) {
		_dashEnterthrusterLoopVFX_R.SetEmissionActive(enable);
		_dashEnterthrusterLoopVFX_L.SetEmissionActive(enable);

		_dashSmokeVFX.SetEmissionActive(enable);
	}
}
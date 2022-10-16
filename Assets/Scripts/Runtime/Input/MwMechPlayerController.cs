



public class MwMechPlayerController : MwPlayerController {
	public MwMechCharacter currentMechCharacter => _currentCharacter as MwMechCharacter;

	protected override void OnEnable() {
		base.OnEnable();
	}

	protected override void OnDisable() {
		base.OnDisable();
	}

	protected override void Awake() {
		base.Awake();
	}

	protected override void Update() {
		base.Update();
	}

	protected override void BindInputEvent(MwCharacter targetCharacter_) {
		if (!targetCharacter_) return;
		if (!_input) return;

		if (targetCharacter_.movementComp) {
			_input.RegisterJumpEvent(targetCharacter_.movementComp.Jump);
			_input.RegisterSprintEvent(targetCharacter_.movementComp.Sprint);
			_input.RegisterDashEvent(targetCharacter_.movementComp.Dash);
			_input.RegisterBoostUpEvent(targetCharacter_.movementComp.BoostUp);
		}

		var mechChar = (MwMechCharacter)targetCharacter_;

		if (mechChar && mechChar.equipComp) {
			_input.RegisterFireEvent(mechChar.equipComp.TriggerEquipment);
			_input.RegisterFireHoldEvent(mechChar.equipComp.TriggerHoldEquipment);
			_input.RegisterFireReleaseEvent(mechChar.equipComp.TriggerReleaseEquipment);
			_input.RegisterSwitchMainEvent(mechChar.equipComp.SwtichMain);
			_input.RegisterSwitchSubEvent(mechChar.equipComp.SwtichSub);
		}
	}


	protected override void UnBindInputEvent(MwCharacter prevCharacter_) {
		if (!_input) return;
		_input.UnRegisterAllEvents();
	}
}

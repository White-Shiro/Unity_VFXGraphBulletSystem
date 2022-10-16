using UnityEngine;

public abstract class MwPlayerController : MonoBehaviour {

	[SerializeField] protected int _playerID = 0;
	public int playerID => _playerID;

	protected MwCharacter _currentCharacter = null;
	public MwCharacter currentCharacter => _currentCharacter;

	//Components

	//Input Components
	protected MwPlayerInput _input = null;

	//Camera Input Components
	protected MwCameraInput _camInput = null;

	//HUD Components
	//protected MwPlayerHUD _hud = null;

	[Header("Settings")]
	[SerializeField] protected bool _blockInput = false;

	public Vector3 Forward => _camInput.camFowardVector;
	public Vector3 Right => _camInput.camRightVector;
	public Vector3 aimTarget => _camInput.aimTargetPos;
	public Vector2 aimAngle  => _camInput.currentAngle;

	protected virtual void OnEnable() {
		MwGameModeBase.RegisterPlayer(this);
	}

	protected virtual void OnDisable() {
		MwGameModeBase.UnRegisterPlayer(this);
	}

	//Entry Point of PlayerController and its Components
	protected virtual void Awake() {

		//TryGetComponent 
		_input		??= GetComponentInChildren<MwPlayerInput>();
		_camInput	??= GetComponentInChildren<MwCameraInput>();

		//AddComponent if GetComponent failed
		_input		??= gameObject.AddComponent<MwPlayerInput>();
		_camInput	??= gameObject.AddComponent<MwCameraInput>();

		//Construct cameraInput
		if (_input && _camInput) {
			_camInput.OnConstruct(_input);
		}

		Debug.Assert(_input, "MwPlayerInput Component is missing", this);
		Debug.Assert(_camInput, "MwCameraInput Component is missing", this);
	}

	protected virtual void Update() {

		if (_blockInput)		return;
		if (!_currentCharacter)	return;
		if (!_input)			return;
		if (!_camInput)			return;

		_camInput.UpdateCameraRotation(_input.lookInput);
		_currentCharacter.ReceiveInputs(_input.moveInput);
	}

	protected virtual void FixedUpdate() {
		_camInput.UpdateCameraAim();
	}

    protected virtual void BindInputEvent(MwCharacter targetCharacter_) {}
	protected virtual void UnBindInputEvent(MwCharacter prevCharacter_) {}

	#region Possess Func
	public void Possess(MwCharacter char_) {
		if (!char_) {
			Debug.Assert(char_, "Trying to Possess NullPtr Character");
			return;
		}

		if (char_.playerController != null) {
			WxLogger.Error("Trying to Possess an controlled Character");
			return;
		} 

		 char_.AttachPlayerController(this, () => OnPossess(char_));
		_currentCharacter = char_;
	}
	public void UnPossess(MwCharacter char_ = null) {
		//Detach specify Character and check if the Character is possesed by this player.
		if (char_ && char_.playerController == this) {
			char_.DetachPlayerController ( () => OnUnPossess(char_));
			return;
		}

		//Detach current Character if any;
		if (_currentCharacter) {
			_currentCharacter.DetachPlayerController( () => OnUnPossess(char_));
			return;
		}

	}
	public void SwapChar(MwCharacter char_) {
		if (!char_) {
			Debug.Assert(char_, "Trying to Possess NullPtr Character");
			return;
		}

		if (_currentCharacter) {
			UnPossess(_currentCharacter);
		}

		Possess(char_);
	}

	protected virtual void OnPossess(MwCharacter targetCharacter) {
		if (!targetCharacter) return;

		_currentCharacter = targetCharacter;
		_camInput?.SetCamTarget(targetCharacter.cameraPivot);

		BindInputEvent(targetCharacter);

		WxLogger.Log($"<color=#16c444>Player</color> [ <color=#16c444>{playerID}</color> ]  has possessed {targetCharacter.name}");
	}
	protected virtual void OnUnPossess(MwCharacter prevCharacter) {
		_currentCharacter = null;
		_camInput?.SetCamTarget(null);

		UnBindInputEvent(prevCharacter);

		WxLogger.Log($"Player {playerID} has unpossessed {prevCharacter.name}");
	}

	#endregion
}

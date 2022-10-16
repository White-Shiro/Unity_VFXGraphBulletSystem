using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MwCharacterMovementComponent : MwCharacterComponent {

    #region Fields

    [Header("General")]
	[SerializeField] float _speedScaler = 1f;
	const float _gravity = 9.81f;

	[SerializeField]				bool	_useAnalogMovement	= false;
	[SerializeField] [Min(0)]		float	_movementAccelRate	= 2f;
	[SerializeField] [Min(0)]		float	_gravityScaler		= 7f;

	[Header("Walk")]
	[SerializeField]				float	_walkSpeed = 15f;

	[Header("Jump")]
	[SerializeField] [Min(0)]		float _jumpHeight	= 10f;
	[SerializeField] [Min(0)]		int	_maxJumpTimes	= 1;	//For Double Jump , set to 2

	[Header("Air")]
	[SerializeField] [Range(0,1)]	float	_airControlScaler	= 0.5f;
	[SerializeField]				Vector3 _terminalVelocity	= new Vector3(0, -60f, 0);

	[Header("Physics")]
	[SerializeField] [Min(0)]		float		_stepDownSpeed		= 15f;
	[SerializeField]				float		_groundCheckOffset	= 0f;
	[SerializeField] [Min(0.01f)]	float		_groundCheckRadius	= 1f;
	[SerializeField]				LayerMask	_groundCheckLayer;

    #endregion

	#region Properties
    public Transform rootTransform { get;protected set;}

	public MwCharMovementStateMachine stateMachine { get; private set; }
	public MwCharacterMovementStateType? currentStateType => stateMachine?.currentMoveState;

	public bool useAnalogMovement => _useAnalogMovement;
	/// <summary> Returns a normalized Input X & Y </summary>
	public Vector2	movementInput { get; private set; }

	private Vector2	_smoothedMovementInput = Vector2.zero;
	/// <summary> Returns a smoothed Input X & Y </summary>
	public Vector2	smoothedMovementInput {
		get {
			_smoothedMovementInput = Vector2.MoveTowards(_smoothedMovementInput, movementInput, _movementAccelRate * Time.deltaTime);
			return _smoothedMovementInput;
		}
	}
	public	float	inputMagnitude => movementInput.magnitude;

	private float	_smoothedMovementMag = 0f;
	public	float	smoothedMovementMag {
		get {
			_smoothedMovementMag = Mathf.MoveTowards(_smoothedMovementMag, movementInput.magnitude, _movementAccelRate * Time.deltaTime);
			return _smoothedMovementMag;
		}
	}
	public	Vector3	playerForward {
		get {
			if (character.playerController) return transform.rotation * character.playerController.Forward;
			if (rootTransform) return rootTransform.forward;
			return transform.forward;
		}
	}
	public	Vector3	playerRight {
		get {
			if (character.playerController) return transform.rotation * character.playerController.Right;
			if (rootTransform) return rootTransform.right;
			return transform.right;
		}
	}
	/// <summary> Normalized Vector3 of PlayerCameraView's Foward/Right * Input Directions </summary>
	public	Vector3	playerViewInputDir => playerRight * movementInput.x + playerForward * movementInput.y;
	/// <summary> Normalized Vector3 of PlayerView's Foward/Right * Input Directions in 2D Flattend (Horizontal, 0, Vertical); </summary>
	public Vector3	player2DInputDir => (playerRight * movementInput.x + playerForward * movementInput.y).x0z();
	public	Vector3	playerViewSmoothedInputDir => playerRight * smoothedMovementInput.x + playerForward * smoothedMovementInput.y;

	/// <summary> return Vector3 of PlayerView's Foward/Right * Smoothed Input Directions in 2D Flattend (Horizontal, 0, Vertical); </summary>
	public Vector3	player2DSmoothedInputDir => (playerRight * smoothedMovementInput.x + playerForward * smoothedMovementInput.y).x0z();


	//General
	public		float	stepDownSpeed	=> _stepDownSpeed;
	public		float	speedScaler		=> _speedScaler;
	public		float	speedMag { get; private set; }

	//Walk
	public		float	walkSpeed		=> _walkSpeed * _speedScaler;

	//Jump And Air
	public		float	jumpHeight		=> _jumpHeight;
	/// <summary> return -9.81 * scaler </summary>
	const		float	_jumpOffTimeOut = 0.5f;
	public		float	jumpOffTimeout { get; set; } = _jumpOffTimeOut;
	protected	int		_jumpsUsed = 0;
	public		float	gravity => _gravity * _gravityScaler;
	public		float	airControlScaler => _airControlScaler;

	//Velcity
	public Vector3 terminalVelocity => _terminalVelocity;
	protected Vector3 _velocity = Vector3.zero;
	/// <summary> CurrentVelocity in WorldSpace </summary>
	public Vector3 currentVelocity => _velocity;
	public Vector3 currentLocalVelocity => transform.InverseTransformDirection(_velocity);

	//Unity CharacterController
	CharacterController _charController = null;

	//Allow Bool
	public bool allowMove		{ get; set; }	= true;		public bool allowRot		{ get; set; }	= true;
	public bool allowJump		{ get; set; }	= true;		public bool allowDash		{ get; set; }	= true;
	public bool allowBoostUp	{ get; set; }	= true;		public bool allowSprint		{ get; set; }	= true;

	//Movement Bool
	public bool isGrounded			{ get; set; }	= false;
	public bool isDashing			{ get; set; }	= false;
	public bool isJumping			{ get; set; }	= false;
	public bool isBoostingUp		{ get; set; }	= false;
	public bool isSprinting			{ get; set; }	= false;
	public bool isLanding			{ get; set; }	= false;
	public bool isUpward			{ get; set; }	= false;
	public bool isFalling			{ get; set; }	= false;
	public bool isTerminalFalling	{ get; set; }	= false;

    #endregion

    //Action Events
    public event Action OnBoostUpEnterEvent			, OnBoostUpEndEvent			,
						OnSprintEnterEvent			, OnSprintEndEvent			,
						OnJumpEnterEvent			, OnJumpEndEvent			,	OnJumpResetEvent,
						OnDashGroundEnterEvent		, OnDashGroundEndEvent,
						OnLandEnterEvent			, OnLandEndEvent			,
						OnTerminalSpeedEnterEvent	, OnTerminalSpeedEndEvent	,

						_;

	public event Action<bool> OnDashEnterEvent, OnDashEndEvent;

	//Functions
	protected override void OnConstruct(MwCharacter character_) {
		if (stateMachine == null) stateMachine = new MwCharMovementStateMachine(this);

		rootTransform = character_.rootComp.transform;

		_charController ??= GetComponent<CharacterController>();
		Debug.Assert(_charController, "CharacterController is missing");

		stateMachine.SetMovementState(MwCharacterMovementStateType.Gound);
	}

	protected override void onUpdate() {
		stateMachine?.OnUpdate();
	}

	protected override void onFixedUpdate() {
		stateMachine?.OnFixedUpdate();
	}

	protected override void onLateUpdate() {
		stateMachine?.OnLateUpdate();
		UpdateCharacterYawRot();
	}

	//For PlayerController / InputController
	public void SetMovementInput(Vector2 movementInput_) => movementInput = movementInput_;

	public void Move(Vector3 worldSpaceVelocity) {
		if (!allowMove) return;

		//Cache Current Velocity
		_velocity = worldSpaceVelocity;
		//speedMag  = _velocity.magnitude;

		if(_charController) _charController.Move(worldSpaceVelocity * Time.deltaTime);

	}

	public void Jump() {
		if (!allowMove) return;
		if (!allowJump) return;
		if (_jumpsUsed >= _maxJumpTimes) return;

		isJumping = true;
		jumpOffTimeout = _jumpOffTimeOut;
		_jumpsUsed++;

		OnJump();
		OnJumpEnterEvent?.Invoke();
	}

	protected virtual void OnJump() {
		_velocity.y = Mathf.Sqrt(2 * gravity * _jumpHeight);
		stateMachine.SetMovementState(MwCharacterMovementStateType.Air);
	}

	public void JumpReset() {
		OnJumpReset();
		OnJumpResetEvent?.Invoke();
	}


	protected virtual void OnJumpReset() {
		_jumpsUsed = 0;
	}

	public void Dash() {
		if (!allowMove) return;
		if (!allowDash) return;

		isDashing = !isDashing;

		if (isDashing) {
			OnDash();
			OnDashEnterEvent?.Invoke(isGrounded);
		} else {
			OnDashEnd();
			OnDashEndEvent?.Invoke(isGrounded);
		}
	}

	protected virtual void OnDash() {
		stateMachine.SetMovementState(MwCharacterMovementStateType.Dash);
	}

	protected virtual void OnDashEnd() {

	}

	public void BoostUp() {
		if (!allowMove)		return;
		if (!allowBoostUp)	return;
		if (CheckIsGround)	return;

		isBoostingUp = true;

		OnBoostUp();
		OnBoostUpEnterEvent?.Invoke();
	}

	protected virtual void OnBoostUp() {

	}

	public void Sprint() {
		if (!allowMove)		return;
		if (!allowSprint)	return;

		isSprinting = true;

		OnSprint();
		OnSprintEnterEvent?.Invoke();
	}

	protected virtual void OnSprint() {
		
	}

	public void SetVelocity(Vector3 velocity_) {
		if (!allowMove) return;
		_velocity = velocity_;
	}

	public void UpdateCharacterYawRot() {
		if (!allowRot)			return;
		if (!rootTransform)	return;

		//SLerp the rotation with given Speed Later

		var YawAngle = character.cameraPivot.eulerAngles.y;
		var YawQuat = Quaternion.AngleAxis(YawAngle, Vector3.up);
		rootTransform.rotation = YawQuat;
	}

	//Physics
	public bool CheckIsGround {
		get {
			if (!_charController) return false;

			var heightOffset = _charController.height / 2f;
			var offset = (heightOffset + _groundCheckOffset) * Vector3.down;
			var pos = transform.position + offset;
			bool o = Physics.CheckSphere(pos, _groundCheckRadius, _groundCheckLayer.value,QueryTriggerInteraction.Ignore);

#if UNITY_EDITOR
			/*
			var hits = Physics.OverlapSphere(pos, _groundCheckRadius, _groundCheckLayer.value, QueryTriggerInteraction.Ignore);
			if(hits != null && hits.Length > 0) {
				foreach (var col in hits) {
					Debug.LogWarning(col.name);
				}
			}
			*/
#endif

			isGrounded = o;
			return o;
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected() {

		if(_charController == null)
			_charController = GetComponent<CharacterController>();

		if (_charController == null) { return; }

		var heightOffset = _charController.height / 2f;
		var offset = (heightOffset + _groundCheckOffset) * Vector3.down;
		var pos = transform.position + offset;

		Gizmos.color = isGrounded? Color.cyan : Color.red;
		Gizmos.DrawWireSphere(pos,_groundCheckRadius);

		UnityEditor.Handles.Label(pos, $"GroundCheckDis {_groundCheckOffset}");
	}

#endif
}

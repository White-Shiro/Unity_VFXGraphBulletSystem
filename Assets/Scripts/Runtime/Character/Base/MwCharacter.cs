using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class MwCharacter : MonoBehaviour {

	public MwPlayerController playerController { get; private set; }

	#region CharComponents
	//RootClass that holds a transform of the Character Coliders and Meshes.
	public MwRootComponent rootComp { get; protected set;}

	//Array of CharacterComponents for Looping
	protected List<MwCharacterComponent> _charComponents;

	//Contains MovementStateMachine and handle Movement Logic
	public MwCharacterMovementComponent		movementComp	{ get; protected set; }

	//Contains Animation Behaviour and handle Animation State
	public MwCharacterAnimationComponent	animComp		{ get; protected set; }

	//Contains VFX and SFX Effect
	public MwCharacterEffectComponent		effectComp		{ get; protected set;}
	SpringJoint _effectComp = null;

	#endregion

	#region CameraBoom class
	[Header("CameraPivot Settings")]

	[SerializeField] private Vector3 _camPivotOffset = Vector3.up * 5;

	public Vector3 camPivotOffet { get { return _camPivotOffset; } set {
			_camPivotOffset = value;
			if (cameraPivot) {
				_cameraPivot.localPosition = _camPivotOffset;
			}
		} }

	[SerializeField] private Transform _cameraPivot = null;

	public Transform cameraPivot {
		get {
			if (_cameraPivot == null) {
				_cameraPivot = new GameObject("CameraPivot").transform;
				_cameraPivot.parent = movementComp.transform;
				_cameraPivot.localPosition = Vector3.zero + _camPivotOffset;
			}

			return _cameraPivot;
		}
	}

	#endregion

	//Entry Point of whole Character, Inject reference into each Components.
	#region Mono Function
	void Awake() {

		var startTime = Time.realtimeSinceStartup;

		//Set Up RootComp
		rootComp		= gameObject.GetComponentInChildrenWithChecking<MwRootComponent>();
		//Set Up MovementComp
		movementComp	= gameObject.GetComponentWithChecking<MwCharacterMovementComponent>();
		//Set Up AnimComp
		animComp		= gameObject.GetComponentWithChecking<MwCharacterAnimationComponent>();
		//Set Up EffectComp
		effectComp		= gameObject.GetComponentWithChecking<MwCharacterEffectComponent>();

		//Construct Componets
		var components = GetComponentsInChildren<MwComponent>();
		_charComponents = new List<MwCharacterComponent>();

		if (components != null && components.Length > 0) {
			foreach (var comp in components) {
				if (!comp) continue;

				if (comp.GetType().IsSubclassOf(typeof(MwCharacterComponent))) {
					var charComp = comp as MwCharacterComponent;
					if (!charComp) continue;

					if (!_charComponents.Contains(charComp)) {
						_charComponents.Add(charComp);
						charComp.Construct(this);
					}

				} else {
					comp.Construct();
				}
			}
		}

		WxLogger.Log("CharConstructTime:" + (Time.realtimeSinceStartup - startTime) * 1000f + "ms");

		OnConstruct();
	}
	void Update() {

		foreach (var comp in _charComponents) {
			if(comp) comp.OnUpdate();
		}

		OnUpdate();
	}
	void FixedUpdate() {

		foreach (var comp in _charComponents) {
			if(comp) comp.OnFixedUpdate();
		}

		OnFixedUpdate();
	}
	void LateUpdate() {

		foreach (var comp in _charComponents) {
			if (comp) comp.OnLateUpdate();
		}

		OnLateUpdate();
	}
	#endregion

	public void ReceiveInputs(Vector2 movementInput_) {
		movementComp.SetMovementInput(movementInput_);
	}
	public void DetachPlayerController(Action callback = null) {

		//No need to Detach if no PC is posscessing
		if (playerController == null) return;

		playerController.UnPossess(this);

		playerController = null;
		callback?.Invoke();
	}
	public void AttachPlayerController(MwPlayerController pc, Action callback = null) {

		//Prevent SamePlayer Possess same Character twice
		if (pc == playerController) return;

		//Detach PC if any
		DetachPlayerController();

		//Assign New PC
		playerController = pc;
		callback?.Invoke();
	}

	protected virtual void OnConstruct() { }
	protected virtual void OnUpdate() { }
	protected virtual void OnFixedUpdate() { }
	protected virtual void OnLateUpdate() { }


#if UNITY_EDITOR

	private void OnValidate() {
		if (_cameraPivot) {
			_cameraPivot.localPosition = _camPivotOffset;
		}
	}

	private void OnDrawGizmosSelected() {

		Vector3 center = transform.position + _camPivotOffset;
		UnityEditor.Handles.Label(center, "CameraPivot");

		Gizmos.color = new Color(0,1,0,0.25f);
		Gizmos.DrawWireSphere(center, 0.1f);
		Gizmos.DrawSphere(center, 0.1f);
	}
#endif
}
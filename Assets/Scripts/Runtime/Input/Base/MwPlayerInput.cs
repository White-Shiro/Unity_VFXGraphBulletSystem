
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Phase = WxInputPhase;
public class MwPlayerInput : MwComponent
{
	/*
	MwPlayerInputActions _masterInputActions;
	MwPlayerInputActions.PlayerActions	_playerActions;
	MwPlayerInputActions.UIActions		_UIActions;
	MwPlayerInputActions.CombatActions	_CombatActions;
	*/

	public Vector2 moveInput => Vector3.zero;                     //_playerActions.Move.ReadValue<Vector2>();
	public Vector2 lookInput => Vector3.zero;                     //_playerActions.Look.ReadValue<Vector2>();

	bool _fireButtonDown = false;

	private event Action OnJumpAction, OnDashAction, OnBoostUpAction, OnSprintAction;
	private event Action OnFireAction, OnFireHoldAction, OnFireReleaseAction, OnSwitchMainAction, OnSwitchSubAction;

	private void OnEnable() {

		/*

		_masterInputActions = new MwPlayerInputActions();


		_playerActions		= _masterInputActions.Player;
		_UIActions			= _masterInputActions.UI;
		_CombatActions		= _masterInputActions.Combat;

		_masterInputActions.Enable();
		_playerActions.Enable();
		_CombatActions.Enable();
		_UIActions.Enable();

		SetInputCallBacks(true);
		*/
	}

	private void OnDisable() {
		SetInputCallBacks(false);
		UnRegisterAllEvents();

		/*

		_UIActions.Disable();
		_CombatActions.Disable();
		_playerActions.Disable();	
		_masterInputActions.Disable();
		*/
	}

	private void OnApplicationFocus(bool focus) {
		CursorToggle(focus);
	}

	void SetInputCallBacks(bool value = true) {

		/*

		//Input
		_playerActions.Jump.Bind(Phase.Performed,OnJump,value);
		_playerActions.Dash.Bind(Phase.Performed,OnDash,value);
		_playerActions.BoostUp.Bind(Phase.Performed,OnBoostUp,value);
		_playerActions.Sprint.Bind(Phase.Performed,OnSprint,value);

		//Combat
		_CombatActions.Enable();
		_CombatActions.Fire.Bind(Phase.Started,OnFireDown,value);
		_CombatActions.Fire.Bind(Phase.Performed,OnFire,value);
		_CombatActions.Fire.Bind(Phase.Canceled ,OnFireRelease,value);
		_CombatActions.SwitchMainWeapon.Bind(Phase.Performed ,OnSwitchMainWeapon,value);
		_CombatActions.SwitchSubWeapon.Bind(Phase.Performed ,OnSwitchSubWeapon,value);


		//UI
		_UIActions.CursorMode.Bind(Phase.Performed,OnCursorToggle,value);

		*/
	}


	//InputEvents

	void OnFireDown(InputAction.CallbackContext ctx) {
		_fireButtonDown = true;
	}


	async void OnFire(InputAction.CallbackContext ctx) {
		if (ctx.performed) {
			OnFireAction?.Invoke();
		}

		while (_fireButtonDown) {
			OnFireHoldAction?.Invoke();
			await Task.Yield();
		}
	}

	void OnFireRelease(InputAction.CallbackContext ctx) {
		if (ctx.canceled) {
			OnFireReleaseAction?.Invoke();
			_fireButtonDown = false;
		}
	}

	void OnJump(InputAction.CallbackContext ctx) {
		if (ctx.performed) {
			OnJumpAction?.Invoke();
		}
	}
	void OnDash(InputAction.CallbackContext ctx) {
		if (ctx.performed) {
			OnDashAction?.Invoke();
		}
	}

	void OnBoostUp(InputAction.CallbackContext ctx) {
		if (ctx.performed) {

			var btnDown = ctx.ReadValueAsButton();
			if (btnDown) {
				OnBoostUpAction?.Invoke();
			} else {
				//btnRelease

			}
		}
	}

	void OnSprint(InputAction.CallbackContext ctx) {
		if (ctx.performed) {

			var btnDown = ctx.ReadValueAsButton();
			if (btnDown) {
				OnSprintAction?.Invoke();
			} else {
				//btnRelease
			}
		}
	}

	void OnSwitchMainWeapon(InputAction.CallbackContext ctx) {
		if(ctx.performed) {
			OnSwitchMainAction?.Invoke();
		}
	}

	void OnSwitchSubWeapon(InputAction.CallbackContext ctx) {
		if(ctx.performed) {
			OnSwitchSubAction?.Invoke();
		}
	}

	//Action Event

	//Jump
	public void RegisterJumpEvent(Action cb)			=> WxAction.AddCallback(	ref OnJumpAction	, ref cb);
	//public void UnRegisterJumpEvent(Action cb)			=> WxAction.RemoveCallback(	ref OnJumpAction	, ref cb);

	//Dash
	public void RegisterDashEvent(Action cb)			=> WxAction.AddCallback(	ref OnDashAction	, ref cb);
	//public void UnRegisterDashEvent(Action cb)			=> WxAction.RemoveCallback(	ref OnDashAction	, ref cb);
	//JetPack
	public void RegisterBoostUpEvent(Action cb)			=> WxAction.AddCallback(	ref OnBoostUpAction, ref cb);
	//public void UnRegisterBoostUpEvent(Action cb)		=> WxAction.RemoveCallback(	ref OnBoostUpAction, ref cb);
	//Sprint
	public void RegisterSprintEvent(Action cb)			=> WxAction.AddCallback(	ref OnSprintAction	, ref cb);
	//public void UnRegisterSprintEvent(Action cb)		=> WxAction.RemoveCallback(	ref OnSprintAction	, ref cb);

	//Combat
	public void RegisterFireEvent(Action cb) => WxAction.AddCallback(ref OnFireAction, ref cb);
	//public void UnRegisterFireEvent(Action cb) => WxAction.RemoveCallback(ref OnFireAction, ref cb);
	public void RegisterFireHoldEvent(Action cb) => WxAction.AddCallback(ref OnFireHoldAction, ref cb);
	//public void UnRegisterFireHoldEvent(Action cb) => WxAction.RemoveCallback(ref OnFireHoldAction, ref cb);
	public void RegisterFireReleaseEvent(Action cb) => WxAction.AddCallback(ref OnFireReleaseAction, ref cb);
	//public void UnRegisterFireReleaseEvent(Action cb) => WxAction.RemoveCallback(ref OnFireReleaseAction, ref cb);

	public void RegisterSwitchMainEvent(Action cb)		=> WxAction.AddCallback(	ref OnSwitchMainAction, ref cb);
	//public void UnRegisterSwitchMainEvent(Action cb) 	=> WxAction.RemoveCallback(	ref OnSwitchMainAction, ref cb);
	public void RegisterSwitchSubEvent(Action cb)		=> WxAction.AddCallback(	ref OnSwitchSubAction, ref cb);
	//public void UnRegisterSwitchSubEvent(Action cb)		=> WxAction.RemoveCallback(	ref OnSwitchSubAction, ref cb);


	public void OnCursorToggle(InputAction.CallbackContext contex) {
		if (contex.performed) {
			var state = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.lockState = state;
		}
	}
	public void CursorToggle(bool state_) {
		var state = state_ ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.lockState = state;
	}

	public virtual void UnRegisterAllEvents() {
		OnJumpAction	= null;
		OnDashAction	= null;
		OnBoostUpAction	= null;
		OnSprintAction	= null;

		OnFireAction	= null;
		OnFireHoldAction = null;
		OnFireReleaseAction	= null;

		OnSwitchMainAction = null;
		OnSwitchSubAction  = null;
	}
}
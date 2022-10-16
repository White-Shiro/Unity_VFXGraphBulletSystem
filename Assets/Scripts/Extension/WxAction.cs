using System;
public static class WxAction {
	public static void Bind(this UnityEngine.InputSystem.InputAction action, WxInputPhase phase, Action<UnityEngine.InputSystem.InputAction.CallbackContext> cb, bool add = true) {

		switch (phase) {
			case WxInputPhase.Started:
				action.started -= cb;
				if (add) action.started += cb;
				break;
			case WxInputPhase.Performed:
				action.performed -= cb;
				if (add) action.performed += cb;
				break;
			case WxInputPhase.Canceled:
				action.canceled -= cb;
				if (add) action.canceled += cb;
				break;
			default: break;
		}
	}

	public static void Bind(ref Action action, Action cb, bool add = true) {
		action -= cb;
		if(add) action += cb;
	}
	public static void Bind<T>(ref Action<T> action, Action<T> cb, bool add = true) {
		action -= cb;
		if (add) action += cb;
	}
	public static void Bind<T1,T2>(ref Action<T1,T2> action, Action<T1,T2> cb, bool add = true) {
		action -= cb;
		if (add) action += cb;
	}

	public static void AddCallback<T>(ref Action<T> action, ref Action<T> cb) {
		action -= cb;
		action += cb;
	}

	public static void AddCallback(ref Action action, ref Action cb) {
		action -= cb;
		action += cb;
	}

	public static void RemoveCallback<T>(ref Action<T> action, Action<T> cb) {
		action -= cb;
	}

	public static void RemoveCallback(ref Action action, Action cb) {
		action -= cb;
	}
}

public enum WxInputPhase {
	Started,
	Performed,
	Canceled,
}
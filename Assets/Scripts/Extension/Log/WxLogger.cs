using UnityEngine;

public static class WxLogger {
	public static void Log(object msg) {
#if UNITY_EDITOR
		Debug.Log(msg);
#endif
	}

	public static void Warning(object msg) {
#if UNITY_EDITOR
		Debug.LogWarning(msg);
#endif
	}

	public static void Error(object msg) {
#if UNITY_EDITOR
		Debug.LogError(msg);
#endif
	}

	public static void Log(object msg,Object context) {
#if UNITY_EDITOR
		Debug.Log(msg, context);
#endif
	}

	public static void Warning(object msg, Object context) {
#if UNITY_EDITOR
		Debug.LogWarning(msg, context);
#endif
	}

	public static void Error(object msg, Object context) {
#if UNITY_EDITOR
		Debug.LogError(msg,context);
#endif
	}
}


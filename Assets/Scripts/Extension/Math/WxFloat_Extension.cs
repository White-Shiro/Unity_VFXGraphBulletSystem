using UnityEngine;

public static class WxFloat_Extension {

	//Angle
	public static float ClampAngle(this float value, float min = float.MinValue, float max = float.MaxValue) {
		if(value < -360f) value += 360f;
		if (value > 360f) value -= 360f;
		return Mathf.Clamp(value, min, max);
	}

	public static void Clamp(this ref float value, float min = float.MinValue, float max = float.MaxValue) {
		if(value < min) value = min;
		if(value > max) value = max;
	}

	public static void ClampMax(this ref float value, float max) {
		value = value > max? max : value;
    }
}

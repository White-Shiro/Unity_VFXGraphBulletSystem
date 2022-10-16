using UnityEngine;

public static class WxVector2_Extensions {
	public static Vector2 xy(this Vector2 v) => new Vector2(v.x, v.y);
	public static Vector3 x0z(this Vector2 v) => new Vector3(v.x, 0, v.y);
	public static Vector3 xy0(this Vector2 v) => new Vector3(v.x, v.y, 0);

	public static bool CompareSqrDistance(this Vector2 origin, Vector2 target, float threshold) {
		//True is Greater / False smaller
		var sqrMag = (target - origin).sqrMagnitude;
		bool o = sqrMag > threshold * threshold;
		return o;
	}

	public static Vector2 SafeDivide(this Vector2 v, float divisor) {
		if (Mathf.Approximately(0, divisor)) { return Vector2.zero; }
		return v / divisor;
	}
}

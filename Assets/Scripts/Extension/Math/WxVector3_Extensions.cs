using UnityEngine;

public static class WxVector3_Extensions {
	public static Vector2 xy(this Vector3 v) => new Vector2(v.x, v.y);
	public static Vector2 xz(this Vector3 v) => new Vector2(v.x, v.z);
	public static Vector3 xyz(this Vector3 v) => new Vector3(v.x, v.y, v.z);
	public static Vector3 x0z(this Vector3 v) => new Vector3(v.x, 0, v.z);
	public static Vector3 xy0(this Vector3 v) => new Vector3(v.x, v.y, 0);

	public static bool CompareSqrDistance(this Vector3 origin, Vector3 target, float threshold) {
		//True is Greater / False smaller
		var sqrMag = (target - origin).sqrMagnitude;
		bool o = sqrMag > threshold * threshold;
		return o;
	}

	public static Vector3 SafeDivide(this Vector3 v, float divisor) {
		if (Mathf.Approximately(0, divisor)) { return Vector3.zero; }
		return v / divisor;
	}
}

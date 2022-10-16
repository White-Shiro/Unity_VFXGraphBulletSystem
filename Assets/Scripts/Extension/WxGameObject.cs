using System;
using UnityEngine;
using Object = UnityEngine.Object;
public static class WxGameObject {
	public static bool TryGetComponentWithChecking<T>(this Component go, out T comp, string msg = null, Object context = null) where T : Component {
		go.TryGetComponent(out comp);
		msg ??= $"{typeof(T)} is Missing in {go.name}";
		Debug.Assert(comp, msg, context);
		return comp != null;
	}

	public static bool TryGetComponentWithChecking<T>(this GameObject go, out T comp, string msg = null, Object context = null) where T : Component {
		go.TryGetComponent(out comp);
		msg ??= $"{typeof(T)} is Missing in {go.name}";
		Debug.Assert(comp, msg, context);
		return comp != null;
	}

	public static bool TryGetComponentWithChecking<T>(this Transform go, out T comp, string msg = null, Object context = null) where T : Component {
		go.TryGetComponent(out comp);
		msg ??= $"{typeof(T)} is Missing in {go.name}";
		Debug.Assert(comp, msg, context);
		return comp != null;
	}

	public static bool TryGetComponentInChildrenWithChecking<T>(this Component go, out T comp, string msg = null, Object context = null) where T : Component {
		comp = go.GetComponentInChildren<T>();
		msg ??= $"{typeof(T)} is Missing in {go.name}";
		Debug.Assert(comp, msg, context);
		return comp != null;
	}

	public static bool TryGetComponentInChildrenWithChecking<T>(this GameObject go, out T comp, string msg = null, Object context = null) where T : Component {
		comp = go.GetComponentInChildren<T>();
		msg ??= $"{typeof(T)} is Missing in {go.name}";
		Debug.Assert(comp, msg, context);
		return comp != null;
	}

	public static bool TryGetComponentInChildrenWithChecking<T>(this Transform go, out T comp, string msg = null, Object context = null) where T : Component {
		comp = go.GetComponentInChildren<T>();
		msg ??= $"{typeof(T)} is Missing in {go.name}";
		Debug.Assert(comp, msg, context);
		return comp != null;
	}

	public static T GetComponentWithChecking<T>(this GameObject go, string msg = null, Object context = null) {
		var o = go.GetComponent<T>();
		Debug.Assert(o != null, msg, context);
		return o;
	}

	public static T GetComponentWithChecking<T>(this Transform go, string msg = null, Object context = null) {
		var o = go.GetComponent<T>();
		Debug.Assert(o != null, msg, context);
		return o;
	}

	public static T GetComponentWithChecking<T>(this Component go, string msg = null, Object context = null) {
		var o = go.GetComponent<T>();
		Debug.Assert(o != null, msg, context);
		return o;
	}
	public static T GetComponentInChildrenWithChecking<T>(this GameObject go, string msg = null, Object context = null) {
		var o = go.GetComponentInChildren<T>();
		Debug.Assert(o != null, msg, context);
		return o;
	}

	public static T GetComponentInChildrenWithChecking<T>(this Transform go, string msg = null, Object context = null) {
		var o = go.GetComponentInChildren<T>();
		Debug.Assert(o != null, msg, context);
		return o;
	}

	public static T GetComponentInChildrenWithChecking<T>(this Component go, string msg = null, Object context = null) {
		var o = go.GetComponentInChildren<T>();
		Debug.Assert(o != null, msg, context);
		return o;
	}

	public static GameObject CreateChildGameObject(this GameObject parent, string name, params Type[] components) {
		var go = new GameObject(name, components);
		go.transform.SetParent(parent.transform);
		return go;
	}
	public static GameObject CreateChildGameObject(this Transform parent, string name, params Type[] components) {
		var go = new GameObject(name, components);
		go.transform.SetParent(parent);
		return go;
	}
	public static GameObject CreateChildGameObject(this GameObject parent) {
		var go = new GameObject();
		go.transform.SetParent(parent.transform);
		return go;
	}
	public static GameObject CreateChildGameObject(this Transform parent) {
		var go = new GameObject();
		go.transform.SetParent(parent);
		return go;
	}

	public static T CreateChildComponent<T>(this GameObject parent, string name, Type component) where T : Component {
		var go = new GameObject(name, component);
		go.transform.SetParent(parent.transform);
		return go.GetComponent<T>();
	}

	public static T CreateChildComponent<T>(this Transform parent, string name, Type component) where T : Component {
		var go = new GameObject(name, component);
		go.transform.SetParent(parent);
		return go.GetComponent<T>();
	}
}
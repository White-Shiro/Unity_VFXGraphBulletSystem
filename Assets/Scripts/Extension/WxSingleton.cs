using System;
using UnityEngine;

public class WxSingleton<T> where T : class ,new() {
	protected static T _instance = null;
	
	public static T instance {
		get {
			if(_instance == null) {
				_instance = new T();
			}
			return _instance;
		}
	}

	/*
	public static T Create(params object[] args) {

		if(_instance != null) return _instance;

		var newInstance =  (T)Activator.CreateInstance(typeof(T), args);
		return newInstance;
	} */
}

//For Unity MonoObject
public class WxMonoSingleton<T> : MonoBehaviour where T : class {

	protected static T _instance = null;
	public static T instance {
		get {
			if (_instance == null) {
				var type = typeof(T);
				var otherInstance = FindObjectOfType(type);

				if (otherInstance != null) {
					_instance = otherInstance as T;
				} else {
					var name = $"---{type}---";
					_instance = new GameObject(name, type).GetComponent<T>();
				}
			}

			return _instance;
		}
	}

	private void Awake() {
		if (_instance == null) {
			var type = typeof(T);
			var otherInstance = FindObjectOfType(type);

			if (otherInstance == null) {
				_instance = otherInstance as T;
			} else {
				_instance = this as T;
			}

		} else {
			Destroy(gameObject);
		}

		OnAwake();
	}

	protected virtual void OnAwake() { }
}


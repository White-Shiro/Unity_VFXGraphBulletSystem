using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AddressableAssets;

public class MwObjectPoolManager : MonoBehaviour {
	// Singleton reference


	private static MwObjectPoolManager _instance;
	public static MwObjectPoolManager instance {
		get {
			if (_instance == null) {
				_instance = new GameObject("---MwObjectPoolManager---", typeof(MwObjectPoolManager)).GetComponent<MwObjectPoolManager>();
			}
			return _instance;
		}
	}

	private Dictionary<string,MwObjectPool> _poolMap = new Dictionary<string, MwObjectPool>();

	public static GameObject GetPooledObject(GameObject prefab) {
		CheckAndCreatePool(prefab);
		return instance._poolMap[prefab.name].Get();
	}

	public static async void Release(GameObject obj,float delay = 0) {
		if (!instance._poolMap.ContainsKey(obj.name)) {
			WxLogger.Warning("item doesnt not belongs to any Pool");
			return;
		}

		await Task.Delay((int)(delay * 1000));
		instance._poolMap[obj.name].Release(obj);
	}

	public static void ClearPool(GameObject prefab) {
		instance._poolMap[prefab.name].Clear();
	}

	public static bool CheckAndCreatePool(GameObject prefab, int defaultAmount_ = 10 , int maxAmout_ = 10000) {

		if(!Application.isPlaying) return false;
		if(instance._poolMap.ContainsKey(prefab.name)) return false;

		var parent = new GameObject($"{prefab.name}_pool");
		parent.transform.parent = instance.transform;

		var newPool = new MwObjectPool(prefab, () => {

			//Change To Address
			var instance = Instantiate(prefab,parent.transform);
			instance.name = prefab.name;
			return instance;
		},
			p => { p.SetActive(true);},
			p => { p.SetActive(false);},
			p => { Destroy(p); },
			false, defaultAmount_, maxAmout_
		);

		instance._poolMap[prefab.name] = newPool;
		return true;
	}
}

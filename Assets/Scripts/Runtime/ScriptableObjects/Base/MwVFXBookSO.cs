using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VFXBook_", menuName = "Data/VFX/VFXBook")]
[Serializable]
public class MwVFXBookSO : MwAssetBookSO {

	[SerializeField]
	protected	MwVFXCategory	_vfxCategory;
	public		MwVFXCategory	vfxCategory => _vfxCategory;
	public		MwVFXPair[]		vfxList;
	protected	Dictionary<byte, GameObject> _vfxmap = new Dictionary<byte, GameObject>();

	public bool isInited { get; protected set;} = false;

	void Awake() {
		VerifyAndMakeVFXMap();
	}

	public override void Init() {
		VerifyAndMakeVFXMap();
	}

	public bool VerifyAndMakeVFXMap() {
		if (isInited) return true;

		bool hasError = false;
		_vfxmap.Clear();
		foreach (var pair in vfxList) {
			//TODO Check Error
			if (pair.vfxPrefab == null) continue;
			if (!_vfxmap.ContainsKey(pair.ID)) _vfxmap[pair.ID] = pair.vfxPrefab;
		}

		isInited = true;

		if (!hasError) WxLogger.Log($"{name} Verified");
		return !hasError;
	}
	//Editor
	protected	Dictionary<string, GameObject> _savedData;
	public		void ResetEntries() {
		OnResetEntries(_vfxCategory);
	}
	protected	void OnResetEntries(MwVFXCategory cat_) {
		if (vfxList != null) { CacheSavedData(ref _savedData); }

		Type enumType;

		switch (cat_) {
			case MwVFXCategory.None: vfxList = null; _vfxmap.Clear(); return;
			default: WxLogger.Error("Unsupported AudioCategory"); return;

			case MwVFXCategory.Combat: enumType = typeof(MwVFXID.Combat); break;
			case MwVFXCategory.Enviroment: enumType = typeof(MwVFXID.Enviroment); break;
			case MwVFXCategory.MechMovement: enumType = typeof(MwVFXID.MechMovement); break;
		}

		var values = Enum.GetValues(enumType);
		var names = Enum.GetNames(enumType);

		vfxList = new MwVFXPair[values.Length];

		for (int i = 0; i < vfxList.Length; i++) {

			var key = names[i];
			var prefab = _savedData.ContainsKey(key) ? _savedData[key] : null;
			var pair = new MwVFXPair((byte)i, prefab, key);
			vfxList[i] = pair;
		}

		WxLogger.Log($"[ {this.name} ] <color=#16c444>Updated</color>");
	}
	protected	void CacheSavedData(ref Dictionary<String, GameObject> src) {

		if (src == null) {
			src = new Dictionary<string, GameObject>();
		} else {
			src.Clear();
		}

		foreach (var pair in vfxList) {
			if (!src.ContainsKey(pair.Editor_PrefabName))
				src[pair.Editor_PrefabName] = pair.vfxPrefab;
		}
	}

	[Serializable]
	public class MwVFXPair {
		public MwVFXPair(byte ID_, GameObject prefeb_ = null, string name_ = null) {
			ID = ID_;
			vfxPrefab = prefeb_;
			Editor_PrefabName = name_;
		}


		[WxShowOnly] public string Editor_PrefabName;
		[WxShowOnly] public byte ID;
		public GameObject vfxPrefab;
	}

	public enum MwVFXCategory {
		None,
		MechMovement,
		Combat,
		Enviroment,
	}
	public struct MwVFXID {
		public enum MechMovement : byte {
			None,

			MechFootStep_01,
			MechFootStep_02,
			MechFootStep_03,
			MechFootStep_04,

			DashEnter_01,
			DashEnter_02,

			DashLoop_01,
			DashLoop_02,

			DashRecoil_01,
			DashRecoil_02,

			Land_01,
			Land_02,
			Land_03,

			JumpEnter_01,
			JumpEnter_02,
			JumpEnter_03,
		}

		public enum Combat : byte { }
		public enum Enviroment : byte { }

	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AudioBook_", menuName = "Data/Audio/AudioBook")]
[Serializable]
public class MwAudioBookSO : MwAssetBookSO {

	[SerializeField] 
	protected	MwAudioCategory		_audioCategory;
	public		MwAudioCategory		audioCategory => _audioCategory;
	public		MwAudioPair[]		SEList;

	protected	Dictionary<byte, AudioClip>	_SEmap = new Dictionary<byte, AudioClip>();

	public bool isInited { get; private set;} = false ;

	void Awake() {
		VerifyAndMakeSEMap();
	}

	public override void Init() {
		VerifyAndMakeSEMap();
	}

	public AudioClip GetClipByID(byte id_) {
		if(!_SEmap.ContainsKey(id_)) return null;
		return _SEmap[id_];
	}

	public bool TryGetClipByID(byte id_,out AudioClip clip) {
		if(_SEmap.TryGetValue(id_,out clip)) {
			return true;
		}

		WxLogger.Error($"{name} Clip {id_} is missing");
		return false;
	}

	public bool VerifyAndMakeSEMap() {

		//if(isInited) return true;

		bool hasError = false;
		_SEmap.Clear();
		foreach (var pair in SEList) {
			//TODO Check Error
			if(pair.clip == null) continue;
			if(!_SEmap.ContainsKey(pair.ID)) _SEmap[pair.ID] = pair.clip;
		}

		isInited = true;

		if(!hasError) WxLogger.Log($"{name} Verified");
		return !hasError;
	}
	//Editor
	protected	Dictionary<string, AudioClip> _savedData;
	public		void ResetEntries() {
		OnResetEntries(_audioCategory);
	}
	protected	void OnResetEntries(MwAudioCategory cat_) {

		if (SEList != null) { CacheSavedData(ref _savedData); }

		Type enumType;

		switch (cat_) {

			case MwAudioCategory.None: SEList = null; _SEmap.Clear(); return;
			default: WxLogger.Error("Unsupported AudioCategory"); return;

			case MwAudioCategory.Common_UI: enumType = typeof(MwAudioID.Common_UI); break;
			case MwAudioCategory.Common_Combat: enumType = typeof(MwAudioID.Common_Combat); break;
			case MwAudioCategory.Common_Mech: enumType = typeof(MwAudioID.Common_Mech); break;
		}

		var values = Enum.GetValues(enumType);
		var names = Enum.GetNames(enumType);

		SEList = new MwAudioPair[values.Length];

		for (int i = 0; i < SEList.Length; i++) {
			var key = names[i];
			var clip = _savedData.ContainsKey(key) ? _savedData[key] : null;
			var pair = new MwAudioPair((byte)i, clip, names[i]);
			SEList[i] = pair;
		}

		WxLogger.Log($"[ {this.name} ] <color=#16c444>Updated</color>");
	}
	protected	void CacheSavedData(ref Dictionary<String, AudioClip> src) {

		if (src == null) {
			src = new Dictionary<string, AudioClip>();
		} else {
			src.Clear();
		}

		foreach (var pair in SEList) {
			if (!src.ContainsKey(pair.Editor_clipName))
				src[pair.Editor_clipName] = pair.clip;
		}
	}
}

[Serializable]
public class MwAudioPair {
	public MwAudioPair(byte ID_, AudioClip clip_ = null, string name_ = null) {
		ID = ID_;
		clip = clip_;
		Editor_clipName = name_;
	}

	[WxShowOnly] public string Editor_clipName;
	[WxShowOnly] public byte ID;
	public AudioClip clip;
}

public struct MwAudioID {
	public enum Common_Combat : byte {


	
	}
	public enum Common_Mech : byte{ 
		None,

		FootStep_01, 
		FootStep_02,
		FootStep_03,
		FootStep_04,

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
	public enum Common_UI {


	
	
	
	}
}

public enum MwAudioCategory {
	None,
	Common_UI,
	Common_Combat,
	Common_Mech,
}
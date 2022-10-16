using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MwAudioManager : WxMonoSingleton<MwAudioManager> {

	MwAudioBookSO _audioBook_CommonUI		= null;
	MwAudioBookSO _audioBook_CommonMech		= null;
	MwAudioBookSO _audioBook_CommonCombat	= null;

	public MwAudioBookSO audioBook_CommonUI		=> _audioBook_CommonUI;
	public MwAudioBookSO audioBook_CommonMech	=> _audioBook_CommonMech;
	public MwAudioBookSO audioBook_CommonCombat	=> _audioBook_CommonCombat;

	List<MwAudioBookSO> _audiobooks = new List<MwAudioBookSO>();

	//for 2D Effect
	WxAudioSource _seSoruce;
	WxAudioSource _uiSoruce;
	WxAudioSource _bgmSoruce;
	WxAudioSource _voSoruce;


	protected override void OnAwake() {
		Addressables.InitializeAsync().Completed += OnBookInit;

		var type = typeof(WxAudioSource);

		_seSoruce	= transform.CreateChildComponent<WxAudioSource>("AudioSource_SE",	type);
		_uiSoruce	= transform.CreateChildComponent<WxAudioSource>("AudioSource_UI",	type);
		_bgmSoruce	= transform.CreateChildComponent<WxAudioSource>("AudioSource_BGM",	type);
		_voSoruce	= transform.CreateChildComponent<WxAudioSource>("AudioSource_VO",	type);
	}

	void OnBookInit(AsyncOperationHandle<IResourceLocator> obj) {
		Addressables.LoadAssetAsync<MwAudioBookSO>("AudioBook_CommonUI").Completed		+= OnUIAudioAssetLoaded;
		Addressables.LoadAssetAsync<MwAudioBookSO>("AudioBook_CommonCombat").Completed	+= OnCombatAudioAssetLoaded;
		Addressables.LoadAssetAsync<MwAudioBookSO>("AudioBook_CommonMech").Completed	+= OnMechAudioAssetLoaded;
	}

	void OnUIAudioAssetLoaded(AsyncOperationHandle<MwAudioBookSO> book) => OnBookLoaded(out _audioBook_CommonUI, book);
	void OnMechAudioAssetLoaded(AsyncOperationHandle<MwAudioBookSO> book) => OnBookLoaded(out _audioBook_CommonMech, book);
	void OnCombatAudioAssetLoaded(AsyncOperationHandle<MwAudioBookSO> book) => OnBookLoaded(out _audioBook_CommonCombat, book);
	void OnBookLoaded(out MwAudioBookSO outbook, AsyncOperationHandle<MwAudioBookSO> loadedObj) {

		if (!loadedObj.IsValid()) { outbook = null; return;}

		outbook = loadedObj.Result;
		outbook.Init();

		if (!_audiobooks.Contains(outbook)) _audiobooks.Add(outbook);
		WxLogger.Log($"{outbook.name} loaded");
	}

	public static void PlayOneShot(WxAudioConfig config, Vector3 point, params AudioClip[] clips) {

		if (clips == null) return;

		switch (config.space) {
			case WxAudioSpace.Screen: PlayMultiple2DSEOnce(config.volume, config.pitchRange.x, config.pitchRange.y, clips); break;
			case WxAudioSpace.World:  WxAudioSource.PlayClipsAtPoint(clips, point, config.playType, config.volume); break;
			default: break;
		}
	}
	public static void PlayOneShot(WxAudioConfig config, params AudioClip[] clips) {

		if (clips == null) return;

		switch (config.space) {
			case WxAudioSpace.Screen: PlayMultiple2DSEOnce(config.volume, config.pitchRange.x, config.pitchRange.y, clips); break;
			case WxAudioSpace.World: WxAudioSource.PlayClipsAtPoint(clips, Vector3.zero, config.playType, config.volume); break;
			default: break;
		}
	}

	public static void PlayClipAtPoint(MwAudioCategory clipType, Vector3 point, byte id, float volume = 1f) {
		instance.TryGetClipByCategory(clipType,id, out AudioClip clip);
		if(clip) WxAudioSource.PlayClipAtPoint(clip, point,volume);
	}

	public static void PlayClipAtPoint(AudioClip clip, Vector3 point, float volume = 1f) {
		if (clip) WxAudioSource.PlayClipAtPoint(clip, point,volume);
	}

	public static void Play2DSEOnce(MwAudioCategory clipType, byte id, float volume = 1f,float minPitch = 1f,float maxPitch = 1f) {
		instance.TryGetClipByCategory(clipType, id, out AudioClip clip);
		if (clip) { instance._seSoruce.PlayOneShot(clip,minPitch,maxPitch, volume); }
	}


	public static void Play2DSEOnce(AudioClip clip, float volume = 1f, float minPitch = 1f, float maxPitch = 1f) {
		if (clip) { instance._seSoruce.PlayOneShot(clip, minPitch, maxPitch,volume); }
	}

	public static void PlayMultiple2DSEOnce(float volume = 1f, float minPitch = 1f, float maxPitch = 1f, params AudioClip[] clips) {
		if (clips != null) { instance._seSoruce.PlayClipsOneShot(minPitch, maxPitch, volume, clips); }
	}

	public static void PlayUISEOnce(MwAudioCategory clipType, byte id, float volume = 1f, float minPitch = 1f, float maxPitch = 1f) {
		instance.TryGetClipByCategory(clipType, id, out AudioClip clip);
		if (clip) { instance._uiSoruce.PlayOneShot(clip, minPitch, maxPitch, volume); }
	}

	public static void PlayUISEOnce(AudioClip clip, float volume = 1f, float minPitch = 1f, float maxPitch = 1f) {
		if (clip) { instance._uiSoruce.PlayOneShot(clip, minPitch, maxPitch,volume); }
	}

	public static void PlayBGM(MwAudioCategory clipType, byte id) {
		instance.TryGetClipByCategory(clipType, id, out AudioClip clip);
		if (clip) { instance._bgmSoruce.Play(clip); }
	}

	/// <summary> Avoid PlayBGM by Direct Reference </summary>
	public static void PlayBGM(AudioClip clip) {
		if (clip) { instance._bgmSoruce.Play(clip); }
	}

	void TryGetClipByCategory(MwAudioCategory clipCat, byte id_,out AudioClip clip) {
		clip = null;
		switch (clipCat) {
			case MwAudioCategory.None: clip = null; break;
			default: clip = null; break;

			case MwAudioCategory.Common_UI :	if (_audioBook_CommonUI)	 _audioBook_CommonUI.TryGetClipByID(id_, out clip);		break;
			case MwAudioCategory.Common_Combat:	if (_audioBook_CommonCombat) _audioBook_CommonCombat.TryGetClipByID(id_, out clip);	break;
			case MwAudioCategory.Common_Mech:	if (_audioBook_CommonMech)	 _audioBook_CommonMech.TryGetClipByID(id_, out clip);	break;
		}
	}

	public static void PlayBGM() { }

	void OnDestroy() {
		foreach (var book in _audiobooks) {
			Addressables.Release(book);
		}
	}
}

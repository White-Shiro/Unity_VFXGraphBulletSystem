using System;
using System.Threading.Tasks;
using UnityEngine;

public enum WxAudioPlayType : byte {
	First,
	All,
	Random,
	Last,
}

public enum WxAudioSpace {
	Screen,
	World,
}

[Serializable]
public struct WxAudioConfig {
	public WxAudioSpace space;
	public WxAudioPlayType playType;

	public float volume;
	public Vector2 pitchRange;

	public WxAudioConfig(WxAudioConfig src) {
		space		= src.space;
		playType	= src.playType;
		volume		= src.volume;
		pitchRange	= src.pitchRange;
	}
	public WxAudioConfig(WxAudioSpace space_ = WxAudioSpace.Screen ,WxAudioPlayType playType_ = WxAudioPlayType.First, float volume_ = 1, float minPitch_ = 1,float maxPitch_ = 1) {
		space			= space_;
		playType		= playType_;
		volume			= volume_;
		pitchRange.x	= minPitch_;
		pitchRange.y	= maxPitch_;
	}
}


[DisallowMultipleComponent]
[RequireComponent(typeof(AudioSource))]
public class WxAudioSource : MwComponent {
	public AudioSource audioSource;

	private void Awake() {
		gameObject.TryGetComponentWithChecking(out audioSource);
	}

	//single
	public static void PlayClipAtPoint(AudioClip clip, Vector3 point, float volume = 1f) {
		if (clip == null) return;
		_PlayClipAtPoint(clip, point, volume);
	}

	//Multi
	public static void PlayClipsAtPoint(AudioClip[] clips, Vector3 point, WxAudioPlayType playType = WxAudioPlayType.All, float volume = 1f) {

		if(clips == null || clips.Length < 1) return;

		switch (playType) {
			case WxAudioPlayType.First: { if(clips[0]) _PlayClipsAtPoint(point, volume, clips[0]); } break;
			case WxAudioPlayType.All: { _PlayClipsAtPoint(point, volume, clips); } break;
			case WxAudioPlayType.Random: {
					int idx = UnityEngine.Random.Range(0, clips.Length);
					_PlayClipsAtPoint(point, volume,clips[idx]);
				}

				break;
			case WxAudioPlayType.Last: { _PlayClipsAtPoint(point, volume,clips[clips.Length - 1]); } break;
			default: break;
		}
	}

	public void Play(AudioClip clip,float volume_ = -1f,bool loop_ = false){
		if (audioSource) {
			audioSource.loop = loop_;
			if(volume_ >= 0) audioSource.volume = volume_;
			audioSource.clip = clip;
			audioSource.Play();
		}
	}

	public void PlayOneShot(AudioClip clip,float minPitch = 1f,float maxPitch = 1f, float volume = 1) {
		if (audioSource) {
			var pitch = minPitch == 1f && maxPitch == 1f ? 1f : UnityEngine.Random.Range(minPitch, maxPitch);

			audioSource.volume = volume;
			audioSource.pitch = pitch;
			audioSource.PlayOneShot(clip);
		}
	}
	public void PlayClipsOneShot(float minPitch = 1f, float maxPitch = 1f, float volume = 1f, params AudioClip[] clips ) {
		if (audioSource) {
			var pitch = minPitch == 1 && maxPitch == 1 ? 1f : UnityEngine.Random.Range(minPitch, maxPitch);
			audioSource.volume = volume;
			audioSource.pitch = pitch;
			_PlayClips2D(clips);
		}
	}
	public void PlayClipsOneShot(WxAudioConfig config, params AudioClip[] clips) {
		if (audioSource) {
			var pitch = config.pitchRange.x == 1f && config.pitchRange.y == 1f ? 1f : UnityEngine.Random.Range(config.pitchRange.x, config.pitchRange.y);
			audioSource.volume = config.volume;
			audioSource.pitch = pitch;
			_PlayClips2D(clips);
		}
	}

	#region PrivateFunc

	//Unity Default
	static void _PlayClipAtPoint(AudioClip clip, Vector3 position, [UnityEngine.Internal.DefaultValue("1.0F")] float volume ) {
		GameObject gameObject = new GameObject("MwOne shot audio");
		gameObject.transform.position = position;
		AudioSource audioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
		audioSource.clip = clip;
		audioSource.spatialBlend = 1f;
		audioSource.volume = volume;
		audioSource.Play();
		Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
	}

	static void _PlayClipsAtPoint(Vector3 point, float volume = 1, params AudioClip[] clips_) {
		for (int i = 0; i < clips_.Length; i++) {
			if (!clips_[i]) continue;
			_PlayClipAtPoint(clips_[i], point, volume);
		}
	}
	void _PlayClips2D(params AudioClip[] clips_) {
		for (int i = 0; i < clips_.Length; i++) {
			if (!clips_[i]) continue;
			audioSource.PlayOneShot(clips_[i]);
		}
	}

	//Pooled Version
	static void PlayAtPointPooled() {
		//MwObjectPoolManager.GetPooledObject();
	}

	#endregion

}
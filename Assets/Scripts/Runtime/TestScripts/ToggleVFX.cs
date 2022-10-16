using UnityEngine;
using UnityEngine.VFX;

public class ToggleVFX : MonoBehaviour {

	VisualEffect _vfx;
	bool _enable;

	public enum VFXPlayType { Invoke, Toggle }
	[SerializeField] bool holdInvoke = false;
	public VFXPlayType playtype;
	public uint emitCount => _vfx.GetParticleSystemInfo(ShaderPropertyUtil.bulletliveCount).aliveCount;


	private void Awake() {
		_vfx = GetComponent<VisualEffect>();
	}

	void Start() {
		_vfx.Stop();
	}

	void Update() {
#if ENABLE_LEGACY_INPUT_MANAGER

		if (holdInvoke) {
			if (Input.GetKey(KeyCode.Space)) {
				Invoke();
			}
		} else {
			if (Input.GetKeyDown(KeyCode.Space)) {
				switch (playtype) {
					case VFXPlayType.Invoke: Invoke(); break;
					case VFXPlayType.Toggle: Toggle(); break;
					default: break;
				}
			}
		}

#endif
	}

	void Invoke() {
		_vfx.SendEvent(ShaderPropertyUtil.emitEvent);
	}

	void Toggle() {
		_enable = !_enable;

		if (!_enable) {
			_vfx.Play();
		} else {
			_vfx.Stop();
		}
	}
}

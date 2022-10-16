using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleAimController : MonoBehaviour {

	Vector2 angle;
	[SerializeField] TextMeshProUGUI _debugText;
	ToggleVFX[] vfxs;
	private void Awake() {
		vfxs = FindObjectsOfType<ToggleVFX>();
	}

	void Update() {
		angle.y = Input.GetAxis("Horizontal");
		angle.x = Input.GetAxis("Vertical") * -1f;

		transform.eulerAngles += angle.xy0();

		uint count = 0;
		for (int i = 0; i < vfxs.Length; ++i) {
			count += vfxs[i].emitCount;
		}

		if(_debugText)_debugText.text = $"FPS: {Mathf.Ceil(1f/Time.deltaTime)}\nAliveBullets: {count}";
	}
}

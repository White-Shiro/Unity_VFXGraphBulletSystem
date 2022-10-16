using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour {

	[SerializeField]	MaterialPropertyBlock block;
	MeshRenderer		meshRdr;
	[SerializeField]	Color color;
	[SerializeField]	Texture2D tex;

	[SerializeField] bool useHash = true;

	public static readonly int baseColorID = Shader.PropertyToID("_BaseColor");
	public static readonly int baseMapID = Shader.PropertyToID("_BaseMap");


	void Start() {
		block   = new MaterialPropertyBlock();
		meshRdr = GetComponent<MeshRenderer>();
	}

	private void OnValidate() {
		if (Application.isPlaying) {
			//SetColor();
			//meshRdr.GetPropertyBlock(block);
			if(meshRdr)
			meshRdr.material.color = color;
		}
	}

	private void SetColor() {
		block?.SetColor(baseColorID, color);
		block?.SetTexture(baseMapID, tex);
	}
}

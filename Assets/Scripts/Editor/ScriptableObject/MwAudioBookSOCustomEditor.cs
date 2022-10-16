using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(MwAudioBookSO), true)]
public class MwAudioBookSOCustomEditor : Editor {

	MwAudioBookSO bookSO;
	public override void OnInspectorGUI() {

		Color bgColor = new Color(0.2f, 0.4f, 0.4f);

		bookSO = (MwAudioBookSO)target;
		GUILayout.Label("Select Audio Category and Update the Entries");

		GUI.backgroundColor = bgColor;

		var btnColor = new Color(0.4f,0.8f,0.6f);
		GUI.backgroundColor = btnColor;

		if (GUILayout.Button("Update Entries", GUILayout.Height(50f))) {

			Undo.RecordObject(target, "Update Entries");
			bookSO.ResetEntries();
		}

		GUI.backgroundColor = bgColor;
		base.OnInspectorGUI();
	}

}

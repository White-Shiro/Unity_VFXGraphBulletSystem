using System;
using System.Collections.Generic;
using UnityEngine;
using MechSEID = MwAudioID.Common_Mech;

[CreateAssetMenu(fileName = "_MechSfxIDConfig", menuName = "Data/Audio/MechSeIDConfig")]
[Serializable]
public class MwMechSEConfigSO : ScriptableObject {

	[Header("LocoMotion")]
	public MechSEID SE_footStepID;
	public MechSEID SE_dashEnterID;
	public MechSEID SE_dashLoopID;
	public MechSEID SE_dashRecoilID;
	public MechSEID SE_landID;
	public MechSEID SE_JumpEnterID;

	[Header("Stat")]
	public MechSEID SE_recoverID;
}

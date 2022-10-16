using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "_MechVfxIDConfig", menuName = "Data/VFX/MechVfxIDConfig")]
public class MwMechVFXconfigSO : ScriptableObject {

    [Header("LocoMotion")]
    public byte footStepID;
    public byte dashEnterID;
    public byte dashLoopID;
    public byte dashRecoilID;
    public byte landID;
    public byte JumpEnterID;

    [Header("Stat")]
    public byte recoverID;

}


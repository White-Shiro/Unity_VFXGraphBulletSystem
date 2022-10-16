using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class MwWeaponSO : ScriptableObject {
    [field:SerializeField] public MwDamage damage { get; protected set;}
}


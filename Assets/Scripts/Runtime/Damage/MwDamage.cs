using UnityEngine;
public enum MwDamageType {
    None,
    Physical,
    Explosive,
    Energy,
}

[System.Serializable]
public struct MwDamage {
    public float damage;
    public MwDamageType damageType;
}
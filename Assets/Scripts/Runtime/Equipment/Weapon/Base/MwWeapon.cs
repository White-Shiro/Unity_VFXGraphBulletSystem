using UnityEngine;

public enum MwWeaponType {
	None,
	Ranged,
	Melee,
}

[System.Flags]
public enum MwFireMode {
	Single		= 1<<0,
	FullAuto	= 1<<1,
	Burst		= 1<<2,
}

public abstract class MwWeapon : MwEquipment {

	[SerializeField] [WxShowOnly]	protected MwEquipmentType _equipmentType = MwEquipmentType.Weapon;
	[SerializeField]				protected MwMountingType  _mountingType;

	public override MwEquipmentType equipmentType	=> _equipmentType;
	public override MwMountingType	mountingType	=> _mountingType;
	public abstract	MwWeaponType	weaponType { get; }

    //protected override void OnTriggerOnce() {}
}
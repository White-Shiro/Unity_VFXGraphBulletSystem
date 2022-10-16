using System;
using UnityEngine;
public enum MwMountingType {
	None,
	Main,
	Sub,
	BackMount,
	ShoulderR,
	ShoulderL,
	Hidden,
}

public enum MwEquipmentType {
	None,
	Weapon,
	Device,
}


public abstract class MwEquipment : MwComponent {

    public		bool enableTrigger = true;
    public		abstract MwEquipmentType equipmentType { get; }
    public		abstract MwMountingType mountingType { get; }
	public		void TriggerOnce(Vector3 aimPos)	{ if(enableTrigger) OnTriggerOnce(aimPos);}
	public		void TriggerHold(Vector3 aimPos)	{ if(enableTrigger) OnTriggerHold(aimPos);}
	public		void TriggerRelease(Vector3 aimPos)	{ if(enableTrigger) OnTriggerRelease(aimPos);}
	public		void TriggerOnce()					{ if(enableTrigger) OnTriggerOnce();}
	public		void TriggerHold()					{ if(enableTrigger) OnTriggerHold();}
	public		void TriggerRelease()				{ if(enableTrigger) OnTriggerRelease();}

	protected	virtual void OnTriggerOnce(Vector3 aimPos) { }
	protected	virtual void OnTriggerHold(Vector3 aimPos) { }
	protected	virtual void OnTriggerRelease(Vector3 aimPos) { }

	protected	virtual void OnTriggerOnce() { }
	protected	virtual void OnTriggerHold() { }
	protected	virtual void OnTriggerRelease() { }

}
[Serializable]
public class MwEquipmentSlot {

	[WxShowOnly] public int slotIndex;
	public MwMountingType mountingType;
	public Transform equipHolder;
	public Transform mountingHolder;

	[WxShowOnly] public MwEquipment currentEquipment = null;
	public bool isOccupied => currentEquipment != null;

	public void Equip() {
		if (!currentEquipment) return;

		if (equipHolder) {
			currentEquipment.transform.SetParentReset(equipHolder, true);
		}
	}

	public void Mount() {
		if (!currentEquipment) return;
		if (mountingHolder) {
			currentEquipment.transform.SetParentReset(mountingHolder, true);
		}
	}

	public void Equip(MwEquipment src_) {
		if (src_ == null) {
			UnEquip();
			return;
		}

		if (currentEquipment != src_) {
			UnEquip();
		}

		if (equipHolder) {
			src_.transform.SetParentReset(equipHolder, true);
		}

		currentEquipment = src_;
	}

	public void Mount(MwEquipment src_) {
		if (src_ == null) {
			UnEquip();
			return;
		}

		if (currentEquipment != src_) {
			UnEquip();
		}

		if (mountingHolder) {
			src_.transform.SetParentReset(mountingHolder, true);
		}

		currentEquipment = src_;
	}

	public void UnEquip() {
		if (currentEquipment) {
			currentEquipment.transform.SetParentReset(null, false);
		}

		currentEquipment = null;
	}

	public bool SetEquipmentActive(bool active) {
		if (active) {
			Equip();
			return true;
		} else {
			Mount();
			return false;
		}
	}


	//TriggerOnce
	public void TriggerEquipment() {
		if (!currentEquipment) return;
		currentEquipment.TriggerOnce();
	}

	public void TriggerEquipment(Vector3 aimPos) {
		if (!currentEquipment) return;
		currentEquipment.TriggerOnce(aimPos);
	}

	//TriggerHold
	public void TriggerHoldEquipment(Vector3 aimPos) {
		if (!currentEquipment) return;
		currentEquipment.TriggerHold(aimPos);
	}

	public void TriggerHoldEquipment() {
		if (!currentEquipment) return;
		currentEquipment.TriggerHold();
	}

	//TriggerRelease
	public void TriggerReleaseEquipment(Vector3 aimPos) {
		if (!currentEquipment) return;
		currentEquipment.TriggerRelease(aimPos);
	}

	public void TriggerReleaseEquipment() {
		if (!currentEquipment) return;
		currentEquipment.TriggerRelease();
	}

}
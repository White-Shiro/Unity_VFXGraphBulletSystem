using System;
using System.Collections.Generic;
using UnityEngine;

public class MwCharacterEquipmentComponent : MwCharacterComponent {

	[SerializeField] [WxShowOnly] int _currentSlotIndex = 0;
	[SerializeField] List<MwEquipmentSlot> _slots = new List<MwEquipmentSlot>();

	[SerializeField] MwEquipment defaultMainWeapon = null;
	[SerializeField] MwEquipment defaultSubWeapon = null;

	//Properties
	public IReadOnlyList<MwEquipmentSlot> slots => _slots;

	protected override void OnConstruct(MwCharacter character_) {

		if (defaultMainWeapon) {
			defaultMainWeapon = Instantiate(defaultMainWeapon);
			Mount(defaultMainWeapon);
		}

		if (defaultSubWeapon) {
			defaultSubWeapon = Instantiate(defaultSubWeapon);
			Mount(defaultSubWeapon);
		}
	}

	public void SwtichMain() => SwitchSlot(1);
	public void SwtichSub() => SwitchSlot(2);
	protected void SwitchSlot(int slotIndex) {
		_currentSlotIndex = SetActiveSlot(slotIndex);
	}

	/// <summary> Handles Slot Status Checking and toggle Equipment Holder </summary>
	/// <returns>Target Slot Index if succeed </returns>
	protected int SetActiveSlot(int slotIndex) {
		if (_slots == null) return _currentSlotIndex;

		if (slotIndex != 0 && !HasEquipment(slotIndex)) {
			WxLogger.Log($"Slot{slotIndex} has no Equipment");
			return _currentSlotIndex;
		}

		for (int i = 0; i < _slots.Count; ++i) {
			if (_slots[i] == null) continue;

			var activeState = i.Equals(slotIndex);
			_slots[i].SetEquipmentActive(activeState);
		}

		return slotIndex;
	}

	public void TriggerEquipment() {
		if(_slots == null || _slots[_currentSlotIndex] == null) return;
		_slots[_currentSlotIndex].TriggerEquipment(character.playerController.aimTarget);
	}

	public void TriggerHoldEquipment() {
		if (_slots == null || _slots[_currentSlotIndex] == null) return;
		_slots[_currentSlotIndex].TriggerHoldEquipment(character.playerController.aimTarget);
	}

	public void TriggerReleaseEquipment() {
		if (_slots == null || _slots[_currentSlotIndex] == null) return;
		_slots[_currentSlotIndex].TriggerReleaseEquipment(character.playerController.aimTarget);
	}

	/// <returns>True if slot is not null and is occupied by a MwEquipment </returns>
	public bool HasEquipment(int slotIdx) {
		if (_slots == null) return false;
		return _slots[slotIdx] != null && _slots[slotIdx].isOccupied;
	}

	public void Equip(MwEquipment equipment, MwMountingType type = MwMountingType.None) {
		if(!equipment)		return;
		if(_slots == null)	return;

		var mountingType = type == MwMountingType.None? equipment.mountingType : type;

		for (int i = 0; i < _slots.Count; ++i) {
			if (_slots[i] == null) continue;
			if (_slots[i].isOccupied) continue;

			if (_slots[i].mountingType == mountingType) {
				_slots[i].Equip(equipment);

				//OnEquipementEquip?.invoke(moutingType);
				return;
			}
		}

		WxLogger.Log($"Not Available Slots for {equipment.name}");
	}

	public void Mount(MwEquipment equipment, MwMountingType type = MwMountingType.None) {

		if (!equipment)		return;
		if (_slots == null) return;

		var mountingType = type == MwMountingType.None ? equipment.mountingType : type;

		for (int i = 0; i < _slots.Count; ++i) {
			if (_slots[i] == null) continue;
			if (_slots[i].isOccupied) continue;

			if (_slots[i].mountingType == mountingType) {
				_slots[i].Mount(equipment);

				//OnEquipementMount?.invoke(moutingType);
				return;
			}
		}

		WxLogger.Log($"No Available Slots for {equipment.name}");
	}

	private void OnValidate() {
		if (_slots.Count < 1) return;
		for (int i = 0; i < _slots.Count; ++i) {
			_slots[i].slotIndex = i;
		}
	}



}

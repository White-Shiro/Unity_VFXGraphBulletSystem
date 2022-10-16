using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class MwMechCharacter : MwCharacter {

	//Conatains Equipment Control Logic
	public MwCharacterEquipmentComponent equipComp { get; protected set;}
	public MwMechEffectComponent mechEffectComp  => effectComp as MwMechEffectComponent;
	public MwMechAnimationComponent mechAnimComp => animComp   as MwMechAnimationComponent;

	protected override void OnConstruct() {
		equipComp = gameObject.GetComponentWithChecking<MwCharacterEquipmentComponent>();
	}
}
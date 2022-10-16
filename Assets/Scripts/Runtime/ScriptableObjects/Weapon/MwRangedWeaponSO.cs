using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MwRangedWeaponData", menuName = "Data/Weapon")]
public class MwRangedWeaponSO : MwWeaponSO {

	[Header("Weapon")]
	[field: SerializeField] [Min(0)] float fireRPM		= 50f;
	[field: SerializeField] public MwFireMode defaultFireMode	= MwFireMode.Single;
	[field: SerializeField] public MwFireMode fireModes			= MwFireMode.Single;

	[Header("Accuracy")]
	[field: SerializeField] public float recoilForce = 0f;
	[field: SerializeField] public float spreadRate  = 0.5f;
	[field: SerializeField] [Range(0f,1f)]public float spreadAddRate = 0.1f;
	[SerializeField] AnimationCurve _spreadCurve;

	[Header("Reload")]
	[field: SerializeField] int maxMagazineAmmo = 20;
	[field: SerializeField] int maxCarryAmmo    = 200;

	[field: SerializeField] float ReloadSpeed   = 5f;

	[Header("Bullet")]
	[field: SerializeField] public float bulletSpeed = 100f;
	[field: SerializeField] public float bulletDropRate = 0f;
	[field: SerializeField] public float lifeTime = 3f;

	[field: SerializeField] public GameObject bulletPrefab;

	[SerializeField] protected List<GameObject> _hitEffectPrefab;
	public IReadOnlyList<GameObject> hitEffectsPrefab => _hitEffectPrefab;
    public float GetEvaluatedSpreadWeight(float time) => spreadRate * _spreadCurve.Evaluate(time);
    public float fireInterval => 60f / fireRPM ;

    [Header("SoundEffect")]
	[field: SerializeField] public AudioClip[] fireSE;
	[field: SerializeField] public WxAudioConfig fireSEConfig = new WxAudioConfig(WxAudioSpace.Screen, WxAudioPlayType.First, 1, 1, 1);
}

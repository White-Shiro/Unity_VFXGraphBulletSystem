using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Threading.Tasks;

public class MwRangeWeapon : MwWeapon {

	[WxShowOnly][SerializeField] protected MwWeaponType _weaponType = MwWeaponType.Ranged;
	public override MwWeaponType weaponType => _weaponType;

	[SerializeField] MwRangedWeaponSO _config;

	[SerializeField] [WxShowOnly] MwFireMode _currentFireMode = 0;

	bool _isFiring = false;
	float _nextFireTime = 0;
	float _spreadWeight	= 0;
	[SerializeField] Vector3 _firePointOffSet;

	Vector3 _firePoint => transform.TransformPoint(_firePointOffSet);

	private void Awake() {
		if(_config) _currentFireMode = _config.defaultFireMode;
	}

	protected override void OnTriggerOnce(Vector3 aimPos) {
		switch (_currentFireMode) {
			case MwFireMode.Single: SingleFire(aimPos); break;
			case MwFireMode.Burst : BurstFire(aimPos,_config.fireInterval); break;
			default: break;
		}
	}

	protected override void OnTriggerHold(Vector3 aimPos) {
		if (_currentFireMode == MwFireMode.FullAuto) {
			FullAutoFire(aimPos);
		}
	}

	protected override void OnTriggerRelease(Vector3 aimPos) {
		_isFiring		= false;
		_spreadWeight			= 0f;
	}

	void FireBullet(Vector3 aimPos) {
		if (!_config) return;
		if (!_config.bulletPrefab) return;
		if (Time.time < _nextFireTime) return;

		//SpawnBullet
		var bulletGO = MwObjectPoolManager.GetPooledObject(_config.bulletPrefab).GetComponent<MwBullet>();

		if (!bulletGO)	return;
		var bullet = bulletGO.GetComponent<MwBullet>();
		if (!bullet)	return;

		var spread = UnityEngine.Random.insideUnitCircle * _config.GetEvaluatedSpreadWeight(_spreadWeight);

		var spreadRot = Quaternion.Euler(spread.x,spread.y,0);
		var bulletVelocity = spreadRot * ((aimPos) - _firePoint).normalized;

		bullet.InitializeBullet(_firePoint, bulletVelocity * _config.bulletSpeed, _config);

		//Test SE
		MwAudioManager.PlayOneShot(_config.fireSEConfig,_firePoint,_config.fireSE);
		

		_isFiring = true;

		_spreadWeight += _config.spreadAddRate;
		_spreadWeight.ClampMax(1);
		_nextFireTime = Time.time + _config.fireInterval;
	}
	protected virtual void SingleFire(Vector3 aimPos) {
		FireBullet(aimPos);
	}

	protected virtual void FullAutoFire(Vector3 aimPos) {
		FireBullet(aimPos);
	}

	protected async void BurstFire(Vector3 aimPos, float interval) {

		//Have to Cache current Player Aimming Direction

		FireBullet(aimPos);
		await Task.Delay((int)(interval * 1000));
		FireBullet(aimPos);
		await Task.Delay((int)(interval * 1000));
		FireBullet(aimPos);
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected() {
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(_firePoint, 0.05f);
#endif

	}


}
using UnityEngine;

public class MwBullet : MwProjectile {
	protected MwDamage _damage;
	public void InitializeBullet(Vector3 spawnPos_, Vector3 velocity_, MwRangedWeaponSO config,System.Action callback = null) {
		_damage = config.damage;
		Initialize(spawnPos_, velocity_, config.bulletDropRate, config.lifeTime,callback);
	}
}
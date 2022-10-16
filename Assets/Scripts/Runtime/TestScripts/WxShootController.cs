using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Cinemachine;
using System;

public class WxShootController : MonoBehaviour {

	StarterAssets.StarterAssetsInputs inputs;


	Camera _camera = null;

	Transform _camTrans = null;

	[SerializeField] float _speed = 50f;
	[SerializeField]float _bulletlifeTime = 5f;

	[SerializeField] float shootIntervel =0.05f;
	float nextShootTime = 0;
	[SerializeField] Transform[] _shootPoint;

	[SerializeField] LayerMask _rayCastLayer = Physics.DefaultRaycastLayers;

	[SerializeField] Transform shootPt;
	[SerializeField] CinemachineImpulseSource recoilShakeSrc;

	public enum ShootMethod {
		Hitscan,
		Projectile,
		ProjectileVFX,
	}

	public ShootMethod shootMethod = ShootMethod.Hitscan;
	[SerializeField] bool _multipleShoot;

	void Start() {
		TryGetComponent(out inputs);

		if (Camera.main) {
			_camera = Camera.main;
			_camTrans = _camera.transform;
		}
	}

	private static readonly Vector3 viewPortPoint = new Vector3(0.5f,0.5f,0f);

	void Update() {
		Shoot(inputs.shoot);
	}

	private void Shoot(bool pressed) {
		if(!pressed) return;
		if(Time.time < nextShootTime) return;

		if (!_camera || !_camTrans) { return;}
		if(_shootPoint == null) { return;}

		var ray = _camera.ViewportPointToRay(viewPortPoint);
		var isHitting = Physics.Raycast(ray,out RaycastHit hit, 100000f, _rayCastLayer.value);

		switch (shootMethod) {
			case ShootMethod.Projectile:  ShootProjectile(in hit,isHitting); break;
			case ShootMethod.Hitscan: ShootHitScan(in hit); break;
			case ShootMethod.ProjectileVFX: ShootProtileVFX(in hit); break;
		}


		nextShootTime = Time.time + shootIntervel;
	}

	private void ShootProtileVFX(in RaycastHit hit) {

		if (_multipleShoot) {
			foreach (var item in _shootPoint) {
				WxBulletManager.DrawVFXProjectile(in hit, item.position,_speed);
			}
		} else {
			WxBulletManager.DrawVFXProjectile(in hit, shootPt.position, _speed);
		}

		if (recoilShakeSrc) {
			recoilShakeSrc.GenerateImpulse();
		}
	}

	void ShootHitScan(in RaycastHit hit) {

		if (_multipleShoot) {
			foreach (var item in _shootPoint) {
				WxBulletManager.DrawBulletLine(in hit, item.position);
			}
		} else {
			WxBulletManager.DrawBulletLine(in hit, shootPt.position);
		}

		if (recoilShakeSrc) {
			recoilShakeSrc.GenerateImpulse();
		}
	}

	private void ShootProjectile(in RaycastHit hit, bool isHitting) {
		for (int i = 0; i < _shootPoint.Length; ++i) {
			if (!_shootPoint[i]) continue;

			Vector3 bulletVel = isHitting ? (hit.point - _shootPoint[i].position).normalized : _shootPoint[i].forward;
			bulletVel *= _speed;

			WxBulletManager.SpawnBullet(_shootPoint[i].position,
										_camTrans.localRotation,
										bulletVel,
										_bulletlifeTime
										);
		}
	}
}

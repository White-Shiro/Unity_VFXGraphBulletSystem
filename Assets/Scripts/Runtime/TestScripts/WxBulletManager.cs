using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.VFX;

[Serializable]
public class WxBullet {
	public Vector3 pos;
	public Quaternion rot;
	public float lifeTime = 0f;
	public Vector3 velocity;
	public float mag;

	public WxBullet(Vector3 pos_ = default, Quaternion rot_ = default, Vector3 velocity_ = default, float lifeTime_ = 0f) {
		pos = pos_;
		rot = rot_;
		velocity = velocity_;
		lifeTime = lifeTime_;
		mag = velocity.magnitude;
	}
}

public class WxBulletManager : MonoBehaviour {

	static WxBulletManager _instance = null;

	[SerializeField] static List<List<WxBullet>> _bulletList	= new();
	[SerializeField] static List<List<Matrix4x4>>_batches		= new();

	[SerializeField] Text debugText;

	//Can Only Draw 1023 Meshes per Batch

	[SerializeField] Mesh _bulletMesh;
	[SerializeField] Material _bulletMat;
	[SerializeField] ShadowCastingMode _castingMode = ShadowCastingMode.Off;
	[SerializeField] bool _recieveShadow = false;
	[SerializeField] Vector3 offset;

	[SerializeField] ParticleSystem _hitEffect = null;
	Transform _hitTrans;

	[SerializeField] VisualEffect hitScanVfx;
	VFXEventAttribute hitScanAttrbute;

	[SerializeField] VisualEffect projectileVfx;
	VFXEventAttribute projectileAttrbute;
	[SerializeField] Texture2D aliveMap;
	Color aliveC	= new Color(1,0,0);
	Color deadC		= new Color(0,0,0);

	int bulletCount = -1;
	bool mapIsDirty = false;

	public static WxBulletManager instance {
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<WxBulletManager>();
				if (!_instance) _instance = new GameObject("BulletManager", typeof(WxBulletManager)).GetComponent<WxBulletManager>();
			}

			return _instance;
		}
	}

	private void Awake() {
		if (!_instance) _instance = this;
		else Destroy(gameObject);

		_bulletList.Add(new());
		_batches.Add(new());

		if(hitScanVfx) hitScanAttrbute			= hitScanVfx.CreateVFXEventAttribute();
		if(projectileVfx) projectileAttrbute	= projectileVfx.CreateVFXEventAttribute();
		InitAliveMap(ref aliveMap);

		if (_hitEffect) _hitTrans = _hitEffect.transform;
	}

	private void InitAliveMap(ref Texture2D map) {
		
		var mapSize = 1024;
		map = new Texture2D(mapSize, 1,TextureFormat.R8,false);

		for (int i = 0; i < mapSize; ++i) {
			map.SetPixel(i,1, new Color(0, 0, 0));
		}

		map.Apply();
	}

	private void ApplyAliveMap() {
		if(!mapIsDirty) return;

		aliveMap.Apply();
		projectileVfx.SetTexture(ShaderPropertyUtil.aliveMapAtt, aliveMap);
		mapIsDirty = false;
	}

	void SetBulletAliveMap(int id ,bool alive) {
		aliveMap.SetPixel(id,1,alive? aliveC: deadC);
	}


	void Update() {
		if (_bulletList == null) return;

		var count = _bulletList.Count;
		for (int i = 0; i < count; ++i) {

			var _subBulletList = _bulletList[i];
			var _subMat4List = _batches[i];

			var subListCount = _subBulletList.Count;
			for (int j = 0; j < subListCount; ++j) {

				var bullet = _subBulletList[j];
				if (bullet == null) continue;

				//SimBullet;

				Ray ray = new Ray(bullet.pos, bullet.velocity);
				bullet.pos += bullet.velocity * Time.deltaTime;
				bullet.lifeTime -= Time.deltaTime;

				//Translate Matrix
				//_batch[i].SetTRS(bullet.pos, bullet.rot, Vector3.one);
				_subMat4List[j] = Matrix4x4.TRS(bullet.pos, bullet.rot, Vector3.one);

				if (Physics.Raycast(ray, out RaycastHit hit, bullet.mag)) {
					bullet.lifeTime = 0;
					if (_hitEffect) {
						_hitTrans.position = hit.point;
						_hitTrans.forward = hit.normal;
						_hitEffect.Emit(1);
					}
				}


				if (bullet.lifeTime < 0) {
					_bulletList[i].RemoveAt(j);
					_batches[i].RemoveAt(j);
					j--;
					subListCount--;
				}
			}

			//DoRaycast;
		}

		if (_bulletMesh && _bulletMat) {
			for (int i = 0; i < _batches.Count; ++i) {
				Graphics.DrawMeshInstanced(_bulletMesh, 0, _bulletMat, _batches[i], null, _castingMode, _recieveShadow);
			}
		}

/*
		if (debugText) {

			int allInstanceCount = 0;

			for (int i = 0; i < _bulletList.Count; i++) {
				allInstanceCount += _bulletList[i].Count;
			}


			bool isCastShadow = _castingMode == ShadowCastingMode.On;
			debugText.text = $"Alive Bullets: {allInstanceCount} \n CastShadow: {isCastShadow}";
		}*/
	}

	private void LateUpdate() {
		ApplyAliveMap();
	}


	public static void SpawnBullet(Vector3 pos, Quaternion rot, Vector3 vel, float lifeTime) {

		var adjustedRot = rot * Quaternion.Euler(_instance.offset);
		Matrix4x4 bulletMat4 = Matrix4x4.TRS(pos, adjustedRot, Vector3.one);
		var bullet = new WxBullet(pos, adjustedRot, vel, lifeTime);

		var listCount = _bulletList.Count;

		for (int i = 0; i < listCount; ++i) {


			if (_bulletList[i].Count >= 1023) {
				if (i == _bulletList[i].Count) {

					Debug.Log("Add new List");

					_bulletList.Add(new());
					_batches.Add(new());

					_bulletList[i].Add(bullet);
					_batches[i].Add(bulletMat4);
					listCount++;
				}
			} else {
				_bulletList[i].Add(bullet);
				_batches[i].Add(bulletMat4);
				break;
			}

			Debug.Log(listCount);
		}
	}

	public static void EmitHitFX(in RaycastHit hit) {

		if (_instance._hitEffect) {
			_instance._hitTrans.position = hit.point;
			_instance._hitTrans.forward = hit.normal;
			_instance._hitEffect.Play(true);
		}
	}

	public static void DrawVFXProjectile(in RaycastHit hit, in Vector3 startPos,float speed) {

		var attribute	= _instance.projectileAttrbute;
		var vfx			= _instance.projectileVfx;
		var dir = hit.point - startPos;
		var mag = dir.magnitude;
		var vel = dir.SafeDivide(mag) * speed;
		var etaTime = mag / speed;

		++_instance.bulletCount;

		if(!_instance.mapIsDirty) _instance.mapIsDirty = true;

		var id =  _instance.bulletCount % 1024 - 1;
		_instance.SetBulletAliveMap(id, true);

		attribute.SetFloat(ShaderPropertyUtil.spawnCountAtt, 1f);
		attribute.SetVector3(ShaderPropertyUtil.startPosAtt, startPos);
		attribute.SetVector3(ShaderPropertyUtil.velocityAtt, vel);
		attribute.SetFloat(ShaderPropertyUtil.lifeTimeAtt, etaTime);
		vfx.SendEvent(ShaderPropertyUtil.emitEvent, _instance.projectileAttrbute);
		_instance.ShowModNum(id);

	}
	void ShowModNum(int id) {
		if (debugText) {
			debugText.text = $"Bullets Count: {bulletCount} | lastIDset = {id}";
		}
	}




	public static void DrawBulletLine(in RaycastHit hit, in Vector3 startPos) {

		_instance.hitScanAttrbute.SetFloat(ShaderPropertyUtil.spawnCountAtt,1f);
	//VFXGraph cannot handle multiple spawnEvent per frame. Use Direct Link to byPass SpawnContext instead
	//https://forum.unity.com/threads/new-feature-direct-link.1137253/

		_instance.hitScanAttrbute.SetVector3(ShaderPropertyUtil.startPosAtt, startPos);
		_instance.hitScanAttrbute.SetVector3(ShaderPropertyUtil.targetPosAtt, hit.point);
		_instance.hitScanVfx.SendEvent(ShaderPropertyUtil.emitEvent, _instance.hitScanAttrbute);

		EmitHitFX(in hit);
	}



}

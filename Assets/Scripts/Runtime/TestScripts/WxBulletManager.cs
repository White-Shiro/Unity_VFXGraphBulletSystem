using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEditor;




public class WxBulletManager : MonoBehaviour {

	static WxBulletManager _instance = null;

	[SerializeField] Text debugText;

	[SerializeField] ParticleSystem _hitEffect = null;
	Transform _hitTrans;

	//hitScanVFX
	[SerializeField] VisualEffect hitScanVfx;
	VFXEventAttribute hitScanAttrbute;

	//ProjectileVFX
	[SerializeField] VisualEffect projectileVfx;
	VFXEventAttribute projectileAttrbute;
	[SerializeField] Texture2D aliveMap;
	Color aliveC = new Color(1, 0, 0);
	Color deadC = new Color(0, 0, 0);

	int bulletFiredCount = -1;
	bool mapIsDirty = false;

	List<WxBullet> _bulletList;

	[SerializeField] RawImage debugImg;


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

		Application.targetFrameRate = 60;

		if (!_instance) _instance = this;
		else Destroy(gameObject);

		_bulletLists_old.Add(new());
		_batches_old.Add(new());

		if (hitScanVfx) hitScanAttrbute = hitScanVfx.CreateVFXEventAttribute();
		if (projectileVfx) projectileAttrbute = projectileVfx.CreateVFXEventAttribute();
		InitAliveMap(ref aliveMap);

		_bulletList = new(512);

		if (_hitEffect) _hitTrans = _hitEffect.transform;
	}

	private void InitAliveMap(ref Texture2D map) {

		var mapSize = 10;
		map = new Texture2D(mapSize, 1, TextureFormat.R8,false,true);
		map.filterMode = FilterMode.Point;
		map.wrapMode = TextureWrapMode.Clamp;

		for (int i = 0; i < mapSize; ++i) {
			SetBulletAliveToMap(i,false);
		}

		map.Apply();
		projectileVfx.SetTexture(ShaderPropertyUtil.aliveMapAtt, aliveMap);
		debugImg.texture = map;
	}
	private void UpdateAliveMapTexture() {
		//if (!mapIsDirty) return;

		//var startTime = Time.realtimeSinceStartup;

		aliveMap.Apply(false, false);
		//projectileVfx.SetTexture(ShaderPropertyUtil.aliveMapAtt, aliveMap);
		mapIsDirty = false;

		//var endTime = Time.realtimeSinceStartup;
		//WxLogger.Warning((endTime - startTime) * 1000f);

	}

	void SetBulletAliveToMap(int id, bool alive) {
		var beforeC = aliveMap.GetPixel(id,0);
		aliveMap.SetPixel(id, 0, alive ? aliveC.linear : deadC.linear);
		var color = aliveMap.GetPixel(id,0);

		if(!alive)
		//WxLogger.Warning($"{id} before:{beforeC.r} | after: {color.r}");
		mapIsDirty = true;
		//aliveMap.Apply(false,false);
	}


	void Update() {
		Update_bullets();
		MatrixDrawMeshInstance();
	}

	void Update_bullets() {

		if(_bulletList == null || _bulletList.Count <= 0) return;

		var dTime = Time.deltaTime;

		foreach (var bullet in _bulletList ) {
			var alive = bullet.Simulate(dTime);
			SetBulletAliveToMap(bullet.id, alive);
		}


		//ReverseForLoop to Remove Bullets
		//for (int i = _bulletList.Count - 1; i >= 0 ; i--) {
		//	if (!_bulletList[i].alive) {
		//		_bulletList.RemoveAt(i);
		//	}
		//}


		//Predicate method
		_bulletList.RemoveAll(i => !i.alive);
	}



	private void LateUpdate() {
		UpdateAliveMapTexture();
		DebugShowBulletCount(_bulletList.Count);
	}

	public static void EmitHitFX(in RaycastHit hit) {

		if (_instance._hitEffect) {
			_instance._hitTrans.position = hit.point;
			_instance._hitTrans.forward = hit.normal;
			_instance._hitEffect.Play(true);
		}
	}

	public class WxBullet {

		public int id;
		public Vector3 position;
		public Vector3 velocity;
		public float lifeTime = 3;
		public float age = 0;
		public bool alive = true;

		public WxBullet(in CreateDesc desc) {
			id			= desc.id;
			position	= desc.startPos;
			velocity	= desc.velocity;
			lifeTime	= desc.etaTime;
		}

		public bool Simulate(float deltaTime) {
			if(!alive) return false;

			position += velocity * Time.deltaTime;
			age += deltaTime;

			if(age > lifeTime) {
				alive = false;
			}

			return alive;
		}

		public struct CreateDesc {

			public int    id;
			public Vector3 startPos;
			public Vector3 targetPos;

			public Vector3 velocity;

			public float distance;
			public float etaTime;

			public CreateDesc(int id,in RaycastHit target, in Vector3 startPos, float speed) {

				this.id = id;

				this.startPos = startPos;
				targetPos = target.point;

				var direction = target.point - startPos;
				distance = direction.magnitude;
				velocity = direction.SafeDivide(distance) * speed; //Unit Vector * speed
				etaTime = distance / speed;
			}

			public CreateDesc(int id,in Vector3 direction, in Vector3 startPos, float speed) {
				this.id = id;
				this.startPos = startPos;
				targetPos = startPos + direction;

				distance = direction.magnitude;
				velocity = direction.normalized * speed;

				etaTime = distance / speed;

			}
		}
	}

	public static void SpawnBullet(in RaycastHit target, in Vector3 shootPos, float speed) {
		++_instance.bulletFiredCount;
		var id = _instance.bulletFiredCount % (_instance.aliveMap.width);

		var bulletDESC = new WxBullet.CreateDesc(id,in target,in shootPos,speed);
		_instance.CreateBullet(in bulletDESC);
		_instance.DrawVFXProjectile(in bulletDESC);
	}

	private void CreateBullet(in WxBullet.CreateDesc bulletDESC) {
		var bullet = new WxBullet(in bulletDESC);
		_bulletList.Add(bullet);
		SetBulletAliveToMap(bulletDESC.id, true);
	}

	private void DrawVFXProjectile(in WxBullet.CreateDesc bulletDESC) {
		var attribute	= projectileAttrbute;
		var vfx			= projectileVfx;

		attribute.SetFloat(ShaderPropertyUtil.spawnCountAtt, 1f);
		attribute.SetVector3(ShaderPropertyUtil.startPosAtt, bulletDESC.startPos);
		attribute.SetVector3(ShaderPropertyUtil.velocityAtt, bulletDESC.velocity);
		//attribute.SetFloat(ShaderPropertyUtil.lifeTimeAtt, bulletDESC.etaTime);
		attribute.SetFloat(ShaderPropertyUtil.lifeTimeAtt, 10f);
		attribute.SetFloat(ShaderPropertyUtil.meshIdAtt, bulletDESC.id);
		vfx.SendEvent(ShaderPropertyUtil.emitEvent, attribute);
	}

	void DebugShowModNum(int id) {
		if (debugText) {
			debugText.text = $"Bullets Fired: {bulletFiredCount} lastIDset = {id}";
		}
	}

	void DebugShowBulletCount(int aliveCount) {
		if (debugText) {
			debugText.text = $"Bullets Fired: {bulletFiredCount} AliveBullets\nClass: {aliveCount} & VFX:{projectileVfx.aliveParticleCount}";
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

	void OnDrawGizmos() {

		if(!Application.isPlaying || _bulletList.Count <= 0 ) return;

		foreach (var bullet in _bulletList) {
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(bullet.position,0.1f);
			Handles.Label(bullet.position + Vector3.up *0.1f,bullet.id.ToString());
		}
	}

	#region MatrixDrawMeshInstance_approach 

	[SerializeField] static List<List<WxBullet_old>> _bulletLists_old = new();
	[SerializeField] static List<List<Matrix4x4>> _batches_old = new();

	//Can Only Draw 1023 Meshes per Batch

	[SerializeField] Mesh _bulletMesh;
	[SerializeField] Material _bulletMat;
	[SerializeField] ShadowCastingMode _castingMode = ShadowCastingMode.Off;
	[SerializeField] bool _recieveShadow = false;
	[SerializeField] Vector3 offset;

	private void MatrixDrawMeshInstance() {
		if (_bulletLists_old == null) return;

		var count = _bulletLists_old.Count;
		for (int i = 0; i < count; ++i) {

			var _subBulletList = _bulletLists_old[i];
			var _subMat4List = _batches_old[i];

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
					_bulletLists_old[i].RemoveAt(j);
					_batches_old[i].RemoveAt(j);
					j--;
					subListCount--;
				}
			}

			//DoRaycast;
		}

		if (_bulletMesh && _bulletMat) {
			for (int i = 0; i < _batches_old.Count; ++i) {
				Graphics.DrawMeshInstanced(_bulletMesh, 0, _bulletMat, _batches_old[i], null, _castingMode, _recieveShadow);
			}
		}
	}

	[Serializable]
	public class WxBullet_old {
		public Vector3 pos;
		public Quaternion rot;
		public float lifeTime = 0f;
		public Vector3 velocity;
		public float mag;

		public WxBullet_old(Vector3 pos_ = default, Quaternion rot_ = default, Vector3 velocity_ = default, float lifeTime_ = 0f) {
			pos = pos_;
			rot = rot_;
			velocity = velocity_;
			lifeTime = lifeTime_;
			mag = velocity.magnitude;
		}
	}
	public static void SpawnBullet_DrawMeshInstance(Vector3 pos, Quaternion rot, Vector3 vel, float lifeTime) {

		var adjustedRot = rot * Quaternion.Euler(_instance.offset);
		Matrix4x4 bulletMat4 = Matrix4x4.TRS(pos, adjustedRot, Vector3.one);
		var bullet = new WxBullet_old(pos, adjustedRot, vel, lifeTime);

		var listCount = _bulletLists_old.Count;

		for (int i = 0; i < listCount; ++i) {


			if (_bulletLists_old[i].Count >= 1023) {
				if (i == _bulletLists_old[i].Count) {

					Debug.Log("Add new List");

					_bulletLists_old.Add(new());
					_batches_old.Add(new());

					_bulletLists_old[i].Add(bullet);
					_batches_old[i].Add(bulletMat4);
					listCount++;
				}
			} else {
				_bulletLists_old[i].Add(bullet);
				_batches_old[i].Add(bulletMat4);
				break;
			}

			Debug.Log(listCount);
		}
	}
	#endregion
}

using System;using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MwProjectile : MonoBehaviour {
	[SerializeField] [WxShowOnly]	protected bool		_ignoreTriggerColliders = true;

	[SerializeField] [WxShowOnly]	protected float		_lifeTime	= 1f;
	[SerializeField]				protected float		_dropRate	= 0f;
	[SerializeField]				protected LayerMask _hitMask	= Physics.DefaultRaycastLayers;
	bool	_enableUpdate = false;

	protected Vector3 _initialVelocity;
	protected Vector3 _initialPos;

	[SerializeField] bool _usePool = true;

	protected event Action OnIdleAction ,OnHitAction;
	
	protected float _time = 0.0f;

	protected Ray _segmentRay;
	RaycastHit _hit;

	protected void FixedUpdate() {
		if(!_enableUpdate) return;
		SimulateProjectile(Time.fixedDeltaTime);
		OnIdleAction?.Invoke();
	}

	public void Initialize(Vector3 spawnPos_, Vector3 velocity_,float dropRate_, float lifeTime_, Action callback = null) {

		transform.position  = spawnPos_;
		transform.forward	= velocity_;

		_initialPos			= spawnPos_;
		_initialVelocity	= velocity_;

		_dropRate	= dropRate_;
		_lifeTime	= lifeTime_;
		_time		= 0f;

		_enableUpdate = true;

		OnInitialize();
		callback?.Invoke();
	}

	protected virtual void OnInitialize() { }

	protected virtual void DisableProjectile() {
		OnHitAction  = null;
		OnIdleAction = null;

		_enableUpdate = false;
		_initialVelocity = Vector3.zero;

		if (_usePool) {
			MwObjectPoolManager.Release(gameObject,0.5f);
		} else {
			Destroy(this.gameObject, 0.5f);
		}
	}

	void SimulateProjectile(float deltaTime) {
		var p0 = GetPosition();
		_time += deltaTime;
		var p1 = GetPosition();

		transform.position = RayCastScan(p0,p1);

		if (_time >= _lifeTime) {
			DisableProjectile();
		}
	}

	Vector3 GetPosition() {
		// p + v*t + 0.5*g*t*t
		Vector3 gravity = Vector3.down * _dropRate;
		return (_initialPos) + (_initialVelocity * _time) + (0.5f *  _time * _time * gravity);
	}

	protected Vector3 RayCastScan(Vector3 start, Vector3 end) {

		var direction	= end - start;
		var distance	= direction.magnitude;

		_segmentRay.origin = start;
		_segmentRay.direction = direction;

		//Debug.DrawRay(_segmentRay.origin,_segmentRay.direction * distance ,Color.red,0.5f);

		//raycast
		var hasHit = Physics.Raycast(_segmentRay,out _hit,distance,_hitMask.value, _ignoreTriggerColliders ? QueryTriggerInteraction.Ignore : QueryTriggerInteraction.Collide);

		if(!hasHit) {
			//Hited Nothing
			return end;
		}

		_time = _lifeTime;
		return _hit.point;
	}

	protected virtual void OnHit(Collider col) {
		//WxLogger.Log($"On Hit {col.name}");
		OnHitAction?.Invoke();
	}

	public void RegisterHitEvent(Action eventCallBack) {
		OnHitAction -= eventCallBack;
		OnHitAction += eventCallBack;
	}
	public void UnRegisterHitEvent(Action eventCallBack) {
		OnHitAction -= eventCallBack;
	}
}

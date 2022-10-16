using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MwCameraInput : MwComponent {

	//[SerializeField] 
	CinemachineVirtualCamera _TpsVCam;

	[Header("CameraInput Settings")]
	[SerializeField] bool _clampYaw    = false;
	[SerializeField] bool _clampPitch  = true;
	[SerializeField] bool _invertPitch = false;

	[SerializeField] bool _keyBoardMouse = true;

	[SerializeField] [Range(-180,0)] float _minYaw = -60;
	[SerializeField] [Range(0,180)] float _maxYaw =  60;

	[SerializeField] [Range(-90,0)] float  _minPitch = -45;
	[SerializeField] [Range(0,90)]  float  _maxPitch =  45;

	[SerializeField] [Range(0.1f,100f)] float _rotationXSpeed = 30f;
	[SerializeField] [Range(0.1f,100f)] float _rotationYSpeed = 30f;

	[SerializeField] LayerMask _raycastLayer = Physics.DefaultRaycastLayers;
	public Vector3 camFowardVector { get; private set; }
	public Vector3 camRightVector { get; private set; }
	public Vector3 aimTargetPos { get; private set;}
	public Vector3 currentAngle => _currentAngle;
	public RaycastHit[] hitBuffer => _hitBuffer;

	MwPlayerInput	_input = null;
	Transform		_currentCamPivot = null;
	Vector2			_currentAngle = Vector2.zero;
	Camera			_cam;
	Ray				_camRay;
	RaycastHit[]	_hitBuffer	= new RaycastHit[16];

	[SerializeField] float _rayDepthOffset = 1f;

	public void OnConstruct(MwPlayerInput input_) {
		_input	= input_;
		_cam	= Camera.main;
		Debug.Assert(_cam,"MainCamera is missing");

		_TpsVCam = GetComponentInChildren<CinemachineVirtualCamera>();
		Debug.Assert(_TpsVCam, "TPS_CinemachineVirtualCamera is missing");
	}

	public void SetCamTarget(Transform followTarget) {
		_currentCamPivot = followTarget;
		if(_TpsVCam) _TpsVCam.Follow = followTarget;

		_currentAngle.x = followTarget.eulerAngles.x;
		_currentAngle.y = followTarget.eulerAngles.y;
	}

	public void UpdateCameraRotation(Vector2 lookInputReadings_, Transform targetPivot_ = null) {

		var camPivot = targetPivot_ != null ? targetPivot_ : _currentCamPivot;

		if (!camPivot) return;

		var invertY = _invertPitch ? -1f : 1f;
		var dTime = _keyBoardMouse ? 1 : Time.deltaTime; 

		_currentAngle.x += lookInputReadings_.x  * dTime * _rotationXSpeed;
		_currentAngle.y -= (lookInputReadings_.y * dTime * _rotationYSpeed) * invertY;

		Vector2 yawClamp = _clampYaw ? new Vector2(_minYaw, _maxYaw) : new Vector2(float.MinValue, float.MaxValue);
		Vector2 pitchClamp = _clampPitch? new Vector2(_minPitch, _maxPitch) : new Vector2(float.MinValue, float.MaxValue);

		_currentAngle.x = _currentAngle.x.ClampAngle(yawClamp.x, yawClamp.y);
		_currentAngle.y = _currentAngle.y.ClampAngle(pitchClamp.x,pitchClamp.y);

		var localrot = Quaternion.Euler(_currentAngle.y , _currentAngle.x, 0);

		//Rotate TargetPivot
		camPivot.localRotation = localrot;

		//Update PlayerCameras Vector
		camFowardVector =  localrot * Vector3.forward;
		camRightVector  = Vector3.Cross(camFowardVector, Vector3.up) * -1f;
		camRightVector.Normalize();

		//var trans = Camera.main.transform;
		//Debug.DrawRay(trans.position, camFowardVector * 20f,Color.blue);
		//Debug.DrawRay(trans.position, camRightVector  * 20f, Color.red);
		//Debug.LogWarning($"{camRightVector.magnitude} | {camFowardVector.magnitude}");
	}

	const float rayDistance = 10000f;

	public void UpdateCameraAim() {
		if(!_cam) return;

		_camRay = _cam.ViewportPointToRay(new Vector3(0.5f,0.5f,0f));

		int count = Physics.RaycastNonAlloc(_camRay, _hitBuffer, rayDistance, _raycastLayer.value, QueryTriggerInteraction.UseGlobal);

		Debug.DrawRay(_camRay.origin,_camRay.direction * rayDistance,Color.green);
		
		//var starttime = Time.realtimeSinceStartup;
		Array.Sort(_hitBuffer, (a, b) => a.distance.CompareTo(b.distance));
		//WxLogger.Warning((Time.realtimeSinceStartup - starttime) * 1000f + "ms");

		_count = count;

		aimTargetPos = count > 0? _hitBuffer[0].point : _camRay.origin + _camRay.direction * rayDistance;

	}

	public void FixedUpdate() {
		
	}


	int _count;

	private void OnDrawGizmos() {

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(aimTargetPos,0.5f);

		Color[] carray = { Color.red,Color.green,Color.blue,Color.yellow};



		for (int i = 0; i < _count; ++i) {

			if(i >= 4) break;

			Gizmos.color = carray[i];
			Gizmos.DrawWireCube(_hitBuffer[i].point,Vector3.one * 0.5f);
		}


	}
}

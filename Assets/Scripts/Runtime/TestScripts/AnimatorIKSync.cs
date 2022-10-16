using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AnimatorIKSync : MonoBehaviour
{
    [SerializeField] private Transform _IKTarget = null;

    [SerializeField] private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnAnimatorMove()
    {
        this.transform.position = anim.rootPosition;
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (_IKTarget == null) return;

        Vector3 p = _IKTarget.position;

        anim.SetLookAtPosition(p);
        anim.SetLookAtWeight(1f);
        
    }

    void LookAtTarget(Transform bone, Transform target)
    {
        Vector3 targetDirection = target.position - transform.position;
        bone.rotation = Quaternion.LookRotation(targetDirection);
    }



}

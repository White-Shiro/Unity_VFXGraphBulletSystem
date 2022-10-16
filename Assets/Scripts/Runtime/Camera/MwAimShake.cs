using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MwAimShake : MonoBehaviour
{
    private CinemachineImpulseSource _impulseSource;
    // Start is called before the first frame update
    void Start()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey)
            Shake();
    }

    public void Shake()
    {
        _impulseSource.GenerateImpulse();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MwOrbitDisplay : MonoBehaviour {


    public Transform origin;
    public Transform target;

    public float speed = 30f;

    void Update() {

        if(origin && target) {
            target.RotateAround(origin.position,Vector3.up, speed *100f * Time.deltaTime);

        }
    }

    private void OnDrawGizmosSelected() {
        if(origin && target) {
            Gizmos.DrawWireSphere(origin.position,0.2f);
            Gizmos.DrawWireSphere(target.position, 0.2f);
            Gizmos.DrawLine(origin.position,target.position);
        }
    }
}

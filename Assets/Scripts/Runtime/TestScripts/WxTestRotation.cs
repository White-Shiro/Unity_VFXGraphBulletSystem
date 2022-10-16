using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WxTestRotation : MonoBehaviour {

    [SerializeField] Vector3 offset;

	[SerializeField] bool useLocal =false;

    void Update() {
        if(useLocal) {
            transform.localRotation = Quaternion.Euler(offset);
        } else {
            transform.rotation =  Quaternion.Euler(offset);
		}
    }
}

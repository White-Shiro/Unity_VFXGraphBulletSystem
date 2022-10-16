using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DrawDebugLine : MonoBehaviour
{

    [SerializeField] private Transform root;
    [SerializeField] private Transform target;

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var direction = (root.transform.forward - root.transform.position).normalized;
        Gizmos.DrawLine(transform.position, transform.InverseTransformDirection(direction)  * 100f);


        var testDirection = (target.position - transform.position).normalized;
        //Debug.Log(transform.InverseTransformDirection(testDirection));
        
        
        Gizmos.color = Color.black;
        Gizmos.DrawLine(root.transform.position, root.transform.forward * 50f);
    }
}

 using System;
 using System.Collections;
using System.Collections.Generic;
 using System.Diagnostics;
 using System.Numerics;
 using UnityEngine;
using UnityEngine.InputSystem;
 using Debug = UnityEngine.Debug;
 using Quaternion = UnityEngine.Quaternion;
 using Vector2 = UnityEngine.Vector2;
 using Vector3 = UnityEngine.Vector3;

 public class RobotController : MonoBehaviour
{
    
    public Animator anim;

    [SerializeField]
    private Transform aimBone;
    
    [SerializeField] 
    private Transform aimTransform;

    [SerializeField]
    private Transform aimTarget;

    public Vector2 moveDirection;

    public float currentHorizontal, targetHorizontal;
    
    public float currentVertical, targetVertical;

    public float moveSpeed = 3f;

    public float dashLength = 3f;

    public float dashForce = 5f;

    public Rigidbody rb;

    public bool isDashed = false;
    public bool isFired = false;
    public bool isFireCoroutine = false;
    public bool isResetFireLayerCoroutine = false;

    public bool isDashCoroutine = false;
    public bool isResetDashCoroutine = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //when fire
        if (isFired)
        {    
            //then call the fire coroutine 
            if (!isFireCoroutine)
                StartCoroutine(FireCoroutine());
        }
        //if reset fire layer weight
        if (isResetFireLayerCoroutine)
        {
            // but keep firing
            if (isFired)
            {
                //then stop the count down
                StopCoroutine(ResetFireLayerCoroutine());
                isResetFireLayerCoroutine = false;
            }
        }

        
    }

    private void LateUpdate()
    {
        var isWalking = false;

        if (Mathf.Abs(currentHorizontal - targetHorizontal) > 0.01f)
        {
            currentHorizontal = Mathf.Lerp(currentHorizontal, targetHorizontal, 0.8f);
            anim.SetFloat("Horizontal", currentHorizontal);
        }

        if (Mathf.Abs(currentVertical - targetVertical) > 0.01f)
        {
            currentVertical = Mathf.Lerp(currentVertical, targetVertical, 0.8f);
            anim.SetFloat("Vertical", currentVertical);
        }

        if (Mathf.Abs(targetHorizontal) > 0.01f || Mathf.Abs(targetVertical) > 0.01f ||
            Mathf.Abs(currentHorizontal) > 0.01f || Mathf.Abs(currentVertical) > 0.01f)
        {
            isWalking = true;
        }

        if (isWalking)
        {
            rb.velocity = new Vector3(currentHorizontal * moveSpeed, 0, currentVertical * moveSpeed);
        }

        if (isDashed)
        {
            if (!isDashCoroutine)
                StartCoroutine(DashCoroutine());
        }


        anim.SetBool("isWalk", isWalking);
        anim.SetBool("isDash", isDashed);
    }


    public void Move(InputAction.CallbackContext context) {
        //Debug.Log(context.ReadValue<Vector2>());
        moveDirection = context.ReadValue<Vector2>();
        if (context.phase == InputActionPhase.Performed)
        {
            // anim.SetBool("isWalk", true);

            targetHorizontal = moveDirection.x;
            targetVertical = moveDirection.y;
            // anim.SetFloat("Horizontal", x);
            // anim.SetFloat("Vertical", y);
        }
        else {
            // anim.SetBool("isWalk", false);
            // anim.SetFloat("Horizontal", 0);
            // anim.SetFloat("Vertical", 0);
            targetHorizontal = 0;
            targetVertical = 0;
        }


    }

    public void Jump(InputAction.CallbackContext context) {
        
        if (context.phase == InputActionPhase.Performed)
            anim.SetTrigger("isJump");
        else
            anim.ResetTrigger("isJump");
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (!isDashed)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                isDashed = true;
            }
        }

    }

    public void Fire(InputAction.CallbackContext context)
    {

        if (!isFired)
        {
            // check the state of fire input
            if (context.phase == InputActionPhase.Performed)
            {
                //isFired = true;
            }
        }

    }

    void AimAtTarget(Transform bone, Transform target)
    {
        Vector3 aimForward = aimTransform.forward;
        Vector3 dir = target.position - aimForward;
        bone.rotation = Quaternion.FromToRotation(aimForward,dir);

    }

    // fire animation coroutine 
    IEnumerator FireCoroutine() {
        isFireCoroutine = true;
        // switch to layer 1 
        anim.SetLayerWeight(1, 1);
        // start fire animation
        anim.SetBool("isFire", true);

        //aim at target
       // AimAtTarget(aimBone,aimTarget);
        
        
        yield return new WaitForSeconds(0.3f);

        // stop fire animation
        anim.SetBool("isFire", false);

        //reset fire state
        isFired = false;

        //reset fire coroutine
        isFireCoroutine = false;

        // call fire layer weight reset coroutine
        if (!isResetFireLayerCoroutine)

        yield return StartCoroutine(ResetFireLayerCoroutine()); 

 

    }
    IEnumerator ResetFireLayerCoroutine() {

        isResetFireLayerCoroutine = true;
        float time = 0;
        // while animtor is in fire layer ,  and fire animation completed
        while (anim.GetCurrentAnimatorStateInfo(1).IsName("Fire") && !anim.IsInTransition(1))
        {
            time += Time.time;
            print(time);
            yield return new WaitForSeconds(5f);
            anim.SetLayerWeight(1, 0); 
        }
        isResetFireLayerCoroutine = false;


    }

    IEnumerator DashCoroutine()
    {
        isDashCoroutine = true;
        Vector3 targetVelocity = rb.velocity + new Vector3(currentHorizontal * dashLength, 0, currentVertical * dashLength);
        float dist = Vector3.Distance(rb.velocity, targetVelocity);
        Debug.Log("Dashing : " + dist);
        while (dist > 1f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, targetVelocity, 0.5f);
        }

        yield return new WaitUntil(() => dist < 1f);
        
        //yield return new WaitForSeconds(1f);

        isDashed = false;
        isDashCoroutine = false;
    }

}

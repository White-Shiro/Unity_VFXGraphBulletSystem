using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CinemachineSwitcher : MonoBehaviour
{
    public int previouscamIndex = 0;
    public int currentCamIndex = 0;
    public bool isSwitched =false;

    public CinemachineBrain camBrains;
    public List<CinemachineVirtualCamera> cams = new List<CinemachineVirtualCamera>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(cams.Count);
    }

    public void OnSwitch(InputAction.CallbackContext context) 
    {
      

        if (context.performed) {
            isSwitched = true;
        }

        if (isSwitched)
        {
            previouscamIndex = currentCamIndex;

            if (currentCamIndex + 1 < cams.Count)
            {
                currentCamIndex += 1;
            }
            else {
                currentCamIndex = 0;
            }
            cams[previouscamIndex].Priority = 10 + previouscamIndex;
            cams[currentCamIndex].Priority = 1000;
            isSwitched = false;
        }
        
    
    }
}

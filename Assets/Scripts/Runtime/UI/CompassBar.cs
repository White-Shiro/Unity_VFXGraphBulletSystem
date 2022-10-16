using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CompassBar : MonoBehaviour
{
    public RawImage compassImage;

    public Transform player;

    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        compassImage.uvRect = new Rect(player.localEulerAngles.y / 360, 0, 1, 1);

        Vector3 forward = player.transform.forward;

        forward.y = 0;

        float headAngle = Quaternion.LookRotation(forward).eulerAngles.y;
        headAngle = 5 * (Mathf.RoundToInt(headAngle/5f ));
        //Debug.Log(Mathf.RoundToInt(headAngle));

        int displayAngleText = Mathf.RoundToInt(headAngle);
        ShowCompassText(displayAngleText);

    }

    void ShowCompassText(int angle)
    {
        text.text = angle switch
        {
            0 => "N",
            360 => "N",
            315 => "NW",
            45 => "NE",
            270 => "W",
            90 => "E",
            225 => "SW",
            180 => "S",
            135 => "SE",
            _ => angle.ToString()
        };
    }
}

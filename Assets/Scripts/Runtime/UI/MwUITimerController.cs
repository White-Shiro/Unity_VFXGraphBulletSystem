using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MwUITimerController : MonoBehaviour {


    [SerializeField] MwUITimerView _timerView = null;
    [SerializeField] MwMVXCModelTest _model = null;

    void Update() {
        SetTimer();
    }
    
    void SetTimer() {

        if (!_timerView) return;

        //Update Model->Data
        MwMVXCModelTest.data = (int)Time.time;



        //Use Model's Data for UI view

        _timerView.SetTimerText((float)MwMVXCModelTest.data);

    }

}

using UnityEngine;
using UnityEngine.UI;

public class MwUITimerView : MonoBehaviour {
    [SerializeField] Text _timerText;

    public void SetTimerText(float time_) {
        if (!_timerText) return;
        _timerText.text = time_.ToString();
    }
}

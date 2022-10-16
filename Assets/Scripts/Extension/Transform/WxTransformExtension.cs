using UnityEngine;

public static class WxTransformExtension {
    public static void SetParentReset(this Transform t,Transform parent, bool affectScale = false) {
        t.SetParent(parent,true);
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;

        var scale = parent == null? Vector3.one : affectScale? parent.localScale : t.localScale;

        t.localScale = scale;

    }



}


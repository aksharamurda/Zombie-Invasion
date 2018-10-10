using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : UIElement {

    public float defaultSpread = 30;
    public float targetSpread = 30;
    public float maxSpread = 80;
    public float spreadSpeed = 5;

    float t;
    float curSpread;

    public CrosshairPart[] crosshairParts;

    public override void Tick(float d)
    {
        t = d * spreadSpeed;
        curSpread = Mathf.Lerp(curSpread, targetSpread, t);
        for (int i = 0; i < crosshairParts.Length; i++)
        {
            CrosshairPart p = crosshairParts[i];
            p.recTransform.anchoredPosition = p.pos * curSpread;
        }

        targetSpread = Mathf.Lerp(targetSpread, defaultSpread, t);
    }

    public void AddSpread(float value)
    {
        targetSpread = value;
    }

    [System.Serializable]
    public class CrosshairPart
    {
        public RectTransform recTransform;
        public Vector2 pos;
    }
}

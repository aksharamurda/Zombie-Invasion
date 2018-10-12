using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICrosshair : UIElement {

    public GameObject uiCrosshair;

    public float defaultSpread = 30;
    public float targetSpread = 30;
    public float maxSpread = 80;
    public float spreadSpeed = 5;

    float t;
    float curSpread;

    public CrosshairPart[] crosshairParts;

    public GameObject uiReload;
    public Image reloadImage;
    float reloadAmount;
    float reloadTime;

    private void Awake()
    {
        uiReload.SetActive(false);
    }

    public void EnableReloadUI(float a, float rt)
    {
        uiReload.SetActive(true);
        uiCrosshair.SetActive(false);
        reloadAmount = a;
        reloadTime = rt;
    }

    public override void Tick(float d)
    {
        t = d * spreadSpeed;

        if (uiCrosshair.activeInHierarchy)
        {
            curSpread = Mathf.Lerp(curSpread, targetSpread, t);
            for (int i = 0; i < crosshairParts.Length; i++)
            {
                CrosshairPart p = crosshairParts[i];
                p.recTransform.anchoredPosition = p.pos * curSpread;
            }

            targetSpread = Mathf.Lerp(targetSpread, defaultSpread, t);
        }
        else if(uiReload.activeInHierarchy)
        {
            reloadImage.fillAmount = reloadAmount / reloadTime;
            if(reloadImage.fillAmount == 1)
            {
                uiReload.SetActive(false);
                uiCrosshair.SetActive(true);
            }
        }
        
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArea : MonoBehaviour {

    public delegate void OnHitArea(float damage);
    public event OnHitArea onHitArea;

    public float damagePercent = 5;
    [HideInInspector]
    public float healthAmount;
    public bool isVital;

    public void OnHit(float damageFromWeapon)
    {
        if (isVital)
        {
            onHitArea(healthAmount);
        }
        else
        {
            float calculatePart = (damagePercent / 100) * damageFromWeapon;
            calculatePart = (calculatePart * 5) + damageFromWeapon;
            onHitArea(calculatePart);
        }

    }

}

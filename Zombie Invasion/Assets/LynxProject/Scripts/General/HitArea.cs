using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitArea : MonoBehaviour {

    public float damagePercent = 5;
    public bool isVital;
    public Enemy enemy;

    public void OnHit(float damageFromWeapon)
    {
        if (isVital)
        {
            enemy.OnHitArea(10000);
        }
        else
        {
            float calculatePart = (damagePercent / 100) * damageFromWeapon;
            calculatePart = (calculatePart * 5) + damageFromWeapon;
            enemy.OnHitArea(calculatePart);
        }

    }

}

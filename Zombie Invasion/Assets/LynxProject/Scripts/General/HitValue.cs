using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitValue : MonoBehaviour {

    public float percentDamage = 5;

    public void OnHit(int damage)
    {
        float pDamage = ((percentDamage * 100) / 100);
        //float h = 100 / pDamage;

        Debug.Log(pDamage);
    }
}

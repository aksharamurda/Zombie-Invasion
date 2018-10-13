using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    public float healthAmount = 100;

    public void OnHitTaken(float damage)
    {
        if (healthAmount <= 0)
            return;

        healthAmount -= damage;
    }
}

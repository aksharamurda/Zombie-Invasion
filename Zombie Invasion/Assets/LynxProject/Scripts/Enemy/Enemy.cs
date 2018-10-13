using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour {

    [HideInInspector]
    public Health health;

    public bool isDead;
    void Awake()
    {
        health = GetComponent<Health>();
    }

    public virtual void OnHitArea(float damage)
    {
        health.OnHitTaken(damage);

        if (isDead)
            return;

        if (health.healthAmount <= 0)
        {
            isDead = true;
            health.healthAmount = 0;
        }

    }
}

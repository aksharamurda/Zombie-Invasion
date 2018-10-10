using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour {

    private Health health;

    public bool isDead;
    void Start()
    {
        health = GetComponent<Health>();
    }

    public virtual void OnHit()
    {
        if (isDead)
            return;

        if (health.healthAmount <= 0)
            isDead = true;
    }
}

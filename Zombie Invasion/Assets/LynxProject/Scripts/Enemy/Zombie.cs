using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy {

    void Start()
    {
        foreach (HitArea hit in GetComponentsInChildren(typeof(HitArea)))
        {
            hit.enemy = this;
        }
    }

    public override void OnHitArea(float damage)
    {
        base.OnHitArea(damage);
    }
}

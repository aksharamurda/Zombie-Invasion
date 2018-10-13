using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy {

    List<HitArea> hitAreas = new List<HitArea>();

    void Start()
    {
        foreach (HitArea hit in GetComponentsInChildren(typeof(HitArea)))
        {
            hit.onHitArea += OnHitArea;
            hit.healthAmount = health.healthAmount;
            hitAreas.Add(hit);
        }
    }

    public override void OnHitArea(float damage)
    {
        base.OnHitArea(damage);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ballistics{
    public static RaycastHit RaycastShoot(Vector3 origin, Vector3 direction, ref bool success, LayerMask layerMask)
    {
        Ray ray = new Ray(origin, direction);
        RaycastHit hit;

        if(Physics.Raycast(origin, direction, out hit, 200, layerMask))
        {
            success = true;
            return hit;
        }

        return hit;
    }
}

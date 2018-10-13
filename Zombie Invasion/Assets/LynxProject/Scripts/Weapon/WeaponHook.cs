using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHook : MonoBehaviour {

    public Transform aimPosition;
    public Transform leftHandIK;

    ParticleSystem[] particles;

    public GameObject shootFx;

    GameObject newFx;

    private void OnEnable()
    {
        
    }

    public void Shoot()
    {


        newFx = Instantiate(shootFx, aimPosition.position, aimPosition.rotation);
        particles = newFx.GetComponentsInChildren<ParticleSystem>();

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].Play();
        }

        Destroy(newFx, 2f);
    }
}

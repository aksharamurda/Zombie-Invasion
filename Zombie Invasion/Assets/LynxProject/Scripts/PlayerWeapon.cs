using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerWeapon {

    public bool isMainWeapon;
    public string mainWeaponID;
    public string secondWeaponID;

    RuntimeWeapon curWeapon;
    public RuntimeWeapon GetCurrent()
    {
        return curWeapon;
    }

    public void SetCurrent(RuntimeWeapon rw)
    {
        curWeapon = rw;
    }

    public RuntimeWeapon main_Weapon;
    public RuntimeWeapon second_Weapon;
}

[System.Serializable]
public class RuntimeWeapon
{
    public int curAmmo;
    public int curCarryingAmmo;
    public float lastFired;
    public GameObject m_instance;
    public WeaponHook weaponHook;
    public Weapon weapon;

    public void ShootWeapon()
    {
        weaponHook.Shoot();
        curAmmo--;

        //Debug.Log("Shoot Weapon Working.");
    }
}

[System.Serializable]
public class RuntimeReferences
{
    public List<RuntimeWeapon> runtimeWeapons = new List<RuntimeWeapon>();

    public void Init()
    {
        runtimeWeapons.Clear();
    }

    public RuntimeWeapon WeaponToRuntimeWeapon(Weapon w)
    {
        RuntimeWeapon rw = new RuntimeWeapon();
        rw.weapon = w;
        rw.curAmmo = w.magazineAmmo;
        rw.curCarryingAmmo = w.maxAmmo;

        runtimeWeapons.Add(rw);

        return rw;
    }

    public void RemoveRuntimeWeapon(RuntimeWeapon rw)
    {
        //if (rw.m_instance)
        //    Destroy(rw.m_instance);

        if (runtimeWeapons.Contains(rw))
            runtimeWeapons.Remove(rw);
    }

    public void OnDestroy()
    {
        runtimeWeapons.Clear();
    }
}

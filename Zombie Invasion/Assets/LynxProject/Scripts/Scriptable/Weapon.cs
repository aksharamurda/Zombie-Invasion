using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Weapon", fileName = "Weapon")]
public class Weapon : ScriptableObject {

    public string id;
    public Sprite icon;
    public int price = 1000;
    public IKPosition m_h_ik;
    public GameObject modelPrefab;

    public int bulletAmount = 1;
    public float fireRate = 0.1f;
    public int magazineAmmo = 30;
    public int maxAmmo = 160;
    public bool onIdleDiableOh;
    public int WeaponType;
    public int damageWeapon = 10;

    public AnimationCurve recoilY;
    public AnimationCurve recoilZ;
}

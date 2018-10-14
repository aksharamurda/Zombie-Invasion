using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeapon : MonoBehaviour {
    public Image iconWeapon;
    public Text textAmmo;

    PlayerController playerController;

    void Awake()
    {
        playerController = GameObject.FindObjectOfType(typeof(PlayerController)) as PlayerController;
    }

    public void Update()
    {
        RuntimeWeapon rw = playerController.playerWeapon.GetCurrent();
        iconWeapon.sprite = rw.weapon.icon;
        textAmmo.text = rw.curAmmo + "/" + rw.curCarryingAmmo;
    }
}

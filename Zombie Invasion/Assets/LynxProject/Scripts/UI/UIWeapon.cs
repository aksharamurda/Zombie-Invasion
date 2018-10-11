using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeapon : UIElement {
    public Image iconWeapon;
    public Text textAmmo;

    PlayerController playerController;

    void Awake()
    {
        playerController = GameObject.FindObjectOfType(typeof(PlayerController)) as PlayerController;
    }

    public override void Tick(float d)
    {
        RuntimeWeapon rw = playerController.playerWeapon.GetCurrent();
        iconWeapon.sprite = rw.weapon.icon;
        textAmmo.text = rw.curAmmo + "/" + rw.curCarryingAmmo;
    }
}

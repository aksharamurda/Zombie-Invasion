using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonReload : Button
{

    PlayerInput playerInput;

    protected override void Awake()
    {
        playerInput = GameObject.FindObjectOfType(typeof(PlayerInput)) as PlayerInput;
        onClick.AddListener(delegate { playerInput.OnMobileReloadWeapon(); });
    }
}

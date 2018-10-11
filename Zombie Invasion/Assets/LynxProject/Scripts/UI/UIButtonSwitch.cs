using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSwitch : Button
{

    PlayerInput playerInput;

    protected override void Awake()
    {
        playerInput = GameObject.FindObjectOfType(typeof(PlayerInput)) as PlayerInput;
        onClick.AddListener(delegate { playerInput.OnMobileSwitchWeapon(); });
    }
}

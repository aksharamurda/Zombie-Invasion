using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonFire : Button {

    PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GameObject.FindObjectOfType(typeof(PlayerInput)) as PlayerInput;
    }

	void Update () {
        playerInput.OnMobileFireWeapon(IsPressed());
    }
}

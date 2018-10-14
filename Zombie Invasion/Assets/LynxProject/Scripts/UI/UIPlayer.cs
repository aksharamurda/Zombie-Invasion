using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour {

    public static UIPlayer instance;
    public bool useMobileConsole;

    public Image playerHealthUI;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {

    }

    public void OnHitTaken(float curHealth, float health)
    {
        float h = curHealth / health;
        playerHealthUI.fillAmount = h;
    }
}

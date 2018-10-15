using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour {

    public static UIPlayer instance;
    public bool useMobileConsole;

    public Image playerHealthUI;
    public GameObject consoleUIPanel;

    void Awake()
    {
        instance = this;

        if (!useMobileConsole)
        {
            consoleUIPanel.SetActive(false);
        }
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

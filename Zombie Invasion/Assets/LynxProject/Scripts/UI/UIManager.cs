using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public static UIManager instance;

    public bool useMobileConsole;

    public List<UIElement> uiElements = new List<UIElement>();

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        float delta = Time.deltaTime;
        for (int i = 0; i < uiElements.Count; i++)
        {
            uiElements[i].Tick(delta);
        }
    }
}

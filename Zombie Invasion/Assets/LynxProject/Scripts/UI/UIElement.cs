using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement : MonoBehaviour {

    void Awake()
    {

        if (UIManager.instance != null)
            UIManager.instance.uiElements.Add(this);
    }

    public virtual void InitUIElement()
    {

    }

    public virtual void Tick(float d)
    {

    }
}

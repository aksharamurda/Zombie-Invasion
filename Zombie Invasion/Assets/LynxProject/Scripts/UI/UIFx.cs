using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFx : MonoBehaviour
{
    public static UIFx instance;

    public Image splatBlood;
    public Color startColor;
    public Color endColor;
    public float speed = 1.0f;

    float startTime;
    private void Awake()
    {
        instance = this;
        startTime = Time.time;
        splatBlood.enabled = false;
    }

    public void OnHitTaken()
    {
        splatBlood.enabled = true;
        startTime = Time.time;
        Debug.Log("Enemy Damage Taken");
    }

    void Update()
    {
        float time = (Time.time - startTime) * speed;
        splatBlood.color = Color.Lerp(startColor, endColor, time);
    }
}

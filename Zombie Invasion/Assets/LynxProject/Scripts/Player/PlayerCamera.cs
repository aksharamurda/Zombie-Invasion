﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCamera : MonoBehaviour {

    public Transform camTrans;
    public Transform target;
    public Transform pivot;
    [HideInInspector]
    public Transform mTranform;
    //[HideInInspector]
    public Transform rayCamera;
    public bool leftPivot;
    float delta;

    float mouseX;
    float mouseY;
    float smoothX;
    float smoothY;

    float smoothXVelocity;
    float smoothYVelocity;

    float lookAngle;
    float tiltAngle;

    [HideInInspector]
    public CameraValues values;

    PlayerController playerController;

    float shake_decay;
    float shake_intensity;
    Vector3 originPosition;
    Quaternion originRotation;

    public void InitPlayerCamera(PlayerInput inp)
    {

        values = Resources.Load("Camera Values") as CameraValues;

        mTranform = this.transform;
        playerController = inp.playerController;
        playerController.playerCamera = this;
        target = playerController.mTransform;
        rayCamera = camTrans.GetChild(0);
        
    }

    public void FixedTick(float d)
    {
        delta = d;
        if (target == null)
            return;

        HandlePositions();

        if(!playerController.isDead)
            HandleRotation();

        float speed = values.aimSpeed;
        if (playerController.controllerStates.isAiming)
            speed = values.aimSpeed;

        Vector3 targetPosition = Vector3.Lerp(mTranform.position, target.position, delta * speed);
        mTranform.position = targetPosition;
    }

    public void Tick(float d)
    {
        delta = d;

        if (shake_intensity > 0)
        {
            transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
            transform.rotation = new Quaternion(
                            originRotation.x + Random.Range(-shake_intensity, shake_intensity) * .1f,
                            originRotation.y + Random.Range(-shake_intensity, shake_intensity) * .1f,
                            originRotation.z + Random.Range(-shake_intensity, shake_intensity) * .1f,
                            originRotation.w + Random.Range(-shake_intensity, shake_intensity) * .1f);
            shake_intensity -= shake_decay;
        }
    }

    public void Shake()
    {
        originPosition = transform.position;
        originRotation = transform.rotation;
        shake_intensity = .01f;
        shake_decay = 0.002f;
    }

    public void Shake(float shake_inten, float shake_dec)
    {
        originPosition = transform.position;
        originRotation = transform.rotation;
        shake_intensity = shake_inten;
        shake_decay = shake_dec;
    }

    void HandlePositions()
    {
        float targetX = values.normalX;
        float targetZ = values.normalZ;
        float targetY = values.normalY;


        if (playerController.controllerStates.isAiming)
        {
            targetX = values.aimX;
            targetZ = values.aimZ;
        }

        

        if (leftPivot)
            targetX = -targetX;



        Vector3 newPivotPosition = pivot.localPosition;
        newPivotPosition.x = targetX;
        newPivotPosition.y = targetY;

        Vector3 newCamPositon = camTrans.localPosition;
        newCamPositon.z = targetZ;
        if (playerController.isDead)
        {
            newCamPositon.x = values.camXDeath;
            newCamPositon.y = values.camYDeath;
            newCamPositon.z = values.camZDeath;
        }

        float t = delta * values.adaptSpeed;
        pivot.localPosition = Vector3.Lerp(pivot.localPosition, newPivotPosition, t);
        camTrans.localPosition = Vector3.Lerp(camTrans.localPosition, newCamPositon, t);
    }

    void HandleRotation()
    {

        if (UIPlayer.instance.useMobileConsole)
        {
            JoystickController mobileJoystick = GameObject.FindObjectOfType(typeof(JoystickController)) as JoystickController;
            mouseX = mobileJoystick.Horizontal;
            mouseY = mobileJoystick.Vertical;
        }
        else
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
        }

        if (values.turnSmooth > 0)
        {
            smoothX = Mathf.SmoothDamp(smoothX, mouseX, ref smoothXVelocity, values.turnSmooth);
            smoothY = Mathf.SmoothDamp(smoothY, mouseY, ref smoothYVelocity, values.turnSmooth);
        }
        else
        {
            smoothX = mouseX;
            smoothY = mouseY;
        }

        lookAngle += smoothX * values.y_rotate_speed;
        Quaternion targetRot = Quaternion.Euler(0, lookAngle, 0);
        mTranform.rotation = targetRot;
        tiltAngle -= smoothY * values.x_rotate_speed;
        tiltAngle = Mathf.Clamp(tiltAngle, values.minAngle, values.maxAngle);

        pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
    }

    public static bool IsPointerOverGameObject()
    {
        //check mouse
        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        for (int i = 0; i < Input.touchCount; ++i)
        {
            //check touch
            if (Input.touchCount > 0 && Input.touches[i].phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                    return true;
            }

        }

        return false;
    }
}

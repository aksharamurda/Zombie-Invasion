using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerCamera : MonoBehaviour {

    public Transform camTrans;
    public Transform target;
    public Transform pivot;
    public Transform mTranform;
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
        target = playerController.mTransform;
    }

    public void FixedTick(float d)
    {
        delta = d;
        if (target == null)
            return;

        HandlePositions();
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

        float t = delta * values.adaptSpeed;
        pivot.localPosition = Vector3.Lerp(pivot.localPosition, newPivotPosition, t);
        camTrans.localPosition = Vector3.Lerp(camTrans.localPosition, newCamPositon, t);
    }

    void HandleRotation()
    {

        if (UIManager.instance.useMobileConsole)
        {

            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; ++i)
                {
                    if (Input.GetTouch(i).position.x < ((Screen.width / 2) + Screen.width / 4))
                    {
                        if (Input.GetTouch(i).phase == TouchPhase.Moved)
                        {
                            Vector2 TouchDirection = Input.GetTouch(i).deltaPosition;

                            mouseX = TouchDirection.x;
                            mouseY = TouchDirection.y;
                        }
                        else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                        {
                            mouseX = 0;
                            mouseY = 0;
                        }
                    }
                    else
                    {
                        mouseX = 0;
                        mouseY = 0;
                    }
                }
            }
            
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

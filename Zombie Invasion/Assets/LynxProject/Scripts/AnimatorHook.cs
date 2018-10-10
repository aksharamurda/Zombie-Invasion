using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHook : MonoBehaviour {

    Animator animator;
    PlayerController playerController;

    float m_h_weight;
    float o_h_weight;
    float l_weight;
    float b_weight;

    Transform rh_target;
    public Transform lh_target;
    Transform shoulder;
    Transform aimPivot;

    Vector3 lookDir;

    public bool onIdleDiableOh;
    public bool disable_o_h;
    public bool disable_m_h;

    public void InitAnimatorHook(PlayerController st)
    {
        playerController = st;
        animator = playerController.animator;

        shoulder = animator.GetBoneTransform(HumanBodyBones.RightShoulder).transform;
        aimPivot = new GameObject().transform;
        aimPivot.name = "Aim Pivot";
        aimPivot.SetParent(playerController.transform);

        rh_target = new GameObject().transform;
        rh_target.name = "Right Hand Target";
        rh_target.SetParent(aimPivot);

        playerController.inputVariables.aimPosition = playerController.transform.position + transform.forward * 15;
        playerController.inputVariables.aimPosition.y += 1.4f;

    }

    
    public void EquipWeapon(RuntimeWeapon rw)
    {
        Weapon w = rw.weapon;
        lh_target = rw.weaponHook.leftHandIK;

        rh_target.localPosition = w.m_h_ik.pos;
        rh_target.localEulerAngles = w.m_h_ik.rot;

        basePosition = w.m_h_ik.pos;
        baseRotation = w.m_h_ik.rot;

        onIdleDiableOh = rw.weapon.onIdleDiableOh;
    }
    

    private void OnAnimatorMove()
    {
        lookDir = playerController.inputVariables.aimPosition - aimPivot.position;
        HandleShoulder();
    }

    void HandleShoulder()
    {
        HandleShoulderPosition();
        HandleShoulderRotation();
    }

    void HandleShoulderPosition()
    {
        aimPivot.position = shoulder.position;
    }

    void HandleShoulderRotation()
    {
        Vector3 targetDir = lookDir;
        if (targetDir == Vector3.zero)
            targetDir = aimPivot.forward;

        Quaternion tr = Quaternion.LookRotation(targetDir);
        aimPivot.rotation = Quaternion.Slerp(aimPivot.rotation, tr, playerController.delta * 15);
    }

    void HandleWeights()
    {
        if (playerController.controllerStates.isInteracting)
        {
            m_h_weight = 0;
            o_h_weight = 0;
            l_weight = 0;
            return;
        }

        float t_l_weight = 0;
        float t_m_weight = 0;

        if (playerController.controllerStates.isAiming)
        {
            t_m_weight = 1;
            b_weight = 0.4f;
        }
        else
        {
            b_weight = 0.3f;
        }

        if (disable_m_h)
            t_m_weight = 0;

        if (lh_target != null)
            o_h_weight = 1;
        else
            o_h_weight = 0;

        if (disable_o_h)
            o_h_weight = 0;

        Vector3 ld = playerController.inputVariables.aimPosition - playerController.mTransform.position;
        float angle = Vector3.Angle(playerController.mTransform.forward, ld);
        if (angle < 76)
            t_l_weight = 1;
        else
            t_l_weight = 0;

        if (angle > 45)
            t_m_weight = 0;

        if (!playerController.controllerStates.isAiming)
        {
            if (onIdleDiableOh)
                o_h_weight = 0;
        }

        l_weight = Mathf.Lerp(l_weight, t_l_weight, playerController.delta * 3);
        m_h_weight = Mathf.Lerp(m_h_weight, t_m_weight, playerController.delta * 3);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        HandleWeights();
        animator.SetLookAtWeight(l_weight, b_weight, 1, 1, 1);
        animator.SetLookAtPosition(playerController.inputVariables.aimPosition);

        if (lh_target != null)
        {
            UpdateIK(AvatarIKGoal.LeftHand, lh_target, o_h_weight);
        }

        UpdateIK(AvatarIKGoal.RightHand, rh_target, m_h_weight);
    }

    void UpdateIK(AvatarIKGoal goal, Transform t, float w)
    {
        animator.SetIKPositionWeight(goal, w);
        animator.SetIKRotationWeight(goal, w);
        animator.SetIKPosition(goal, t.position);
        animator.SetIKRotation(goal, t.rotation);
    }

    public void Tick()
    {
        RecoilActual();
    }

    float recoilT;
    Vector3 offsetPosition;
    Vector3 offsetRotation;
    Vector3 basePosition;
    Vector3 baseRotation;
    bool recoilIsInit;

    public void RecoilAnim()
    {
        if (!recoilIsInit)
        {
            recoilIsInit = true;
            recoilT = 0;
            offsetPosition = Vector3.zero;
        }
    }

    public void RecoilActual()
    {
        if (recoilIsInit)
        {
            recoilT += playerController.delta * 10; //3
            if (recoilT > 1)
            {
                recoilT = 1;
                recoilIsInit = false;
            }

            
            offsetPosition = Vector3.forward * playerController.playerWeapon.GetCurrent().weapon.recoilZ.Evaluate(recoilT);
            offsetRotation = Vector3.right * 90 * -playerController.playerWeapon.GetCurrent().weapon.recoilY.Evaluate(recoilT);
            

            rh_target.localPosition = basePosition + offsetPosition;
            rh_target.localEulerAngles = baseRotation + offsetRotation;

        }
    }
}

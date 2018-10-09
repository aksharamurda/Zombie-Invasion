using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public InputVariables inputVariables;

    [System.Serializable]
    public class InputVariables
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public Vector3 moveDirection;
        public Vector3 aimPosition;
        public Vector3 rotateDirection;
    }

    public ControllerStates controllerStates;

    [System.Serializable]
    public class ControllerStates
    {
        public bool onGround;
        public bool isAiming;
        public bool isInteracting;
        public bool isReloading;
    }

    public Animator animator;
    public GameObject activeModel;
    [HideInInspector]
    public AnimatorHook animatorHook;

    [HideInInspector]
    public Rigidbody rigidBody;
    [HideInInspector]
    public Collider controllerCollider;

    [HideInInspector]
    public LayerMask ignoreLayer;
    [HideInInspector]
    public LayerMask ignoreForGround;

    [HideInInspector]
    public Transform mTransform;

    [HideInInspector]
    public float delta = 0;

    public void InitPlayerController()
    {
        mTransform = this.transform;
        SetupAnimator();

        rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = false;
        rigidBody.drag = 4;
        rigidBody.angularDrag = 999;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        controllerCollider = GetComponent<Collider>();

        ignoreLayer = ~(1 << 9);
        ignoreForGround = ~(1 << 9 | 1 << 10);


        animatorHook = activeModel.AddComponent<AnimatorHook>();
        animatorHook.InitAnimatorHook(this);
    }

    void SetupAnimator()
    {
        if (activeModel == null)
        {
            animator = GetComponentInChildren<Animator>();
            activeModel = animator.gameObject;
        }

        if (animator == null)
            animator = activeModel.GetComponent<Animator>();

        animator.applyRootMotion = false;
    }

    public void FixedTick(float d)
    {
        delta = d;
        controllerStates.onGround = OnGround();

        RotationNormal();
    }

    void RotationNormal()
    {
        if (!controllerStates.isAiming)
            inputVariables.rotateDirection = inputVariables.moveDirection;

        Vector3 targetDir = inputVariables.rotateDirection;
        targetDir.y = 0;

        if (targetDir == Vector3.zero)
            targetDir = mTransform.forward;

        Quaternion lookDir = Quaternion.LookRotation(targetDir);
        Quaternion targetRot = Quaternion.Slerp(mTransform.rotation, lookDir, 8 * delta);
        mTransform.rotation = targetRot;

    }

    public void Tick(float d)
    {
        delta = d;
        controllerStates.onGround = OnGround();
        HandleAnimationAll();
        animatorHook.Tick();
    }

    void HandleAnimationAll()
    {
        animator.SetBool(StaticStrings.animParamAiming, controllerStates.isAiming);

        if (controllerStates.isAiming)
        {
            HandleAnimationAiming();
        }
        else
        {
            HandleAnimationNormal();
        }
    }

    void HandleAnimationNormal()
    {
        float anim_v = inputVariables.moveAmount;
        animator.SetFloat(StaticStrings.animParamVertical, anim_v, 0.15f, delta);
    }

    void HandleAnimationAiming()
    {
        float v = inputVariables.vertical;
        float h = inputVariables.horizontal;

        animator.SetFloat(StaticStrings.animParamHorizontal, h, 0.2f, delta);
        animator.SetFloat(StaticStrings.animParamVertical, v, 0.2f, delta);
    }

    bool OnGround()
    {
        Vector3 origin = mTransform.position;
        origin.y += 0.06f;

        Vector3 dir = -Vector3.up;
        float dis = 0.7f;
        RaycastHit hit;
        if (Physics.Raycast(origin, dir, out hit, dis, ignoreForGround))
        {
            Vector3 tp = hit.point;
            mTransform.position = tp;
            return true;
        }

        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [HideInInspector]
    public ResourcesManager resourcesManager;
    [HideInInspector]
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

    public float reloadTime = 2.6f;
    public PlayerWeapon playerWeapon;

    [HideInInspector]
    public RuntimeReferences runtimeRef;

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

    PlayerCamera m_playerCamera;
    public PlayerCamera playerCamera { get { return m_playerCamera; } set { m_playerCamera = value; } }

    [HideInInspector]
    public float delta = 0;

    bool switchingWeapon;
    float interactTime;

    public void InitPlayerController()
    {

        resourcesManager = Resources.Load("Resources") as ResourcesManager;

        resourcesManager.InitResourcesManager();

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

        InitWeapon();
    }

    void InitWeapon()
    {
        CreateRuntimeWeapon(playerWeapon.mainWeaponID, ref playerWeapon.main_Weapon);
        CreateRuntimeWeapon(playerWeapon.secondWeaponID, ref playerWeapon.second_Weapon);
        EquipRuntimeWeapon(playerWeapon.main_Weapon);
        playerWeapon.isMainWeapon = true;
    }

    public void CreateRuntimeWeapon(string id, ref RuntimeWeapon r_w_m)
    {
        Weapon w = resourcesManager.GetWeapon(id);
        RuntimeWeapon rw = runtimeRef.WeaponToRuntimeWeapon(w);

        GameObject go = Instantiate(w.modelPrefab);
        rw.m_instance = go;
        rw.weapon = w;
        rw.weaponHook = go.GetComponent<WeaponHook>();
        go.SetActive(false);

        Transform p = animator.GetBoneTransform(HumanBodyBones.RightHand);
        go.transform.SetParent(p);
        go.transform.localPosition = Vector3.zero;
        go.transform.localEulerAngles = Vector3.zero;
        go.transform.localScale = Vector3.one;

        r_w_m = rw;
    }

    public void EquipRuntimeWeapon(RuntimeWeapon rw)
    {
        if (playerWeapon.GetCurrent() != null)
        {
            animator.CrossFade("Switch", 0.2f);
            controllerStates.isAiming = false;
            switchingWeapon = true;
            controllerStates.isInteracting = true;
            UnEquipWeapon(playerWeapon.GetCurrent());
        }

        rw.m_instance.SetActive(true);
        animatorHook.EquipWeapon(rw);

        animator.SetFloat(StaticStrings.animParamWeaponType, rw.weapon.WeaponType);
        playerWeapon.SetCurrent(rw);

    }

    public void UnEquipWeapon(RuntimeWeapon rw)
    {
        rw.m_instance.SetActive(false);
    }

    public bool ShootWeapon(float t)
    {
        bool retVal = false;

        RuntimeWeapon c = playerWeapon.GetCurrent();

        if (c.curAmmo > 0)
        {
            if (t - c.lastFired > c.weapon.fireRate)
            {
                c.lastFired = t;
                retVal = true;
                c.ShootWeapon();
                animatorHook.RecoilAnim();
                HandleShooting(c);
            }
        }

        return retVal;
    }

    void HandleShooting(RuntimeWeapon c)
    {

        //Vector3 origin = animatorHook.aimPivot.position;
        Vector3 origin = playerCamera.rayCamera.position;
        origin += playerCamera.rayCamera.forward * 0.5f;

        for (int i = 0; i < c.weapon.bulletAmount; i++)
        {
            Vector3 targetPosition = inputVariables.aimPosition;
            Vector3 targetDirection = targetPosition - origin;

            bool isHit = false;

            RaycastHit hit =  Ballistics.RaycastShoot(origin, targetDirection, ref isHit, ignoreLayer);

            if (isHit)
            {
                HandleBulletHit(hit, c);
            }

        }
    }

    void HandleBulletHit(RaycastHit hit, RuntimeWeapon rw)
    {
        //if Hit enemy ?
        hit.transform.SendMessage("OnHit", rw.weapon.damageWeapon, SendMessageOptions.DontRequireReceiver);
        //else ?

        //Debug.Log("Send Message to " + hit.transform.name + ", that he was shot.");
    }


    public bool Reload()
    {
        bool retVal = false;
        RuntimeWeapon c = playerWeapon.GetCurrent();

        if (c.curAmmo < c.weapon.magazineAmmo)
        {
            if (c.weapon.magazineAmmo <= c.curCarryingAmmo)
            {
                c.curAmmo = c.weapon.magazineAmmo;
                c.curCarryingAmmo -= c.curAmmo;
            }
            else
            {
                c.curAmmo = c.curCarryingAmmo;
                c.curCarryingAmmo = 0;
            }

            retVal = true;
            animator.CrossFade("Rifle Reload", 0.2f);
            controllerStates.isAiming = false;
            controllerStates.isInteracting = true;
        }

        return retVal;
    }

    public void SwitchWeapon()
    {
        if (controllerStates.isInteracting)
            return;

        playerWeapon.isMainWeapon = !playerWeapon.isMainWeapon;
        EquipRuntimeWeapon((playerWeapon.isMainWeapon) ? playerWeapon.main_Weapon : playerWeapon.second_Weapon);
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

        if (controllerStates.isInteracting)
        {
            interactTime += delta;
            if (switchingWeapon)
            {
                if (interactTime > 1f)
                {
                    switchingWeapon = false;
                    controllerStates.isInteracting = false;
                    controllerStates.isAiming = true;
                    interactTime = 0;
                }
            }
            else
            {

                (GameObject.FindObjectOfType(typeof(UICrosshair)) as UICrosshair).EnableReloadUI(interactTime, reloadTime);
                if (interactTime > reloadTime - 0.1f)
                {
                    controllerStates.isAiming = true;
                }
                if (interactTime > reloadTime)
                {
                    controllerStates.isInteracting = false;
                    interactTime = 0;
                }
            }

        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    float horizontal;
    float vertical;

    [HideInInspector]
    public bool shootInput;
    [HideInInspector]
    public bool reloadInput;
    [HideInInspector]
    public bool switchInput;

    bool isInit;

    float delta;

    [HideInInspector]
    public PlayerController playerController;
    public PlayerCamera playerCamera;

    public bool alwaysAim = true;
    public bool autoAim;
    void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        

        InitInGame();
    }

    public void InitInGame()
    {
        playerController.InitPlayerController();
        playerCamera.InitPlayerCamera(this);
        isInit = true;

        if (alwaysAim)
            playerController.controllerStates.isAiming = true;
    }

    private void FixedUpdate()
    {
        if (!isInit)
            return;

        delta = Time.fixedDeltaTime;

        InGame_UpdateStates_FixedUpdate();
        playerController.FixedTick(delta);

        playerCamera.FixedTick(delta);

    }

    void InGame_UpdateStates_FixedUpdate()
    {
        playerController.inputVariables.rotateDirection = playerCamera.mTranform.forward;
    }
    

    private void Update()
    {
        if (!isInit)
            return;

        delta = Time.deltaTime;

        if (!UIManager.instance.useMobileConsole)
        {
            GetInput_Update();  
            InGame_UpdateStates_Update();
        }

        AimPosition();
        playerController.Tick(delta);
    }

    void GetInput_Update()
    {
        shootInput = Input.GetMouseButton(0);
        reloadInput = Input.GetButtonDown(StaticStrings.inputReload);
        switchInput = Input.GetButtonDown(StaticStrings.inputSwitch);
    }

    public void OnMobileFireWeapon(bool isPressed)
    {
        if (isPressed && !playerController.controllerStates.isInteracting)
        {
            playerController.controllerStates.isAiming = true;
            bool shootActual = playerController.ShootWeapon(Time.realtimeSinceStartup);
            if (shootActual)
            {
                Fire();
            }
        }
    }

    public void OnMobileSwitchWeapon()
    {
        if (!playerController.controllerStates.isInteracting)
            playerController.SwitchWeapon();
    }

    public void OnMobileReloadWeapon()
    {
        bool isReloading = playerController.Reload();
        if (isReloading)
        {
            shootInput = false;
            playerController.playerWeapon.GetCurrent().m_instance.SendMessage("OnReloadStart", 0, SendMessageOptions.DontRequireReceiver);
        }
    }

    void InGame_UpdateStates_Update()
    {
        if (reloadInput)
        {
            bool isReloading = playerController.Reload();
            if (isReloading)
            {
                shootInput = false;
                playerController.playerWeapon.GetCurrent().m_instance.SendMessage("OnReloadStart", 0, SendMessageOptions.DontRequireReceiver);
            }
        }

        if (shootInput && !playerController.controllerStates.isInteracting)
        {
            playerController.controllerStates.isAiming = true;
            bool shootActual = playerController.ShootWeapon(Time.realtimeSinceStartup);
            if (shootActual)
            {
                Fire();
                //Update UI (Ammo, Mag, etc)
            }
        }

        if (switchInput && !playerController.controllerStates.isInteracting)
            playerController.SwitchWeapon();

    }


    void Fire()
    {
        (GameObject.FindObjectOfType(typeof(UICrosshair)) as UICrosshair).targetSpread = 120;
        playerController.playerWeapon.GetCurrent().m_instance.SendMessage("OnFire", 0, SendMessageOptions.DontRequireReceiver);
    }
    void AimPosition()
    {
        Ray ray = new Ray(playerCamera.camTrans.position, playerCamera.camTrans.forward);
        playerController.inputVariables.aimPosition = ray.GetPoint(30);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, playerController.ignoreLayer))
        {
            if(autoAim)
                playerController.inputVariables.aimPosition = hit.point;
        }
    }
}

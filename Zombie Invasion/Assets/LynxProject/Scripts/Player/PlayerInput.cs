using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    float horizontal;
    float vertical;

    bool runInput;
    bool shootInput;
    bool crouchInput;
    bool reloadInput;
    bool switchInput;
    bool pivotInput;

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

        GetInput_Update();
        AimPosition();
        InGame_UpdateStates_Update();

        playerController.Tick(delta);
    }

    void GetInput_Update()
    {
        shootInput = Input.GetMouseButton(0);
        reloadInput = Input.GetButtonDown(StaticStrings.inputReload);
        switchInput = Input.GetButtonDown(StaticStrings.inputSwitch);
    }

    void InGame_UpdateStates_Update()
    {
        if (reloadInput)
        {
            bool isReloading = playerController.Reload();
            if (isReloading)
            {
                shootInput = false;
            }
        }

        if (shootInput && !playerController.controllerStates.isInteracting)
        {
            playerController.controllerStates.isAiming = true;
            bool shootActual = playerController.ShootWeapon(Time.realtimeSinceStartup);
            if (shootActual)
            {
                (GameObject.FindObjectOfType(typeof(Crosshair)) as Crosshair).targetSpread = 120;
                //Update UI (Ammo, Mag, etc)
            }
        }

        if (switchInput && !playerController.controllerStates.isInteracting)
            playerController.SwitchWeapon();

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

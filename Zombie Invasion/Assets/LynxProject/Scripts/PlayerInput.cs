using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {

    float horizontal;
    float vertical;

    bool aimInput;
    bool runInput;
    bool shootInput;
    bool crouchInput;
    bool reloadInput;
    bool switchInput;
    bool pivotInput;

    bool isInit;

    float delta;

    public PlayerController playerController;
    public PlayerCamera playerCamera;

    public bool alwaysAim;

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

        //GetInput_Update();
        AimPosition();
        //InGame_UpdateStates_Update();

        playerController.Tick(delta);
    }

    void AimPosition()
    {
        Ray ray = new Ray(playerCamera.camTrans.position, playerCamera.camTrans.forward);
        playerController.inputVariables.aimPosition = ray.GetPoint(30);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, playerController.ignoreLayer))
        {
            playerController.inputVariables.aimPosition = hit.point;
        }
    }
}

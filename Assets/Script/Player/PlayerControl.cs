using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PlayerControl : MonoBehaviour
{
    private PlayerView playerView;
    private PlayerModel playerModel;
    private PlayerCombat playerCombat;
    private PhotonView pv;

    public Vector3 dir { get; private set; }
    public Vector3 moveDir { get; set; }

    public bool view { get; private set; }
    public bool jump { get; private set; }
    public bool boost { get; private set; }
    public bool fire { get; private set; }
    public bool reload { get; private set; }
    public bool interact { get; private set; }

    public float camRotationX { get; private set; }
    public float camRotationY { get; private set; }

    void Start()
    {
        playerView = GetComponent<PlayerView>();
        playerModel = GetComponent<PlayerModel>();
        playerCombat = GetComponent<PlayerCombat>();
        pv = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pv.IsMine) return;


        if (Input.GetKeyDown(KeyCode.F10))
            PlayerUI.instance.AddKillLog("tony", "tom");
        if (Input.GetKeyDown(KeyCode.F11))
            PlayerUI.instance.AddKillLog("tony", "rachel");

        if (Input.GetKeyDown(KeyCode.Escape))
            playerView.FixCursor(Cursor.visible);
        TryFire();
        TryInteract();
        TryBoost();
        ControlMove();
        ControlSight();
        ControlView();
    }

    private void TryFire()
    {
        fire = Input.GetMouseButton(0);
    }
    private void TryInteract()
    {
        interact = Input.GetButtonDown("Interact");
    }
    private void TryBoost()
    {
        if (Input.GetButton("Boost") && playerModel.breath < 2f)
            boost = true;
        else
            boost = false;
    }
    private void ControlMove()
    {
        dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        dir.Normalize();
        moveDir = transform.TransformDirection(dir);

        jump = Input.GetButtonDown("Jump");
        reload = Input.GetButtonDown("Reload");
    }
    private void ControlSight()
    {
        float xRotation = Input.GetAxis("Mouse X");
        float vRotation = Input.GetAxis("Mouse Y");// * Setting.Instance.GetMouseSensitivity();

        camRotationX -= vRotation;
        camRotationX = Mathf.Clamp(camRotationX, -80.0f, 80.0f);
        camRotationY += xRotation;
    }
    private void ControlView()
    {
        if (Input.GetKeyDown(KeyCode.F3))
            playerView.ChangeCamera(!playerView.cameraView);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerView : MonoBehaviour
{
    private SkinnedMeshRenderer skinRender;
    private Animator animator;
    private PlayerControl playerControl;
    private PlayerModel playerModel;
    private PhotonView pv;
    public  bool cameraView { get; private set; }
    private GameObject firstCamera;
    private GameObject thirdCamera;

    public string stateInfo = "";

    void Start()
    {
        playerControl = GetComponent<PlayerControl>();
        playerModel = GetComponent<PlayerModel>();
        pv = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        skinRender = transform.Find("Soldier_mesh").GetComponent<SkinnedMeshRenderer>();


        firstCamera = transform.Find("CamGear").Find("FirstCamera").gameObject;
        thirdCamera = transform.Find("CamGear").Find("Spring").Find("ThirdCamera").gameObject;
        if (!playerModel.IsLocalPlayer()) return;
        cameraView = true;
        ChangeCamera(false);
        //FixCursor(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!pv.IsMine) return;

        animator.SetInteger("MoveLevel", (int)playerModel.state);
        animator.SetFloat("Horizontal", playerControl.dir.x);
        animator.SetFloat("Vertical", playerControl.dir.z);
        animator.SetBool("IsGrounded", playerModel.isGrounded);
        animator.SetBool("Jumpped", playerControl.jump);
        animator.SetBool("Attacking", playerControl.fire);
        if (playerControl.reload) animator.SetTrigger("Reload");
        animator.SetBool("IsDead", playerModel.state == PlayerMoveState.DEAD);
    }

    void OnAnimatorIK()
    {
        //if (!playerModel.isGrounded) return;

        print("OnAnimatorIK");

        RaycastHit hit;
        if (Physics.Raycast(transform.position + (Vector3.left * 0.16f) + (Vector3.up * 0.9f), Vector3.down, out hit, 1f, -LayerMask.GetMask("Map")))
        {
            print("Raycast "+ hit.point);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0.5f);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, hit.point);
            animator.SetLookAtWeight(1.0f);
            animator.SetLookAtPosition(hit.point);
        }
    }

    public void OnDamaged()
    {
        animator.SetTrigger("Damaged");
    }
    public void ShowNotice(string _str)
    {
        PlayerUI.instance.ShowNotice(_str, 0f);
    }
    public void ShowKillLog(string _murderer, string _victom)
    {
        playerModel.photonView.RPC("AddKillLog", RpcTarget.All, _murderer, _victom);
    }

    [PunRPC]
    void AddKillLog(string _murderer, string _victom) { PlayerUI.instance.AddKillLog(_murderer, _victom); }
    public void TurnOffCamera()
    {
        firstCamera.GetComponent<Camera>().enabled = false;
        thirdCamera.GetComponent<Camera>().enabled = false;
    }
    public void ChangeCamera(bool _cameraView)
    {
        if (cameraView == _cameraView) return;

        cameraView = _cameraView;
        if (cameraView)
        {
            firstCamera.GetComponent<Camera>().enabled = true;
            thirdCamera.GetComponent<Camera>().enabled = false;
        }
        else
        {
            firstCamera.GetComponent<Camera>().enabled = false;
            thirdCamera.GetComponent<Camera>().enabled = true;
        }
    }

    public void TurnOffVisible(float _duration)
    {
        pv.RPC("RPCturnOffVisible", RpcTarget.All, _duration);
    }

    [PunRPC]
    public void RPCturnOffVisible(float _duration)
    {
        StartCoroutine(TurnOff(_duration));
    }
    IEnumerator TurnOff(float _duration)
    {
        yield return new WaitForSeconds(_duration);
        SetVisible(false);
    }

    public void SetVisible(bool _visible)
    {
        pv.RPC("RPCSetVisible", RpcTarget.All, _visible);
    }
    [PunRPC]
    public void RPCSetVisible(bool _visible)
    {
        skinRender.enabled = _visible;
    }
    public void FixCursor(bool _fix)
    {
        if (_fix)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        } else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}

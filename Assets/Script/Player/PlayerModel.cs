using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public enum PlayerMoveState { IDLE = 0, WALK = 1, RUN = 2, JUMP, ATTACK, DEAD, BOARD, FALL, PARACHUTE };


public class PlayerModel : MonoBehaviourPunCallbacks, IPunObservable
{
    public PlayerMoveState state { get; private set; }
    private CapsuleCollider capsuleCollider;
    private Rigidbody rigidBody;
    private PlayerView playerView;
    private PlayerControl playerControl;
    private PlayerCombat playerCombat;
    //private CharacterController characterController;
    public PhotonView pv;
    public float breath { get; private set; }
    public bool isGrounded { get; private set; }
    public Vector3 gravity { get; private set; }
    private Vector3 jumpPower;
    private Vector3 jumpDir;

    private Vector3 curPos;
    private Vector3 curRot;

    public int curBulletCount;

    private Transform camGear;
    private GameObject plane;
    public string playerName = "player";

    void Start()
    {
        capsuleCollider     = GetComponent<CapsuleCollider>();
        rigidBody           = GetComponent<Rigidbody>();
        playerView          = GetComponent<PlayerView>();
        playerControl       = GetComponent<PlayerControl>();
        playerCombat         = GetComponent<PlayerCombat>();
        //characterController = GetComponent<CharacterController>();
        pv                  = GetComponent<PhotonView>();

        camGear             = transform.Find("CamGear");
        state               = PlayerMoveState.FALL;
        gravity             = Vector3.up * -50f;
        curBulletCount      = 0; 
        
        if (pv.IsMine) InitializeNickname(GameObject.Find("Account").GetComponent<Account>().nickname);

        GameObject.Find("Chicken").GetComponent<TextMeshProUGUI>().enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            CheckGround();
            UpdateSight();


            switch (state)
            {
                case PlayerMoveState.IDLE:
                    Idle();
                    break;
                case PlayerMoveState.WALK:
                    Walk();
                    break;
                case PlayerMoveState.RUN:
                    Run();
                    break;
                case PlayerMoveState.JUMP:
                    Jump();
                    break;
                case PlayerMoveState.FALL:
                    Fall();
                    break;
                case PlayerMoveState.DEAD:
                    Dead();
                    break;
                case PlayerMoveState.BOARD:
                    Board();
                    break;
                case PlayerMoveState.PARACHUTE:
                    Parachute();
                    break;
            }
            Interact();
            transform.position += (playerControl.moveDir) * Time.deltaTime;
        } else
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime);
        }
    }

    private void Interact()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f, -LayerMask.NameToLayer("Item")))
        {
            //playerView.ShowNotice("Grab [F] "+ hit.collider.gameObject.name);

            if (playerControl.interact)
            {
                if (hit.collider.gameObject.GetComponent<Gun>())
                {
                    playerCombat.EquipGun(hit.collider.gameObject.GetComponent<PhotonView>().ViewID, pv.ViewID);
                    hit.collider.gameObject.GetComponent<Gun>().playerModel = this;
                }
                else if (hit.collider.gameObject.GetComponent<BulletBox>())
                {
                    pv.RPC("RemoveObject", RpcTarget.MasterClient, hit.collider.gameObject.GetComponent<PhotonView>().ViewID);

                    AddBullet(30);
                }
            }
        } 
        else
        {
            //playerView.ShowNotice("");
        }
    }

    public void InitializeNickname(string _nickname)
    {
        print("InitializeNickname " + _nickname);
        playerName = _nickname;
        pv.RPC("RPCSetNickname", RpcTarget.OthersBuffered, _nickname);
    }

    [PunRPC]
    public void RPCSetNickname(string _nickname)
    {
        print("RPCSetNickname " + _nickname);
        playerName = _nickname;
    }

    [PunRPC]
    public void RemoveObject(int _viewId)
    {
        //if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(PhotonNetwork.GetPhotonView(_viewId));
    }

    /** �ּ� */
    private void AddBullet(int _bulletCount)
    {
        curBulletCount += _bulletCount;
        PlayerUI.instance.ShowBulletNotice(playerCombat.GetCurBullet(), curBulletCount);
    }
    private void Idle()
    {
        if (playerControl.moveDir != Vector3.zero)
        {
            state = PlayerMoveState.WALK;
            return;
        }
        if (playerControl.jump)
        {
            jumpDir = playerControl.moveDir;
            jumpPower = Vector3.up * 15f;
            state = PlayerMoveState.JUMP;
            return;
        }
    }
    private void Walk()
    {
        if (playerControl.moveDir == Vector3.zero)
        {
            state = PlayerMoveState.IDLE;
            return;
        }
        if (playerControl.jump)
        {
            jumpDir = playerControl.moveDir * 5f;
            jumpPower = Vector3.up * 15f;
            state = PlayerMoveState.JUMP;
            return;
        }
        if (playerControl.boost)
        {
            state = PlayerMoveState.RUN;
            return;
        }
        playerControl.moveDir *= 5f;
        //rigidBody.MovePosition(transform.position + (playerControl.moveDir * Time.deltaTime * 50f));
    }
    private void Run()
    {
        if (playerControl.moveDir == Vector3.zero)
        {
            state = PlayerMoveState.IDLE;
            return;
        }
        if (playerControl.jump)
        {
            jumpDir = playerControl.moveDir * 8f;
            jumpPower = Vector3.up * 15f;
            state = PlayerMoveState.JUMP;
            return;
        }
        if (!playerControl.boost)
        {
            state = PlayerMoveState.WALK;
            return;
        }
        playerControl.moveDir *= 8f;
        //rigidBody.MovePosition(transform.position + (playerControl.moveDir * Time.deltaTime * 80f));
    }
    private void Jump()
    {
        jumpPower += Vector3.down * 7 * Time.deltaTime;
        playerControl.moveDir = jumpDir + jumpPower;

        if (jumpPower.y < 0f)
            state = PlayerMoveState.FALL;

    }
    private void Fall()
    {
        if (playerControl.interact) state = PlayerMoveState.PARACHUTE;
        if (isGrounded) state = PlayerMoveState.IDLE;

        jumpPower += Vector3.down * 7f * Time.deltaTime;
        if (jumpPower.y < 0f)
            jumpPower = Vector3.zero * 7f;


        playerControl.moveDir = jumpDir + jumpPower;
    }
    private void Dead()
    {

    }
    private void Board()
    {
        transform.position = plane.transform.position;
        transform.position += new Vector3(0, 3, 0);

        if (playerControl.interact)
        {
            playerView.SetVisible(true);
            state = PlayerMoveState.FALL;
        }

    }
    private void Parachute()
    {
        if (isGrounded) { state = PlayerMoveState.IDLE; gravity = Vector3.down * 30f; }

        if (gravity.y < -30f)
            gravity -= Vector3.down * 10f * Time.deltaTime;
    }
    public void Die()
    {
        state = PlayerMoveState.DEAD;
        ///capsuleCollider.enabled = false;
        playerView.TurnOffVisible(2f);

        playerCombat.DropGun();
        //GameObject.Find("ExitRoom").SetActive(true);

        if (pv.IsMine)
            GameObject.Find("Canvas").transform.Find("ExitRoom").gameObject.SetActive(true);
    }


    private void UpdateSight()
    {
        camGear.localEulerAngles = new Vector3(playerControl.camRotationX, 0, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, playerControl.camRotationY, 0));
    }
    private void CheckGround()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }
    public void BoardPlane()
    {
        plane = GameObject.Find("Airplane(Clone)");
        playerView.SetVisible(false);
        state = PlayerMoveState.BOARD;
    }
    public int GetPvId()
    {
        return pv.ViewID;
    }
    public bool IsLocalPlayer()
    {
        return GetComponent<PhotonView>().IsMine;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.localEulerAngles);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            transform.localEulerAngles = (Vector3)stream.ReceiveNext();
        }
    }
}

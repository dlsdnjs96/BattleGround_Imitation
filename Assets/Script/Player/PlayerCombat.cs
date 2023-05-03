using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PlayerCombat : MonoBehaviour
{
    private PlayerModel playerModel;
    private PlayerControl playerControl;
    private PlayerView playerView;
    public PhotonView pv;
    private CapsuleCollider capsuleCollider;

    private GameObject hand;
    private Gun gun;
    public GameObject gunObj;

    private float maxHp;
    public float hp;
    private float breath;


    void Start()
    {
        playerModel = GetComponent<PlayerModel>();
        playerControl = GetComponent<PlayerControl>();
        playerView = GetComponent<PlayerView>();
        pv = GetComponent<PhotonView>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        maxHp = 100;
        hp = 100;
        breath = 100.0f;

        FindHand();
    }

    // Update is called once per frame
    void Update()
    {
        if (!pv.IsMine) return;
        if (playerControl.fire) Fire();
        if (playerControl.reload) Reload();
    }

    public int GetCurBullet()
    {
        if (!gun) return 0;
        return gun.magAmmo; }
    public bool IsLocalPlayer()
    {
        return playerModel.photonView.IsMine;
    }
    public void DamageByField(float _damage)
    {
        hp -= _damage;
        playerView.OnDamaged();

        if (pv.IsMine)
        {
            PlayerUI.instance.SetHp(hp / maxHp);
            GameObject.Find("InGameManager").GetComponent<InGameManager>().Dead();
        }
        if (hp <= 0 && playerModel.state != PlayerMoveState.DEAD)
        {
            PlayerUI.instance.AddKillLog("Electronic Field", playerModel.playerName);

            GameObject.Find("InGameManager").GetComponent<InGameManager>().KillPlayer(pv.ViewID, 0);

            playerModel.Die();
            return;
        }
    }
    public void ApplyDamage(float _damage, int _attackerID)
    {
        hp -= _damage;
        // 데미지 받는 모션
        playerView.OnDamaged();

        if (pv.IsMine)
        {
            // 플레이어 본인일경우 HP UI 업데이트
            PlayerUI.instance.SetHp(hp / maxHp);
            GameObject.Find("InGameManager").GetComponent<InGameManager>().Dead();
        }
        if (hp <= 0 && playerModel.state != PlayerMoveState.DEAD)
        {
            // 공격받은 플레이어가 죽을 경우 KILL LOG 출력
            PlayerUI.instance.AddKillLog(
                PhotonNetwork.GetPhotonView(_attackerID).GetComponent<PlayerModel>().playerName, 
                playerModel.playerName);
            // 킬 카운트, 생존자 수 업데이트
            GameObject.Find("InGameManager").GetComponent<InGameManager>().KillPlayer(pv.ViewID, _attackerID);
            playerModel.Die();
            return;
        }
    }

    [PunRPC]
    public void Damaged()
    {
        playerView.OnDamaged();
    }
    public void Reload()
    {
        if (!gun) return;
        gun.Reload();
    }
    public void Fire()
    {
        if (!gun) return;
        gun.Fire();
    }

    [PunRPC]
    public void ShowKillLog(string _attacker, string _victom)
    {
        PlayerUI.instance.AddKillLog(_attacker, _victom);
    }

    public void EquipGun(int _gunID, int _wearerID)
    {
        // 착용중인 총이 있을 경우 내려놓기
        DropGun();
        // 총 오브젝트를 손 오브젝트의 하위로 설정
        pv.RPC("SetGunParent", RpcTarget.All, _gunID, _wearerID);

    }

    public void DropGun()
    { if (gun != null) pv.RPC("RPCDropGun", RpcTarget.All, gun.photonView.ViewID); }

    [PunRPC]
    public void RPCDropGun(int _gunID)
    {
        gun.gameObject.transform.SetParent(null, false);
        gun.gameObject.transform.position = gun.playerModel.transform.position;
        gun.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        gun.playerModel = null;
    }
    [PunRPC]
    public void SetGunParent(int _gunID, int _wearerID)
    {
        gun = PhotonNetwork.GetPhotonView(_gunID).gameObject.GetComponent<Gun>();
        PhotonNetwork.GetPhotonView(_gunID).gameObject.transform.position = GetGunPos(gun.gameObject.name);
        PhotonNetwork.GetPhotonView(_gunID).gameObject.transform.rotation = GetGunRot(gun.gameObject.name);
        PhotonNetwork.GetPhotonView(_gunID).gameObject.transform.SetParent(hand.transform, false);
        gun.playerModel = PhotonNetwork.GetPhotonView(_wearerID).GetComponent<PlayerModel>();
    }


    private Vector3 GetGunPos(string _gun)
    {
        print(_gun);
        if (_gun == "benelliM4(Clone)") return new Vector3(-0.2063673f, 0.04217921f, 08572362f);
        else if (_gun == "m4(Clone)") return new Vector3(-0.2029536f, 0.03991704f, 0.07779779f);
        else if (_gun == "M110(Clone)") return new Vector3(-0.1981031f, 0.03915207f, 0.06491418f);
        else if (_gun == "M249(Clone)") return new Vector3(-0.2000125f, 0.01619087f, 0.09924736f);
        else if (_gun == "mp5(Clone)") return new Vector3(-0.2006111f, 0.03788034f, 0.05669676f);
        else if (_gun == "smaw(Clone)") return new Vector3(-0.1490482f, 0.03939275f, 0.06180187f);
        

        return Vector3.zero;
    }
    private Quaternion GetGunRot(string _gun)
    {
        if (_gun == "benelliM4(Clone)") return Quaternion.Euler(-10.416f, -2.167f, -87.689f);
        else if (_gun == "m4(Clone)") return Quaternion.Euler(-10.416f, -2.167f, -87.689f);
        else if (_gun == "M110(Clone)") return Quaternion.Euler(-10.416f, -2.167f, -87.689f);
        else if (_gun == "M249(Clone)") return Quaternion.Euler(-10.453f, -1.198f, -87.865f);
        else if (_gun == "mp5(Clone)") return Quaternion.Euler(-10.406f, -2.411f, -87.645f);
        else if (_gun == "smaw(Clone)") return Quaternion.Euler(-4.473f, -2.104f, 91.194f);
        

        return Quaternion.identity;
    }

    private void FindHand()
    {
        Transform[] children = transform.Find("Bip001").GetComponentsInChildren<Transform>();

        foreach (Transform it in children)
        {
            if (it.name == "Bip001 R Hand")
            {
                hand = it.gameObject;
                break;
            }
        }
    }

    public int GetPvId()
    {
        return playerModel.GetPvId();
    }


}

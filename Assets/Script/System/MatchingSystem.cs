using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class MatchingSystem : MonoBehaviourPunCallbacks
{

    //NetworkManager PhotonNet;
    private int roomNumber;

    private readonly byte minPlayerToStart = 2;


    void Start()
    {
        gameObject.SetActive(false);
        //PhotonNetwork.LocalPlayer.NickName = "nick2";
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void TryToJoinRoom()
    {
        print("TryToJoinRoom");
        if (PhotonNetwork.InRoom)
        {
            CancelInvoke("TryToJoinRoom");
            if (PhotonNetwork.CurrentRoom.PlayerCount == minPlayerToStart)
            {
                photonView.RPC("LoadRoomScene", RpcTarget.AllBuffered);
                //PhotonNetwork.CurrentRoom.IsOpen = false;
            }

            return;
        }
        Debug.Log("room" + roomNumber);
        //PhotonNetwork.JoinRoom("room" + roomNumber);
        PhotonNetwork.JoinOrCreateRoom("room" + roomNumber, new RoomOptions { MaxPlayers = 4 }, null);
        roomNumber++;
    }
    public void StartMatching()
    {
        gameObject.SetActive(true);
        roomNumber = 0;
        InvokeRepeating("TryToJoinRoom", 1.0f, 1.0f);
    }

    public void CancelMatching()
    {
        gameObject.SetActive(false);
        CancelInvoke("JoinRoomRecursive");
    }

    public override void OnJoinedRoom()
    {
        //PhotonNetwork.Instantiate("Soldier", Vector3.zero, Quaternion.identity);
        // GameObject.Find("InGameManager").GetComponent<InGameManager>().UpdatePlayerCount(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    [PunRPC]
    void LoadRoomScene()
    {
        GameObject.Find("Account").GetComponent<Account>().nickname = GameObject.Find("InputNickname").GetComponent<TMP_InputField>().text;
        print("Account " + GameObject.Find("Account").GetComponent<Account>().nickname);
        PhotonNetwork.LoadLevel("InGame");
        //PhotonNetwork.Instantiate("Soldier", new Vector3(-100, 1000, -100), Quaternion.identity);
        //PhotonNetwork.Instantiate("Soldier", new Vector3(-100, 1000, -100), Quaternion.identity);
        //gameObject.SetActive(false);
    }
}
